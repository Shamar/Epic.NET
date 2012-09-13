//
//  SelectionVisitor.cs
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
using Epic.Query.Object.Expressions;
using Epic.Query.Relational;
using Epic.Query.Relational.Predicates;
using Epic.Query.Relational.Operations;

namespace Epic.Query.Object.Relational.Visitors
{
    /// <summary>
    /// Visitor that translate to a <see cref="RelationalExpression"/> a <see cref="Selection{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    public sealed class SelectionVisitor<TEntity> : CompositeVisitor<RelationalExpression>.VisitorBase,
                                                    IVisitor<RelationalExpression, Selection<TEntity>>
        where TEntity : class
    {
        private readonly PredicateBuilder _predicateBuilder;
        /// <summary>
        /// Initializes a new <see cref="SelectionVisitor{TEntity}"/> as part of the <paramref name="composition"/>.
        /// </summary>
        /// <param name="composition">Composite visitor to enhance.</param>
        /// <param name="predicateBuilder">Predicate builder to use while visiting <see cref="Epic.Specifications.ISpecification{TEntity}"/>.</param>
        public SelectionVisitor(CompositeVisitor<RelationalExpression> composition, PredicateBuilder predicateBuilder)
            : base(composition)
        {
            if(null == predicateBuilder)
                throw new ArgumentNullException("predicateBuilder");
            _predicateBuilder = predicateBuilder;
        }

        #region IVisitor implementation
        RelationalExpression IVisitor<RelationalExpression, Selection<TEntity>>.Visit(Selection<TEntity> target, IVisitContext context)
        {
            RelationalExpression source = VisitInner(target.Source, context);
            SourceRelationBuilder builder = new SourceRelationBuilder(source);
            Predicate predicate = target.Specification.Accept(_predicateBuilder, context.With(builder));
            return new Selection(builder.ToRelation(), predicate);
        }
        #endregion
    }
}

