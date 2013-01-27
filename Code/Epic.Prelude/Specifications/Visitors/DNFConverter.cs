//
//  Normalizer.cs
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
using Epic.Visitors;

namespace Epic.Specifications.Visitors
{
    /// <summary>
    /// Converts a <see cref="ISpecification"/> in the disjunctive normal form.
    /// </summary>
    /// <typeparam name="TCandidate">The type of the objects that can be tested with the specifications produced by this visitor.</typeparam>
    public class DNFConverter<TCandidate> : CompositeVisitorBase<ISpecification<TCandidate>, ISpecification>
        where TCandidate : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DNFConverter{TCandidate}"/> class.
        /// </summary>
        /// <param name="name">Name of the converter.</param>
        public DNFConverter(string name)
            : base(name)
        {
            // unknown specifications are returned upcasted as ISpecification<TCandidate>
            new UpcastingVisitor<TCandidate>(this);
            // variants are distributed according to their semantics
            new VariantDNFDistributor<TCandidate>(this);
            // negations are moved inside applying De Morgan's laws
            new DeMorganLaws<TCandidate>(this);
            // apply distributive law
            new ConjunctionDistributor<TCandidate>(this);
        }

        #region implemented abstract members of CompositeVisitorBase

        /// <summary>
        /// Initializes the context of the visit.
        /// </summary>
        /// <returns>The visit context.</returns>
        /// <param name="target">Target.</param>
        /// <param name="context">Context.</param>
        protected override IVisitContext InitializeVisitContext(ISpecification target, IVisitContext context)
        {
            return context;
        }

        #endregion
    }
}

