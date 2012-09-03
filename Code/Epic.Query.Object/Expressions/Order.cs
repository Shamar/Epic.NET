//  
//  Order.cs
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
using System.Collections.Generic;

namespace Epic.Query.Object.Expressions
{
    /// <summary>
    /// Represent the application of an ordering condition to an <see cref="IEnumerable{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    [Serializable]
    public sealed class Order<TEntity> : Expression<IEnumerable<TEntity>>
        where TEntity : class
    {
        private readonly Expression<IEnumerable<TEntity>> _source;
        private readonly OrderCriterion<TEntity> _criterion; 

        /// <summary>
        /// Initializes a new instance of the <see cref="Order{TEntity}"/> class.
        /// </summary>
        /// <param name='source'>
        /// <see cref="IEnumerable{TEntity}"/> to sort.
        /// </param>
        /// <param name='criterion'>
        /// Order criterion.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="criterion"/> is <see langword="null"/>.</exception>
        public Order (Expression<IEnumerable<TEntity>> source, OrderCriterion<TEntity> criterion)
        {
            if (null == source)
                throw new ArgumentNullException ("source");
            if (null == criterion)
                throw new ArgumentNullException ("criterion");
            _source = source;
            _criterion = criterion;
        }

        /// <summary>
        /// Provide a new <see cref="Order{TEntity}"/> that represents the application of 
        /// the current order criterion and then of <paramref name="criterion"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="Order{TEntity}"/>.
        /// </returns>
        /// <param name='criterion'>
        /// The order criterion to apply after the current one.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="criterion"/> is <see langword="null"/>.</exception>
        public Order<TEntity> ThanBy(OrderCriterion<TEntity> criterion)
        {
            if(null == criterion)
                throw new ArgumentNullException("criterion");
            return new Order<TEntity>(_source, _criterion.Chain(criterion));
        }
        
        /// <summary>
        /// The <see cref="IEnumerable{TEntity}"/> to sort.
        /// </summary>
        public Expression<IEnumerable<TEntity>> Source {
            get { return _source; }
        }

        /// <summary>
        /// The order criterion.
        /// </summary>
        public OrderCriterion<TEntity> Criterion {
            get { return _criterion; }
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
        /// <returns>Result of the visit.</returns>
        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe (this, visitor, context);
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
            info.AddValue ("R", _source, typeof(Expression<IEnumerable<TEntity>>));
            info.AddValue ("S", _criterion, typeof(OrderCriterion<TEntity>));
        }

        private Order (SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _source = (Expression<IEnumerable<TEntity>>)info.GetValue ("R", typeof(Expression<IEnumerable<TEntity>>));
            _criterion = (OrderCriterion<TEntity>)info.GetValue ("S", typeof(OrderCriterion<TEntity>));
        }
    }
}

