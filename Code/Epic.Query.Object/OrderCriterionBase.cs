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

        /// <summary>
        /// Determines whether the specified <see cref="OrderCriterion{TEntity}"/> is equal to the
        /// current <see cref="OrderCriterion{TEntity}"/>, given that <see cref="EqualsA(OrderCriterion{TEntity})"/>
        /// grant that it is not <see langword="null"/>, <see langword="this"/> and that it has the same type of the current instance.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if equals was safed, <c>false</c> otherwise.
        /// </returns>
        /// <param name='other'>
        /// Other.
        /// </param>
        protected override sealed bool EqualsA (OrderCriterion<TEntity> other)
        {
            TOrderCriterion otherCriterion = other as TOrderCriterion;
            // we don't need to check the cast again, since it's granted from the OrderCriterionBase 
            // constructor and the last OrderCriterion.Equals() check.

            return EqualsA(otherCriterion);
        }

        /// <summary>
        /// Determines whether the specified <typeparamref name="TOrderCriterion"/> is equal to the
        /// current <typeparamref name="TOrderCriterion"/>, given that <see cref="EqualsA(OrderCriterion{TEntity})"/>
        /// grant that it is not <see langword="null"/>, <see langword="this"/> and that it has the same type of the current instance.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if the current criterion is equal to the <paramref name="other"/>, <c>false</c> otherwise.
        /// </returns>
        /// <param name='other'>
        /// Another criterion.
        /// </param>
        protected abstract bool EqualsA (TOrderCriterion other);

        /// <summary>
        /// Accept the specified visitor and context as a <typeparamref name="TOrderCriterion"/>.
        /// </summary>
        /// <param name='visitor'>
        /// Visitor.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        /// <typeparam name='TResult'>
        /// Type of the result of the visit.
        /// </typeparam>
        public sealed override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe<TResult, TOrderCriterion>(this as TOrderCriterion, visitor, context);
        }

        /// <summary>
        /// Chain the specified <see cref="OrderCriterion{TEntity}"/> to compare <typeparamref name="TEntity"/>
        /// when they are equivalent according to the current criterion.
        /// </summary>
        /// <param name='other'>
        /// Other order criterion to evaluate after the current one.
        /// </param>
        public override sealed OrderCriterion<TEntity> Chain(OrderCriterion<TEntity> other)
        {
            return new OrderCriteria<TEntity>(this, other);
        }

        /// <summary>
        /// Reverse this order criterion.
        /// </summary>
        /// <returns>
        /// A criterion that is the reverse of this.
        /// </returns>
        /// <seealso cref="ReverseOrder{TEntity}"/>
        public override sealed OrderCriterion<TEntity> Reverse()
        {
            return new ReverseOrder<TEntity>(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderCriterionBase{TEntity, TOrderCriterion}"/> class,
        /// after deserialization.
        /// </summary>
        /// <param name='info'>
        /// Info.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        protected OrderCriterionBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

