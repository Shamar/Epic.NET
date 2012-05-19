//  
//  Projection.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Epic.Query.Relational.Operations
{
    /// <summary>
    /// This class models the Projection operation, which extracts a given set of fields from a given
    /// <see cref="Relation"/>.
    /// </summary>
    [Serializable]
    public sealed class Projection: Relation, IEquatable<Projection>
    {
        private readonly Relation relation;
        private readonly IEnumerable<RelationAttribute> attributes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Query.Relational.Operations.Projection"/> class.
        /// </summary>
        /// <param name='relation'>
        /// The <see cref="Relation"/> used as source for the projection.
        /// </param>
        /// <param name='attributes'>
        /// The collection of <see cref="RelationAttributes"/> extracted
        /// </param>
        /// <param name='name'>
        /// A user-defined name used to identify the Projection relation.
        /// </param>
        public Projection (Relation relation, IEnumerable<RelationAttribute> attributes, string name):
            base(RelationType.Projection, name)
        {
            if (null == relation) throw new ArgumentNullException("relation");
            if (null == attributes) throw new ArgumentNullException("attributes");
            if (null == name) throw new ArgumentNullException("name");
            this.relation = relation;
            this.attributes = attributes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Query.Relational.Operations.Projection"/> class.
        /// </summary>
        /// <param name='relation'>
        /// The <see cref="Relation"/> used as source for the projection.
        /// </param>
        /// <param name='attributes'>
        /// The collection of <see cref="RelationAttributes"/> extracted
        /// </param>
        public Projection(Relation relation, IEnumerable<RelationAttribute> attributes):
            base(RelationType.Projection, getDefaultName(relation, attributes))
        {
            // if (null == relation) throw new ArgumentNullException("relation");
            // if (null == attributes) throw new ArgumentNullException("attributes");
            this.relation = relation;
            this.attributes = attributes;
        }

        /// <summary>
        /// Gets the <see cref="Relation"/> used as source for the projection.
        /// </summary>
        /// <value>
        /// The table.
        /// </value>
        public Relation Relation { get { return this.relation; } }

        /// <summary>
        /// Gets the attributes extracted from the given table.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        public IEnumerable<RelationAttribute> Attributes { get { return this.attributes; } }

        /// <summary>
        /// Determines whether the specified <see cref="Epic.Query.Relational.Relation"/> is equal to the current <see cref="Epic.Query.Relational.Operations.Projection"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="Epic.Query.Relational.Relation"/> to compare with the current <see cref="Epic.Query.Relational.Operations.Projection"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Epic.Query.Relational.Relation"/> is equal to the current
        /// <see cref="Epic.Query.Relational.Operations.Projection"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (Relation other)
        {
            return this.Equals (other as Projection);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Epic.Query.Relational.Operations.Projection"/> is equal to the
        /// current <see cref="Epic.Query.Relational.Operations.Projection"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="Epic.Query.Relational.Operations.Projection"/> to compare with the current <see cref="Epic.Query.Relational.Operations.Projection"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Epic.Query.Relational.Operations.Projection"/> is equal to the
        /// current <see cref="Epic.Query.Relational.Operations.Projection"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Projection other)
        {
            if (null == other) return false;
            return this.Relation.Equals (other.Relation) && this.Attributes.SequenceEqual(other.Attributes);
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
            return AcceptMe (this, visitor, context);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Epic.Query.Relational.Operations.Projection"/> object.
        /// </summary>
        /// <returns>
        /// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.
        /// </returns>
        public override int GetHashCode ()
        {
            int hash = this.Relation.GetHashCode ();
            foreach (RelationAttribute attribute in this.Attributes)
                hash ^= attribute.GetHashCode ();
            return hash;
        }

        /// <summary>
        /// Gets the default name for the projection.
        /// </summary>
        /// <returns>
        /// The default name.
        /// </returns>
        /// <param name='relation'>
        /// the <see cref="Relation"/> used as source for the projection.
        /// </param>
        /// <param name='attributes'>
        /// The attributes to be extracted from the relation.
        /// </param>
        private static string getDefaultName (Relation relation, IEnumerable<RelationAttribute> attributes)
        {
            if (null == relation) throw new ArgumentNullException("relation");
            if (null == attributes) throw new ArgumentNullException("attributes");
            StringBuilder sb = new StringBuilder();
            sb.Append("Take ");
            foreach (RelationAttribute attribute in attributes)
                sb.Append (attribute.Name).Append(",");
            sb.Append (" from ");
            sb.Append (relation.Name);
            return sb.ToString ();
        }

    }
}

