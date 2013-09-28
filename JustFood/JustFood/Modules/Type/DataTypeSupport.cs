
using System;

namespace JustFood.Modules.Type {
    public class DataTypeSupport {

        /// <summary>
        /// Returns true type is primitive type or guid or string or datetime.
        /// </summary>
        /// <param name="o">Pass the object of any type.</param>
        /// <returns>Returns true type is primitive type or guid or string or datetime. If complex or custom class then returns false.</returns>
        public static bool Support(object o) {
            var checkLong = o is long;
            var checkInt = o is int || o is Int16 || o is Int32 || o is Int64;
            var checkDecimal = o is float || o is decimal || o is double;
            var checkString = o is string;
            var checkGuid = o is Guid;
            var checkBool = o is bool || o is Boolean;
            var checkDateTime = o is DateTime;
            var checkByte = o is byte || o is Byte;

            if (checkString || checkByte || checkLong || checkInt || checkDecimal || checkGuid || checkBool || checkDateTime)
                return true;
            else
                return false;
        }
    }
}