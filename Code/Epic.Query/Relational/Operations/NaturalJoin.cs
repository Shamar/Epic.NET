//  
//  NaturalJoin.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2013 Giacomo Tesio
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
    /// This class models the Natural join operation, which returns all combinations of tuples in the joined relations
    /// that are equal on their common attribute names.
    /// </summary>
    [Serializable]
    public sealed class NaturalJoin : RelationalExpression, IEquatable<NaturalJoin>
    {
        private readonly RelationalExpression _left;
        private readonly RelationalExpression _right;

        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Query.Relational.Operations.NaturalJoin"/> class.
        /// </summary>
        /// <param name="leftRelation">Left relation.</param>
        /// <param name="rightRelation">Right relation.</param>
        /// <exception cref="ArgumentNullException"><paramref name="leftRelation"/> or <paramref name="rightRelation"/> is <see langword="null"/>.</exception>
        public NaturalJoin(RelationalExpression leftRelation, RelationalExpression rightRelation)
            : base(RelationType.NaturalJoin)
        {
            if(null == leftRelation)
                throw new ArgumentNullException("leftRelation");
            if(null == rightRelation)
                throw new ArgumentNullException("rightRelation");
            _left = leftRelation;
            _right = rightRelation;
        }

        /// <summary>
        /// The left relation joined.
        /// </summary>
        public RelationalExpression LeftRelation
        {
            get
            {
                return _left;
            }
        }

        /// <summary>
        /// The right relation joined.
        /// </summary>
        public RelationalExpression RightRelation
        {
            get
            {
                return _right;
            }
        }

        #region implemented abstract members of VisitableBase

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
        public override TResult Accept<TResult>(IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe(this, visitor, context);
        }

        #endregion

        #region implemented abstract members of RelationalExpression

        /// <summary>
        /// Determines whether the specified <see cref="Epic.Query.Relational.RelationalExpression"/> is equal to the
        /// current <see cref="Epic.Query.Relational.Operations.NaturalJoin"/>.
        /// </summary>
        /// <param name="other">The <see cref="Epic.Query.Relational.RelationalExpression"/> to compare with the current <see cref="Epic.Query.Relational.Operations.NaturalJoin"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="Epic.Query.Relational.RelationalExpression"/> is equal to the
        /// current <see cref="Epic.Query.Relational.Operations.NaturalJoin"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(RelationalExpression other)
        {
            return Equals(other as NaturalJoin);
        }

        #endregion

        #region IEquatable implementation

        /// <summary>
        /// Determines whether the specified <see cref="Epic.Query.Relational.Operations.NaturalJoin"/> is equal to the
        /// current <see cref="Epic.Query.Relational.Operations.NaturalJoin"/>.
        /// </summary>
        /// <remarks>
        /// Two <see cref="NaturalJoin"/> are equals if the joined relations are equals, regardless of the order. 
        /// Indeed, the natural join is a commutative operation.
        /// </remarks>            
        /// <param name="other">The <see cref="Epic.Query.Relational.Operations.NaturalJoin"/> to compare with the current <see cref="Epic.Query.Relational.Operations.NaturalJoin"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="Epic.Query.Relational.Operations.NaturalJoin"/> is equal to the
        /// current <see cref="Epic.Query.Relational.Operations.NaturalJoin"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(NaturalJoin other)
        {
            if(null == other)
                return false;
            return (_left.Equals(other._left) && _right.Equals(other._right))
                || (_right.Equals(other._left) && _left.Equals(other._right));
        }
        #endregion
    }
}

