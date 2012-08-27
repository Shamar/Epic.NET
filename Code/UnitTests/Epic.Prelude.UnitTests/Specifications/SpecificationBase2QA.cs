//
//  SpecificationBaseQA.cs
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
using Epic.Math;
using System.IO;

namespace Epic.Specifications
{
    interface DummySpec<T> : ISpecification<T>, IEquatable<DummySpec<T>>
        where T : class
    {
    }
    [TestFixture()]
    public class SpecificationBase2QA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_aSpecificationTooAbstract_throwsTypeInitializationException()
        {
            // assert:
            Assert.Throws<TypeInitializationException>(delegate { 
                new Fakes.WrongSpecification<object, DummySpec<object>>();
            });
            Assert.Throws<TypeInitializationException>(delegate { 
                new Fakes.WrongSpecification<string, ISpecification<string>>();
            });
        }

        [Test]
        public void Initalize_aSpecificationThatNotImplementTheInterface_throwInvalidOperationException()
        {
            // assert:
            Assert.Throws<InvalidOperationException>(delegate { 
                new Fakes.WrongSpecification<string, DummySpec<string>>();
            });
        }

        [Test]
        public void Initialize_aValidSpecification_works()
        {
            // act:
            Fakes.ISampleSpecification<string> toTest = new Fakes.SampleSpecification<string>();

            // assert:
            Assert.IsNotNull(toTest);
            Assert.AreEqual(typeof(string), toTest.CandidateType);
            Assert.AreEqual(typeof(Fakes.ISampleSpecification<string>), toTest.SpecificationType);
        }

        [Test]
        public void Equals_toNull_isFalse()
        {
            // arrange:
            Fakes.SampleSpecification<string> toTest = new Fakes.SampleSpecification<string>();

            // assert:
            Assert.IsFalse (toTest.Equals(null as Object));
            Assert.IsFalse (toTest.Equals(null as Fakes.ISampleSpecification<string>));
            Assert.IsFalse (toTest.Equals(null as ISpecification<string>));
        }

        [Test]
        public void Equals_toItself_isTrue()
        {
            // arrange:
            Fakes.SampleSpecification<string> toTest = new Fakes.SampleSpecification<string>();

            // assert:
            Assert.IsTrue (toTest.Equals(toTest as Object));
            Assert.IsTrue (toTest.Equals(toTest as Fakes.ISampleSpecification<string>));
            Assert.IsTrue (toTest.Equals(toTest as ISpecification<string>));
            Assert.AreEqual(typeof(Fakes.ISampleSpecification<string>).GetHashCode(), toTest.GetHashCode());
        }

        [Test]
        public void Equals_toAnotherType_isFalse()
        {
            // arrange:
            Fakes.SampleSpecification<string> toTest = new Fakes.SampleSpecification<string>();
            Fakes.SampleSpecification<string> other = new Fakes.DerivedSampleSpecification<string>();

            // assert:
            Assert.IsFalse (toTest.Equals(other as Object));
            Assert.IsFalse (toTest.Equals(other as Fakes.ISampleSpecification<string>));
            Assert.IsFalse (toTest.Equals(other as ISpecification<string>));
        }

        [Test]
        public void Equals_toAnotherSpecificationOfTheSameType_callEqualsA()
        {
            // arrange:
            Fakes.SampleSpecification<string> toTest = new Fakes.SampleSpecification<string>((a, b) => true);
            Fakes.SampleSpecification<string> other = new Fakes.SampleSpecification<string>((a, b) => false);

            // assert:
            Assert.IsTrue (toTest.Equals(other as Object));
            Assert.IsTrue (toTest.Equals(other as Fakes.ISampleSpecification<string>));
            Assert.IsTrue (toTest.Equals(other as ISpecification<string>));
            Assert.IsFalse (other.Equals(toTest as Object));
            Assert.IsFalse (other.Equals(toTest as Fakes.ISampleSpecification<string>));
            Assert.IsFalse (other.Equals(toTest as ISpecification<string>));
            Assert.AreEqual(3, toTest.EqualsACalled); 
            Assert.AreEqual(3, other.EqualsACalled);
        }

