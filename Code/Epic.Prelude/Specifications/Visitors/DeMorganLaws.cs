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

namespace Epic.Specifications.Visitors
{
    /// <summary>
    /// Applies De Morgan laws to obtain a Negation normal form to a <seealso cref="ISpecification{TCandidate}"/>.
    /// </summary>
    /// <typeparam name="TCandidate">Type of the candidate.</typeparam>
    /// <seealso href="http://en.wikipedia.org/wiki/Negation_normal_form"/>
    internal sealed class DeMorganLaws<TCandidate> : CompositeVisitor<ISpecification>.VisitorBase, IVisitor<ISpecification, IMonadicSpecificationComposition<TCandidate>>
        where TCandidate : class
    {
        public DeMorganLaws(CompositeVisitor<ISpecification> composition)
            : base(composition)
        {
        }

        protected override IVisitor<ISpecification, TExpression> AsVisitor<TExpression>(TExpression target)
        {
            IVisitor<ISpecification, TExpression> result = base.AsVisitor(target);

            if (null != result)
            {
                var monadic = target as IMonadicSpecificationComposition<TCandidate>;
                if(!(monadic.Operand is IPolyadicSpecificationComposition<TCandidate>))
                    result = null;
            }

            return result;
        }


        #region IVisitor implementation
        ISpecification IVisitor<ISpecification, IMonadicSpecificationComposition<TCandidate>>.Visit(IMonadicSpecificationComposition<TCandidate> target, IVisitContext context)
        {
            var operand = target.Operand as IPolyadicSpecificationComposition<TCandidate>;
            ISpecification<TCandidate> result = null;

            Func<ISpecification<TCandidate>, ISpecification<TCandidate>, ISpecification<TCandidate>> distributionLaw;

            if(operand.SpecificationType.GetGenericTypeDefinition().Equals(typeof(Disjunction<>)))
            {
                distributionLaw = (r, s) => null == r ? r : r.And(s.Negate());
            }
            else
            {
                distributionLaw = (r, s) => null == r ? r : r.Or(s.Negate());
            }
            foreach(ISpecification<TCandidate> specificationToVisit in operand.Operands)
            {
                ISpecification<TCandidate> specification = VisitInner(specificationToVisit, context) as ISpecification<TCandidate>;
                result = distributionLaw(result, specification);
            }

            return result;
        }
        #endregion
    }
}

