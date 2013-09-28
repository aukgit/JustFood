using System.Collections.Generic;

namespace JustFood.Models {
    public class AccountBalanceMultiple {
        public IEnumerable<ViewAccountBalance> ViewAccountBalance { get; set; }
        public AccountBalanceSearch SearchBox { get; set; }
    }
}