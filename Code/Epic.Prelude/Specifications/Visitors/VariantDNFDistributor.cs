//
//  VariantDNFDistributor.cs
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
    /// Variant DNF distributor.
    /// </summary>
    internal sealed class VariantDNFDistributor<TCandidate> : VariantVisitorBase<ISpecification, TCandidate> where TCandidate : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariantDNFDistributor{TCandidate}"/> class.
        /// </summary>
        /// <param name="composition">Composition.</param>
        public VariantDNFDistributor(CompositeVisitor<ISpecification> composition)
            : base(composition)
        {
        }

        #region implemented abstract members of VariantVisitorBase

        /// <summary>
        /// Visits the downcasting variant.
        /// </summary>
        /// <returns>The downcasting variant.</returns>
        /// <param name="target">Target.</param>
        /// <param name="context">Context.</param>
        /// <typeparam name="FromCandidate">The 1st type parameter.</typeparam>
        /// <typeparam name="ToCandidate">The 2nd type parameter.</typeparam>
        protected override ISpecification VisitDowncastingVariant<FromCandidate, ToCandidate>(Variant<FromCandidate, ToCandidate> target, IVisitContext context)
        {
            ISpecification<FromCandidate> innerSpecification = VisitInner(target.InnerSpecification, context);
            Conjunction<FromCandidate> conjunction = innerSpecification as Conjunction<FromCandidate>;
            if(null != conjunction)
                return DistributeDowncastOverConjunction<FromCandidate, ToCandidate>(conjunction, context);
            Disjunction<FromCandidate> disjunction = innerSpecification as Disjunction<FromCandidate>;
            if(null != disjunction)
                return DistributeDowncastOverDisjunction<FromCandidate, ToCandidate>(disjunction, context);
            Negation<TCandidate> negation = innerSpecification as Negation<FromCandidate>;
            if (null != negation)
                return DistributeDowncastOverNegation<FromCandidate, ToCandidate>(negation, context);
            return innerSpecification.OfType<ToCandidate>();
        }

        /// <summary>
        /// Visits the upcasting variant.
        /// </summary>
        /// <returns>The upcasting variant.</returns>
        /// <param name="target">Target.</param>
        /// <param name="context">Context.</param>
        /// <typeparam name="FromCandidate">The 1st type parameter.</typeparam>
        /// <typeparam name="ToCandidate">The 2nd type parameter.</typeparam>
        protected override ISpecification VisitUpcastingVariant<FromCandidate, ToCandidate>(Variant<FromCandidate, ToCandidate> target, IVisitContext context)
        {
            ISpecification<FromCandidate> innerSpecification = VisitInner(target.InnerSpecification, context);
            Conjunction<FromCandidate> conjunction = innerSpecification as Conjunction<FromCandidate>;
            if(null != conjunction)
                return DistributeUpcastOverConjunction<FromCandidate, ToCandidate>(conjunction, context);
            Disjunction<FromCandidate> disjunction = innerSpecification as Disjunction<FromCandidate>;
            if(null != disjunction)
                return DistributeUpcastOverDisjunction<FromCandidate, ToCandidate>(disjunction, context);
            return innerSpecification.OfType<ToCandidate>();
        }

        #endregion implemented abstract members of VariantVisitorBase
        
        private ISpecification<ToCandidate> DistributeDowncastOverConjunction<FromCandidate, ToCandidate>(Conjunction<FromCandidate> conjunction, IVisitContext context)
        {
            throw new NotImplementedException();
        }
        
        private ISpecification<ToCandidate> DistributeDowncastOverDisjunction<FromCandidate, ToCandidate>(Disjunction<FromCandidate> disjunction, IVisitContext context)
        {
            throw new NotImplementedException();
        }
        
        private ISpecification<ToCandidate> DistributeDowncastOverNegation<FromCandidate, ToCandidate>(Negation<FromCandidate> negation, IVisitContext context)
        {
            throw new NotImplementedException();
        }

        private ISpecification<ToCandidate> DistributeUpcastOverDisjunction<FromCandidate, ToCandidate>(Disjunction<FromCandidate> conjunction, IVisitContext context)
        {
            ISpecification<ToCandidate> result = null;
            foreach(ISpecification<FromCandidate> specification in conjunction)
            {
                if(null == result)
                    result = specification.OfType<ToCandidate>();
                else
                    result = result.Or(specification.OfType<ToCandidate>());
            }
            return result;
        }

        private ISpecification<ToCandidate> DistributeUpcastOverConjunction<FromCandidate, ToCandidate>(Conjunction<FromCandidate> conjunction, IVisitContext context)
        {
            ISpecification<ToCandidate> result = null;
            foreach(ISpecification<FromCandidate> specification in conjunction)
            {
                if(null == result)
                    result = specification.OfType<ToCandidate>();
                else
                    result = result.And(specification.OfType<ToCandidate>());
            }
            return result;
        }
    }
}

