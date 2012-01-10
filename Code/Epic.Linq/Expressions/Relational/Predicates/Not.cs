//  
//  Not.cs
//  
//  Author:
//       Marco Veglio <m.veglio@gmail.com>
// 
//  Copyright (c) 2010-2011 Giacomo Tesio
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

namespace Epic.Linq.Expressions.Relational.Predicates
{
    /// <summary>
    /// Implementation of the Not predicate. It is true if its operand is false
    /// </summary>
    [Serializable]
    public sealed class Not<TPredicate>: UnaryPredicateBase<TPredicate>, IEquatable<Not<TPredicate>> where TPredicate: Predicate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Linq.Expressions.Relational.Predicates.Not`1"/> class.
        /// </summary>
        /// <param name='operand'>
        /// Operand.
        /// </param>
        public Not (TPredicate operand): base(operand)
        {
        }

        /// <summary>
        /// Determines whether the specified <see cref="UnaryPredicateBase<TPredicate>"/> is equal to the current <see cref="Epic.Linq.Expressions.Relational.Predicates.Not`1"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="UnaryPredicateBase<TPredicate>"/> to compare with the current <see cref="Epic.Linq.Expressions.Relational.Predicates.Not`1"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="UnaryPredicateBase<TPredicate>"/> is equal to the current
        /// <see cref="Epic.Linq.Expressions.Relational.Predicates.Not`1"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (UnaryPredicateBase<TPredicate> other)
        {
            return Equals (other as Not<TPredicate>);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Not<TPredicate>"/> is equal to the current <see cref="Epic.Linq.Expressions.Relational.Predicates.Not`1"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="Not<TPredicate>"/> to compare with the current <see cref="Epic.Linq.Expressions.Relational.Predicates.Not`1"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Not<TPredicate>"/> is equal to the current
        /// <see cref="Epic.Linq.Expressions.Relational.Predicates.Not`1"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Not<TPredicate> other)
        {
            if (null == other) return false;
            return this.Operand.Equals (other.Operand);
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
            return AcceptMe(this, visitor, context);
        }
    }
}

