using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DataPath.Tests {
    class Context {
        private Dictionary<Type, IContextList> Data = new Dictionary<Type, IContextList>();

        public Context Register<T>() where T : class {
            Data.Add(typeof(T), new ContextList<T>());
            return this;
        }
        
        public void AddData<T>(T item) where T : class {
            if(!Data.ContainsKey(typeof(T)))
                Register<T>();
            
            Data[typeof(T)].Data.Add(item);
        }

        public TClass GetByKey<TClass, TKey>(TKey key) where TClass : class {
            return null;
        }

        interface IContextList {
            object GetByKey<TKey>(TKey key);
            IList Data { get; }
        }

        class ContextList<T> : IContextList where T : class {
            private static PropertyInfo KeyProperty;

            static ContextList() {
                KeyProperty = GetKeyProperty();
            }

            private static PropertyInfo GetKeyProperty() {
                foreach (var prop in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)) {
                    var attrs = prop.GetCustomAttributes(typeof(KeyAttribute), true);
                    if (attrs.Length != 0)
                        return prop;
                }

                return null;
            }

            private List<T> data;

            public ContextList() { data = new List<T>(); }

            public IList Data { get { return data; } }

            public object GetByKey<TKey>(TKey key) {
                // key not defined
                if (KeyProperty.PropertyType != typeof(TKey))
                    return null;

                foreach (var item in data)
                    if (object.Equals(KeyProperty.GetValue(this, null), key))
                        return item;

                // key not found
                return null;
            }
        }
    }
}
