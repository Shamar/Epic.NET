//
//  InvariantDNFConverterQA.cs
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
    public class InvariantDNFConverterQA
    {
        public static readonly ISpecification<C> p = new Fakes.NamedSpecification<C>("p");
        public static readonly ISpecification<C> q = new Fakes.NamedSpecification<C>("q");
        public static readonly ISpecification<C> r = new Fakes.NamedSpecification<C>("r");
        public static readonly ISpecification<C> s = new Fakes.NamedSpecification<C>("s");
        public static readonly ISpecification<C> t = new Fakes.NamedSpecification<C>("t");
        public static readonly ISpecification<C> u = new Fakes.NamedSpecification<C>("u");
        public static readonly ISpecification<C> v = new Fakes.NamedSpecification<C>("v");
        public static readonly ISpecification<C> w = new Fakes.NamedSpecification<C>("w");
        public static readonly ISpecification<C> x = new Fakes.NamedSpecification<C>("x");


        [Test]
        public void Initialization_withAName_works()
        {
            // arrange:
            string name = "Test";

            // act:
            new InvariantDNFConverter<C>(name);
        }

        [Test]
        public void Visit_aNegatedDisjunction_applyDeMorganLaws()
        {
            // arrange:
            var toTest = new InvariantDNFConverter<C>("Test");
            var toVisit = r.Or(s).Negate();
            var expected = r.Negate().And(s.Negate());

            // act:
            var result = toVisit.Accept(toTest, VisitContext.New);

            // assert:
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Visit_aNegatedConjunction_applyDeMorganLaws()
        {
            // arrange:
            var toTest = new InvariantDNFConverter<C>("Test");
            var toVisit = r.And(s).Negate();
            var expected = r.Negate().Or(s.Negate());
            
            // act:
            var result = toVisit.Accept(toTest, VisitContext.New);
            
            // assert:
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Visit_aNegatedConjunctionNestedInADisjunction_returnsASimpleDisjunction()
        {
            // arrange:
            var toTest = new InvariantDNFConverter<C>("Test");
            var toVisit = t.Or(r.And(s).Negate());
            var expected = t.Or(r.Negate()).Or(s.Negate());
            
            // act:
            var result = toVisit.Accept(toTest, VisitContext.New);
            
            // assert:
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Visit_aNegatedDisjunctionNestedInAConjunction_returnsASimpleConjunction()
        {
            // arrange:
            var toTest = new InvariantDNFConverter<C>("Test");
            var toVisit = t.And(r.Or(s).Negate());
            var expected = t.And(r.Negate()).And(s.Negate());
            
            // act:
            var result = toVisit.Accept(toTest, VisitContext.New);
            
            // assert:
            Assert.AreEqual(expected, result);
        }

        static object[] ComplexMultilevelSpecifications =
        {
            new object[] {
                x.And(w.Or(r.And(s.Or(t)))), 
                x.And(w)
                    .Or(x.And(r.And(s)))
                        .Or(x.And(r).And(t))
            },
            new object[] {
                x.And(w.Or(r.And(s.Or(t)))), 
                Any<C>.Specification.And(x.And(w))
                    .Or(Any<C>.Specification.And(x.And(r.And(s))))
                        .Or(Any<C>.Specification.And(x.And(r).And(t)))
            },
            new  object[] {
                r.And(s.Or(t)).And(u.Or(v)),
                r.And(s).And(u)
                    .Or(r.And(t).And(u))
                    .Or(r.And(s).And(v))
                    .Or(r.And(t).And(v))
            },
            new object[] {
                w.Or(r.Negate().And(s)).Negate(),
                w.Negate().And(r).Or(w.Negate().And(s.Negate()))
            },
            new object[] {
                w.Or(r.Negate().Or(s)).Negate(),
                w.Negate().And(r).And(s.Negate())
            }
        };   

        [Test, TestCaseSource("ComplexMultilevelSpecifications")]
        public void Visit_aComplexMultilevelSpecification_worksAsExpected(ISpecification toVisit, ISpecification expected)
        {
            // arrange:
            var toTest = new InvariantDNFConverter<C>("Test");

            // act:
            var result = toVisit.Accept(toTest, VisitContext.New);
            
            // assert:
            Assert.AreEqual(expected, result);
        }
    }
}

