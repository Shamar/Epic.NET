//  
//  Or.cs
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
    /// Implementation of the Or predicate. It is true if any operand is true.
    /// </summary>
    [Serializable]
    public sealed class Or : BinaryPredicateBase,
        IEquatable<Or>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Linq.Expressions.Relational.Predicates.Or`2"/> class.
        /// </summary>
        /// <param name='leftOperand'>
        /// Left operand.
        /// </param>
        /// <param name='rightOperand'>
        /// Right operand.
        /// </param>
        public Or (Predicate leftOperand, Predicate rightOperand)
            : base(leftOperand, rightOperand)
        {
        }

        /// <summary>
        /// Determines whether the specified <see cref="BinaryPredicateBase<TPredicate1,TPredicate2>"/> is equal to the
        /// current <see cref="Epic.Linq.Expressions.Relational.Predicates.Or`2"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="BinaryPredicateBase<TPredicate1,TPredicate2>"/> to compare with the current <see cref="Epic.Linq.Expressions.Relational.Predicates.Or`2"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="BinaryPredicateBase<TPredicate1,TPredicate2>"/> is equal to the
        /// current <see cref="Epic.Linq.Expressions.Relational.Predicates.Or`2"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (BinaryPredicateBase other)
        {
            return Equals (other as Or);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Or<TPredicate1,TPredicate2>"/> is equal to the current <see cref="Epic.Linq.Expressions.Relational.Predicates.Or`2"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="Or<TPredicate1,TPredicate2>"/> to compare with the current <see cref="Epic.Linq.Expressions.Relational.Predicates.Or`2"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Or<TPredicate1,TPredicate2>"/> is equal to the current
        /// <see cref="Epic.Linq.Expressions.Relational.Predicates.Or`2"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals (Or other)
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
        /// The 1st type parameter.
        /// </typeparam>
        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe (this, visitor, context);
        }
    }
}

