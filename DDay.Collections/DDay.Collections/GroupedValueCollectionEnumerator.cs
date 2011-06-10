﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDay.Collections
{
    public class GroupedValueCollectionEnumerator<TGroup, TOriginal, TOriginalValue, TNewValue> :
        IEnumerator<TNewValue>
        where TOriginal : class, IGroupedObject<TGroup>, IValueObject<TOriginalValue>
        where TNewValue : TOriginalValue
    {
        GroupedCollection<TGroup, TOriginal> _List;
        TGroup _Key;
        IEnumerator<TOriginalValue> _ValueEnumerator;
        IEnumerator<TOriginal> _ObjectEnumerator;

        public GroupedValueCollectionEnumerator(GroupedCollection<TGroup, TOriginal> list, TGroup group)
        {
            _List = list;
            _Key = group;
        }

        virtual public TNewValue Current
        {
            get
            {
                if (_ValueEnumerator != null &&
                    _ValueEnumerator.Current is TNewValue)
                    return (TNewValue)_ValueEnumerator.Current;
                return default(TNewValue);
            }
        }

        virtual public void Dispose()
        {
            Reset();
        }

        void DisposeValueEnumerator()
        {
            if (_ValueEnumerator != null)
            {
                _ValueEnumerator.Dispose();
                _ValueEnumerator = null;
            }
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                if (_ObjectEnumerator != null)
                    return _ObjectEnumerator.Current;
                return default(TOriginal);
            }
        }

        private bool MoveNextList()
        {
            if (_ObjectEnumerator == null)
            {
                _ObjectEnumerator = _List.AllOf(_Key).GetEnumerator();
            }

            if (_ObjectEnumerator != null)
            {
                if (_ObjectEnumerator.MoveNext())
                {
                    DisposeValueEnumerator();
                    if (_ObjectEnumerator.Current != null &&
                        _ObjectEnumerator.Current.Values != null)
                    {
                        if (_ObjectEnumerator.Current.Values != null)
                        {
                            return MoveNextList();
                        }
                        else
                        {
                            _ValueEnumerator = _ObjectEnumerator.Current.Values.GetEnumerator();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        virtual public bool MoveNext()
        {
            if (_ObjectEnumerator != null)
            {
                if (_ObjectEnumerator.MoveNext())
                {
                    if (_ValueEnumerator.Current is TNewValue)
                    {
                        return true;
                    }
                    else
                    {
                        return MoveNext();
                    }
                }
                else
                {
                    DisposeValueEnumerator();
                    if (MoveNextList())
                        return MoveNext();
                }
            }
            else
            {
                if (MoveNextList())
                    return MoveNext();
            }
            return false;
        }

        virtual public void Reset()
        {
            DisposeValueEnumerator();
            if (_ObjectEnumerator != null)
            {
                _ObjectEnumerator.Dispose();
                _ObjectEnumerator = null;
            }
        }
    }
}