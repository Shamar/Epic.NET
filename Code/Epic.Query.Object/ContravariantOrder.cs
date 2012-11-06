//
//  ContravariantOrder.cs
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
    /// Contravariant order criterion.
    /// </summary>
    [Serializable]
    public sealed class ContravariantOrder<TEntity, TSpecializedEntity> : OrderCriterion<TSpecializedEntity> 
        where TEntity : class
        where TSpecializedEntity : class, TEntity
    {
        private readonly OrderCriterion<TEntity> _inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContravariantOrder{TEntity, TSpecializedEntity}"/> class
        /// that apply the <paramref name="innerOrderCriterion"/> to <typeparamref name="TSpecializedEntity"/>.
        /// </summary>
        /// <param name='innerOrderCriterion'>
        /// Inner order criterion.
        /// </param>
        public ContravariantOrder (OrderCriterion<TEntity> innerOrderCriterion)
        {
            if(null == innerOrderCriterion)
                throw new ArgumentNullException("innerOrderCriterion");
            _inner = innerOrderCriterion; 
        }

        #region implemented abstract members of VisitableBase

        /// <summary>
        /// Accept the specified visitor and context.
        /// </summary>
        /// <param name='visitor'>
        /// Visitor.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        /// <returns>The result of the visit.</returns>
        /// <typeparam name='TResult'>
        /// The type of the visit's result.
        /// </typeparam>
        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return _inner.Accept(visitor, context);
        }

        #endregion

        #region implemented abstract members of OrderCriterion

        /// <summary>
        /// Returns the current OrderCriterion wrapped to handle any <typeparamref name="TSpecializedEntity"/>.
        /// </summary>
        /// <returns>The current OrderCriterion wrapped to handle any <typeparamref name="TSpecializedEntity"/>.</returns>
        /// <typeparam name='TMoreSpecializedEntity'>
        /// Type of the entities to order.
        /// </typeparam>
        public override OrderCriterion<TMoreSpecializedEntity> For<TMoreSpecializedEntity> ()
        {
            return _inner.For<TMoreSpecializedEntity>();
        }

        /// <summary>
        /// Chain the specified criterion after the current one.
        /// </summary>
        /// <param name='other'>
        /// Another criterion.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        /// <returns>A new set of <see cref="OrderCriteria{TEntity}"/> that evaluates
        /// the <paramref name="other"/> criterion after the current one.</returns>
        public override OrderCriterion<TSpecializedEntity> Chain (OrderCriterion<TSpecializedEntity> other)
        {
            return new OrderCriteria<TSpecializedEntity>(this, other);
        }

        /// <summary>
        /// Reverse this order criterion.
        /// </summary>
        /// <returns>
        /// A criterion that is the reverse of this.
        /// </returns>
        /// <seealso cref="ReverseOrder{TEntity}"/>
        public override OrderCriterion<TSpecializedEntity> Reverse ()
        {
            return new ReverseOrder<TSpecializedEntity>(this);
        }

        /// <summary>
        /// Compares two <typeparamref name="TSpecializedEntity"/> and returns a value indicating 
        /// whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name='x'>
        /// The first <typeparamref name="TSpecializedEntity"/>.
        /// </param>
        /// <param name='y'>
        /// The second <typeparamref name="TSpecializedEntity"/>.
        /// </param>
        /// <returns><c>0</c> if <paramref name="x"/> and <paramref name="y"/> are equivalent
        /// for the current order criterion, a positive number if 
        /// <paramref name="x"/> goes after <paramref name="y"/> and a negative number 
        /// if <paramref name="x"/> goes before <paramref name="y"/>.</returns>
        /// <remarks>The comparison is delegated to the wrapped <see cref="OrderCriterion{TEntity}"/>
        /// that was provided in the constructor.</remarks>
        /// <exception cref="InvalidOperationException"><paramref name="x"/> and <paramref name="y"/> cannot be sorted by the current criterion.</exception>
        public override int Compare (TSpecializedEntity x, TSpecializedEntity y)
        {
            return _inner.Compare(x, y);
        }

        /// <summary>
        /// Determines whether the specified <see cref="OrderCriterion{TEntity}"/> is a <see cref="ContravariantOrder{TEntity, TSpecializedEntity}"/>
        /// that wraps the same <see cref="OrderCriterion{TEntity}"/> as the current instance.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if the current criterion and <paramref name="other"/> are equivalent, <c>false</c> otherwise.
        /// </returns>
        /// <param name='other'>
        /// Other.
        /// </param>
        protected override bool EqualsA (OrderCriterion<TSpecializedEntity> other)
        {
            ContravariantOrder<TEntity, TSpecializedEntity> otherContravariant = other as ContravariantOrder<TEntity, TSpecializedEntity>;
            return _inner.Equals(otherContravariant._inner);
        }

        private ContravariantOrder(SerializationInfo info, StreamingContext context)
        {
            _inner = (OrderCriterion<TEntity>)info.GetValue("I", typeof(OrderCriterion<TEntity>));
        }

        /// <summary>
        /// Gets the object data to be serialized.
        /// </summary>
        /// <param name='info'>
        /// Info.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        protected override void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            info.AddValue("I", _inner, typeof(OrderCriterion<TEntity>));
        }

        #endregion
    }
}

