//  
//  PredicateExtension.cs
//  
//  Author:
//       Marco Veglio <m.veglio@gmail.com>
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
    /// Extension methods for <see cref="Predicate"/> (and its specializations).
    /// </summary>
    public static class PredicateExtension
    {
        /// <summary>
        /// Negate the specified predicate.
        /// </summary>
        /// <param name='predicate'>
        /// Predicate.
        /// </param>
        /// <returns>A negation of <paramref name="predicate"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/></exception>
        public static Not Not(this Predicate predicate)
        {
            return new Not(predicate);
        }

        /// <summary>
        /// Conjunges logically the specified predicate and other.
        /// </summary>
        /// <param name='predicate'>
        /// Predicate.
        /// </param>
        /// <param name='other'>
        /// Another predicate.
        /// </param>
        /// <returns>A conjunction between <paramref name="predicate"/> and <paramref name="other"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> or <paramref name="other"/> is <see langword="null"/> </exception>
        public static And And(this Predicate predicate, Predicate other)
        {
            return new And(predicate, other);
        }

        /// <summary>
        /// Disjunge logically the specified predicate and other.
        /// </summary>
        /// <param name='predicate'>
        /// Predicate.
        /// </param>
        /// <param name='other'>
        /// Other.
        /// </param>
        /// <returns>A disjunction between <paramref name="predicate"/> and <paramref name="other"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> or <paramref name="other"/> is <see langword="null"/> </exception>
        public static Or Or(this Predicate predicate, Predicate other)
        {
            return new Or(predicate, other);
        }

        /// <summary>
        /// Produce an <see cref="Equal"/> predicate for the specified scalars.
        /// </summary>
        /// <param name='scalar'>
        /// A scalar.
        /// </param>
        /// <param name='other'>
        /// Another scalar.
        /// </param>
        /// <returns>An <see cref="Equal"/> predicate for the specified scalar.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="scalar"/> or <paramref name="other"/> is <see langword="null"/> </exception>
        public static Equal Equal(this Scalar scalar, Scalar other)
        {
            return new Equal(scalar, other);
        }

        /// <summary>
        /// Produce an <see cref="Greater"/> predicate for the specified scalar and other.
        /// </summary>
        /// <param name='scalar'>
        /// Scalar.
        /// </param>
        /// <param name='other'>
        /// Another scalar.
        /// </param>
        /// <returns>A <see cref="Greater"/> predicate for the specified scalar.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="scalar"/> or <paramref name="other"/> is <see langword="null"/> </exception>
        public static Greater Greater(this Scalar scalar, Scalar other)
        {
            return new Greater(scalar, other);
        }

        /// <summary>
        /// Produce an <see cref="Less"/> predicate for the specified scalar and other.
        /// </summary>
        /// <param name='scalar'>
        /// Scalar.
        /// </param>
        /// <param name='other'>
        /// Other.
        /// </param>
        /// <returns>A <see cref="Less"/> predicate for the specified scalar.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="scalar"/> or <paramref name="other"/> is <see langword="null"/> </exception>
        public static Less Less(this Scalar scalar, Scalar other)
        {
            return new Less(scalar, other);
        }

    }
}

