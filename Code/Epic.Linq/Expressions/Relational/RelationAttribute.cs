//  
//  RelationAttribute.cs
//  
//  Author:
//       Marco Veglio <m.veglio@gmail.com>
// 
//  Copyright (c) 2010-2011 Giacomo Tesio.
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

namespace Epic.Linq.Expressions.Relational
{
    /// <summary>
    /// Relation attribute. Models one attribute of a <see cref="Relation"/> (e.g. a column in a database table)
    /// </summary>
    /// <exception cref='ArgumentNullException'>
    /// Is thrown when an argument passed to a method is invalid because it is <see langword="null" /> .
    /// </exception>
    [Serializable]
    public sealed class RelationAttribute: VisitableBase, IEquatable<RelationAttribute>
    {
        private readonly string _name;
        private readonly Relation _relation;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Linq.Expressions.Relational.RelationAttribute"/> class.
        /// </summary>
        /// <param name='name'>
        /// Name of the attribute.
        /// </param>
        /// <param name='relation'>
        /// The <see cref="Relation"/> the attribute is tied to.
        /// </param>
        /// <exception cref='ArgumentNullException'>
        /// Is thrown when either arguments are null.
        /// </exception>
        public RelationAttribute (string name, Relation relation)
        {
            if (null == name)
                throw new ArgumentNullException("name");
            if (null == relation)
                throw new ArgumentNullException("relation");
            this._name = name;
            this._relation = relation;
        }
        
        /// <summary>
        /// Gets the name of the attribute.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get { return this._name; } }
        
        /// <summary>
        /// Gets the relation the attribute is tied to.
        /// </summary>
        /// <value>
        /// The relation.
        /// </value>
        public Relation Relation { get { return this._relation; } }
        
        /// <summary>
        /// Serves as a hash function for a <see cref="Epic.Linq.Expressions.Relational.RelationAttribute"/> object.
        /// </summary>
        /// <returns>
        /// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.
        /// </returns>
        public override int GetHashCode ()
        {
            return GetType().GetHashCode() ^ this._name.GetHashCode() ^ this.Relation.GetHashCode ();
        }
        
        /// <summary>
        /// Determines whether the specified <see cref="RelationAttribute"/> is equal to the current <see cref="Epic.Linq.Expressions.Relational.RelationAttribute"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="RelationAttribute"/> to compare with the current <see cref="Epic.Linq.Expressions.Relational.RelationAttribute"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="RelationAttribute"/> is equal to the current
        /// <see cref="Epic.Linq.Expressions.Relational.RelationAttribute"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals (RelationAttribute other)
        {
            if (other == null) return false;
            return other.Name == this.Name && other.Relation.Equals (this.Relation);
        }
        
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Epic.Linq.Expressions.Relational.RelationAttribute"/>.
        /// </summary>
        /// <param name='obj'>
        /// The <see cref="System.Object"/> to compare with the current <see cref="Epic.Linq.Expressions.Relational.RelationAttribute"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="Epic.Linq.Expressions.Relational.RelationAttribute"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (object obj)
        {
            RelationAttribute attribute = obj as RelationAttribute;
            if (null != attribute)
                return this.Equals (attribute);
            return false;
        }
        
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
        /// The 1st type parameter.
        /// </typeparam>
        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe(this, visitor, context);
        }
    }
}

