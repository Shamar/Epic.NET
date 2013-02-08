//
//  EquatableDictionary.cs
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
using System.Runtime.Serialization;

namespace Epic
{
    [Serializable]
    public class EquatableDictionary<K, V> : Dictionary<K,V>
    {
        public EquatableDictionary()
            : base()
        {
        }

        #region IEquatable implementation
        
        public bool Equals(EquatableDictionary<K, V> other)
        {
            if(null == other || this.Count != other.Count)
                return false;
            
            foreach(var kvp in this)
            {
                if(!other.ContainsKey(kvp.Key) || !kvp.Value.Equals(other[kvp.Key]))
                    return false;
            }
            
            return true;
        }
        
        #endregion
        
        private EquatableDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EquatableDictionary<K, V>);
        }
        
        public override int GetHashCode()
        {
            return typeof(V).GetHashCode();
        }
    }
}

