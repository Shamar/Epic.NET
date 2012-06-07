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
using System.Runtime.Serialization;


namespace Epic.Query.Object
{
    /// <summary>
    /// Base class for custom order criteria.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity of interest.</typeparam>
    /// <typeparam name="TOrderCriterion">Type of the order criterion.</typeparam>
    /// <remarks>
    /// <para>This base class is designed to be derived by sealed class only 
    /// (the constructor will throw an <see cref="EpicException"/> otherwise) so that the 
    /// interpretation of <see cref="OrderCriterionBase{TEntity, TOrderCriterion}.Accept{TResult}"/> 
    /// can be done right here.</para>
    /// </remarks>
    [Serializable]
    public abstract class OrderCriterionBase<TEntity, TOrderCriterion> : OrderCriterion<TEntity>
        where TOrderCriterion : OrderCriterion<TEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderCriterionBase{TEntity, TOrderCriterion}"/> class.
        /// </summary>
        /// <exception cref="EpicException">This class is not of type <typeparamref name="TOrderCriterion"/>.</exception>
        protected OrderCriterionBase ()
            : base()
        {
            if(this.GetType() != typeof(TOrderCriterion))
            {
                string message = string.Format("The order criterion {0} must extend OrderCriterionBase<{1}, {0}> (it extends OrderCriterionBase<{1}, {2}> instead).", this.GetType(), typeof(TEntity), typeof(TOrderCriterion));
                throw new EpicException(message);
            }
        }

        protected override sealed bool SafeEquals (OrderCriterion<TEntity> other)
        {
            TOrderCriterion otherCriterion = other as TOrderCriterion;
            // we don't need to check the cast again, since it's granted from the OrderCriterionBase 
            // constructor and the last OrderCriterion.Equals() check.

            return SafeEquals(otherCriterion);
        }

        protected abstract bool SafeEquals (TOrderCriterion other);

        public sealed override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe<TResult, TOrderCriterion>(this as TOrderCriterion, visitor, context);
        }

        public override sealed OrderCriterion<TEntity> Chain(OrderCriterion<TEntity> other)
        {
            return new OrderCriteria<TEntity>(this, other);
        }

        public override sealed OrderCriterion<TEntity> Reverse()
        {
            return new ReverseOrder<TEntity>(this);
        }

        protected OrderCriterionBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

