//
//  SampleSpecification.cs
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
using Epic.Specifications;

namespace Epic.Fakes
{
    public interface ISampleSpecification<TCandidate> : ISpecification<TCandidate>, IEquatable<ISampleSpecification<TCandidate>>
        where TCandidate : class
    {
    }

    public interface ISampleSpecification : ISpecification<FakeCandidate1>, ISpecification<FakeCandidate2>,
                                            IEquatable<ISampleSpecification>
    {
    }


    [Serializable]
    public class SampleSpecification<TCandidate1> : Specifications.SpecificationBase<ISampleSpecification<TCandidate1>,TCandidate1>,
                                                   ISampleSpecification<TCandidate1>
        where TCandidate1 : class
    {
        public int EqualsACalled;
        public int IsSatisfiedByACalled;
        private readonly Func<TCandidate1, bool> _satifactionRule;
        private readonly Func<ISampleSpecification<TCandidate1>, ISampleSpecification<TCandidate1>, bool> _equalityComparison;
        public SampleSpecification (Func<ISampleSpecification<TCandidate1>, ISampleSpecification<TCandidate1>, bool> equalityComparison)
        {
            _equalityComparison = equalityComparison;
        }

        public SampleSpecification(Func<TCandidate1, bool> satifactionRule)
        {
            _satifactionRule = satifactionRule;
        }

        public SampleSpecification ()
        {
        }

        #region implemented abstract members of Epic.Specifications.SpecificationBase
        protected override bool EqualsA (ISampleSpecification<TCandidate1> otherSpecification)
        {
            EqualsACalled++;
            return _equalityComparison(this, otherSpecification);
        }

        protected override bool IsSatisfiedByA (TCandidate1 candidate)
        {
            IsSatisfiedByACalled++;
            return _satifactionRule(candidate);
        }
        #endregion
    }

    public class DerivedSampleSpecification<TCandidate> : SampleSpecification<TCandidate>
        where TCandidate : class
    {
    }

    [Serializable]
    public class SampleSpecification : Specifications.SpecificationBase<ISampleSpecification, FakeCandidate2, FakeCandidate1>,
                                ISampleSpecification
    {
        public SampleSpecification ()
        {
        }

        protected override bool EqualsA (ISampleSpecification otherSpecification)
        {
            return true;
        }

        public int IsSatisfiedByAFakeCandidate1Calls = 0;
        public int IsSatisfiedByAFakeCandidate2Calls = 0;
        public Func<FakeCandidate1, bool> SatifactionRule;

        #region implemented abstract members of Epic.Specifications.SpecificationBase
        protected override bool IsSatisfiedByA (FakeCandidate1 candidate)
        {
            IsSatisfiedByAFakeCandidate1Calls++;
            return SatifactionRule(candidate);
        }
        protected override bool IsSatisfiedByA (FakeCandidate2 candidate)
        {
            IsSatisfiedByAFakeCandidate2Calls++;
            return true;
        }

        #endregion
    }

}

