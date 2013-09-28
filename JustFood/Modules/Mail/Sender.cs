using System;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using JustFood.Models;
using JustFood.Modules.StaticContents;
using JustFood.Modules.UserError;
using Mvc.Mailer;
using JustFood.Modules.Extensions;


namespace JustFood.Modules.Mail {
    public class Sender {

        //private Emailer _Emailer = AppConfig.EmailSender;
        private bool _IsDiposeded = false;
        public bool IsDiposeded { get { return _IsDiposeded; } }

        public const bool SEND_ASY = true;


        public string GetSubject(string sub, string type = "") {
            if (!string.IsNullOrEmpty(type))
                return "[" + AppVar.Name + "][" + AppVar.CompanyName + "] " + sub;
            else
                return "[" + AppVar.Name + "][" + AppVar.CompanyName + "][" + type + "] " + sub;
        }


        public void QuickMail(string body, string sub, string to = "", bool isHtml = true, bool send = true, bool NotifyDeveloper = false, bool NotifyEditors = false) {

            Thread t = new Thread(() => {
                var mail = new MailMessage();
                try {
                    mail.Subject = sub;
                    mail.Body = body;
                    mail.IsBodyHtml = isHtml;
                    if (send) {
                        if (to != "") {
                            mail.To.Add(to);
                            this.NotifySomeOne(mail, null, null);
                        }
                        if (NotifyDeveloper) {
                            this.NotifyDeveloper(mail.Subject, mail);
                        }
                    }
                } catch (Exception ex) {

                }
            });

            t.Start();

        }


        private void SendEmail(MailMessage mail) {
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.From = new MailAddress(AppVar.MailAddressFrom);
            try {
                new Thread(() => {
                    if (SEND_ASY)
                        mail.SendAsync();
                    else
                        mail.Send();
                }).Start();
            } catch (Exception ex) {

            }
        }

        public void NotifyAdmin(string subject, MailMessage mail) {
            mail.Subject = subject;
            mail.To.Add(AppVar.AdminEmail);
            SendEmail(mail);
        }

        public void NotifySomeOne(MailMessage mail, string email = null, string subject = null) {
            if (subject != null)
                mail.Subject = subject;
            if (email != null)
                mail.To.Add(email);
            SendEmail(mail);
        }

        public void NotifySomeOne(string HtmlMail, string EmailAddress = null, string subject = null) {
            var mail = new MailMessage();
            if (subject != null)
                mail.Subject = subject;
            if (EmailAddress != null)
                mail.To.Add(EmailAddress);
            mail.IsBodyHtml = true;
            mail.Body = HtmlMail;

            SendEmail(mail);
        }

        public void NotifyDeveloper(string subject, MailMessage mail) {
            if (AppVar.IsNotifyDeveloper) {
                mail.Subject = subject;
                mail.To.Add(AppVar.DeveloperEmail);
                SendEmail(mail);

            }
        }

        /// <summary>
        /// Return all admin emails
        /// </summary>
        /// <returns></returns>
        public string[] GetAdmins() {
            using (var db = new JustFoodDBEntities()) {
                return db.Users.Where(n => n.IsAccessToAdmin).Select(n => n.Email).ToArray();
            }

        }


        public void OnMistakeNotifyAllAdmins(ErrorCollector errors) {
            if (!AppConfig.Setting.OnMistakeNotifyAllAdmins)
                return;
            Thread mailSender = new Thread(() => {
                var admins = GetAdmins();
                var mail = new MailMessage();
                mail.Subject = GetSubject("User mistakes recorded");
                string tableStart = "<table>",
                       tableEnd = "</table>", 
                       rowSt = "<tr>", 
                       rowEd = "</tr>", 
                       colSt = "<td>", 
                       colEd = "</td>",
                       thSt = "<th>",
                       thEd = "</th>";
                string rows = "";
                foreach (var item in errors.GetErrors()) {
                    rows += rowSt +
                            colSt +
                            item.OrderID +
                            colEd + colSt +
                            item.Message +
                            colEd;
                }

                string msg = tableStart +
                             "<thead>" +
                             thSt + "Order ID" + thEd +
                             thSt + "Message" + thEd +
                             "</thead><tbody>" +
                             rows +
                             "</tbody>" +
                             tableEnd;
                mail.Body = msg;
                foreach (var admin in admins) {
                    mail.Bcc.Add(admin);
                }
                SendEmail(mail);
            });
            mailSender.Start();

        }


        public void NotifyUser(string subject, MailMessage mail, string UserEamil) {
            mail.Subject = subject;
            mail.To.Add(UserEamil);
            SendEmail(mail);
        }

        public void NotifyUserMulti(string subject, MailMessage mail, params string[] UserEamil) {
            mail.Subject = subject;
            foreach (var uemail in UserEamil) {
                mail.Bcc.Add(uemail);
            }
            SendEmail(mail);
        }

    }
}