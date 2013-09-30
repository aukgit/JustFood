using System.Linq;
using JustFood.Models;
using JustFood.Modules.StaticContents;
using JustFood.Modules.Message;

namespace JustFood.Modules.Extensions {
    public class QuantityTypeExtension {
        /// <summary>
        /// Check if quantity matches with any other converted quantity.
        /// False: means accept the result. (it doesn't mismatch)
        /// True: means do not accept the result. ( it does mismatch )
        /// Use it like !(Mismatch(..,..,..)
        /// </summary>
        /// <param name="CategoryID"></param>
        /// <param name="PassingQuantity">Converting Quantity Type pass and it will be check against the products default quantity in the category table.</param>
        /// <param name="Multiply">Returns the value of multiple to be need to make the conversion.</param>
        /// <returns>Returns false when does not mismatch and returns true when type mismatch like ml. can't be converted into grams.</returns>
        public bool Mismatch(int CategoryID, byte PassingQuantity, out decimal Multiply, bool trackErrors = true) {
            using (var db = new JustFoodDBEntities()) {
                // current category qty
                var category = db.Categories.Find(CategoryID);
                if (category != null) {
                    var currentQtyTypeFromCategory = category.QtyType;
                    if (currentQtyTypeFromCategory == PassingQuantity) {
                        Multiply = 1;
                        return false;
                    }
                    var quantityConverter = db.QuantityConversations.FirstOrDefault(n => n.QuantityTypeID == currentQtyTypeFromCategory && n.QuantityConversationID == PassingQuantity);

                    if (quantityConverter != null) {
                        Multiply = quantityConverter.Difference;
                        return false;
                    }

                }
            }
            if (trackErrors) { Statics.ErrorCollection.Add(Const.QuantityTypeMismatch); }
            Multiply = 0;
            return true;
        }
    }
}