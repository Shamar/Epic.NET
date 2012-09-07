//
//  SpecificationBase3QA.cs
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
using Epic.Fakes;

namespace Epic.Specifications
{
    [TestFixture()]
    public class SpecificationBase3QA : RhinoMocksFixtureBase
    {
        [Test]
        public void CandidateTypes_areCorrect ()
        {
            // arrange:
            SampleSpecification sample = new SampleSpecification();

            // assert:
            Assert.AreEqual(typeof(FakeCandidate1), (sample as ISpecification<FakeCandidate1>).CandidateType);
            Assert.AreEqual(typeof(FakeCandidate2), (sample as ISpecification<FakeCandidate2>).CandidateType);
        }

        [Test]
        public void Equals_toNull_isFalse()
        {
            // arrange:
            SampleSpecification toTest = new SampleSpecification();
            
            // assert:
            Assert.IsFalse (toTest.Equals(null as Object));
            Assert.IsFalse (toTest.Equals(null as ISpecification<FakeCandidate1>));
            Assert.IsFalse (toTest.Equals(null as ISpecification<FakeCandidate2>));
            Assert.IsFalse (toTest.Equals(null as ISampleSpecification));
        }
        
        [Test]
        public void Equals_toItself_isTrue()
        {
            // arrange:
            SampleSpecification toTest = new SampleSpecification();
            
            // assert:
            Assert.IsTrue (toTest.Equals(toTest as Object));
            Assert.IsTrue (toTest.Equals(toTest as ISpecification<FakeCandidate1>));
            Assert.IsTrue (toTest.Equals(toTest as ISpecification<FakeCandidate2>));
            Assert.IsTrue (toTest.Equals(toTest as ISampleSpecification));
        }
        
        [Test]
        public void Equals_toAnotherType_isFalse()
        {
            // arrange:
            ISampleSpecification toTest = new SampleSpecification();
            ISpecification<FakeCandidate2> other = GenerateStrictMock<ISpecification<FakeCandidate2>>();
            
            // assert:
            Assert.IsFalse (toTest.Equals(other as Object));
            Assert.IsFalse (toTest.Equals(other as ISpecification<FakeCandidate2>));
        }


        [Test]
        public void IsSatisfiedBy_null_isFalse()
        {
            // arrange:
            SampleSpecification toTest = new SampleSpecification();
            
            // act:
            bool result1 = toTest.IsSatisfiedBy(null as FakeCandidate1);
            bool result2 = toTest.IsSatisfiedBy(null as FakeCandidate2);

            // assert:
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
            Assert.AreEqual(0, toTest.IsSatisfiedByAFakeCandidate1Calls);
            Assert.AreEqual(0, toTest.IsSatisfiedByAFakeCandidate2Calls);
        }


