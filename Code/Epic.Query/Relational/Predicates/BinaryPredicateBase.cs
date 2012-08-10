//  
//  BinaryPredicateBase.cs
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

namespace Epic.Query.Relational.Predicates
{
    /// <summary>
    /// <para>This is the base class for those predicates having two <see cref="Predicate"/> as operands.</para>
    /// <para>Examples are: <see cref="And"/>, <see cref="Or"/>.</para>
     /// </summary>
    [Serializable]
    public abstract class BinaryPredicateBase : Predicate, IEquatable<BinaryPredicateBase>
    {
        private readonly Predicate _left;
        private readonly Predicate _right;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="BinaryPredicateBase"/> class.
        /// </summary>
        /// <param name='leftOperand'>
        /// Left operand of the predicate. Cannot be <see langword="null"/>.
        /// </param>
        /// <param name='rightOperand'>
        /// Right operand of the predicate. Cannot be <see langword="null"/>.
        /// </param>
        /// <exception cref='ArgumentNullException'>
        /// This exception is thrown if any of the operand is <see langword="null"/>
        /// </exception>
        protected BinaryPredicateBase (Predicate leftOperand, Predicate rightOperand)
        {
            if (null == leftOperand) throw new ArgumentNullException("leftOperand");
            if (null == rightOperand) throw new ArgumentNullException("rightOperand");
            this._left = leftOperand;
            this._right = rightOperand;
        }

        /// <summary>
        /// Gets the left operand of the predicate.
        /// </summary>
        /// <value>
        /// The left operand.
        /// </value>
        public Predicate Left { get { return this._left; } }

        /// <summary>
        /// Gets the right operand of the predicate.
        /// </summary>
        /// <value>
        /// The right operand.
        /// </value>
        public Predicate Right { get { return this._right; } }

        /// <summary>
        /// Determines whether the specified <see cref="BinaryPredicateBase"/> is equal to the current <see cref="BinaryPredicateBase"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="BinaryPredicateBase"/> to compare with the current <see cref="BinaryPredicateBase"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="BinaryPredicateBase"/> is equal to the
        /// current <see cref="BinaryPredicateBase"/>; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool Equals(BinaryPredicateBase other);

        /// <summary>
        /// Determines whether the specified <see cref="Predicate"/> is equal to the current <see cref="BinaryPredicateBase"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="Predicate"/> to compare with the current <see cref="BinaryPredicateBase"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Predicate"/> is equal to the current
        /// <see cref="BinaryPredicateBase"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (Predicate other)
        {
            return Equals(other as BinaryPredicateBase);
        }

        /// <summary>
        /// Serves as a hash function for a
        /// <see cref="BinaryPredicateBase"/> object.
        /// </summary>
        /// <returns>
        /// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.
        /// </returns>
        public override int GetHashCode ()
        {
            return _left.GetHashCode() ^ _right.GetHashCode () ^ GetType ().GetHashCode ();
        }

    }
}

