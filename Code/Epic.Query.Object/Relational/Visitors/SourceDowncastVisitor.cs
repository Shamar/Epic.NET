//
//  SourceDowncastVisitor.cs
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
using Epic.Visitors;
using Epic.Query.Object.Expressions;

namespace Epic.Query.Object.Relational.Visitors
{
    /// <summary>
    /// SourceDowncast visitor.
    /// </summary>
    public sealed class SourceDowncastVisitor<TAbstraction> : CompositeVisitor<RelationalExpression>.VisitorBase where TAbstraction : class
    {
        private readonly Dictionary<Type, IVisitor<RelationalExpression>> _visitors;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceDowncastVisitor{TAbstraction}"/> class.
        /// </summary>
        /// <param name="composition">Composition.</param>
        /// <param name="filters">Dictionary that associate to each specialization of <typeparamref name="TAbstraction"/> a function that
        /// is able to transform a <see cref="RelationalExpression"/> into one another that includes only objects of that specific specialization.</param>
        /// <exception cref="ArgumentNullException"><paramref name="composition"/> or <paramref name="filters"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="ArgumentException">At least one of the keys of <paramref name="filters"/> is not a specialization of <typeparamref name="TAbstraction"/>.</exception>\
        public SourceDowncastVisitor(CompositeVisitor<RelationalExpression> composition, Dictionary<Type, Func<RelationalExpression,  IVisitContext, RelationalExpression>> filters)
            : base(composition)
        {
            if (null == filters || filters.Count == 0)
                throw new ArgumentNullException("filters");
            _visitors = new Dictionary<Type, IVisitor<RelationalExpression>>();
            Type abstractionType = typeof(TAbstraction);
            foreach (var filter in filters)
            {
                if (!abstractionType.IsAssignableFrom(filter.Key))
                {
                    string message = string.Format("Invalid filter provided to SourceDowncastVisitor<{0}>: the type {1} does not derive from {0}.", abstractionType, filter.Key.AssemblyQualifiedName);
                    throw new ArgumentException(message, "filters");
                }
                Type visitorFactory = typeof(DowncastingVisitor<>).MakeGenericType(typeof(TAbstraction), filter.Key);
                IVisitor<RelationalExpression> visitor = Activator.CreateInstance(visitorFactory, this, filter.Value) as IVisitor<RelationalExpression>;
                _visitors.Add(typeof(SourceDowncast<,>).MakeGenericType(typeof(TAbstraction), filter.Key), visitor);
            }
        }

        /// <summary>
        /// Return a proper visitor for <typeparamref name="TExpression"/> if it is a <see cref="SourceDowncast{TAbstraction, TEntity}"/>, <see langword="null"/> otherwise.
        /// </summary>
        /// <returns>The visitor.</returns>
        /// <param name="target">Target.</param>
        /// <typeparam name="TExpression">The 1st type parameter.</typeparam>
        protected sealed override IVisitor<RelationalExpression, TExpression> AsVisitor<TExpression>(TExpression target)
        {
            IVisitor<RelationalExpression> result = null;
            if (_visitors.TryGetValue(typeof(TExpression), out result))
            {
                return result as IVisitor<RelationalExpression, TExpression>;
            }
            return null;
        }

        #region nested visitors

        private sealed class DowncastingVisitor<TEntity> : NestedVisitorBase<RelationalExpression, SourceDowncast<TAbstraction, TEntity>, SourceDowncastVisitor<TAbstraction>> where TEntity : class, TAbstraction
        {
            private readonly Func<RelationalExpression, IVisitContext, RelationalExpression> _selectionBuilder;
            public DowncastingVisitor(SourceDowncastVisitor<TAbstraction> composition, Func<RelationalExpression, IVisitContext, RelationalExpression> selectionBuilder)
                : base(composition)
            {
                _selectionBuilder = selectionBuilder;
            }

            #region implemented abstract members of NestedVisitorBase

            protected override RelationalExpression Visit(SourceDowncast<TAbstraction, TEntity> target, IVisitContext context, SourceDowncastVisitor<TAbstraction> outerVisitor)
            {
                RelationalExpression source = outerVisitor.VisitInner(target.Source, context);
                return _selectionBuilder(source, context);
            }

            #endregion
        }

        #endregion nested visitors
    }
}

