//
//  DNFConverterQA.cs
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
using Epic.Visitors;
using System;
using NUnit.Framework;
using A = Epic.Fakes.FakeCandidate2;
using B = Epic.Fakes.FakeCandidate1Abstraction;
using C = Epic.Fakes.FakeCandidate1;
using D = Epic.Fakes.FakeCandidate1Specialization;
using D2 = Epic.Fakes.FakeCandidate1UnrelatedSpecialization;

namespace Epic.Specifications.Visitors
{
    [TestFixture]
    public class DNFConverterQA
    {
        public static readonly ISpecification<A> P = new NamedPredicate<A>("P");
        public static readonly ISpecification<B> Q = new NamedPredicate<B>("Q");
        public static readonly ISpecification<C> R = new NamedPredicate<C>("R");
        public static readonly ISpecification<C> S = new NamedPredicate<C>("S");
        public static readonly ISpecification<C> T = new NamedPredicate<C>("T");
        public static readonly ISpecification<D> U = new NamedPredicate<D>("U");
        public static readonly ISpecification<D2> V = new NamedPredicate<D2>("V");

        [Test]
        public void Initialization_withAName_works()
        {
            // arrange:
            string name = "Test";

            // act:
            new DNFConverter<B>(name);
        }

        [Test]
        public void Visit_aNegatedDisjunction_applyDeMorganLaws()
        {
            // arrange:
            var toTest = new DNFConverter<B>("Test");
            var toVisit = R.And(S).Negate();
            var expected = R.OfType<B>().Negate().Or(S.OfType<B>().Negate());

            // act:
            var result = toVisit.Accept(toTest, VisitContext.New);

            // assert:
            Assert.AreEqual(expected, result);
        }
    }

    #region utilities
    internal class NamedPredicate<T> : Specifications.SpecificationBase<NamedPredicate<T>, T>, IEquatable<NamedPredicate<T>> where T : class
    {
        public readonly string Name;
        public NamedPredicate(string name)
        {
            Name = name;
        }
        #region implemented abstract members of SpecificationBase
        protected override bool EqualsA(NamedPredicate<T> otherSpecification)
        {
            return Name.Equals(otherSpecification.Name);
        }
        protected override bool IsSatisfiedByA(T candidate)
        {
            throw new NotImplementedException("This is a mock.");
        }
        #endregion
    }
    #endregion utilities
}

