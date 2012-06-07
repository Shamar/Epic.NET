//  
//  OrderCriterion.cs
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
    /// Order criterion for <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity of interest.</typeparam>
    /// <remarks>
    /// <para>The order criteria are used to sort search results.
    /// They are basically visitable comparers that can be either interpreted or evaluated.</para>
    /// <para>This class describe the public API of all order criteria, but can not be derived
    /// directly: you must derive <see cref="OrderCriterionBase{TEntity, TOrderCriterion}"/> that
    /// add some constraints.</para>
    /// </remarks>
    /// <seealso cref="OrderCriterionBase{TEntity, TOrderCriterion}"/>
    [Serializable]
    public abstract class OrderCriterion<TEntity> : VisitableBase, 
                                                    IComparer<TEntity>,
                                                    IEquatable<OrderCriterion<TEntity>>,
                                                    ISerializable
    {
        internal OrderCriterion ()
            : base()
        {
        }

        /// <summary>
        /// Returns a criterion that chain the <paramref name="other"/> order criterion after this.
        /// </summary>
        /// <param name='other'>
        /// Another order criterion.
        /// </param>
        /// <returns>A criterion that chain the <paramref name="other"/> order criterion after this.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c></exception>
        public abstract OrderCriterion<TEntity> Chain(OrderCriterion<TEntity> other);

        /// <summary>
        /// Reverse this criterion.
        /// </summary>
        /// <returns>A criterion that is the reverse of this.</returns>
        public abstract OrderCriterion<TEntity> Reverse();

        #region IComparer implementation

        public abstract int Compare (TEntity x, TEntity y);

        #endregion

        #region IEquatable implementation

        protected abstract bool SafeEquals(OrderCriterion<TEntity> other);

        public bool Equals (OrderCriterion<TEntity> other)
        {
            if(null == other)
                return false;
            if(object.ReferenceEquals(this, other))
                return true;
            if(!this.GetType().Equals(other.GetType()))
                return false;
            return SafeEquals(other);
        }

        public sealed override bool Equals (object obj)
        {
            return Equals (obj as OrderCriterion<TEntity>);
        }

        public sealed override int GetHashCode ()
        {
            return GetType().GetHashCode ();
        }

        #endregion IEquatable implementation

        #region ISerializable implementation

        internal OrderCriterion(SerializationInfo info, StreamingContext context)
            : this()
        {
            if(null == info)
                throw new ArgumentNullException("info");
        }

        protected abstract void GetObjectData (SerializationInfo info, StreamingContext context);

        void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
        {
            if(null == info)
                throw new ArgumentNullException("info");
            GetObjectData(info, context);
        }
        #endregion ISerializable implementation
    }
}

