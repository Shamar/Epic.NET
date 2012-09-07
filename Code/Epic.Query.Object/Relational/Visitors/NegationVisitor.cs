//
//  NegationVisitor.cs
//
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
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
using Epic.Specifications;
using Epic.Query.Relational.Predicates;

namespace Epic.Query.Object.Relational.Visitors
{
    /// <summary>
    /// Visitor of <see cref="Negation{TEntity}"/> that produce the corresponding <see cref="Predicate"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    public sealed class NegationVisitor<TEntity> : CompositeVisitor<Predicate>.VisitorBase, 
                                                   IVisitor<Predicate, Negation<TEntity>>
        where TEntity : class
    {
        /// <summary>
        /// Initialize a new <see cref="NegationVisitor{TEntity}"/> as part of the <paramref name="composition"/>.
        /// </summary>
        /// <param name="composition">Composite visitor to enhance.</param>
        public NegationVisitor (CompositeVisitor<Predicate> composition)
            : base(composition)
        {
        }
        
        #region IVisitor implementation
        /// <summary>
        /// Visit the <paramref name="target"/> to produce a <see cref="Predicate"/>.
        /// </summary>
        /// <param name="target">Negation to visit.</param>
        /// <param name="context">Context of the visit.</param>
        /// <returns>A <see cref="Not"/> predicate.</returns>
        Predicate IVisitor<Predicate, Negation<TEntity>>.Visit (Negation<TEntity> target, IVisitContext context)
        {
            Predicate inner = VisitInner(target.Negated, context);
            if (null == inner)
                return null;
            return new Not(inner);
        }
        #endregion
    }
}

