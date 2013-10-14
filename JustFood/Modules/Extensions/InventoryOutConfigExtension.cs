using System.Collections.Generic;
using System.Linq;
using JustFood.Models;

namespace JustFood.Modules.Extensions {
    public class InventoryOutConfigExtension {
        public List<InventoryOutConfig> GetInvenoryOutByCategory(List<InventoryOutConfig> list ,int CategoryID) {
            return list.Where(n => n.CategoryID == CategoryID).ToList();
        }

        /// <summary>
        /// Return Category SelectList object except the current category.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="CategoryID"></param>
        /// <returns></returns>
        public IEnumerable<Category> GetCategoriesExceptCurrent(List<Category> list, int CategoryID) {
            return list.Where(n => n.CategoryID != CategoryID);
        }
    }
}