        [Test]
        public void IsSatifiedBy_aCandidate_callIsSatisfiedByA()
        {
            // arrange:
            FakeCandidate1 candidate1 = new FakeCandidate1();
            FakeCandidate2 candidate2 = new FakeCandidate2();
            Fakes.SampleSpecification toTest1 = new Fakes.SampleSpecification();
            toTest1.SatifactionRule = c => true;
            Fakes.SampleSpecification toTest2 = new Fakes.SampleSpecification();
            toTest2.SatifactionRule = c => false;

            
            // assert:
            Assert.IsTrue(toTest1.IsSatisfiedBy(candidate1));
            Assert.IsTrue(toTest1.IsSatisfiedBy(candidate2));
            Assert.AreEqual(1, toTest1.IsSatisfiedByAFakeCandidate1Calls);
            Assert.AreEqual(1, toTest1.IsSatisfiedByAFakeCandidate2Calls);
            Assert.IsFalse(toTest2.IsSatisfiedBy(candidate1));
            Assert.IsTrue(toTest2.IsSatisfiedBy(candidate2));
            Assert.AreEqual(1, toTest2.IsSatisfiedByAFakeCandidate1Calls);
            Assert.AreEqual(1, toTest2.IsSatisfiedByAFakeCandidate2Calls);
        }
        [Test]
        public void ApplyTo_aCandidate_callIsSatisfiedByA()
        {
            // arrange:
            FakeCandidate1 candidate1 = new FakeCandidate1();
            FakeCandidate2 candidate2 = new FakeCandidate2();
            Fakes.SampleSpecification toTest1 = new Fakes.SampleSpecification();
            toTest1.SatifactionRule = c => true;
            Fakes.SampleSpecification toTest2 = new Fakes.SampleSpecification();
            toTest2.SatifactionRule = c => false;
            
            
            // assert:
            Assert.IsTrue((toTest1 as IMapping<FakeCandidate1, bool>).ApplyTo(candidate1));
            Assert.IsTrue((toTest1 as IMapping<FakeCandidate2, bool>).ApplyTo(candidate2));
            Assert.AreEqual(1, toTest1.IsSatisfiedByAFakeCandidate1Calls);
            Assert.AreEqual(1, toTest1.IsSatisfiedByAFakeCandidate2Calls);
            Assert.IsFalse((toTest2 as IMapping<FakeCandidate1, bool>).ApplyTo(candidate1));
            Assert.IsTrue((toTest2 as IMapping<FakeCandidate2, bool>).ApplyTo(candidate2));
            Assert.AreEqual(1, toTest2.IsSatisfiedByAFakeCandidate1Calls);
            Assert.AreEqual(1, toTest2.IsSatisfiedByAFakeCandidate2Calls);
        }

        
        [Test]
        public void And_withoutASpecification_throwsArgumentNullException ()
        {
            // arrange:
            ISpecification<FakeCandidate2> toTest = new Fakes.SampleSpecification<FakeCandidate2>(s => true);
            
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                toTest.And(null);
            });
        }
        
        [Test]
        public void And_withNoCandidateSpecification_returnsNoCandidateSpecification ()
        {
            // arrange:
            ISpecification<FakeCandidate1> toTest = new SampleSpecification();
            
            // act:
            ISpecification<FakeCandidate1> result = toTest.And(No<FakeCandidate1>.Specification);
            
            // assert:
            Assert.AreSame(No<FakeCandidate1>.Specification, result);
        }
        
        [Test]
        public void And_withAnyCandidateSpecification_returnsItself ()
        {
            // arrange:
            ISpecification<FakeCandidate1> toTest = new SampleSpecification();
            
            // act:
            ISpecification<FakeCandidate1> result = toTest.And(Any<FakeCandidate1>.Specification);
            
            // assert:
            Assert.AreSame(toTest, result);
        }
        
        [Test]
        public void And_anotherSpecification_returnsAConjunction ()
        {
            // arrange:
            ISpecification<FakeCandidate1> toTest = new SampleSpecification();
            ISpecification<FakeCandidate1> other = GenerateStrictMock<ISpecification<FakeCandidate1>>();
            
            // act:
            ISpecification<FakeCandidate1> result = toTest.And(other);
            
            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Conjunction<FakeCandidate1>>(result);
        }

        [Test]
        public void Or_withoutASpecification_throwsArgumentNullException ()
        {
            // arrange:
            ISpecification<FakeCandidate1> toTest = new SampleSpecification();
            
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                toTest.Or(null);
            });
        }

        [Test]
        public void Or_withAnyCandidateSpecification_returnsAnyCandidateSpecification ()
        {
            // arrange:
            ISpecification<FakeCandidate1> toTest = new SampleSpecification();
            
            // act:
            ISpecification<FakeCandidate1> result = toTest.Or(Any<FakeCandidate1>.Specification);
            
            // assert:
            Assert.AreSame(Any<FakeCandidate1>.Specification, result);
        }
        
        [Test]
        public void Or_withNoCandidateSpecification_returnsItself ()
        {
            // arrange:
            ISpecification<FakeCandidate1> toTest = new SampleSpecification();

            // act:
            ISpecification<FakeCandidate1> result = toTest.Or(No<FakeCandidate1>.Specification);
            
            // assert:
            Assert.AreSame(toTest, result);
        }
        
        [Test]
        public void Or_anotherSpecification_returnsADisjunction ()
        {
            // arrange:
            ISpecification<FakeCandidate1> toTest = new SampleSpecification();
            ISpecification<FakeCandidate1> other = GenerateStrictMock<ISpecification<FakeCandidate1>>();
            
            // act:
            ISpecification<FakeCandidate1> result = toTest.Or(other);
            
            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Disjunction<FakeCandidate1>>(result);
        }

        [Test]
        public void Negate_returnANegation ()
        {
            // arrange:
            ISpecification<FakeCandidate1> toTest1 = new SampleSpecification();
            ISpecification<FakeCandidate2> toTest2 = new SampleSpecification();

            // act:
            ISpecification<FakeCandidate1> result1 = toTest1.Negate();
            ISpecification<FakeCandidate2> result2 = toTest2.Negate();

            // assert:
            Assert.IsNotNull(result1);
            Assert.IsInstanceOf<Negation<FakeCandidate1>>(result1);
            Assert.IsNotNull(result2);
            Assert.IsInstanceOf<Negation<FakeCandidate2>>(result2);
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
            SampleSpecification toSerialize = new SampleSpecification();
            
            // act:
            Stream stream = SerializationUtilities.Serialize(toSerialize);
            
            // assert:
            Assert.IsNotNull (stream);
        }
        
        [Test]
        public void Deserialization_works ()
        {
            // arrange:
            SampleSpecification toSerialize = new SampleSpecification();
            Stream stream = SerializationUtilities.Serialize(toSerialize);
            
            // act:
            SampleSpecification deserialized = SerializationUtilities.Deserialize<SampleSpecification>(stream);
            
            // assert:
            Assert.IsNotNull (deserialized);
        }

    }
}

