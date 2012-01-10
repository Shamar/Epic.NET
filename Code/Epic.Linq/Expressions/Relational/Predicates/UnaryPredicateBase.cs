//  
//  UnaryPredicateBase.cs
//  
//  Author:
//       Marco Veglio <m.veglio@gmail.com>
// 
//  Copyright (c) 2010-2011 Giacomo Tesio
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

namespace Epic.Linq.Expressions.Relational.Predicates
{
    /// <summary>
    /// <para>This is the base class for those predicates having one <see cref="Predicate"/> as operand.</para>
    /// <para>Examples are: <see cref="Not"/>.</para>
    /// </summary>
    [Serializable]
    public abstract class UnaryPredicateBase<TPredicate>: Predicate, IEquatable<UnaryPredicateBase<TPredicate>>
        where TPredicate: Predicate
    {
        TPredicate _operand;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Epic.Linq.Expressions.Relational.Predicates.UnaryPredicateBase`1"/> class.
        /// </summary>
        /// <param name='operand'>
        /// The operand of the predicate. Cannot be <see langword="null".
        /// </param>
        /// <exception cref='ArgumentNullException'>
        /// This exception is thrown if any of the operand is <see langword="null"/>
        /// </exception>
        protected UnaryPredicateBase (TPredicate operand)
        {
            if (null == operand) throw new ArgumentNullException("operand");
            this._operand = operand;
        }

        /// <summary>
        /// Gets the operand of the predicate.
        /// </summary>
        /// <value>
        /// The operand.
        /// </value>
        public TPredicate Operand { get { return this._operand; } }

        /// <summary>
        /// Determines whether the specified <see cref="UnaryPredicateBase<TScalar>"/> is equal to the current <see cref="Epic.Linq.Expressions.Relational.Predicates.UnaryPredicateBase`1"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="UnaryPredicateBase<TPredicate>"/> to compare with the current <see cref="Epic.Linq.Expressions.Relational.Predicates.UnaryPredicateBase`1"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="UnaryPredicateBase<TPredicate>"/> is equal to the current
        /// <see cref="Epic.Linq.Expressions.Relational.Predicates.UnaryPredicateBase`1"/>; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool Equals(UnaryPredicateBase<TPredicate> other);

        /// <summary>
        /// Determines whether the specified <see cref="Predicate"/> is equal to the current <see cref="Epic.Linq.Expressions.Relational.Predicates.UnaryPredicateBase`1"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="Predicate"/> to compare with the current <see cref="Epic.Linq.Expressions.Relational.Predicates.UnaryPredicateBase`1"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Predicate"/> is equal to the current
        /// <see cref="Epic.Linq.Expressions.Relational.Predicates.UnaryPredicateBase`1"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(Predicate other)
        {
            return Equals (other as UnaryPredicateBase<TPredicate>);
        }

        /// <summary>
        /// Serves as a hash function for a
        /// <see cref="Epic.Linq.Expressions.Relational.Predicates.UnaryPredicateBase`1"/> object.
        /// </summary>
        /// <returns>
        /// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return GetType().GetHashCode () ^ _operand.GetHashCode ();
        }
    }
}

