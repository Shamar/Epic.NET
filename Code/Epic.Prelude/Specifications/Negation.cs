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
        public Negation (ISpecification<TCandidate> specification)
        {
            if(null == specification)
                throw new ArgumentNullException("specification");
            _negated = specification;
        }

        public ISpecification<TCandidate> Negated
        {
            get
            {
                return _negated;
            }
        }

        #region implemented abstract members of Epic.Specifications.SpecificationBase
        protected override bool EqualsA (Negation<TCandidate> otherSpecification)
        {
            return _negated.Equals(otherSpecification._negated);
        }

        protected override bool IsSatisfiedByA (TCandidate candidate)
        {
            return !_negated.IsSatisfiedBy(candidate);
        }

        protected override void BuildNegation (out ISpecification<TCandidate> negation)
        {
            negation = _negated;
        }


        #endregion
    }
}

