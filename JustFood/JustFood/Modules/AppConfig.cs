using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevMVCComponent;
using DB = JustFood.Models;
namespace JustFood.Modules.Extensions {
    /// <summary>
    /// Application Configurations
    /// </summary>
    public static class AppConfig {
        private static DB.Config setting = null;
        private static Starter devComponent = null;
        private static int _truncateLength = 25;

        public static int TruncateLength {
            get { return _truncateLength; }
            set { _truncateLength = value; }
        }
        private static void InitalizeDevelopersOrganismComponent() {
            Config.ApplicationName = AppVar.Name;
            Config.AdminEmail = Setting.AdminEmail;
            Config.DeveloperEmail = Setting.DeveloperEmail;
            Config.Assembly = System.Reflection.Assembly.GetExecutingAssembly();
        }

        /// <summary>
        /// Get few common classes from Developers Organism Component.
        /// </summary>
        public static Starter DevComp {
            get {
                if (devComponent == null) {
                    devComponent = new Starter();
                    InitalizeDevelopersOrganismComponent();
                }
                return devComponent;
            }
        }

        public static DB.Config Setting {
            get {
                if (setting == null) {
                    using (var db = new DB.JustFoodDBEntities()) {
                        setting = db.Configs.FirstOrDefault();
                    }
                }
                return setting;
            }
        }

        public static void RefreshSetting() {
            using (var db = new DB.JustFoodDBEntities()) {
                setting = db.Configs.FirstOrDefault();
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

        public static  string DeveloperEmail = AppConfig.Setting.DeveloperEmail;

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