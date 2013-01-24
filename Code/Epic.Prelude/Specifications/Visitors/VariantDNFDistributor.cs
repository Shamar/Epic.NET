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
    internal sealed class VariantDNFDistributor<TCandidate> : VariantVisitorBase<ISpecification<TCandidate>, TCandidate> where TCandidate : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariantDNFDistributor{TCandidate}"/> class.
        /// </summary>
        /// <param name="composition">Composition.</param>
        public VariantDNFDistributor(CompositeVisitor<ISpecification<TCandidate>> composition)
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
        protected override ISpecification<TCandidate> VisitDowncastingVariant<FromCandidate, ToCandidate>(Variant<FromCandidate, ToCandidate> target, IVisitContext context)
        {
            ISpecification<TCandidate> innerSpecification = VisitInner(target.InnerSpecification, context);
            Disjunction<TCandidate> disjunction = innerSpecification as Disjunction<TCandidate>;
            if(null != disjunction)
                return DistributeDowncastOverDisjunction<ToCandidate>(disjunction, context);
            // negations, conjunctions and any other specifications works the same:
            return Any<ToCandidate>.Specification.OfType<TCandidate>().And(innerSpecification);
        }

        /// <summary>
        /// Visits the upcasting variant.
        /// </summary>
        /// <returns>The upcasting variant.</returns>
        /// <param name="target">Target.</param>
        /// <param name="context">Context.</param>
        /// <typeparam name="FromCandidate">The 1st type parameter.</typeparam>
        /// <typeparam name="ToCandidate">The 2nd type parameter.</typeparam>
        protected override ISpecification<TCandidate> VisitUpcastingVariant<FromCandidate, ToCandidate>(Variant<FromCandidate, ToCandidate> target, IVisitContext context)
        {
            return VisitInner(target.InnerSpecification, context);
        }

        #endregion implemented abstract members of VariantVisitorBase

        private ISpecification<TCandidate> DistributeDowncastOverDisjunction<ToCandidate>(Disjunction<TCandidate> disjunction, IVisitContext context) where ToCandidate : class, TCandidate 
        {
            // apply De Morgan's laws here
            ISpecification<TCandidate> result = null;
            ISpecification<TCandidate> anyToCandidate = Any<ToCandidate>.Specification.OfType<TCandidate>();
            foreach(ISpecification<TCandidate> specification in disjunction)
            {
                if(null == result)
                    result = anyToCandidate.And(specification);
                else
                    result = result.Or(anyToCandidate.And(specification));
            }
            return result;
        }
    }
}

