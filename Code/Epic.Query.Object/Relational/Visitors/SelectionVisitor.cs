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

namespace Epic.Query.Object.Relational.Visitors
{
    public sealed class SelectionVisitor<TEntity> : CompositeVisitor<Relation>.VisitorBase, 
                                                    IVisitor<Relation, Selection<TEntity>>
        where TEntity : class
    {
        private readonly PredicateBuilder<TEntity> _predicateBuilder;
        public SelectionVisitor (CompositeVisitor<Relation> composition, PredicateBuilder<TEntity> predicateBuilder)
            : base(composition)
        {
            if(null == predicateBuilder)
                throw new ArgumentNullException("predicateBuilder");
            _predicateBuilder = predicateBuilder;
        }

        #region IVisitor implementation
        public Relation Visit (Selection<TEntity> target, IVisitContext context)
        {
            Relation source = VisitInner(target.Source, context);
            QueryBuilder builder = new QueryBuilder(source);
            Predicate predicate = _predicateBuilder.Visit(target.Specification, context.With(builder));
            // we have to add a constructor without the name argument to the Selection 
            return new Selection("removeThisArgument", builder.ToRelation(), predicate);
        }
        #endregion
    }
}

