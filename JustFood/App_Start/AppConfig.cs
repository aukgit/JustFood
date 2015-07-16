using System.Linq;
using DevMvcComponent;
using DevMvcComponent.Mailer;
using DB = JustFood.Models;
namespace JustFood {
    /// <summary>
    /// Application Configurations
    /// </summary>
    public static class AppConfig {
        private static DB.Config _setting = null;
        private static int _truncateLength = 25;

        public static int TruncateLength {
            get { return _truncateLength; }
            set { _truncateLength = value; }
        }
        private static void InitalizeDevelopersOrganismComponent() {
            var host = "smtp.gmail.com";
            var port = 587;
            var password = "newPasswordPleaseDon'tChangeItForEveryone";

            var mailer = new GmailConfig(AppVar.MailAddressFrom, password, host, port);
            mailer.SendAsynchronousEmails = false;
            mailer.EnableSsl = true;
            Starter.Setup(AppVar.Name,
               AppVar.DeveloperEmail,
               System.Reflection.Assembly.GetExecutingAssembly(),
               mailer);
        }

        /// <summary>
        /// Get few common classes from Developers Organism Component.
        /// </summary>
        public static void Setup() {
            RefreshSetting();
            InitalizeDevelopersOrganismComponent();
        }

        public static DB.Config Setting {
            get {
                if (_setting == null) {
                    using (var db = new DB.JustFoodDBEntities()) {
                        _setting = db.Configs.FirstOrDefault();
                    }
                }
                return _setting;
            }
        }

        public static void RefreshSetting() {
            using (var db = new DB.JustFoodDBEntities()) {
                _setting = db.Configs.FirstOrDefault();
                AppVar.Name = Setting.ApplicationName.ToString();
                AppVar.Subtitle = Setting.ApplicationSubtitle.ToString();
                AppVar.DeveloperEmail = Setting.DeveloperEmail.ToString();
                AppVar.AdminEmail = Setting.AdminEmail.ToString();
                AppVar.IsNotifyAdmin = Setting.IsNotifyAdminOnError;
                AppVar.IsNotifyDeveloper = Setting.IsNotifyDeveloperOnError;
                AppVar.CompanyName = Setting.CompanyName;
                InitalizeDevelopersOrganismComponent();

            }
        }

    }
    /// <summary>
    /// Application Variables
    /// </summary>
    public struct AppVar {

        /// <summary>
        /// Company that is currently using the software.
        /// </summary>
        public static string CompanyName = AppConfig.Setting.CompanyName.ToString();

        /// <summary>
        /// Application Name
        /// </summary>
        public static string Name = AppConfig.Setting.ApplicationName.ToString();
        /// <summary>
        /// Application Subtitle
        /// </summary>
        public static string Subtitle = AppConfig.Setting.ApplicationSubtitle.ToString();

        public static string DeveloperEmail = AppConfig.Setting.DeveloperEmail;

        /// <summary>
        /// Most common admin email or the owner of the business email who conducts/monitor the business everyday.
        /// </summary>
        public static string AdminEmail = AppConfig.Setting.AdminEmail;

        public static bool IsNotifyDeveloper = AppConfig.Setting.IsNotifyDeveloperOnError;

        public static bool IsNotifyAdmin = AppConfig.Setting.IsNotifyAdminOnError;

        public const string MailAddressFrom = "just.food.mailer@gmail.com";

        /// <summary>
        /// Software version.
        /// </summary>
        public static string AppVersion = "1.2";
    }
}