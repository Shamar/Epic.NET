//  
//  RelationalExpression.cs
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

namespace Epic.Query.Relational
{
    /// <summary>
    /// Relation.
    /// </summary>
    /// <exception cref='ArgumentNullException'>
    /// Is thrown when an argument passed to a method is invalid because it is <see langword="null" /> .
    /// </exception>
    [Serializable]
    public abstract class RelationalExpression : VisitableBase, IEquatable<RelationalExpression>
    {
        private readonly RelationType _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Linq.Expressions.Relational.Relation"/> class.
        /// </summary>
        /// <param name='type'>
        /// Type of the relation.
        /// </param>
        /// <param name='name'>
        /// Name of the relation.
        /// </param>
        /// <exception cref='ArgumentNullException'>
        /// Is thrown when <paramref name="name"/> is <see langword="null" /> or empty.
        /// </exception>
        protected RelationalExpression (RelationType type)
        {
            _type = type;
        }

        public RelationType Type { get { return this._type; } }

        #region IEquatable[Relation] implementation

        /// <summary>
        /// Determines whether the specified <see cref="RelationalExpression"/> is equal to the current <see cref="Epic.Linq.Expressions.Relational.Relation"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="RelationalExpression"/> to compare with the current <see cref="Epic.Linq.Expressions.Relational.Relation"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="RelationalExpression"/> is equal to the current
        /// <see cref="Epic.Linq.Expressions.Relational.Relation"/>; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool Equals (RelationalExpression other);
        
        #endregion

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Epic.Linq.Expressions.Relational.Relation"/>.
        /// </summary>
        /// <param name='obj'>
        /// The <see cref="System.Object"/> to compare with the current <see cref="Epic.Linq.Expressions.Relational.Relation"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="Epic.Linq.Expressions.Relational.Relation"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (object obj)
        {
            return Equals (obj as RelationalExpression);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Epic.Linq.Expressions.Relational.Relation"/> object.
        /// </summary>
        /// <returns>
        /// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.
        /// </returns>
        public override int GetHashCode ()
        {
            return this.GetType ().GetHashCode ();
        }
    }
}

