using System.Linq;
using JustFood.Models;
using JustFood.Modules.Query;

namespace JustFood.Modules.Extensions {
    public class UserAccountsExtension {
        JustFoodDBEntities db = new JustFoodDBEntities();
        UserInfo userInfo = new UserInfo();
        
        public User GetProductBoughtUser() {
            const string dailyProductAdder = "Product Bought";
            var accountOf = db.Users.FirstOrDefault(n => n.LogName == dailyProductAdder);
            if (accountOf == null) {
                accountOf = new User() {
                    LogName = dailyProductAdder,
                    Name = dailyProductAdder,
                    IsBlocked = false,
                    IsAccessToAdmin = false,
                    IsEmployee = false,
                    IsOwner = false,
                    TimeZoneID = userInfo.GetUser()
                                         .TimeZoneID,
                    Email = "none",
                    IsValidEmail = false
                };
                db.Users.Add(accountOf);
                db.SaveChanges();
            }
            return accountOf;
        }

        public User GetExpenseAccount() {
            const string dailyExpense = "Daily Expense";
            var accountOf = db.Users.FirstOrDefault(n => n.LogName == dailyExpense);
            if (accountOf == null) {
                accountOf = new User() {
                    LogName = dailyExpense,
                    Name = dailyExpense,
                    IsBlocked = false,
                    IsAccessToAdmin = false,
                    IsEmployee = false,
                    IsOwner = false,
                    TimeZoneID = userInfo.GetUser()
                                         .TimeZoneID,

                };
                db.Users.Add(accountOf);
                db.SaveChanges();
            }
            return accountOf;
        }

    }
}