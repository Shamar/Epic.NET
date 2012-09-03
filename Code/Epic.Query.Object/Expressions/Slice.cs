//  
//  Slice.cs
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
    /// Slice represents the combination of Take and Skip. 
    /// It bypasses a certain number of <typeparamref name="TEntity"/> and then take another number of them.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    [Serializable]
    public sealed class Slice<TEntity> : Expression<IEnumerable<TEntity>>
        where TEntity : class
    {
        private readonly Order<TEntity> _source;
        private readonly uint _toTake;
        private readonly uint _toSkip;

        /// <summary>
        /// Initializes a new instance of the <see cref="Slice{TEntity}"/> class.
        /// </summary>
        /// <param name='source'>
        /// Ordered set of <typeparamref name="TEntity"/> to slice.
        /// </param>
        /// <param name='toTake'>
        /// Number of <typeparamref name="TEntity"/> to take.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="toTake"/> is <c>0</c>.</exception>
        public Slice (Order<TEntity> source, uint toTake)
            : this(source, 0, toTake)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Slice{TEntity}"/> class.
        /// </summary>
        /// <param name='source'>
        /// Ordered set of <typeparamref name="TEntity"/> to slice.
        /// </param>
        /// <param name='toSkip'>
        /// Number of <typeparamref name="TEntity"/> to bypass.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="toSkip"/> is <see cref="uint.MaxValue"/>.</exception>
        public Slice (uint toSkip, Order<TEntity> source)
            : this(source, toSkip, uint.MaxValue)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Slice{TEntity}"/> class.
        /// </summary>
        /// <param name='source'>
        /// Ordered set of <typeparamref name="TEntity"/> to slice.
        /// </param>
        /// <param name='toTake'>
        /// Number of <typeparamref name="TEntity"/> to take.
        /// </param>
        /// <param name='toSkip'>
        /// Number of <typeparamref name="TEntity"/> to bypass.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="toSkip"/> is <see cref="uint.MaxValue"/>.</exception>
        public Slice (Order<TEntity> source, uint toSkip, uint toTake)
        {
            if (null == source)
                throw new ArgumentNullException ("source");
            if (uint.MaxValue == toSkip)
                throw new ArgumentOutOfRangeException ("toSkip", string.Format ("The number of elements to skip must be lower than {0}.", uint.MaxValue));
            if (0 == toTake)
                throw new ArgumentOutOfRangeException ("toTake", "The number of elements to take must be positive.");
            _source = source;
            _toTake = toTake;
            _toSkip = toSkip;
        }

        /// <summary>
        /// The ordered set of <typeparamref name="TEntity"/> to slice.
        /// </summary>
        public Order<TEntity> Source {
            get { return _source; }
        }

        /// <summary>
        /// Number of <typeparamref name="TEntity"/> to take at most.
        /// </summary>
        public uint TakingAtMost {
            get { return _toTake; }
        }

        /// <summary>
        /// Number of <typeparamref name="TEntity"/> to bypass.
        /// </summary>
        public uint Skipping {
            get { return _toSkip; }
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
            info.AddValue ("S", _source, typeof(Order<TEntity>));
            info.AddValue ("L", _toTake);
            info.AddValue ("O", _toSkip);
        }

        private Slice (SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _source = (Order<TEntity>)info.GetValue ("S", typeof(Order<TEntity>));
            _toTake = info.GetUInt32("L");
            _toSkip = info.GetUInt32("O");
        }
    }

}

