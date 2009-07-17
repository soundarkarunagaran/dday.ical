using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace DDay.iCal
{
    /// <summary>
    /// A list of objects that are keyed.  This is similar to a 
    /// Dictionary<T,U> object, except 
    /// </summary>
#if SILVERLIGHT
    [DataContract(Name = "KeyedList", Namespace="http://www.ddaysoftware.com/dday.ical/2009/07/")]
#else
    [Serializable]
#endif
    public class KeyedList<T, U> :
        List<T>,
        IKeyedList<T, U> where T : IKeyedObject<U>
    {
        #region IKeyedList<T, U> Members

        /// <summary>
        /// Returns true if the list contains at least one 
        /// object with a matching key, false otherwise.
        /// </summary>
        public bool ContainsKey(U key)
        {
            return IndexOf(key) >= 0;
        }

        /// <summary>
        /// Returns the index of the first object
        /// with the matching key.
        /// </summary>
        public int IndexOf(U key)
        {
            return ((List<T>)this).FindIndex(
                delegate(T obj)
                {
                    return object.Equals(obj.Key, key);
                }
            );
        }

        public int CountOf(U key)
        {
            return AllOf(key).Count;
        }

        public IList<T> AllOf(U key)
        {
            return ((List<T>)this).FindAll(
                delegate(T obj)
                {
                    return object.Equals(obj.Key, key);
                }
            );
        }

        public T this[U key]
        {
            get
            {
                int index = IndexOf(key);
                if (index >= 0)
                    return this[IndexOf(key)];
                return default(T);
            }
            set
            {
                if (ContainsKey(key))
                    this[IndexOf(key)] = value;
                else
                    Add(value);
            }
        }

        public bool Remove(U key)
        {
            int index = IndexOf(key);
            bool removed = false;

            while (index >= 0)
            {
                RemoveAt(index);
                removed = true;
                index = IndexOf(key);
            }

            return removed;
        }

        #endregion
    }
}
