//  
//  ReverseOrder.cs
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
using System.Runtime.Serialization;

namespace Epic.Query.Object
{
    [Serializable]
    public sealed class ReverseOrder<TEntity> : OrderCriterion<TEntity>
    {
        private readonly OrderCriterion<TEntity> _toReverse;
        internal ReverseOrder (OrderCriterion<TEntity> toReverse)
        {
            if(null == toReverse)
                throw new ArgumentNullException("toReverse");
            _toReverse = toReverse;
        }

        public OrderCriterion<TEntity> Reversed
        {
            get
            {
                return _toReverse;
            }
        }

        public override OrderCriterion<TEntity> Chain (OrderCriterion<TEntity> other)
        {
            return new OrderCriteria<TEntity>(this, other);
        }

        public override OrderCriterion<TEntity> Reverse ()
        {
            return _toReverse;
        }

        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe(this, visitor, context);
        }

        public override int Compare (TEntity x, TEntity y)
        {
            return _toReverse.Compare(y, x);
        }

        protected override bool SafeEquals (OrderCriterion<TEntity> other)
        {
            ReverseOrder<TEntity> reverseOther = other as ReverseOrder<TEntity>;
            return _toReverse.Equals(reverseOther._toReverse);
        }

        #region implemented abstract members of Epic.Query.Object.OrderCriterion
        private ReverseOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _toReverse = (OrderCriterion<TEntity>)info.GetValue("C", typeof(OrderCriterion<TEntity>));
        }

        protected override void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            info.AddValue("C", _toReverse, typeof(OrderCriterion<TEntity>));
        }
        #endregion

    }
}

