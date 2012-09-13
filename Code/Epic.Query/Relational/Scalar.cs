//  
//  Scalar.cs
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

namespace Epic.Query.Relational
{
    /// <summary>
    /// This is the base class for all scalar types.
    /// </summary>
    [Serializable]
    public abstract class Scalar: AlgebraicExpression, IEquatable<Scalar>
    {
        private readonly ScalarType _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Query.Relational.Scalar"/> class.
        /// </summary>
        /// <param name='type'>
        /// The scalar <see cref="ScalarType"/>.
        /// </param>
        internal Scalar (ScalarType type)
        {
            this._type = type;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public ScalarType Type { get { return this._type; } }

        /// <summary>
        /// Serves as a hash function for a <see cref="Epic.Query.Relational.Scalar"/> object.
        /// </summary>
        /// <returns>
        /// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.
        /// </returns>
        public abstract override int GetHashCode ();

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Epic.Query.Relational.Scalar"/>.
        /// </summary>
        /// <remarks>
        /// This method delegate the equality check to the abstract overload <see cref="Equals(Scalar)"/>.
        /// </remarks>
        /// <param name='obj'>
        /// The <see cref="System.Object"/> to compare with the current <see cref="Epic.Query.Relational.Scalar"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="Epic.Query.Relational.Scalar"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (object obj)
        {
            return Equals (obj as Scalar);
        }

        #region IEquatable[Scalar] implementation
        /// <summary>
        /// Determines whether the specified <see cref="Scalar"/> is equal to the current <see cref="Epic.Query.Relational.Scalar"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="Scalar"/> to compare with the current <see cref="Epic.Query.Relational.Scalar"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Scalar"/> is equal to the current
        /// <see cref="Epic.Query.Relational.Scalar"/>; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool Equals (Scalar other);
        #endregion
    }
}

