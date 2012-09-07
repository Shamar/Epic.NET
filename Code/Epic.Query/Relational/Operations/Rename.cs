//  
//  Rename.cs
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
    /// This class models the Rename operation, which allows user to assign a custom name 
    /// to a given <see cref="RelationalExpression"/>.
    /// </summary>
    [Serializable]
    public sealed class Rename: RelationalExpression, IEquatable<Rename>
    {
        private readonly RelationalExpression relation;
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Query.Relational.Operations.Rename"/> class.
        /// </summary>
        /// <param name='relation'>
        /// The <see cref="RelationalExpression"/> to be renamed.
        /// </param>
        /// <param name='name'>
        /// The new name given to the relation.
        /// </param>
        public Rename (RelationalExpression relation, string name): 
            base(RelationType.BaseRelation)
        {
            if(null == relation)
                throw new ArgumentNullException("relation");
            if (String.IsNullOrEmpty (name))
                throw new ArgumentNullException("newRelationName");
            this.relation = relation;
            this.name = name;
        }

        /// <summary>
        /// Gets the <see cref="RelationalExpression"/> to be renamed.
        /// </summary>
        /// <value>
        /// The relation.
        /// </value>
        public RelationalExpression Relation { get { return this.relation; } }

        /// <summary>
        /// Gets the new name of the <see cref="RelationalExpression"/>.
        /// </summary>
        /// <value>
        /// The new name of the relation.
        /// </value>
        public string Name { get { return this.name; } }

        /// <summary>
        /// Determines whether the specified <see cref="RelationalExpression"/> is equal to the current <see cref="Epic.Query.Relational.Operations.Rename"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="RelationalExpression"/> to compare with the current <see cref="Epic.Query.Relational.Operations.Rename"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="RelationalExpression"/> is equal to the current
        /// <see cref="Epic.Query.Relational.Operations.Rename"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (RelationalExpression other)
        {
            return Equals (other as Rename);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Epic.Query.Relational.Operations.Rename"/> object.
        /// </summary>
        /// <returns>
        /// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.
        /// </returns>
        public override int GetHashCode ()
        {
            return this.relation.GetHashCode () ^ this.name.GetHashCode () ;
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

        #region IEquatable implementation
        /// <summary>
        /// Determines whether the specified <see cref="Epic.Query.Relational.Operations.Rename"/> is equal to the
        /// current <see cref="Epic.Query.Relational.Operations.Rename"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="Epic.Query.Relational.Operations.Rename"/> to compare with the current <see cref="Epic.Query.Relational.Operations.Rename"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Epic.Query.Relational.Operations.Rename"/> is equal to the current
        /// <see cref="Epic.Query.Relational.Operations.Rename"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals (Rename other)
        {
            if (null == other) return false;
            return this.relation.Equals (other.relation) && this.name.Equals (other.name);
        }
        #endregion

    }
}

