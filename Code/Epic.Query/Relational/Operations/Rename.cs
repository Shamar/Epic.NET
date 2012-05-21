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
    /// to a given <see cref="Relation"/>.
    /// </summary>
    [Serializable]
    public sealed class Rename: Relation, IEquatable<Rename>
    {
        private readonly Relation relation;
        private readonly string newRelationName;

        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Query.Relational.Operations.Rename"/> class.
        /// </summary>
        /// <param name='relation'>
        /// The <see cref="Relation"/> to be renamed.
        /// </param>
        /// <param name='newRelationName'>
        /// The new name given to the relation.
        /// </param>
        public Rename (Relation relation, string newRelationName): 
            base(RelationType.BaseRelation, getDefaultName (relation, newRelationName))
        {
            this.relation = relation;
            this.newRelationName = newRelationName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Query.Relational.Operations.Rename"/> class.
        /// </summary>
        /// <param name='relation'>
        /// The <see cref="Relation"/> to be renamed.
        /// </param>
        /// <param name='newRelationName'>
        /// The new name given to the relation.
        /// </param>
        /// <param name='operationName'>
        /// A user-defined name used to identify the Projection relation.
        /// </param>
        public Rename (Relation relation, string newRelationName, string operationName):
            base(RelationType.Rename, operationName)
        {
            if(null == relation)
                throw new ArgumentNullException("relation");
            if(string.IsNullOrEmpty(newRelationName))
                throw new ArgumentNullException("newRelationName");

            this.relation = relation;
            this.newRelationName = newRelationName;
        }

        /// <summary>
        /// Gets the <see cref="Relation"/> to be renamed.
        /// </summary>
        /// <value>
        /// The relation.
        /// </value>
        public Relation Relation { get { return this.relation; } }

        /// <summary>
        /// Gets the new name of the <see cref="Relation"/>.
        /// </summary>
        /// <value>
        /// The new name of the relation.
        /// </value>
        public string NewRelationName { get { return this.newRelationName; } }

        /// <summary>
        /// Determines whether the specified <see cref="Epic.Query.Relational.Relation"/> is equal to the current <see cref="Epic.Query.Relational.Operations.Rename"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="Epic.Query.Relational.Relation"/> to compare with the current <see cref="Epic.Query.Relational.Operations.Rename"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Epic.Query.Relational.Relation"/> is equal to the current
        /// <see cref="Epic.Query.Relational.Operations.Rename"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (Relation other)
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
            return this.relation.GetHashCode () ^ this.newRelationName.GetHashCode ();
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
            return this.relation.Equals (other.relation) && this.newRelationName.Equals (other.newRelationName);
        }
        #endregion

        private static string getDefaultName(Relation relation, string newRelationName)
        {
            if(null == relation)
                throw new ArgumentNullException("relation");
            if(string.IsNullOrEmpty(newRelationName))
                throw new ArgumentNullException("newRelationName");
            return string.Format ("{0} as {1}", relation.Name, newRelationName);
        }
    }
}

