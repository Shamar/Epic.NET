//  
//  OrderCriterionBase.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2012 Giacomo Tesio
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

namespace Epic.Query.Object
{
    [Serializable]
    public abstract class OrderCriterion<TEntity> : VisitableBase, 
                                                    IComparer<TEntity>,
                                                    IEquatable<OrderCriterion<TEntity>>
    {
        protected OrderCriterion ()
            : base()
        {
        }

        public OrderCriterion<TEntity> Chain(OrderCriterion<TEntity> other)
        {
            return new OrderCriteria<TEntity>(this, other);
        }

        public virtual OrderCriterion<TEntity> Reverse()
        {
            return new ReverseOrder<TEntity>(this);
        }

        #region IComparer implementation

        public abstract int Compare (TEntity x, TEntity y);

        #endregion

        #region IEquatable implementation

        protected abstract bool SafeEquals(OrderCriterion<TEntity> other);

        public bool Equals (OrderCriterion<TEntity> other)
        {
            if(null == other)
                return false;
            if(object.ReferenceEquals(this, other))
                return true;
            if(!this.GetType().Equals(other.GetType()))
                return false;
            return SafeEquals(other);
        }

        public sealed override bool Equals (object obj)
        {
            return Equals (obj as OrderCriterion<TEntity>);
        }

        public sealed override int GetHashCode ()
        {
            return GetType().GetHashCode ();
        }

        #endregion IEquatable implementation
    }
}

