//
//  NegationQA.cs
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
using NUnit.Framework;
using System;
using Rhino.Mocks;

namespace Epic.Specifications
{
    [TestFixture()]
    public class NegationQA : RhinoMocksFixtureBase
    {

        public static readonly ISpecification<Fakes.FakeCandidate1> p = new Fakes.NamedSpecification<Fakes.FakeCandidate1>("p");
        public static readonly ISpecification<Fakes.FakeCandidate1> q = new Fakes.NamedSpecification<Fakes.FakeCandidate1>("q");
        public static readonly ISpecification<Fakes.FakeCandidate1> r = new Fakes.NamedSpecification<Fakes.FakeCandidate1>("r");
        static object[] ToStringSource =
        {
            new object[] {
                p.Negate(), 
                "¬p"
            },
            new object[] {
                q.And(r).Negate(), 
                "¬(q ∧ r)"
            }
        };
        
        [Test, TestCaseSource("ToStringSource")]
        public void ToString_OfANegation_works(Negation<Fakes.FakeCandidate1> toTest, string expression)
        {
            // act:
            string result = toTest.ToString();
            
            // assert:
            Assert.AreEqual(expression, result);
        }

        [Test]
        public void Initialize_withASpecification_works ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> inner = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>>();

            // act:
            var toTest = new Negation<Fakes.FakeCandidate1>(inner);

            // assert:
            Assert.IsNotNull(toTest);
            Assert.AreSame(inner, toTest.Negated);
            Assert.AreSame(inner, (toTest as IMonadicSpecificationComposition<Fakes.FakeCandidate1Abstraction>).Operand);
        }

        [Test]
        public void Initialize_withoutASpecification_throwsArgumentNullException ()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate() {
                new Negation<Fakes.FakeCandidate1>(null);
            });
        }

        [Test]
        public void Equals_withAnotherWithEqualsInner_isTrue ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> inner = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>>();
            inner.Expect(s => s.Equals(inner)).Return(true).Repeat.AtLeastOnce();
            var toTest1 = new Negation<Fakes.FakeCandidate1>(inner);
            var other = new Negation<Fakes.FakeCandidate1>(inner);
            
            // act:
            bool result = toTest1.Equals(other);
            
            // assert:
            Assert.IsTrue(result);
        }


        [Test]
        public void IsSatisfiedBy_Candidates_thatSatisfyTheInnerSpecification ()
        {
            // arrange:
            Fakes.FakeCandidate1 candidate1 = new Epic.Fakes.FakeCandidate1();
            Fakes.FakeCandidate1 candidate2 = new Epic.Fakes.FakeCandidate1();
            ISpecification<Fakes.FakeCandidate1> inner = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>>();
            inner.Expect(s => s.IsSatisfiedBy(candidate1)).Return(true).Repeat.Once();
            inner.Expect(s => s.IsSatisfiedBy(candidate2)).Return(false).Repeat.Once();
            ISpecification<Fakes.FakeCandidate1> toTest = new Negation<Fakes.FakeCandidate1>(inner);
            
            // act:
            bool candidate1Result = toTest.IsSatisfiedBy(candidate1);
            bool candidate2Result = toTest.IsSatisfiedBy(candidate2);

            // assert:
            Assert.IsFalse(candidate1Result);
            Assert.IsTrue(candidate2Result);
        }

        [Test]
        public void Negate_returnsTheInnerSpecification ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> inner = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>>();
            ISpecification<Fakes.FakeCandidate1> toTest = new Negation<Fakes.FakeCandidate1>(inner);

            // act:
            var result = toTest.Negate();

            // assert:
            Assert.AreSame(inner, result);
        }
    }
}

