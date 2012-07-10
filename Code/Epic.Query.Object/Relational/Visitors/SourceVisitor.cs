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
    public sealed class SourceVisitor<TEntity, TIdentity> : CompositeVisitor<Relation>.VisitorBase, 
                                                     IVisitor<Relation, Source<TEntity, TIdentity>>
        where TEntity : class
        where TIdentity : IEquatable<TIdentity>
    {
        private readonly Relation _relation;
        public SourceVisitor (CompositeVisitor<Relation> composition, Relation relation)
            : base(composition)
        {
            if (relation == null)
                throw new ArgumentNullException ("relation");
            _relation = relation;
        }

        #region IVisitor implementation
        public Relation Visit (Source<TEntity, TIdentity> target, IVisitContext context)
        {
            return _relation;
        }
        #endregion
    }
}

