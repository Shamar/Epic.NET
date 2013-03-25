//
//  SelectionVisitor.cs
//
//  Author:
//       giacomo <${AuthorEmail}>
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
using System.Collections.Generic;
using Epic.Query.Relational;
using Epic.Specifications;
using Epic.Specifications.Visitors;
using Epic.Query.Object.Expressions;

namespace Epic.Query.Object.Relational.Visitors
{
    public sealed class SelectionVisitor<TEntity> : CompositeVisitor<RelationalExpression>.VisitorBase, IVisitor<RelationalExpression, Selection<TEntity>> where TEntity : class
    {
        private readonly Dictionary<Type, IVisitor<RelationalExpression>> _visitors;
        private readonly IVisitor<RelationalExpression, ISpecification<TEntity>> _specificationMapper;
        private readonly IVisitor<ISpecification<TEntity>> _specificationNormalizer;

        public SelectionVisitor(CompositeVisitor<RelationalExpression> composition, IVisitor<ISpecification<TEntity>> specificationNormalizer, IVisitor<RelationalExpression, ISpecification<TEntity>> specificationMapper)
            : base(composition)
        {
            if (specificationNormalizer == null)
                throw new ArgumentNullException("specificationNormalizer");
            if (specificationMapper == null)
                throw new ArgumentNullException("specificationMapper");
            _specificationNormalizer = specificationNormalizer;
            _specificationMapper = specificationMapper;
        }

        #region IVisitor implementation

        RelationalExpression IVisitor<RelationalExpression, Selection<TEntity>>.Visit(Selection<TEntity> target, IVisitContext context)
        {
            RelationalExpression source = VisitInner(target.Source, context);
            ISpecification<TEntity> normalizedSpecification = target.Specification.Accept(_specificationNormalizer, context);
            RelationalExpression selection = normalizedSpecification.Accept(_specificationMapper, context);

            throw new NotImplementedException();
        }

        #endregion
    }
}

