//
//  Negation.cs
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

namespace Epic.Specifications
{
    /// <summary>
    /// Specification that is satisfied by any <typeparamref name="TCandidate"/> that doesn't satisfy the negated one.
    /// </summary>
    /// <typeparam name="TCandidate">The type of the objects that can be tested with this specification.</typeparam>
    [Serializable]
    public sealed class Negation<TCandidate> : SpecificationBase<Negation<TCandidate>, TCandidate>,
                                               IEquatable<Negation<TCandidate>>
        where TCandidate : class
    {
        private readonly ISpecification<TCandidate> _negated;
        /// <summary>
        /// Initialize a new <see cref="Negation{TCandidate}"/> that is satisfied by any <typeparamref name="TCandidate"/>
        /// that does not satisfy <paramref name="specification"/>.
        /// </summary>
        /// <param name="specification">Specification to negate.</param>
        public Negation (ISpecification<TCandidate> specification)
        {
            if(null == specification)
                throw new ArgumentNullException("specification");
            _negated = specification;
        }

        /// <summary>
        /// Negated specification.
        /// </summary>
        public ISpecification<TCandidate> Negated
        {
            get
            {
                return _negated;
            }
        }

        #region implemented abstract members of Epic.Specifications.SpecificationBase
        /// <summary>
        /// Determine whether the current <see cref="Negation{TCandidate}"/> negates the same
        /// <see cref="ISpecification{TCandidate}"/> that <paramref name="otherSpecification"/> negates.
        /// </summary>
        /// <param name="otherSpecification">Another negation.</param>
        /// <returns><see langword="true"/> if the <paramref name="otherSpecification"/> negates
        /// the same specification that the current instance negates, <see langword="false"/> otherwise.</returns>
        protected override bool EqualsA (Negation<TCandidate> otherSpecification)
        {
            return _negated.Equals(otherSpecification._negated);
        }

        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="candidate"/> does not satisfy <see cref="Negated"/>.
        /// </summary>
        /// <param name="candidate">A candidate.</param>
        /// <returns><see langword="true"/> if <paramref name="candidate"/> does not satisfy <see cref="Negated"/>, <see langword="false"/> otherwise.</returns>
        protected override bool IsSatisfiedByA (TCandidate candidate)
        {
            return !_negated.IsSatisfiedBy(candidate);
        }

        /// <summary>
        /// Set <see cref="Negated"/> in <paramref name="negation"/>.
        /// </summary>
        /// <param name="negation">The negation of the current specification.</param>
        protected override void BuildNegation (out ISpecification<TCandidate> negation)
        {
            negation = _negated;
        }


        #endregion
    }
}

