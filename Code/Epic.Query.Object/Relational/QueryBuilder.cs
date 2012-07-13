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
    public sealed class QueryBuilder<TEntity, TIdentity> : CompositeVisitorBase<Relation, Expression<IEnumerable<TEntity>>>
        where TEntity : class
        where TIdentity : IEquatable<TIdentity>
    {
        private readonly PredicateBuilder<TEntity> _predicateBuilder;

        public QueryBuilder (string name, Relation mainRelation)
            : base(name)
        {
            _predicateBuilder = new PredicateBuilder<TEntity>(name);

            new SourceVisitor<TEntity, TIdentity>(this, mainRelation);
            
            new SelectionVisitor<TEntity>(this, _predicateBuilder);
        }
        
        public PredicateBuilder<TEntity> PredicateBuilder
        {
            get
            {
                return _predicateBuilder;
            }
        }
        #region implemented abstract members of Epic.CompositeVisitorBase
        protected override IVisitContext InitializeVisitContext (Expression<IEnumerable<TEntity>> target, IVisitContext context)
        {
            return context;
        }
        #endregion
    }
}

