//
//  LogicOperatorVisitor.cs
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

namespace Epic.Specifications.Visitors
{
    internal sealed class LogicOperatorVisitor<TCandidate> : CompositeVisitor<ISpecification<TCandidate>>.VisitorBase, IVisitor<ISpecification<TCandidate>, Conjunction<TCandidate>>, IVisitor<ISpecification<TCandidate>, Disjunction<TCandidate>>, IVisitor<ISpecification<TCandidate>, Negation<TCandidate>>
        where TCandidate : class
    {
        public LogicOperatorVisitor(CompositeVisitor<ISpecification<TCandidate>> composition)
            : base(composition)
        {
        }

        ISpecification<TCandidate> IVisitor<ISpecification<TCandidate>, Conjunction<TCandidate>>.Visit(Conjunction<TCandidate> target, IVisitContext context)
        {
            ISpecification<TCandidate> result = Any<TCandidate>.Specification;
            foreach (var inner in target)
            {
                result = result.And(VisitInner(inner, context));
            }
            return result;
        }

        ISpecification<TCandidate> IVisitor<ISpecification<TCandidate>, Disjunction<TCandidate>>.Visit(Disjunction<TCandidate> target, IVisitContext context)
        {
            ISpecification<TCandidate> result = No<TCandidate>.Specification;
            foreach (var inner in target)
            {
                result = result.Or(VisitInner(inner, context));
            }
            return result;
        }

        ISpecification<TCandidate> IVisitor<ISpecification<TCandidate>, Negation<TCandidate>>.Visit(Negation<TCandidate> target, IVisitContext context)
        {
            return VisitInner(target.Negated, context).Negate();
        }
    }
}
