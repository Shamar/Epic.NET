//
//  ConjunctionDistributor.cs
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
using System.Collections.Generic;

namespace Epic.Specifications.Visitors
{
    internal sealed class ConjunctionDistributor<TCandidate> : CompositeVisitor<ISpecification<TCandidate>>.VisitorBase, IVisitor<ISpecification<TCandidate>, ISpecification>
        where TCandidate : class
    {
        public ConjunctionDistributor(CompositeVisitor<ISpecification<TCandidate>> composition)
            : base(composition)
        {
        }

        #region IVisitor implementation
        public ISpecification<TCandidate> Visit(ISpecification target, IVisitContext context)
        {
            ISpecification<TCandidate> toAnalyze = ContinueVisit(target, context);
            return Analyze(toAnalyze);
        }
        #endregion

        private static ISpecification<TCandidate> Analyze(ISpecification<TCandidate> toAnalyze)
        {
            Conjunction<TCandidate> toDistributeOverDisjunctions = toAnalyze as Conjunction<TCandidate>;
            if(null == toDistributeOverDisjunctions)
                return toAnalyze; // nothing to do
            List<Disjunction<TCandidate>> disjunctions = new List<Disjunction<TCandidate>>();
            List<ISpecification<TCandidate>> otherSpecificationsToDistribute = new List<ISpecification<TCandidate>>();
            foreach(ISpecification<TCandidate> spec in toDistributeOverDisjunctions)
            {
                Disjunction<TCandidate> disjunction = spec as Disjunction<TCandidate>;
                if (null == disjunction)
                    otherSpecificationsToDistribute.Add(spec);
                else
                    disjunctions.Add(disjunction);
            }
            ISpecification<TCandidate> remainder = Any<TCandidate>.Specification;
            if(otherSpecificationsToDistribute.Count > 0)
            {
                foreach(var spec in otherSpecificationsToDistribute)
                {
                    remainder = remainder.And(spec);
                }
            }

            switch(disjunctions.Count)
            {
                case 0:
                    return toAnalyze;
                case 1:
                    ISpecification<TCandidate> result = No<TCandidate>.Specification;
                    foreach(var spec in disjunctions[0])
                    {
                        result = result.Or(remainder.And(spec));
                    }
                    return result;
                default:
                    break;
            }
        }
    }
}

