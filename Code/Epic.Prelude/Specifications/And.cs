//
//  And.cs
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
    [Serializable]
    public sealed class And<TCandidate> : SpecificationBase<And<TCandidate>, TCandidate>,
                                          IEquatable<And<TCandidate>>
        where TCandidate : class
    {
        private readonly ISpecification<TCandidate>[] _specifications;
        public And (params ISpecification<TCandidate>[] specifications)
        {
            _specifications = specifications.Clone() as ISpecification<TCandidate>;
        }

        #region implemented abstract members of Epic.Specifications.SpecificationBase
        protected override bool EqualsA (And<TCandidate> otherSpecification)
        {
            if(_specifications.Length != otherSpecification._specifications.Length)
                return false;
            for(int i = 0; i < _specifications.Length; ++i)
                if(!_specifications[i].Equals(otherSpecification._specifications[i]))
                    return false;
            return true;
        }

        protected override bool IsSatisfiedByA (TCandidate candidate)
        {
            for(int i = 0; i < _specifications.Length; ++i)
                if(!_specifications[i].IsSatisfiedBy(candidate))
                    return false;
            return true;
        }

        #endregion
    }
}

