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
        public static readonly ISpecification<B> P = new NamedPredicate<B>("P");
        public static readonly ISpecification<B> Q = new NamedPredicate<B>("Q");
        public static readonly ISpecification<C> R = new NamedPredicate<C>("R");
        public static readonly ISpecification<C> S = new NamedPredicate<C>("S");
        public static readonly ISpecification<C> T = new NamedPredicate<C>("T");
        public static readonly ISpecification<D> U = new NamedPredicate<D>("U");
        public static readonly ISpecification<D2> V = new NamedPredicate<D2>("V");
        public static readonly ISpecification<C> W = new NamedPredicate<C>("W");
        public static readonly ISpecification<C> X = new NamedPredicate<C>("X");


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
            var toVisit = R.Or(S).Negate();
            var expected = R.OfType<B>().Negate().And(S.OfType<B>().Negate());

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
            var toVisit = R.And(S).Negate();
            var expected = R.OfType<B>().Negate().Or(S.OfType<B>().Negate());
            
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
            var toVisit = T.Or(R.And(S).Negate());
            var expected = T.OfType<B>().Or(R.OfType<B>().Negate()).Or(S.OfType<B>().Negate());
            
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
            var toVisit = T.And(R.Or(S).Negate());
            var expected = T.OfType<B>().And(R.OfType<B>().Negate()).And(S.OfType<B>().Negate());
            
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
            var toVisit = U.OfType<C>().Negate();
            var expected = U.OfType<B>().Negate();
            
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
            var toVisit = Q.OfType<D>().Negate();
            var expected = Any<D>.Specification.OfType<B>().Negate().Or(Q.OfType<B>().Negate());
            
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
            var toVisit = R.OfType<D>().Negate().OfType<B>();
            var expected = Any<D>.Specification.OfType<B>().Negate().And(R.OfType<B>().Negate());
            
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
            var toVisit = S.Or(R.OfType<D>().Negate().OfType<C>());
            var expected = S.OfType<B>().Or(Any<D>.Specification.OfType<B>().Negate())
                                        .Or(R.OfType<B>().Negate());
            
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
            var toVisit = Q.OfType<D>().OfType<B>();
            var expected = Any<D>.Specification.OfType<B>().And(Q.OfType<B>());
            
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
            var toVisit = Q.OfType<D>().Negate().OfType<C>();
            var expected = Any<D>.Specification.OfType<B>().Negate().Or(Q.OfType<B>().Negate());
            
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
            var toVisit = R.And(S.Or(T)).And(U.OfType<C>().Or(V.OfType<C>()));
            var expected = R.OfType<B>().And(S.OfType<B>()).And(U.OfType<B>())
                    .Or(R.OfType<B>().And(T.OfType<B>()).And(U.OfType<B>()))
                    .Or(R.OfType<B>().And(S.OfType<B>()).And(V.OfType<B>()))
                    .Or(R.OfType<B>().And(T.OfType<B>()).And(V.OfType<B>()));
            
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
            var toVisit = R.Negate().OfType<D>();
            var expected = Any<D>.Specification.OfType<B>().And(R.OfType<B>().Negate());
            
            // act:
            var result = toVisit.Accept(toTest, VisitContext.New);
            
            // assert:
            Assert.AreEqual(expected, result);
        }

        static object[] ComplexMultilevelSpecifications =
        {
            new object[] {
                X.And(W.Or(R.And(S.Or(T)))), 
                X.OfType<B>().And(W.OfType<B>())
                    .Or(X.OfType<B>().And(R.OfType<B>().And(S.OfType<B>())))
                        .Or(X.OfType<B>().And(R.OfType<B>()).And(T.OfType<B>()))
            },
            new object[] {
                X.And(W.Or(R.And(S.Or(T)))).OfType<D>(), 
                Any<D>.Specification.OfType<B>().And(X.OfType<B>().And(W.OfType<B>()))
                    .Or(Any<D>.Specification.OfType<B>().And(X.OfType<B>().And(R.OfType<B>().And(S.OfType<B>()))))
                        .Or(Any<D>.Specification.OfType<B>().And(X.OfType<B>().And(R.OfType<B>()).And(T.OfType<B>())))
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
            var toVisit = U.Or(new StrangeSpecification());

            // assert:
            Assert.Throws<EpicException>(delegate {
                toVisit.Accept(toTest, VisitContext.New);
            });
        }
    }

    #region utilities
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

        public override string ToString()
        {
            return Name;
        }
    }
    #endregion utilities
}

