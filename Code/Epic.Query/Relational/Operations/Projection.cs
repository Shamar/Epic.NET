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

namespace Epic.Query.Relational.Operations
{
    /// <summary>
    /// This class models the Projection operation, which extracts a given set of fields from a given
    /// <see cref="RelationalExpression"/>.
    /// </summary>
    [Serializable]
    public sealed class Projection: RelationalExpression, IEquatable<Projection>
    {
        private readonly RelationalExpression _relation;
        private readonly IEnumerable<RelationAttribute> _attributes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Query.Relational.Operations.Projection"/> class.
        /// </summary>
        /// <param name='relation'>
        /// The <see cref="RelationalExpression"/> used as source for the projection.
        /// </param>
        /// <param name='attributes'>
        /// The collection of <see cref="RelationAttribute">relation attributes</see> extracted
        /// </param>
        public Projection(RelationalExpression relation, IEnumerable<RelationAttribute> attributes):
            base(RelationType.Projection)
        {
            if (null == relation) throw new ArgumentNullException("relation");
            if (null == attributes) throw new ArgumentNullException("attributes");
            this._relation = relation;
            this._attributes = attributes;
        }

        /// <summary>
        /// Gets the <see cref="RelationalExpression"/> used as source for the projection.
        /// </summary>
        public RelationalExpression Relation { get { return this._relation; } }

        /// <summary>
        /// Gets the attributes extracted from the given table.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        public IEnumerable<RelationAttribute> Attributes { get { return this._attributes; } }

        /// <summary>
        /// Determines whether the specified <see cref="RelationalExpression"/> is equal to the current <see cref="Epic.Query.Relational.Operations.Projection"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="RelationalExpression"/> to compare with the current <see cref="Epic.Query.Relational.Operations.Projection"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="RelationalExpression"/> is equal to the current
        /// <see cref="Epic.Query.Relational.Operations.Projection"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (RelationalExpression other)
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
        /// The type of the result of the visit.
        /// </typeparam>
        /// <returns>Result of the visit.</returns>
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
    }
}

