//
//  EquatableEnumerable.cs
//
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
//
//  Copyright (c) 2010-2013 Giacomo Tesio
//
//  This file is part of Epic.NET.
//
//  Epic.NET is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Affero General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  Epic.NET is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Affero General Public License for more details.
//
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
using System;
using System.Collections.Generic;

namespace Epic
{
    [Serializable]
    public class EquatableEnumerable<T> : IEnumerable<T>, IEquatable<IEnumerable<T>>, IEquatable<EquatableEnumerable<T>>
        where T : IEquatable<T>
    {
        private readonly T[] _items;
        public EquatableEnumerable(params T[] items)
        {
            _items = items;
        }
        
        #region IEquatable implementation
        
        public bool Equals(EquatableEnumerable<T> other)
        {
            if(null == other || _items.Length != other._items.Length)
                return false;
            
            for(int i = 0; i < _items.Length; ++i)
                if(!_items[i].Equals(other._items[i]))
                    return false;
            
            return true;
        }
        public override int GetHashCode()
        {
            return typeof(T).GetHashCode();
        }
        #endregion
        
        #region IEquatable implementation
        
        public bool Equals(IEnumerable<T> other)
        {
            return Equals(other as EquatableEnumerable<T>);
        }
        
        #endregion
        
        public override bool Equals(object obj)
        {
            return Equals(obj as IEquatable<IEnumerable<T>>);
        }
        
        #region IEnumerable implementation
        
        public IEnumerator<T> GetEnumerator()
        {
            return (_items as IEnumerable<T>).GetEnumerator();
        }
        
        #endregion
        
        #region IEnumerable implementation
        
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
        
        #endregion
    }
}

