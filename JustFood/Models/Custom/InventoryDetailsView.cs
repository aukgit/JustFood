using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace JustFood.Models {
    public class InventoryDetailsView {
        private readonly int _categoryId;
        private readonly JustFoodDBEntities db = new JustFoodDBEntities();
        public InventoryDetailsView(int CategoryID) { _categoryId = CategoryID; }

        public IEnumerable<InventoryIn> InventoryStrock {
            get {
                return db.InventoryIns
                         .Include(n => n.QuantityType)
                         .Where(n => n.CategoryID == _categoryId)
                         .ToList();
            }
        }

        public IEnumerable<InventoryOut> InventoryDiscards {
            get {
                return db.InventoryOuts
                         .Include(n=> n.QuantityType)
                         .Where(n => n.CategoryID == _categoryId)
                         .ToList();
            }
        }
    }
}