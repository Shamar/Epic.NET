//
//  PredicateBuilder.cs
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
using Epic.Query.Object.Relational.Visitors;

namespace Epic.Query.Object.Relational
{
    /// <summary>
    /// Visitor that can build a <see cref="Predicate"/> by visiting a <see cref="ISpecification"/>.
    /// </summary>
    public class PredicateBuilder : CompositeVisitorBase<Predicate, ISpecification>
    {
        /// <summary>
        /// Initializes a new <see cref="PredicateBuilder"/>.
        /// </summary>
        /// <param name="name">Name of the composition.</param>
        /// <remarks>
        /// Provide the features of <see cref="ConjunctionVisitor{TEntity}"/>, 
        /// <see cref="DisjunctionVisitor{TEntity}"/> and <see cref="NegationVisitor{TEntity}"/>.
        /// </remarks>
        public PredicateBuilder (string name)
            : base(name)
        {
        }

        #region implemented abstract members of Epic.CompositeVisitorBase
        /// <summary>
        /// Returns the <paramref name="context"/> provided.
        /// </summary>
        /// <param name="target">Expression to visit.</param>
        /// <param name="context">Visit context.</param>
        /// <returns>The <paramref name="context"/> provided.</returns>
        protected override IVisitContext InitializeVisitContext(ISpecification target, IVisitContext context)
        {
            return context;
        }
        #endregion
    }
}

