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
    /// <summary>
    /// Set of criteria to sort an <see cref="IEnumerable{TEntity}"/>. 
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity that can be sorted by these criteria.</typeparam>
    [Serializable]
    public sealed class OrderCriteria<TEntity> : OrderCriterion<TEntity>, IEnumerable<OrderCriterion<TEntity>>
    {
        private readonly OrderCriterion<TEntity>[] _criteria;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderCriteria{TEntity}"/> class.
        /// </summary>
        /// <param name='first'>
        /// First criterion to apply.
        /// </param>
        /// <param name='second'>
        /// Second criterion to apply.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is <see langword="null"/>.</exception>
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
                criteria = new List<OrderCriterion<TEntity>>(firstChain._criteria);
            }

            if(null == secondChain)
            {
                criteria.Add(second);
            }
            else
            {
                criteria.AddRange(secondChain._criteria);
            }

            _criteria = criteria.ToArray();
        }

        private OrderCriteria(OrderCriterion<TEntity>[] criteria)
        {
            // This constructor must remain private.
            _criteria = criteria;
        }

        /// <summary>
        /// Chain the specified criterion after the current chain.
        /// </summary>
        /// <remarks>
        /// If <paramref name="other"/> is a set of <see cref="OrderCriteria{TEntity}"/>, 
        /// the contained criteria are merged after those in the current instance.
        /// </remarks>
        /// <param name='other'>
        /// Another order criterion.
        /// </param>
        public override OrderCriterion<TEntity> Chain (OrderCriterion<TEntity> other)
        {
            return new OrderCriteria<TEntity>(this, other);
        }

        /// <summary>
        /// Reverse this criterion producing a new set of <see cref="OrderCriteria{TEntity}"/>.
        /// </summary>
        /// <returns>
        /// A set of <see cref="OrderCriteria{TEntity}"/> that is the reverse of this.
        /// </returns>
        public override OrderCriterion<TEntity> Reverse ()
        {
            OrderCriterion<TEntity>[] criteria = new OrderCriterion<TEntity>[_criteria.Length];
            int lastIndex = _criteria.Length - 1;
            for(int i = 0; i < _criteria.Length; ++i)
                criteria[lastIndex - i] = _criteria[i];
            return new OrderCriteria<TEntity>(criteria);
        }

        /// <summary>
        /// Accept the specified visitor and context.
        /// </summary>
        /// <param name='visitor'>
        /// Visitor.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        /// <typeparam name='TResult'>
        /// The type of the visit's result.
        /// </typeparam>
        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe(this, visitor, context);
        }

        #region implemented abstract members of Epic.Query.Object.OrderCriterion
        /// <summary>
        /// Determines whether the specified <see cref="OrderCriteria{TEntity}"/> is equal to the
        /// current <see cref="OrderCriteria{TEntity}"/>, given that <see cref="EqualsA(OrderCriterion{TEntity})"/>
        /// grant that it is not <see langword="null"/>, <see langword="this"/> and that it has the same type of the current instance.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if the current criterion is equal to the <paramref name="other"/>, <c>false</c> otherwise.
        /// </returns>
        /// <param name='other'>
        /// Another criterion.
        /// </param>
        protected override bool EqualsA (OrderCriterion<TEntity> other)
        {
            OrderCriteria<TEntity> others = other as OrderCriteria<TEntity>;
            if(_criteria.Length != others._criteria.Length)
                return false;
            for(int i = 0; i < _criteria.Length; ++i)
                if(!_criteria[i].Equals(others._criteria[i]))
                    return false;
            return true;
        }
        #endregion

        #region implemented abstract members of Epic.Query.Object.OrderCriterion
        /// <summary>
        /// Compare the specified entities.
        /// </summary>
        /// <param name='x'>
        /// The first entity.
        /// </param>
        /// <param name='y'>
        /// The second entity.
        /// </param>
        /// <remarks>
        /// The comparison is delegated to the criteria in the set in the order thay have been chained. 
        /// The first order criterion that returns a non-zero result stop the chain.
        /// </remarks>
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
        /// <summary>
        /// Gets the enumerator of contained criteria.
        /// </summary>
        /// <returns>
        /// The enumerator of <see cref="OrderCriterion{TEntity}"/>.
        /// </returns>
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

        /// <summary>
        /// Gets the object data to serialize.
        /// </summary>
        /// <param name='info'>
        /// Info.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        protected override void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            info.AddValue("C", _criteria, typeof(OrderCriterion<TEntity>[]));
        }
        #endregion
    }
}

