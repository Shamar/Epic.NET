//
//  SourceVisitor.cs
//
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
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
using Epic.Query.Relational;
using Epic.Query.Relational.Operations;
using Epic.Query.Object.Expressions;

namespace Epic.Query.Object.Relational.Visitors
{
    /// <summary>
    /// Source visitor.
    /// </summary>
    /// <remarks>
    /// During the visit it will apply any <see cref="System.Action{IRepository}"/> 
    /// registered in the <see cref="IVisitContext"/>.
    /// </remarks>
    public sealed class SourceVisitor<TEntity, TIdentity> : CompositeVisitor<RelationalExpression>.VisitorBase, IVisitor<RelationalExpression, Source<TEntity, TIdentity>> where TEntity : class where TIdentity : IEquatable<TIdentity>
    {
        private readonly Func<Source<TEntity, TIdentity>, IVisitContext, Projection> _translation;
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceVisitor{TEntity, TIdentity}"/> class.
        /// </summary>
        /// <param name="composition">Composition.</param>
        /// <param name="translation">Function that translate a <see cref="Source{TEntity, TIdentity}"/> 
        /// into a relational <see cref="Projection"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="composition"/> or <paramref name="translation"/> is <see langword="null"/>.</exception>
        public SourceVisitor(CompositeVisitor<RelationalExpression> composition, Func<Source<TEntity, TIdentity>, IVisitContext, Projection> translation)
            : base(composition)
        {
            if (null == translation)
                throw new ArgumentNullException("translation");
            _translation = translation;
        }

        #region IVisitor implementation

        RelationalExpression IVisitor<RelationalExpression, Source<TEntity, TIdentity>>.Visit(Source<TEntity, TIdentity> target, IVisitContext context)
        {
            context.ApplyTo(target.Repository); // the visitor client probably need the repository.
            return _translation(target, context);
        }

        #endregion
    }
}

