//
//  DeMorganLaws.cs
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
using System.Collections.Concurrent;

namespace Epic.Specifications.Visitors
{
    /// <summary>
    /// Applies De Morgan laws to obtain a Negation normal form to a <seealso cref="ISpecification{TCandidate}"/>.
    /// </summary>
    /// <typeparam name="TCandidate">Type of the candidate.</typeparam>
    /// <seealso href="http://en.wikipedia.org/wiki/Negation_normal_form"/>
    internal sealed class DeMorganLaws<TCandidate> : CompositeVisitor<ISpecification<TCandidate>>.VisitorBase, IVisitor<ISpecification<TCandidate>, IMonadicSpecificationComposition<TCandidate>>
        where TCandidate : class
    {
        public DeMorganLaws(CompositeVisitor<ISpecification<TCandidate>> composition)
            : base(composition)
        {
        }

        #region IVisitor implementation
        ISpecification<TCandidate> IVisitor<ISpecification<TCandidate>, IMonadicSpecificationComposition<TCandidate>>.Visit(IMonadicSpecificationComposition<TCandidate> target, IVisitContext context)
        {
            ISpecification<TCandidate> toAnalyze = ContinueVisit(target, context);
            return Analyze(toAnalyze);
        }
        #endregion

        private ISpecification<TCandidate> Analyze(ISpecification<TCandidate> toAnalyze)
        {
            Negation<TCandidate> negation = toAnalyze as Negation<TCandidate>;
            Conjunction<TCandidate> negatedConjunction = null == negation ? null : negation.Negated as Conjunction<TCandidate>;
            Disjunction<TCandidate> negatedDisjunction = null == negation ? null : negation.Negated as Disjunction<TCandidate>;
            if(null == negation || (null == negatedConjunction && null == negatedDisjunction))
               return toAnalyze;

            if(null != negatedConjunction)
            {
                return DisjunctNegationsOf(negatedConjunction);
            }
            else
            {
                return ConjunctNegationsOf(negatedDisjunction);
            }
        }

        private ISpecification<TCandidate> DisjunctNegationsOf(IEnumerable<ISpecification<TCandidate>> specifications)
        {
            ISpecification<TCandidate> result = No<TCandidate>.Specification;
            foreach(var spec in specifications)
            {
                result = result.Or(VisitInner(spec.Negate()));
            }
            return result;
        }

        private ISpecification<TCandidate> ConjunctNegationsOf(IEnumerable<ISpecification<TCandidate>> specifications)
        {
            ISpecification<TCandidate> result = Any<TCandidate>.Specification;
            foreach(var spec in specifications)
            {
                result = result.And(VisitInner(spec.Negate()));
            }
            return result;
        }
    }
}

