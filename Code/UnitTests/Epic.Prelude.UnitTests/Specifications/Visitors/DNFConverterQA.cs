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
        public static readonly ISpecification<B> p = new Fakes.NamedSpecification<B>("p");
        public static readonly ISpecification<B> q = new Fakes.NamedSpecification<B>("q");
        public static readonly ISpecification<C> r = new Fakes.NamedSpecification<C>("r");
        public static readonly ISpecification<C> s = new Fakes.NamedSpecification<C>("s");
        public static readonly ISpecification<C> t = new Fakes.NamedSpecification<C>("t");
        public static readonly ISpecification<D> u = new Fakes.NamedSpecification<D>("u");
        public static readonly ISpecification<D2> v = new Fakes.NamedSpecification<D2>("v");
        public static readonly ISpecification<C> w = new Fakes.NamedSpecification<C>("w");
        public static readonly ISpecification<C> x = new Fakes.NamedSpecification<C>("x");


        [Test]
        public void Initialization_withAName_works()
        {
            // arrange:
            string name = "Test";

            // act:
            new DNFConverter<B>(name);
        }

        [Test]
        public void Visit_anUnexpectedExpression_throwsInvalidOperationException()
        {
            // arrange:
            var toTest = new BrokenDNFVisitor<Fakes.FakeCandidate1>();
            InvalidOperationException result = null;

            // assert:
            Assert.Throws<InvalidOperationException>(delegate {
                AcceptCaller<Fakes.FakeCandidate1, ISpecification<Fakes.FakeCandidate1>>.CallAccept(new Fakes.FakeCandidate1(), toTest, VisitContext.New);
            });
        }

        [Test]
        public void Visit_aNegatedDisjunction_applyDeMorganLaws()
        {
            // arrange:
            var toTest = new DNFConverter<B>("Test");
            var toVisit = r.Or(s).Negate();
            var expected = r.OfType<B>().Negate().And(s.OfType<B>().Negate());

            // act:
            var result = toVisit.Accept(toTest, VisitContext.New);

            // assert:
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Visit_aNegatedConjunction_applyDeMorganLaws()
        {
            // arrange:
            var toTest = new DNFConverter<B>("Test");
            var toVisit = r.And(s).Negate();
            var expected = r.OfType<B>().Negate().Or(s.OfType<B>().Negate());
            
            // act:
            var result = toVisit.Accept(toTest, VisitContext.New);
            
            // assert:
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Visit_aNegatedConjunctionNestedInADisjunction_returnsASimpleDisjunction()
        {
            // arrange:
            var toTest = new DNFConverter<B>("Test");
            var toVisit = t.Or(r.And(s).Negate());
            var expected = t.OfType<B>().Or(r.OfType<B>().Negate()).Or(s.OfType<B>().Negate());
            
            // act:
            var result = toVisit.Accept(toTest, VisitContext.New);
            
            // assert:
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Visit_aNegatedDisjunctionNestedInAConjunction_returnsASimpleConjunction()
        {
            // arrange:
            var toTest = new DNFConverter<B>("Test");
            var toVisit = t.And(r.Or(s).Negate());
            var expected = t.OfType<B>().And(r.OfType<B>().Negate()).And(s.OfType<B>().Negate());
            
            // act:
            var result = toVisit.Accept(toTest, VisitContext.New);
            
            // assert:
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Visit_aNegatedUpcastedSpecification_returnsThatSpecificationUpcastedToTopAndNegated()
        {
            // arrange:
            var toTest = new DNFConverter<B>("Test");
            var toVisit = u.OfType<C>().Negate();
            var expected = u.OfType<B>().Negate();
            
            // act:
            var result = toVisit.Accept(toTest, VisitContext.New);
            
            // assert:
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Visit_aNegatedDowncastedSpecification_returnsThatSpecificationNegatedAndUpcastedToTopDisjunctedToANegatedSpecificationSatisfiedFromTheDowncastingType()
        {
            // arrange:
            var toTest = new DNFConverter<B>("Test");
            var toVisit = q.OfType<D>().Negate();
            var expected = Any<D>.Specification.OfType<B>().Negate().Or(q.OfType<B>().Negate());
            
            // act:
            var result = toVisit.Accept(toTest, VisitContext.New);
            
            // assert:
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Visit_aUpcastedNegatedDowncastedSpecification_worksAsExpected()
        {
            // arrange:
            var toTest = new DNFConverter<B>("Test");
            var toVisit = r.OfType<D>().Negate().OfType<B>();
            var expected = Any<D>.Specification.OfType<B>().Negate().And(r.OfType<B>().Negate());
            
            // act:
            var result = toVisit.Accept(toTest, VisitContext.New);
            
            // assert:
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Visit_aNegatedDowncastedSpecificationDisjunctedToAnotherSpecification_worksAsExpected()
        {
            // arrange:
            var toTest = new DNFConverter<B>("Test");
            var toVisit = s.Or(r.OfType<D>().Negate().OfType<C>());
            var expected = s.OfType<B>().Or(Any<D>.Specification.OfType<B>().Negate())
                                        .Or(r.OfType<B>().Negate());
            
            // act:
            var result = toVisit.Accept(toTest, VisitContext.New);
            
            // assert:
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Visit_anUpcastedDowncastedPredicate_returnsAConjunctionOfThePredicateAndTheTypeSelection()
        {
            // arrange:
            var toTest = new DNFConverter<B>("Test");
            var toVisit = q.OfType<D>().OfType<B>();
            var expected = Any<D>.Specification.OfType<B>().And(q.OfType<B>());
            
            // act:
            var result = toVisit.Accept(toTest, VisitContext.New);
            
            // assert:
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Visit_anUpcastedDowncastedNegatedPredicate_returnsADisjunctionOfThePredicateAndTheTypeSelection()
        {
            // arrange:
            var toTest = new DNFConverter<B>("Test");
            var toVisit = q.OfType<D>().Negate().OfType<C>();
            var expected = Any<D>.Specification.OfType<B>().Negate().Or(q.OfType<B>().Negate());
            
            // act:
            var result = toVisit.Accept(toTest, VisitContext.New);
            
            // assert:
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Visit_aTopcastedDowncastedNegatedPredicate_correctlyApplyTheMorganLaw()
        {
            // arrange:
            var toTest = new DNFConverter<B>("Test");
            var toVisit = r.And(s.Or(t)).And(u.OfType<C>().Or(v.OfType<C>()));
            var expected = r.OfType<B>().And(s.OfType<B>()).And(u.OfType<B>())
                    .Or(r.OfType<B>().And(t.OfType<B>()).And(u.OfType<B>()))
                    .Or(r.OfType<B>().And(s.OfType<B>()).And(v.OfType<B>()))
                    .Or(r.OfType<B>().And(t.OfType<B>()).And(v.OfType<B>()));
            
            // act:
            var result = toVisit.Accept(toTest, VisitContext.New);

            // assert:
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Visit_aDowncastedNegatedPredicate_returnsAConjunctionOfTypeSelectionAndTheNegatedUpcast()
        {
            // arrange:
            var toTest = new DNFConverter<B>("Test");
            var toVisit = r.Negate().OfType<D>();
            var expected = Any<D>.Specification.OfType<B>().And(r.OfType<B>().Negate());
            
            // act:
            var result = toVisit.Accept(toTest, VisitContext.New);
            
            // assert:
            Assert.AreEqual(expected, result);
        }

        static object[] ComplexMultilevelSpecifications =
        {
            new object[] {
                x.And(w.Or(r.And(s.Or(t)))), 
                x.OfType<B>().And(w.OfType<B>())
                    .Or(x.OfType<B>().And(r.OfType<B>().And(s.OfType<B>())))
                        .Or(x.OfType<B>().And(r.OfType<B>()).And(t.OfType<B>()))
            },
            new object[] {
                x.And(w.Or(r.And(s.Or(t)))).OfType<D>(), 
                Any<D>.Specification.OfType<B>().And(x.OfType<B>().And(w.OfType<B>()))
                    .Or(Any<D>.Specification.OfType<B>().And(x.OfType<B>().And(r.OfType<B>().And(s.OfType<B>()))))
                        .Or(Any<D>.Specification.OfType<B>().And(x.OfType<B>().And(r.OfType<B>()).And(t.OfType<B>())))
            }
        };   

        [Test, TestCaseSource("ComplexMultilevelSpecifications")]
        public void Visit_aComplexMultilevelSpecification_worksAsExpected(ISpecification toVisit, ISpecification expected)
        {
            // arrange:
            var toTest = new DNFConverter<B>("Test");

            // act:
            var result = toVisit.Accept(toTest, VisitContext.New);
            
            // assert:
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Visit_aSpecificationThatImplementISpecificationTwoOrMoreTimes_throwEpicException()
        {
            // arrange:
            var toTest = new DNFConverter<B>("Test");
            var toVisit = u.Or(new StrangeSpecification());

            // assert:
            Assert.Throws<EpicException>(delegate {
                toVisit.Accept(toTest, VisitContext.New);
            });
        }
    }

    #region utilities
    internal class BrokenDNFVisitor<T> : DNFConverter<T>, IVisitor<ISpecification<T>, T>
        where T : class
    {
        public BrokenDNFVisitor()
            : base("Broken")
        {
        }

        public ISpecification<T> Visit(T target, IVisitContext context)
        {
            if (null == target)
                throw new ArgumentNullException("target");
            if (null == context)
                throw new ArgumentNullException("context");
            IVisitor<ISpecification<T>, T> visitor = GetFirstVisitor(target);
            return visitor.Visit(target, context);
        }
    }

    internal class StrangeSpecification : Specifications.SpecificationBase<StrangeSpecification, D, D2>, IEquatable<StrangeSpecification>
    {
        #region implemented abstract members of SpecificationBase
        protected override bool EqualsA(StrangeSpecification otherSpecification)
        {
            return true;
        }
        protected override bool IsSatisfiedByA(D candidate)
        {
            throw new NotImplementedException("This is a mock.");
        }
        #endregion
        #region implemented abstract members of SpecificationBase
        protected override bool IsSatisfiedByA(D2 candidate)
        {
            throw new NotImplementedException("This is a mock.");
        }
        #endregion
    }

    #endregion utilities
}

