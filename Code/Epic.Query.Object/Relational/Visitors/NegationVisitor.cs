//
//  NegationVisitor.cs
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

namespace Epic.Query.Object.Relational.Visitors
{
    public class NegationVisitor<TEntity> : CompositeVisitor<Predicate>.VisitorBase, 
                                            IVisitor<Predicate, Negation<TEntity>>
        where TEntity : class
    {
        public NegationVisitor (CompositeVisitor<Predicate> composition)
            : base(composition)
        {
        }
        
        #region IVisitor implementation
        public Predicate Visit (Negation<TEntity> target, IVisitContext context)
        {
            Predicate inner = VisitInner(target.Negated, context);
            if (null == inner)
                return null;
            return new Not(inner);
        }
        #endregion
    }
}

