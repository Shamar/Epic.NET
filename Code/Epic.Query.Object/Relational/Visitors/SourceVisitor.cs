//
//  SourceVisitor.cs
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
using Epic.Query.Object.Expressions;

namespace Epic.Query.Object.Relational.Visitors
{
    /// <summary>
    /// Visitor of <see cref="Source{TEntity, TIdentity}"/> that produce the right 
    /// <see cref="RelationalExpression"/> for the <see cref="IRepository{TEntity, TIdentity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    /// <typeparam name="TIdentity">Type of the identity of <typeparamref name="TEntity"/>.</typeparam>
    public sealed class SourceVisitor<TEntity, TIdentity> : CompositeVisitor<RelationalExpression>.VisitorBase, 
                                                     IVisitor<RelationalExpression, Source<TEntity, TIdentity>>
        where TEntity : class
        where TIdentity : IEquatable<TIdentity>
    {
        private readonly RelationalExpression _relation;

        /// <summary>
        /// Initializes a new <see cref="SourceVisitor{TEntity, TIdentity}"/> as part of the <paramref name="composition"/>.
        /// </summary>
        /// <param name="composition">Composite visitor to enhance.</param>
        /// <param name="relation">Relation for the <see cref="IRepository{TEntity, TIdentity}"/>.</param>
        public SourceVisitor(CompositeVisitor<RelationalExpression> composition, RelationalExpression relation)
            : base(composition)
        {
            if (relation == null)
                throw new ArgumentNullException ("relation");
            _relation = relation;
        }

        #region IVisitor implementation
        RelationalExpression IVisitor<RelationalExpression, Source<TEntity, TIdentity>>.Visit(Source<TEntity, TIdentity> target, IVisitContext context)
        {
            return _relation;
        }
        #endregion
    }
}

