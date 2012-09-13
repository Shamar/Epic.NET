//
//  QueryBuilder.cs
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
using Epic.Query.Relational;
using Epic.Query.Object.Relational.Visitors;
using System.Collections.Generic;
using Epic.Query.Object.Expressions;

namespace Epic.Query.Object.Relational
{ 
    /// <summary>
    /// Composite visitor that builds a <see cref="RelationalExpression"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    /// <typeparam name="TIdentity">Type of the identity of <typeparamref name="TEntity"/>.</typeparam>
    public class QueryBuilder<TEntity, TIdentity> : CompositeVisitorBase<RelationalExpression, Expression>
        where TEntity : class
        where TIdentity : IEquatable<TIdentity>
    {
        private readonly PredicateBuilder _predicateBuilder;

        /// <summary>
        /// Initialize a new <see cref="QueryBuilder{TEntity, TIdentity}"/>.
        /// </summary>
        /// <param name="name">Name of the composition.</param>
        /// <param name="mainRelation">Main relation for <typeparamref name="TEntity"/>.</param>
        public QueryBuilder(string name, RelationalExpression mainRelation)
            : base(name)
        {
            _predicateBuilder = new PredicateBuilder(name);
            new ConjunctionVisitor<TEntity>(_predicateBuilder);
            new DisjunctionVisitor<TEntity>(_predicateBuilder);
            new NegationVisitor<TEntity>(_predicateBuilder);


            new SourceVisitor<TEntity, TIdentity>(this, mainRelation);
            
            new SelectionVisitor<TEntity>(this, _predicateBuilder);
        }
        
        /// <summary>
        /// Predicate builder for <see cref="Epic.Specifications.ISpecification{TEntity}"/>.
        /// </summary>
        public PredicateBuilder PredicateBuilder
        {
            get
            {
                return _predicateBuilder;
            }
        }
        #region implemented abstract members of Epic.CompositeVisitorBase
        /// <summary>
        /// Returns the <paramref name="context"/> provided.
        /// </summary>
        /// <param name="target">Expression to visit.</param>
        /// <param name="context">Visit context.</param>
        /// <returns>The <paramref name="context"/> provided.</returns>
        protected override IVisitContext InitializeVisitContext (Expression target, IVisitContext context)
        {
            return context;
        }
        #endregion
    }
}

