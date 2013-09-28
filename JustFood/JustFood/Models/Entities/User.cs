using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JustFood.Models {
    public class User {
        public User() {
            this.AccountBalances = new HashSet<AccountBalance>();
            this.AccountBalances1 = new HashSet<AccountBalance>();
            this.DetailedSales = new HashSet<DetailedSale>();
            this.Inventories = new HashSet<Inventory>();
            this.InventoryIns = new HashSet<InventoryIn>();
            this.Notifications = new HashSet<Notification>();
            this.NotificationSeens = new HashSet<NotificationSeen>();
            this.Requisitions = new HashSet<Requisition>();
            this.Requisitions1 = new HashSet<Requisition>();
            this.SalaryPaids = new HashSet<SalaryPaid>();
        }

        public int UserID { get; set; }

        [DisplayName("Log")]
        public string LogName { get; set; }

        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }

        [DisplayName("Last Log")]
        public DateTime LastLogIn { get; set; }

        [DisplayName("Employee")]
        public bool IsEmployee { get; set; }

        [DisplayName("Owner")]
        public bool IsOwner { get; set; }

        public double Percentage { get; set; }
        public double Salary { get; set; }

        [DisplayName("Admin")]
        public bool IsAccessToAdmin { get; set; }


        public bool IsBlocked { get; set; }

        [DisplayName("Time Zone")]
        public short TimeZoneID { get; set; }

        [Required]
        public string Email { get; set; }
        [Required]
        public bool IsValidEmail { get; set; }

        public virtual ICollection<AccountBalance> AccountBalances { get; set; }
        public virtual ICollection<AccountBalance> AccountBalances1 { get; set; }

        public virtual ICollection<DetailedSale> DetailedSales { get; set; }


        public virtual ICollection<Inventory> Inventories { get; set; }
        public virtual ICollection<InventoryIn> InventoryIns { get; set; }
        public virtual ICollection<Requisition> Requisitions { get; set; }
        public virtual ICollection<Requisition> Requisitions1 { get; set; }
        public virtual ICollection<SalaryPaid> SalaryPaids { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<NotificationSeen> NotificationSeens { get; set; }
        public virtual TimeZone TimeZone { get; set; }

    }
}