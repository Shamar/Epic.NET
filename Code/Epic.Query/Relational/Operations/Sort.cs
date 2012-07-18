//  
//  Sort.cs
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
namespace Epic.Query.Relational.Operations
{
    /// <summary>
    /// This class models the Sort operation, which sorts all records of a <see cref="Relation"/>
    /// according to a given criteria.
    /// </summary>
    [Serializable]
    public sealed class Sort: Relation, IEquatable<Sort>
    {
        private readonly Relation _relation;
        private readonly RelationAttribute _attribute;
        private readonly bool _descending;

        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Query.Relational.Operations.Sort"/> class.
        /// </summary>
        /// <param name='relation'>
        /// The <see cref="Relation"/> to be sorted.
        /// </param>
        /// <param name='attribute'>
        /// The <see cref="RelationAttribute"/> used for sorting.
        /// </param>
        /// <param name='descending'>
        /// Whether the sort shall be descendent.
        /// </param>
        /// <param name='name'>
        /// Name of the new relation.
        /// </param>
        public Sort(Relation relation, RelationAttribute attribute, bool descending, string name): 
            base(RelationType.Sort, name)
        {
            if (null == relation) throw new ArgumentNullException("relation");
            if (null == attribute) throw new ArgumentNullException("attribute");

            this._relation = relation;
            this._attribute = attribute;
            this._descending = descending;
        }

        /// <summary>
        /// Gets the <see cref="Relation"/>.
        /// </summary>
        /// <value>
        /// The relation.
        /// </value>
        public Relation Relation { get { return this._relation; } }

        /// <summary>
        /// Gets the <see cref="RelationAttribute"/> used for sorting.
        /// </summary>
        /// <value>
        /// The attribute.
        /// </value>
        public RelationAttribute Attribute { get { return this._attribute; } }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Epic.Query.Relational.Operations.Sort"/> is descending.
        /// </summary>
        /// <value>
        /// <c>true</c> if descending; otherwise, <c>false</c>.
        /// </value>
        public bool Descending { get { return this._descending; } }

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
            return AcceptMe (this, visitor, context);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Epic.Query.Relational.Relation"/> is equal to the current <see cref="Epic.Query.Relational.Operations.Sort"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="Epic.Query.Relational.Relation"/> to compare with the current <see cref="Epic.Query.Relational.Operations.Sort"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Epic.Query.Relational.Relation"/> is equal to the current
        /// <see cref="Epic.Query.Relational.Operations.Sort"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (Relation other)
        {
            return this.Equals (other as Sort);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Epic.Query.Relational.Operations.Sort"/> object.
        /// </summary>
        /// <returns>
        /// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.
        /// </returns>
        public override int GetHashCode ()
        {
            return this.Relation.GetHashCode () ^ this.Attribute.GetHashCode () ^ this.Name.GetHashCode ();
        }

        #region IEquatable implementation
        /// <summary>
        /// Determines whether the specified <see cref="Epic.Query.Relational.Operations.Sort"/> is equal to the current <see cref="Epic.Query.Relational.Operations.Sort"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="Epic.Query.Relational.Operations.Sort"/> to compare with the current <see cref="Epic.Query.Relational.Operations.Sort"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Epic.Query.Relational.Operations.Sort"/> is equal to the current
        /// <see cref="Epic.Query.Relational.Operations.Sort"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals (Sort other)
        {
            if (null == other) return false;
            return this.Relation.Equals (other.Relation) &&
                this.Attribute.Equals (other.Attribute) &&
                    this.Name.Equals(other.Name);
        }
        #endregion
    }
}

