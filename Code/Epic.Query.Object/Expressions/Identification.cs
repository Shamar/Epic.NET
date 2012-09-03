//  
//  Identification.cs
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

namespace Epic.Query.Object.Expressions
{
    /// <summary>
    /// Represent the map between <typeparamref name="TEntity"/> and <typeparamref name="TIdentity"/> over
    /// an <see cref="IEnumerable{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    /// <typeparam name="TIdentity">Type of the identity.</typeparam>
    [Serializable]
    public sealed class Identification<TEntity, TIdentity> : Expression<IEnumerable<TIdentity>>
        where TEntity : class
        where TIdentity : IEquatable<TIdentity>
    {
        private readonly Expression<IEnumerable<TEntity>> _entities;

        /// <summary>
        /// Initializes a new instance of the <see cref="Identification{TEntity, TIdentity}"/> class.
        /// </summary>
        /// <param name='entities'>
        /// Entities to map.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="entities"/> is <see langword="null"/>.</exception>
        public Identification (Expression<IEnumerable<TEntity>> entities)
        {
            if (null == entities)
                throw new ArgumentNullException ("entities");
            _entities = entities;
        }

        /// <summary>
        /// The entities that will be mapped to their identities.
        /// </summary>
        public Expression<IEnumerable<TEntity>> Entities {
            get {
                return _entities;
            }
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
            info.AddValue ("E", _entities, typeof(Expression<IEnumerable<TEntity>>));
        }

        private Identification (SerializationInfo info, StreamingContext context)
                : base(info, context)
        {
            _entities = (Expression<IEnumerable<TEntity>>)info.GetValue ("E", typeof(Expression<IEnumerable<TEntity>>));
        }
    }
}

