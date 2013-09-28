

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JustFood.Modules.ObjectToArray {

    public class ObjectProperty {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class ObjectToArrary {
        public static List<ObjectProperty> Get(object Class) {
            if (Class != null) {
                var typeOfPropertise = BindingFlags.Public | BindingFlags.Instance;
                var propertise = Class.GetType().GetProperties(typeOfPropertise).Where(p =>  /* p.Name != "EntityKey" &&*/ p.Name != "EntityState");
                
                var list = new List<ObjectProperty>(propertise.Count());
                foreach (var prop in propertise) {
                    object val = prop.GetValue(Class, null);
                    string propertyName = prop.Name;
                    var obj = new ObjectProperty {
                        Name = propertyName,
                        Value = val
                    };
                    list.Add(obj);
                }
                return list;
            }
            return null;
        }
    }
}