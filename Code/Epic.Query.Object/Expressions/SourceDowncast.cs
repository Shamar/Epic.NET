//  
//  SourceCast.cs
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
using Epic.Specifications;

namespace Epic.Query.Object.Expressions
{
    /// <summary>
    /// Represent a downcast of an <see cref="IEnumerable{TAbstraction}"/> to an <see cref="IEnumerable{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TAbstraction">Type of the abstraction to be downcasted.</typeparam>
    /// <typeparam name="TEntity">Type of the entity to be obtained.</typeparam>
    [Serializable]
    public sealed class SourceDowncast<TAbstraction, TEntity> : Expression<IEnumerable<TEntity>>
        where TAbstraction : class
        where TEntity : class, TAbstraction
    {
        private readonly Expression<IEnumerable<TAbstraction>> _source;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceDowncast{TAbstraction, TEntity}"/> class.
        /// </summary>
        /// <param name='source'>
        /// Source to downcast.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        public SourceDowncast (Expression<IEnumerable<TAbstraction>> source)
        {
            if (null == source)
                throw new ArgumentNullException ("source");
            _source = source;
        }

        /// <summary>
        /// The original source.
        /// </summary>
        public Expression<IEnumerable<TAbstraction>> Source {
            get { return _source; }
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
            info.AddValue (
                "R",
                _source,
                typeof(Expression<IEnumerable<TAbstraction>>)
            );
        }

        private SourceDowncast (SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _source = (Expression<IEnumerable<TAbstraction>>)info.GetValue (
                "R",
                typeof(Expression<IEnumerable<TAbstraction>>)
            );
        }
    }
}