        [Test]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            Fakes.ISampleSpecification<string> toTest = new Fakes.SampleSpecification<string>();
            object expectedResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            IVisitor<object, Fakes.ISampleSpecification<string>> specificationVisitor = GenerateStrictMock<IVisitor<object, Fakes.ISampleSpecification<string>>>();
            specificationVisitor.Expect(v => v.Visit(toTest, context)).Return(expectedResult).Repeat.Once();
            IVisitor<object> visitor = GenerateStrictMock<IVisitor<object>>();
            visitor.Expect(v => v.AsVisitor(toTest)).Return(specificationVisitor).Repeat.Once ();

            // act:
            object result = toTest.Accept(visitor, context);

            // assert:
            Assert.AreSame(expectedResult, result);
        }

        [Test]
        public void IsSatisfiedBy_null_isFalse()
        {
            // arrange:
            Fakes.SampleSpecification<string> toTest = new Fakes.SampleSpecification<string>();

            // act:
            bool result = toTest.IsSatisfiedBy(null);

            // assert:
            Assert.IsFalse(result);
            Assert.AreEqual(0, toTest.IsSatisfiedByACalled);
        }

        [Test]
        public void IsSatifiedBy_aCandidate_callIsSatisfiedByA()
        {
            // arrange:
            string candidate = "test";
            Fakes.SampleSpecification<string> toTest1 = new Fakes.SampleSpecification<string>(s => true);
            Fakes.SampleSpecification<string> toTest2 = new Fakes.SampleSpecification<string>(s => false);

            // assert:
            Assert.IsTrue(toTest1.IsSatisfiedBy(candidate));
            Assert.AreEqual(1, toTest1.IsSatisfiedByACalled);
            Assert.IsFalse(toTest2.IsSatisfiedBy(candidate));
            Assert.AreEqual(1, toTest2.IsSatisfiedByACalled);
        }

        [Test]
        public void ApplyTo_aCandidate_callIsSatisfiedByA()
        {
            // arrange:
            string candidate = "test";
            Fakes.SampleSpecification<string> toTest1 = new Fakes.SampleSpecification<string>(s => true);
            Fakes.SampleSpecification<string> toTest2 = new Fakes.SampleSpecification<string>(s => false);

            // assert:
            Assert.IsTrue((toTest1 as IMapping<string, bool>).ApplyTo(candidate));
            Assert.AreEqual(1, toTest1.IsSatisfiedByACalled);
            Assert.IsFalse((toTest2 as IMapping<string, bool>).ApplyTo(candidate));
            Assert.AreEqual(1, toTest2.IsSatisfiedByACalled);
        }

        [Test]
        public void And_withoutASpecification_throwsArgumentNullException ()
        {
            // arrange:
            Fakes.SampleSpecification<string> toTest = new Fakes.SampleSpecification<string>(s => true);

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                toTest.And(null);
            });
        }

        [Test]
        public void And_withNoCandidateSpecification_returnsNoCandidateSpecification ()
        {
            // arrange:
            Fakes.SampleSpecification<string> toTest = new Fakes.SampleSpecification<string>(s => true);

            // act:
            ISpecification<string> result = toTest.And(No<string>.Specification);

            // assert:
            Assert.AreSame(No<string>.Specification, result);
        }

        [Test]
        public void And_withAnyCandidateSpecification_returnsItself ()
        {
            // arrange:
            Fakes.SampleSpecification<string> toTest = new Fakes.SampleSpecification<string>();

            // act:
            ISpecification<string> result = toTest.And(Any<string>.Specification);
            
            // assert:
            Assert.AreSame(toTest, result);
        }

        [Test]
        public void And_anotherSpecification_returnsAConjunction ()
        {
            // arrange:
            Fakes.SampleSpecification<string> toTest = new Fakes.SampleSpecification<string>();
            ISpecification<string> other = GenerateStrictMock<ISpecification<string>>();

            // act:
            ISpecification<string> result = toTest.And(other);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Conjunction<string>>(result);
        }

        [Test]
        public void Or_withoutASpecification_throwsArgumentNullException ()
        {
            // arrange:
            Fakes.SampleSpecification<string> toTest = new Fakes.SampleSpecification<string>(s => true);
            
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                toTest.Or(null);
            });
        }
        
        [Test]
        public void Or_withNoCandidateSpecification_returnsItself ()
        {
            // arrange:
            Fakes.SampleSpecification<string> toTest = new Fakes.SampleSpecification<string>(s => true);
            
            // act:
            ISpecification<string> result = toTest.Or(No<string>.Specification);
            
            // assert:
            Assert.AreSame(toTest, result);
        }
        
        [Test]
        public void Or_withAnyCandidateSpecification_returnsAnyCandidateSpecification ()
        {
            // arrange:
            Fakes.SampleSpecification<string> toTest = new Fakes.SampleSpecification<string>();
            
            // act:
            ISpecification<string> result = toTest.Or(Any<string>.Specification);
            
            // assert:
            Assert.AreSame(Any<string>.Specification, result);
        }
        
        [Test]
        public void Or_anotherSpecification_returnsADisjunction ()
        {
            // arrange:
            Fakes.SampleSpecification<string> toTest = new Fakes.SampleSpecification<string>();
            ISpecification<string> other = GenerateStrictMock<ISpecification<string>>();
            
            // act:
            ISpecification<string> result = toTest.Or(other);
            
            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Disjunction<string>>(result);
        }

        [Test]
        public void Negate_returnANegation ()
        {
            // arrange:
            Fakes.ISampleSpecification<string> toTest = new Fakes.SampleSpecification<string>();

            // act:
            ISpecification<string> result = toTest.Negate();

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Negation<string>>(result);
        }

        [Test]
        public void OfType_withAnImplementedType_returnItself ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> toTest = new Fakes.SampleSpecification();

            // act:
            ISpecification<Fakes.FakeCandidate2> result = toTest.OfType<Fakes.FakeCandidate2>();

            // assert:
            Assert.AreSame(toTest, result);
        }

        [Test]
        public void OfType_withAnAbstraction_returnASpecificationOfType ()
        {
            // arrange:
            Fakes.SampleSpecification<Fakes.FakeCandidate1> toTest = new Fakes.SampleSpecification<Fakes.FakeCandidate1>();

            // act:
            ISpecification<Fakes.FakeCandidate1Abstraction> result = toTest.OfType<Fakes.FakeCandidate1Abstraction>();

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Variant<Fakes.FakeCandidate1, Fakes.FakeCandidate1Abstraction>>(result);
        }

        [Test]
        public void Serialization_works ()
        {
            // arrange:
            Fakes.SampleSpecification<Fakes.FakeCandidate1> toSerialize = new Fakes.SampleSpecification<Fakes.FakeCandidate1>();

            // act:
            Stream stream = SerializationUtilities.Serialize(toSerialize);
            
            // assert:
            Assert.IsNotNull (stream);
        }

        [Test]
        public void Deserialization_works ()
        {
            // arrange:
            Fakes.SampleSpecification<Fakes.FakeCandidate1> toSerialize = new Fakes.SampleSpecification<Fakes.FakeCandidate1>();
            Stream stream = SerializationUtilities.Serialize(toSerialize);

            // act:
            Fakes.SampleSpecification<Fakes.FakeCandidate1> deserialized = SerializationUtilities.Deserialize<Fakes.SampleSpecification<Fakes.FakeCandidate1>>(stream);
            
            // assert:
            Assert.IsNotNull (deserialized);
        }

    }
}

