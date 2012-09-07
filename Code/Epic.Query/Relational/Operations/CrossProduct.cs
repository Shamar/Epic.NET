//  
//  CrossProduct.cs
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
    /// This class models the Inner Join operation, which joins two relations and returns those
    /// record matching a given condition.
    /// </summary>
    [Serializable]
    public sealed class CrossProduct: RelationalExpression, IEquatable<CrossProduct>
    {
        private readonly RelationalExpression _leftRelation;
        private readonly RelationalExpression _rightRelation;
 
        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Query.Relational.Operations.CrossProduct"/> class.
        /// </summary>
        /// <param name='leftRelation'>
        /// Left relation in the Join operation.
        /// </param>
        /// <param name='rightRelation'>
        /// Right relation in the Join operation.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when any argument is <see langword="null"/></exception> 
        public CrossProduct (RelationalExpression leftRelation, RelationalExpression rightRelation):
            base(RelationType.CrossProduct)
        {
            if (null == leftRelation)
                throw new ArgumentNullException("leftRelation");
            if (null == rightRelation)
                throw new ArgumentNullException("rightRelation");

            this._leftRelation = leftRelation;
            this._rightRelation = rightRelation;
        }

        /// <summary>
        /// Gets the left relation.
        /// </summary>
        /// <value>
        /// The left relation.
        /// </value>
        public RelationalExpression LeftRelation { get { return this._leftRelation; } }

        /// <summary>
        /// Gets the right relation.
        /// </summary>
        /// <value>
        /// The right relation.
        /// </value>
        public RelationalExpression RightRelation { get { return this._rightRelation; } }

        /// <summary>
        /// Determines whether the specified <see cref="RelationalExpression"/> is equal to the current <see cref="Epic.Query.Relational.Operations.CrossProduct"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="RelationalExpression"/> to compare with the current <see cref="Epic.Query.Relational.Operations.CrossProduct"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="RelationalExpression"/> is equal to the current
        /// <see cref="Epic.Query.Relational.Operations.CrossProduct"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (RelationalExpression other)
        {
            return Equals (other as CrossProduct);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Epic.Query.Relational.Operations.CrossProduct"/> is equal to the
        /// current <see cref="Epic.Query.Relational.Operations.CrossProduct"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="Epic.Query.Relational.Operations.CrossProduct"/> to compare with the current <see cref="Epic.Query.Relational.Operations.CrossProduct"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Epic.Query.Relational.Operations.CrossProduct"/> is equal to the
        /// current <see cref="Epic.Query.Relational.Operations.CrossProduct"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals (CrossProduct other)
        {
            if (null == other) return false;

            return this.LeftRelation.Equals (other.LeftRelation) &&
                this.RightRelation.Equals (other.RightRelation) ;
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Epic.Query.Relational.Operations.CrossProduct"/> object.
        /// </summary>
        /// <returns>
        /// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.
        /// </returns>
        public override int GetHashCode ()
        {
            return this.LeftRelation.GetHashCode () ^ this.RightRelation.GetHashCode () ;
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

