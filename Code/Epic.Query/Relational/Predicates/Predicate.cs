//  
//  Predicate.cs
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
    /// Base class for Predicates.
    /// </summary>
    [Serializable]
    public abstract class Predicate : AlgebraicExpression, IEquatable<Predicate>
    {
        #region IEquatable[Predicate] implementation
        /// <summary>
        /// Determines whether the specified <see cref="Predicate"/> is equal to the current <see cref="Predicate"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="Predicate"/> to compare with the current <see cref="Predicate"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Predicate"/> is equal to the current
        /// <see cref="Predicate"/>; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool Equals (Predicate other);
        
        #endregion
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Predicate"/>.
        /// </summary>
        /// <param name='obj'>
        /// The <see cref="System.Object"/> to compare with the current <see cref="Predicate"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="Predicate"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (object obj)
        {
            return Equals (obj as Predicate);
        }
        /// <summary>
        /// Serves as a hash function for a <see cref="Predicate"/> object.
        /// </summary>
        /// <returns>
        /// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.
        /// </returns>
        public override abstract int GetHashCode ();
    }
}