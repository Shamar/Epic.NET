//
//  DisjunctionQA.cs
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
using NUnit.Framework;
using System;
using Rhino.Mocks;
using System.Linq;
using System.Collections.Generic;

namespace Epic.Specifications
{
    [TestFixture()]
    public class DisjunctionQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutAnyArgument_throwsArgumentNullException ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> inner = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>>();

            // assert:
            Assert.Throws<ArgumentNullException>(delegate() {
                new Disjunction<Fakes.FakeCandidate1>(null, inner);
            });
            Assert.Throws<ArgumentNullException>(delegate() {
                new Disjunction<Fakes.FakeCandidate1>(inner, null);
            });
        }

        [Test]
        public void Initialize_withTwoEqualsSpecification_throwsArgumentException ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> inner = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>>();
            inner.Expect(s => s.Equals(inner)).Return(true).Repeat.AtLeastOnce();

            // assert:
            Assert.Throws<ArgumentException>(delegate() {
                new Disjunction<Fakes.FakeCandidate1>(inner, inner);
            });
        }

        [Test]
        public void Initialize_withTwoDifferentSpecifications_works ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> first = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>>();
            ISpecification<Fakes.FakeCandidate1> second = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>>();
            first.Expect(s => s.Equals(second)).Return(false).Repeat.AtLeastOnce();


            // act:
            var toTest = new Disjunction<Fakes.FakeCandidate1>(first, second);

            // assert:
            Assert.IsNotNull(toTest);
            Assert.AreEqual(2, toTest.NumberOfSpecifications);
            Assert.AreSame(first, toTest.ElementAt(0));
            Assert.AreSame(second, toTest.ElementAt(1));
            CollectionAssert.AreEquivalent(new Object[] { first, second }, toTest as System.Collections.IEnumerable);
            CollectionAssert.AreEquivalent(new ISpecification<Fakes.FakeCandidate1>[] { first, second }, toTest as IEnumerable<ISpecification<Fakes.FakeCandidate1>>);
        }

        [Test]
        public void Initialize_withASpecificationAndADisjunction_createADisjunctionThatSequenceThemAfterFirst ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> first = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            ISpecification<Fakes.FakeCandidate1> second = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            ISpecification<Fakes.FakeCandidate1> third = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            second.Expect(s => s.Equals(third)).Return(false).Repeat.Any();
            var other = new Disjunction<Fakes.FakeCandidate1>(second, third);
            first.Expect(s => s.ToString()).Return("first").Repeat.Any();
            second.Expect(s => s.ToString()).Return("second").Repeat.Any();
            third.Expect(s => s.ToString()).Return("third").Repeat.Any();
            first.Expect(s => s.Equals(other)).Return(false).Repeat.Any();
            first.Expect(s => s.Equals(second)).Return(false).Repeat.Any();
            first.Expect(s => s.Equals(third)).Return(false).Repeat.Any();

            // act:
            var toTest = new Disjunction<Fakes.FakeCandidate1>(first, other);

            // assert:
            Assert.AreEqual(3, toTest.Count());
            Assert.AreEqual(3, toTest.NumberOfSpecifications);
            Assert.AreSame(first, toTest.ElementAt(0));
            Assert.AreSame(second, toTest.ElementAt(1));
            Assert.AreSame(third, toTest.ElementAt(2));
        }

        [Test]
        public void Or_aKnownSpecification_returnItself ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> first = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>>();
            ISpecification<Fakes.FakeCandidate1> second = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>>();
            first.Expect(s => s.Equals(second)).Return(false).Repeat.AtLeastOnce();
            second.Expect(s => s.Equals(first)).Return(false).Repeat.AtLeastOnce();
            second.Expect(s => s.Equals(second)).Return(true).Repeat.AtLeastOnce();
            var toTest = new Disjunction<Fakes.FakeCandidate1>(first, second);

            // act:
            var result = toTest.Or(second);

            // assert:
            Assert.AreSame(toTest, result);
        }

        [Test]
        public void Or_anUnknownSpecification_returnANewConjuction ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> first = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>>();
            ISpecification<Fakes.FakeCandidate1> second = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>>();
            ISpecification<Fakes.FakeCandidate1> third = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>>();
            first.Expect(s => s.Equals(second)).Return(false).Repeat.AtLeastOnce();
            third.Expect(s => s.Equals(first)).Return(false).Repeat.AtLeastOnce();
            third.Expect(s => s.Equals(second)).Return(false).Repeat.AtLeastOnce();
            var toTest = new Disjunction<Fakes.FakeCandidate1>(first, second);
            
            // act:
            var result = toTest.Or(third) as Disjunction<Fakes.FakeCandidate1>;
            
            // assert:
            Assert.IsNotNull(result);
            Assert.AreSame(first, result.ElementAt(0));
            Assert.AreSame(second, result.ElementAt(1));
            Assert.AreSame(third, result.ElementAt(2));
        }

        [Test]
        public void Or_anotherConjunction_sequenceTheOtherAfterThis ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> first = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            ISpecification<Fakes.FakeCandidate1> second = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            ISpecification<Fakes.FakeCandidate1> third = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            ISpecification<Fakes.FakeCandidate1> forth = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            first.Expect(s => s.ToString()).Return("first").Repeat.Any();
            second.Expect(s => s.ToString()).Return("second").Repeat.Any();
            third.Expect(s => s.ToString()).Return("third").Repeat.Any();
            forth.Expect(s => s.ToString()).Return("forth").Repeat.Any();
            first.Expect(s => s.Equals(second)).Return(false).Repeat.Any();
            first.Expect(s => s.Equals(third)).Return(false).Repeat.Any();
            first.Expect(s => s.Equals(forth)).Return(false).Repeat.Any();
            third.Expect(s => s.Equals(first)).Return(false).Repeat.Any();
            third.Expect(s => s.Equals(second)).Return(false).Repeat.Any();
            third.Expect(s => s.Equals(forth)).Return(false).Repeat.Any();
            forth.Expect(s => s.Equals(first)).Return(false).Repeat.Any();
            forth.Expect(s => s.Equals(second)).Return(false).Repeat.Any();
            var toTest = new Disjunction<Fakes.FakeCandidate1>(first, second);
            var other = new Disjunction<Fakes.FakeCandidate1>(third, forth);


            // act:
            var result = toTest.Or(other) as Disjunction<Fakes.FakeCandidate1>;

            // assert:
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.NumberOfSpecifications);
            Assert.AreEqual(4, result.Count());
            Assert.AreSame(first, result.ElementAt(0));
            Assert.AreSame(second, result.ElementAt(1));
            Assert.AreSame(third, result.ElementAt(2));
            Assert.AreSame(forth, result.ElementAt(3));
        }

        [Test]
        public void Or_anotherConjunction_removeDuplicatedSpecifications ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> first = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            ISpecification<Fakes.FakeCandidate1> second = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            ISpecification<Fakes.FakeCandidate1> third = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            first.Expect(s => s.ToString()).Return("first").Repeat.Any();
            second.Expect(s => s.ToString()).Return("second").Repeat.Any();
            third.Expect(s => s.ToString()).Return("third").Repeat.Any();
            first.Expect(s => s.Equals(second)).Return(false).Repeat.Any();
            first.Expect(s => s.Equals(third)).Return(false).Repeat.Any();
            second.Expect(s => s.Equals(second)).Return(true).Repeat.Any();
            second.Expect(s => s.Equals(first)).Return(false).Repeat.Any();
            second.Expect(s => s.Equals(third)).Return(false).Repeat.Any();
            third.Expect(s => s.Equals(first)).Return(false).Repeat.Any();
            third.Expect(s => s.Equals(second)).Return(false).Repeat.Any();
            var toTest = new Disjunction<Fakes.FakeCandidate1>(first, second);
            var other = new Disjunction<Fakes.FakeCandidate1>(third, second);
            
            
            // act:
            var result = toTest.Or(other) as Disjunction<Fakes.FakeCandidate1>;
            
            // assert:
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
            Assert.AreSame(first, result.ElementAt(0));
            Assert.AreSame(second, result.ElementAt(1));
            Assert.AreSame(third, result.ElementAt(2));
        }

        [Test]
        public void Equals_toAnotherDisjuctionWithDifferentLenght_isFalse ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> first = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            ISpecification<Fakes.FakeCandidate1> second = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            ISpecification<Fakes.FakeCandidate1> third = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            first.Expect(s => s.ToString()).Return("first").Repeat.Any();
            second.Expect(s => s.ToString()).Return("second").Repeat.Any();
            third.Expect(s => s.ToString()).Return("third").Repeat.Any();
            first.Expect(s => s.Equals(second)).Return(false).Repeat.AtLeastOnce();
            third.Expect(s => s.Equals(first)).Return(false).Repeat.AtLeastOnce();
            third.Expect(s => s.Equals(second)).Return(false).Repeat.AtLeastOnce();
            var toTest = new Disjunction<Fakes.FakeCandidate1>(first, second);
            var other = toTest.Or(third);

            // act:
            bool result = toTest.Equals(other);

            // assert:
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_toAnotherDisjuctionWithDifferentSpecifications_isFalse ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> first = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            ISpecification<Fakes.FakeCandidate1> second = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            ISpecification<Fakes.FakeCandidate1> third = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            first.Expect(s => s.ToString()).Return("first").Repeat.Any();
            second.Expect(s => s.ToString()).Return("second").Repeat.Any();
            third.Expect(s => s.ToString()).Return("third").Repeat.Any();
            first.Expect(s => s.Equals(first)).Return(true).Repeat.Any();
            first.Expect(s => s.Equals(second)).Return(false).Repeat.Any();
            first.Expect(s => s.Equals(third)).Return(false).Repeat.Any();
            second.Expect(s => s.Equals(first)).Return(false).Repeat.Any();
            second.Expect(s => s.Equals(second)).Return(true).Repeat.Any();
            second.Expect(s => s.Equals(third)).Return(false).Repeat.Any();
            third.Expect(s => s.Equals(first)).Return(false).Repeat.Any();
            third.Expect(s => s.Equals(second)).Return(false).Repeat.Any();
            var toTest = new Disjunction<Fakes.FakeCandidate1>(first, second);
            var other = new Disjunction<Fakes.FakeCandidate1>(first, third);
            
            // act:
            bool result = toTest.Equals(other);
            
            // assert:
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_toAnotherDisjuctionWithEqualSpecificationsInTheSameOrder_isTrue ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> first = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            ISpecification<Fakes.FakeCandidate1> second = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            first.Expect(s => s.ToString()).Return("first").Repeat.Any();
            second.Expect(s => s.ToString()).Return("second").Repeat.Any();
            first.Expect(s => s.Equals(first)).Return(true).Repeat.Any();
            first.Expect(s => s.Equals(second)).Return(false).Repeat.Any();
            second.Expect(s => s.Equals(first)).Return(false).Repeat.Any();
            second.Expect(s => s.Equals(second)).Return(true).Repeat.Any();
            var toTest = new Disjunction<Fakes.FakeCandidate1>(first, second);
            var other = new Disjunction<Fakes.FakeCandidate1>(first, second);
            
            // act:
            bool result = toTest.Equals(other);
            
            // assert:
            Assert.IsTrue(result);
        }

        [Test]
        public void Equals_toAnotherDisjuctionWithEqualSpecificationsInDifferentOrder_isFalse ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> first = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            ISpecification<Fakes.FakeCandidate1> second = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            first.Expect(s => s.ToString()).Return("first").Repeat.Any();
            second.Expect(s => s.ToString()).Return("second").Repeat.Any();
            first.Expect(s => s.Equals(first)).Return(true).Repeat.Any();
            first.Expect(s => s.Equals(second)).Return(false).Repeat.Any();
            second.Expect(s => s.Equals(first)).Return(false).Repeat.Any();
            second.Expect(s => s.Equals(second)).Return(true).Repeat.Any();
            var toTest = new Disjunction<Fakes.FakeCandidate1>(first, second);
            var other = new Disjunction<Fakes.FakeCandidate1>(second, first);
            
            // act:
            bool result = toTest.Equals(other);
            
            // assert:
            Assert.IsFalse(result);
        }

        [Test]
        public void IsSatisfiedBy_aCandidateThatDoesntSatisfyAnyInnerSpecification_isFalse ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> first = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            ISpecification<Fakes.FakeCandidate1> second = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            Fakes.FakeCandidate1 candidate = new Epic.Fakes.FakeCandidate1();
            first.Expect(s => s.Equals(second)).Return(false).Repeat.Any();
            first.Expect(s => s.IsSatisfiedBy(candidate)).Return(false).Repeat.AtLeastOnce();
            second.Expect(s => s.IsSatisfiedBy(candidate)).Return(false).Repeat.AtLeastOnce();
            var toTest = new Disjunction<Fakes.FakeCandidate1>(first, second);

            // act:
            bool result = toTest.IsSatisfiedBy(candidate);
            
            // assert:
            Assert.IsFalse(result);
        }

        [Test]
        public void IsSatisfiedBy_aCandidateThatSatisfiesAnyInnerSpecification_isTrue ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> first = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            ISpecification<Fakes.FakeCandidate1> second = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>, IObject>();
            Fakes.FakeCandidate1 candidate = new Epic.Fakes.FakeCandidate1();
            first.Expect(s => s.Equals(second)).Return(false).Repeat.Any();
            first.Expect(s => s.IsSatisfiedBy(candidate)).Return(false).Repeat.AtLeastOnce();
            second.Expect(s => s.IsSatisfiedBy(candidate)).Return(true).Repeat.AtLeastOnce();
            var toTest = new Disjunction<Fakes.FakeCandidate1>(first, second);
            
            // act:
            bool result = toTest.IsSatisfiedBy(candidate);
            
            // assert:
            Assert.IsTrue(result);
        }
    }
}

