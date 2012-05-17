//  
//  OrderCriteria.cs
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
using System.Runtime.Serialization;

namespace Epic.Query.Object
{
    [Serializable]
    public sealed class OrderCriteria<TEntity> : OrderCriterion<TEntity>, IEnumerable<OrderCriterion<TEntity>>
    {
        private readonly OrderCriterion<TEntity>[] _criteria;

        internal OrderCriteria (OrderCriterion<TEntity> first, OrderCriterion<TEntity> second)
        {
            if(null == first)
                throw new ArgumentNullException("first");
            if(null == second)
                throw new ArgumentNullException("second");
            OrderCriteria<TEntity> firstChain = first as OrderCriteria<TEntity>;
            OrderCriteria<TEntity> secondChain = second as OrderCriteria<TEntity>;

            List<OrderCriterion<TEntity>> criteria = null;
            if(null == firstChain)
            {
                criteria = new List<OrderCriterion<TEntity>>();
                criteria.Add(first);
            }
            else
            {
                if(firstChain._criteria.Length == 0)
                    throw new ArgumentNullException("first");
                criteria = new List<OrderCriterion<TEntity>>(firstChain._criteria);
            }

            if(null == secondChain)
            {
                criteria.Add(second);
            }
            else
            {
                if(secondChain._criteria.Length == 0)
                    throw new ArgumentNullException("second");
                criteria.AddRange(secondChain._criteria);
            }

            _criteria = criteria.ToArray();
        }

        private OrderCriteria(OrderCriterion<TEntity>[] criteria)
        {
            _criteria = criteria;
        }

        public override OrderCriterion<TEntity> Chain (OrderCriterion<TEntity> other)
        {
            return new OrderCriteria<TEntity>(this, other);
        }

        public override OrderCriterion<TEntity> Reverse ()
        {
            OrderCriterion<TEntity>[] criteria = new OrderCriterion<TEntity>[_criteria.Length];
            int lastIndex = _criteria.Length - 1;
            for(int i = 0; i < _criteria.Length; ++i)
                criteria[lastIndex - i] = _criteria[i];
            return new OrderCriteria<TEntity>(criteria);
        }

        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe(this, visitor, context);
        }

        #region implemented abstract members of Epic.Query.Object.OrderCriterion
        protected override bool SafeEquals (OrderCriterion<TEntity> other)
        {
            OrderCriteria<TEntity> others = other as OrderCriteria<TEntity>;
            if(null == others)
                return false;
            if(_criteria.Length != others._criteria.Length)
                return false;
            for(int i = 0; i < _criteria.Length; ++i)
                if(!_criteria[i].Equals(others._criteria[i]))
                    return false;
            return true;
        }
        #endregion

        #region implemented abstract members of Epic.Query.Object.OrderCriterion
        public override int Compare (TEntity x, TEntity y)
        {
            int comparison = 0;
            for(int i = 0; i < _criteria.Length; ++i)
            {
                comparison = _criteria[i].Compare(x, y);
                if(comparison != 0)
                    break;
            }
            return comparison;
        }
        #endregion

        #region IEnumerable implementation
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return _criteria.GetEnumerator();
        }
        #endregion

        #region IEnumerable implementation
        public IEnumerator<OrderCriterion<TEntity>> GetEnumerator ()
        {
            return (_criteria as IEnumerable<OrderCriterion<TEntity>>).GetEnumerator();
        }
        #endregion

        #region implemented abstract members of Epic.Query.Object.OrderCriterion

        private OrderCriteria(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _criteria = (OrderCriterion<TEntity>[])info.GetValue("C", typeof(OrderCriterion<TEntity>[]));
        }

        protected override void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            info.AddValue("C", _criteria, typeof(OrderCriterion<TEntity>[]));
        }
        #endregion
    }
}

