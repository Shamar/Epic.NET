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
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/></exception>
        public abstract OrderCriterion<TEntity> Chain(OrderCriterion<TEntity> other);

        /// <summary>
        /// Reverse this criterion.
        /// </summary>
        /// <returns>A criterion that is the reverse of this.</returns>
        public abstract OrderCriterion<TEntity> Reverse();

        #region IComparer implementation

        /// <summary>
        /// Compare the specified x and y.
        /// </summary>
        /// <param name='x'>
        /// The x entity.
        /// </param>
        /// <param name='y'>
        /// The y entity.
        /// </param>
        public abstract int Compare (TEntity x, TEntity y);

        #endregion

        #region IEquatable implementation

        /// <summary>
        /// Determines whether the specified <see cref="OrderCriterion{TEntity}"/> is equal to the
        /// current <see cref="OrderCriterion{TEntity}"/>, given that <see cref="Equals(OrderCriterion{TEntity})"/>
        /// grant that it is not <see langword="null"/>, <see langword="this"/> and that it has the same type of the current instance.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if equals was safed, <c>false</c> otherwise.
        /// </returns>
        /// <param name='other'>
        /// Other.
        /// </param>
        protected abstract bool EqualsA(OrderCriterion<TEntity> other);

        /// <summary>
        /// Determines whether the specified <see cref="OrderCriterion{TEntity}"/> is equal to the
        /// current <see cref="OrderCriterion{TEntity}"/>.
        /// </summary>
        /// <remarks>
        /// This is a template method thet test <paramref name="other"/> for reference equality with <see langword="null"/>
        /// and <see langword="this"/> and than for type equality (<see cref="object.GetType()"/>).
        /// If both tests fails, it delegates to the abstract method <see cref="EqualsA"/> the equality determination.
        /// </remarks>
        /// <param name='other'>
        /// The <see cref="OrderCriterion{TEntity}"/> to compare with the current <see cref="OrderCriterion{TEntity}"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="OrderCriterion{TEntity}"/> is equal to the current
        /// <see cref="OrderCriterion{TEntity}"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals (OrderCriterion<TEntity> other)
        {
            if(null == other)
                return false;
            if(object.ReferenceEquals(this, other))
                return true;
            if(!this.GetType().Equals(other.GetType()))
                return false;
            return EqualsA(other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current 
        /// <see cref="OrderCriterion{TCandidate}"/>.
        /// </summary>
        /// <remarks>
        /// Delegates the evaluation to <see cref="OrderCriterion{TEntity}.Equals(OrderCriterion{TEntity})"/>.
        /// </remarks>            
        /// <param name='obj'>
        /// The <see cref="System.Object"/> to compare with the current <see cref="OrderCriterion{TEntity}"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="OrderCriterion{TEntity}"/>; otherwise, <c>false</c>.
        /// </returns>
        public sealed override bool Equals (object obj)
        {
            return Equals (obj as OrderCriterion<TEntity>);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="OrderCriterion{TCandidate}"/> object.
        /// </summary>
        /// <returns>
        /// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.
        /// </returns>
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

        /// <summary>
        /// Gets the object data to be serialized.
        /// </summary>
        /// <param name='info'>
        /// Info.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
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

