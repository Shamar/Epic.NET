//  
//  Select.cs
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
using Epic.Query.Relational.Predicates;

namespace Epic.Query.Relational.Operations
{

    /// <summary>
    /// This class models the Select operation which extracts those record from a given table matching
    /// a given <see cref="Predicate"/>
    /// </summary>
    [Serializable]
    public sealed class Selection: RelationalExpression, IEquatable<Selection>
    {
        private readonly Predicate _condition;
        private readonly RelationalExpression _relation;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Query.Relational.Operations.Selection"/> class.
        /// </summary>
        /// <param name='relation'>
        /// The <see cref="RelationalExpression"/> used as source for the selection.
        /// </param>
        /// <param name='condition'>
        /// The <see cref="Predicate"/> against which the records are matched.
        /// </param>
        public Selection (RelationalExpression relation, Predicate condition)
            : base(RelationType.Selection)
        {
            if (null == relation)  throw new ArgumentNullException("relation");
            if (null == condition) throw new ArgumentNullException("condition");
            this._condition = condition;
            this._relation = relation;
        }
        
        /// <summary>
        /// Gets the condition against which the table's records are matched.
        /// </summary>
        /// <value>
        /// The condition.
        /// </value>
        public Predicate Condition { get { return this._condition; } }
        
        /// <summary>
        /// Gets the <see cref="RelationalExpression"/> used as source for the selection.
        /// </summary>
        /// <value>
        /// The table.
        /// </value>
        public RelationalExpression Relation { get { return this._relation; } }
        
        /// <summary>
        /// Determines whether the specified <see cref="RelationalExpression"/> is equal to the current <see cref="Epic.Query.Relational.Operations.Selection"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="RelationalExpression"/> to compare with the current <see cref="Epic.Query.Relational.Operations.Selection"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="RelationalExpression"/> is equal to the current
        /// <see cref="Epic.Query.Relational.Operations.Selection"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (RelationalExpression other)
        {
            return this.Equals (other as Selection);
        }
        
        /// <summary>
        /// Determines whether the specified <see cref="Epic.Query.Relational.Operations.Selection"/> is equal to the
        /// current <see cref="Epic.Query.Relational.Operations.Selection"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="Epic.Query.Relational.Operations.Selection"/> to compare with the current <see cref="Epic.Query.Relational.Operations.Selection"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Epic.Query.Relational.Operations.Selection"/> is equal to the current
        /// <see cref="Epic.Query.Relational.Operations.Selection"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Selection other)
        {
            if (null == other) return false;
            return this.Relation.Equals (other.Relation) && this.Condition.Equals (other.Condition);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Epic.Query.Relational.Operations.Selection"/> object.
        /// </summary>
        /// <returns>
        /// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.
        /// </returns>
        public override int GetHashCode ()
        {
            return this.Relation.GetHashCode () ^ this.Condition.GetHashCode ();
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
    }
}

