//  
//  ScalarFunction.cs
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
    /// Models a function which has a <see cref="Scalar"/> as output.
    /// </summary>
    /// <exception cref='ArgumentNullException'>
    /// Is thrown when the argument null exception.
    /// </exception>
    [Serializable]
    public abstract class ScalarFunction: Scalar, IEquatable<ScalarFunction>
    {
        private readonly string _name;
        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Query.Relational.ScalarFunction"/> class.
        /// </summary>
        /// <param name='name'>
        /// Name.
        /// </param>
        /// <exception cref='ArgumentNullException'>
        /// Is thrown when the function name is null.
        /// </exception>
        public ScalarFunction (string name): base(ScalarType.Function)
        {
            if (string.IsNullOrEmpty (name))
                throw new ArgumentNullException("name");
            this._name = name;
        }

        /// <summary>
        /// Gets the function name.
        /// </summary>
        public string Name { get { return this._name; } }

        /// <summary>
        /// Serves as a hash function for a <see cref="Epic.Query.Relational.ScalarFunction"/> object.
        /// </summary>
        /// <returns>
        /// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.
        /// </returns>
        public abstract override int GetHashCode ();

        #region implemented abstract members of Epic.Linq.Expressions.Relational.Scalar
        /// <summary>
        /// Determines whether the specified <see cref="Scalar"/> is equal to the current <see cref="Epic.Query.Relational.ScalarFunction"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="Scalar"/> to compare with the current <see cref="Epic.Query.Relational.ScalarFunction"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Scalar"/> is equal to the current
        /// <see cref="Epic.Query.Relational.ScalarFunction"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (Scalar other)
        {
            return Equals (other as ScalarFunction);
        }
        #endregion

        #region IEquatable[ScalarFunction] implementation
        /// <summary>
        /// Determines whether the specified <see cref="ScalarFunction"/> is equal to the current <see cref="Epic.Query.Relational.ScalarFunction"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="ScalarFunction"/> to compare with the current <see cref="Epic.Query.Relational.ScalarFunction"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="ScalarFunction"/> is equal to the current
        /// <see cref="Epic.Query.Relational.ScalarFunction"/>; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool Equals (ScalarFunction other);
        #endregion
    }
}

