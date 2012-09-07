//  
//  Selection.cs
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
using Epic.Specifications;
using System.Collections.Generic;


namespace Epic.Query.Object.Expressions
{
    /// <summary>
    /// Represent the selection of those <typeparamref name="TEntity"/> that satisfy a specification.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    [Serializable]
    public sealed class Selection<TEntity> : Expression<IEnumerable<TEntity>>
        where TEntity : class
    {
        private readonly Expression<IEnumerable<TEntity>> _source;
        private readonly ISpecification<TEntity> _specification; 

        /// <summary>
        /// Initializes a new instance of the <see cref="Selection{TEntity}"/>.
        /// </summary>
        /// <param name='source'>
        /// Source of <typeparamref name="TEntity"/>.
        /// </param>
        /// <param name='specification'>
        /// Specification to satisfy.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="specification"/> is <see langword="null"/>.</exception>
        public Selection (Expression<IEnumerable<TEntity>> source, ISpecification<TEntity> specification)
        {
            if (null == source)
                throw new ArgumentNullException ("source");
            if (null == specification)
                throw new ArgumentNullException ("specification");
            _source = source;
            _specification = specification;
        }

        /// <summary>
        /// The source of <typeparamref name="TEntity"/>.
        /// </summary>
        public Expression<IEnumerable<TEntity>> Source {
            get { return _source; }
        }

        /// <summary>
        /// The specification to satisfy.
        /// </summary>
        public ISpecification<TEntity> Specification {
            get { return _specification;}
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
            info.AddValue ("S", _specification, typeof(ISpecification<TEntity>));
        }

        private Selection (SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _source = (Expression<IEnumerable<TEntity>>)info.GetValue ("R", typeof(Expression<IEnumerable<TEntity>>));
            _specification = (ISpecification<TEntity>)info.GetValue ("S", typeof(ISpecification<TEntity>));
        }
    }
}

