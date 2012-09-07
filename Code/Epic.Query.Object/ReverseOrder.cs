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
    /// <summary>
    /// Reppresent a reversed order criterion.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity of interest.</typeparam>
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

        /// <summary>
        /// The <see cref="OrderCriterion{TEntity}"/> that has been reversed.
        /// </summary>
        /// <value>
        /// The reversed.
        /// </value>
        public OrderCriterion<TEntity> Reversed
        {
            get
            {
                return _toReverse;
            }
        }

        /// <summary>
        /// Chain the specified criterion after the current one.
        /// </summary>
        /// <param name='other'>
        /// Another criterion.
        /// </param>
        public override OrderCriterion<TEntity> Chain (OrderCriterion<TEntity> other)
        {
            return new OrderCriteria<TEntity>(this, other);
        }

        /// <summary>
        /// Reverse this criterion.
        /// </summary>
        /// <returns>
        /// <see cref="Reversed"/> since it's the reverse of the current criterion.
        /// </returns>
        public override OrderCriterion<TEntity> Reverse ()
        {
            return _toReverse;
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
        /// This calls the <see cref="Reversed"/>'s <c>Compare</c> 
        /// method inverting the arguments (<c>Reversed.Compare(y, x)</c>).
        /// </remarks>
        public override int Compare (TEntity x, TEntity y)
        {
            return _toReverse.Compare(y, x);
        }

        /// <summary>
        /// Determines whether the specified <see cref="OrderCriterion{TEntity}"/> is equal to the
        /// current <see cref="ReverseOrder{TEntity}"/>, given that <see cref="EqualsA(OrderCriterion{TEntity})"/>
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
            ReverseOrder<TEntity> reverseOther = other as ReverseOrder<TEntity>;
            return _toReverse.Equals(reverseOther._toReverse);
        }

        #region implemented abstract members of Epic.Query.Object.OrderCriterion
        private ReverseOrder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _toReverse = (OrderCriterion<TEntity>)info.GetValue("C", typeof(OrderCriterion<TEntity>));
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
            info.AddValue("C", _toReverse, typeof(OrderCriterion<TEntity>));
        }
        #endregion

    }
}

