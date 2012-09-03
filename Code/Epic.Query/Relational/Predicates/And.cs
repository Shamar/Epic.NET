//  
//  And.cs
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

namespace Epic.Query.Relational.Predicates
{
    /// <summary>
    /// Implementation of the And predicate. It is true if both operands are true.
    /// </summary>
    [Serializable]
    public sealed class And : BinaryPredicateBase,
        IEquatable<And>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Predicates.And"/> class.
        /// </summary>
        /// <param name='leftOperand'>
        /// Left operand.
        /// </param>
        /// <param name='rightOperand'>
        /// Right operand.
        /// </param>
        public And (Predicate leftOperand, Predicate rightOperand)
            : base(leftOperand, rightOperand)
        {
        }

        /// <summary>
        /// Determines whether the specified <see cref="BinaryPredicateBase"/> is equal to the
        /// current <see cref="Predicates.And"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="BinaryPredicateBase"/> to compare with the current <see cref="Predicates.And"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="BinaryPredicateBase"/> is equal to the
        /// current <see cref="Predicates.And"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (BinaryPredicateBase other)
        {
            return Equals (other as And);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Predicates.And"/> is equal to the current <see cref="Predicates.And"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="Predicates.And"/> to compare with the current <see cref="Predicates.And"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Predicates.And"/> is equal to the current
        /// <see cref="Predicates.And"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals (And other)
        {
            if (null == other) return false;
            return this.Left.Equals (other.Left) && this.Right.Equals (other.Right);
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

