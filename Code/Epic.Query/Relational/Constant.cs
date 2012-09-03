//
//  Constant.cs
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

namespace Epic.Query.Relational
{
    /// <summary>
    /// Constant value in a relational predicate.
    /// </summary>
    /// <remarks>This abstract class cannot be inherited on its own: you must derive <see cref="Constant{TValue}"/>.</remarks>
    [Serializable]
    public abstract class Constant : Scalar, IEquatable<Constant>
    {
        internal Constant(): base(ScalarType.Constant)
        {
        }

        #region IEquatable[Constant] implementation
        /// <summary>
        /// Determines whether the specified <see cref="Epic.Query.Relational.Constant"/> is equal to the current <see cref="Epic.Query.Relational.Constant"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="Epic.Query.Relational.Constant"/> to compare with the current <see cref="Epic.Query.Relational.Constant"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Epic.Query.Relational.Constant"/> is equal to the current
        /// <see cref="Epic.Query.Relational.Constant"/>; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool Equals (Constant other);
        #endregion

        /// <summary>
        /// Determines whether the specified <see cref="Epic.Query.Relational.Scalar"/> is equal to the current <see cref="Epic.Query.Relational.Constant"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="Epic.Query.Relational.Scalar"/> to compare with the current <see cref="Epic.Query.Relational.Constant"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Epic.Query.Relational.Scalar"/> is equal to the current
        /// <see cref="Epic.Query.Relational.Constant"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (Scalar other)
        {
            return Equals (other as Constant);
        }
    }
    
    /// <summary>
    /// Constant value in a relational predicate.
    /// </summary>
    /// <typeparam name="TValue">Type of the value of the constant.</typeparam>
    [Serializable]
    public sealed class Constant<TValue> : Constant, IEquatable<Constant<TValue>>
    {
        private readonly TValue _value;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Query.Relational.Constant{TValue}"/> class.
        /// </summary>
        /// <param name='value'>
        /// Value of the constant.
        /// </param>
        public Constant (TValue value)
        {
            _value = value;
        }
  
        /// <summary>
        /// Gets the value of the constant.
        /// </summary>
        public TValue Value
        {
            get
            {
                return _value;
            }
        }
        
        #region implemented abstract members of Epic.Linq.Expressions.VisitableBase
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
        /// Type of the result produced from the visitor.
        /// </typeparam>
        /// <returns>Result of the visit.</returns>
        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe(this, visitor, context);
        }
        #endregion

        #region IEquatable[Constant[TValue]] implementation
        /// <summary>
        /// Determines whether the specified <see cref="Constant{TValue}"/> is equal to the
        /// current <see cref="Constant{TValue}"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="EqualityComparer{TValue}.Default"/> is used to test the <see cref="Constant{TValue}.Value"/> for equality.
        /// </remarks>            
        /// <param name='other'>
        /// The <see cref="Constant{TValue}"/> to compare with the current <see cref="Constant{TValue}"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Constant{TValue}"/> express the same value of the current
        /// <see cref="Constant{TValue}"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals (Constant<TValue> other)
        {
            if(null == other)
                return false;
            return EqualityComparer<TValue>.Default.Equals(_value, other._value);
        }
        #endregion
        
        #region IEquatable[Constant] implementation
        /// <summary>
        /// Determines whether the specified <see cref="Constant"/> express
        /// the same value of the current <see cref="Constant{TValue}"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="Epic.Query.Relational.Constant"/> to compare with the current <see cref="Constant{TValue}"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Constant"/> is equal to the current
        /// <see cref="Constant{TValue}"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (Constant other)
        {
            return Equals(other as Constant<TValue>);
        }
        #endregion

        /// <summary>
        /// Serves as a hash function for a <see cref="Constant{TValue}"/> object.
        /// </summary>
        /// <returns>
        /// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.
        /// </returns>
        public override int GetHashCode ()
        {
            if(null == (object)_value)
                return 0;
            return _value.GetHashCode();
        }
    }
}

