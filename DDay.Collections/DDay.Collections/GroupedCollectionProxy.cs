using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections;

namespace DDay.Collections
{
    /// <summary>
    /// A proxy for a keyed list.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class GroupedCollectionProxy<TGroup, TOriginal, TNew> :
        GroupedCollection<TGroup, TNew>
        where TOriginal : class, IGroupedObject<TGroup>
        where TNew : class, TOriginal
    {
        #region Private Fields

        IGroupedCollection<TGroup, TOriginal> _RealObject;        
        Predicate<TNew> _Predicate;

        #endregion

        #region Constructors

        public GroupedCollectionProxy(IGroupedCollection<TGroup, TOriginal> realObject, Predicate<TNew> predicate = null)
        {
            _Predicate = predicate ?? new Predicate<TNew>(o => true);
            SetProxiedObject(realObject);

            _RealObject.ItemAdded += new EventHandler<ObjectEventArgs<TOriginal, int>>(_RealObject_ItemAdded);
            _RealObject.ItemRemoved += new EventHandler<ObjectEventArgs<TOriginal, int>>(_RealObject_ItemRemoved);
        }

        #endregion

        #region Event Handlers

        void _RealObject_ItemRemoved(object sender, ObjectEventArgs<TOriginal, int> e)
        {
            TNew item = e.First as TNew;
            if (item != null && _Predicate(item))
                Remove(item);
        }

        void _RealObject_ItemAdded(object sender, ObjectEventArgs<TOriginal, int> e)
        {
            TNew item = e.First as TNew;
            if (item != null && _Predicate(item))
                Add(item);
        }

        #endregion

        #region Public Methods

        virtual public void SetProxiedObject(IGroupedCollection<TGroup, TOriginal> realObject)
        {
            _RealObject = realObject;
            Clear();

            foreach (var obj in realObject)
            {
                TNew item = obj as TNew;
                if (item != null && _Predicate(item))
                    Add(item);
            }
        }

        #endregion        
    }
}