//
//  NoQA.cs
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
using System.IO;
using Rhino.Mocks;

namespace Epic.Specifications
{
    [TestFixture]
    public class NoQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Singleton_Specification_isNotNull ()
        {
            // assert:
            Assert.IsNotNull(No<string>.Specification);
        }
        
        [Test]
        public void Serialization_works ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> toSerialize = No<Fakes.FakeCandidate1>.Specification;
            
            // act:
            Stream stream = SerializationUtilities.Serialize(toSerialize);
            
            // assert:
            Assert.IsNotNull (stream);
        }
        
        [Test]
        public void Deserialization_works ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> toSerialize = No<Fakes.FakeCandidate1>.Specification;
            Stream stream = SerializationUtilities.Serialize(toSerialize);
            
            // act:
            ISpecification<Fakes.FakeCandidate1> deserialized = SerializationUtilities.Deserialize<ISpecification<Fakes.FakeCandidate1>>(stream);
            
            // assert:
            Assert.IsNotNull (deserialized);
            Assert.AreSame(No<Fakes.FakeCandidate1>.Specification, deserialized);
        }
        
        [Test]
        public void IsSatisfiedBy_aCandidate_returnFalse ()
        {
            // arrange:
            Fakes.FakeCandidate1 candidate = new Epic.Fakes.FakeCandidate1();
            ISpecification<Fakes.FakeCandidate1> toTest = No<Fakes.FakeCandidate1>.Specification;
            
            // act:
            bool result = toTest.IsSatisfiedBy(candidate);
            
            // assert:
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsSatisfiedBy_null_returnFalse ()
        {
            // arrange:
            Fakes.FakeCandidate1 candidate = null;
            ISpecification<Fakes.FakeCandidate1> toTest = No<Fakes.FakeCandidate1>.Specification;
            
            // act:
            bool result = toTest.IsSatisfiedBy(candidate);
            
            // assert:
            Assert.IsFalse(result);
        }
        
        [Test]
        public void Equals_toItself_isTrue ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> toTest = No<Fakes.FakeCandidate1>.Specification;
            
            // assert:
            Assert.IsTrue(toTest.Equals((object)toTest));
            Assert.IsTrue(toTest.Equals(toTest));
            Assert.IsTrue(toTest.Equals(No<Fakes.FakeCandidate1>.Specification));
        }
        
        [Test]
        public void Negate_returnsAnAnySpecification ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> toTest = No<Fakes.FakeCandidate1>.Specification;
            
            // act:
            ISpecification<Fakes.FakeCandidate1> result = toTest.Negate();
            
            // assert:
            Assert.IsNotNull(toTest);
            Assert.IsInstanceOf<Any<Fakes.FakeCandidate1>>(result);
        }
        
        [Test]
        public void Or_anotherSpecification_returnsTheOtherSpecification ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> toTest = No<Fakes.FakeCandidate1>.Specification;
            ISpecification<Fakes.FakeCandidate1> other = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>>();
            
            // act:
            ISpecification<Fakes.FakeCandidate1> result = toTest.Or(other);

            // assert:
            Assert.AreSame(other, result);
        }
        
        [Test]
        public void And_anotherSpecification_returnsItself ()
        {
            // arrange:
            ISpecification<Fakes.FakeCandidate1> toTest = No<Fakes.FakeCandidate1>.Specification;
            ISpecification<Fakes.FakeCandidate1> other = GenerateStrictMock<ISpecification<Fakes.FakeCandidate1>>();
            
            // act:
            ISpecification<Fakes.FakeCandidate1> result = toTest.And(other);
            
            // assert:
            Assert.AreSame(toTest, result);
        }

        [Test]
        public void OfType_withASpecialization_returnASpecializedSpecification()
        {
            // arrange:
            var toTest = No<Fakes.FakeCandidate1>.Specification;
            
            // act:
            var result = toTest.OfType<Fakes.FakeCandidate1Specialization>();
            
            // assert:
            Assert.AreSame(No<Fakes.FakeCandidate1Specialization>.Specification, result);
        }
        
        [Test]
        public void OfType_withAGeneralization_returnAnUpcastingVariant()
        {
            // arrange:
            var toTest = No<Fakes.FakeCandidate1>.Specification;
            
            // act:
            var result = toTest.OfType<Fakes.FakeCandidate1Abstraction>();
            
            // assert:
            Assert.IsInstanceOf<Variant<Fakes.FakeCandidate1, Fakes.FakeCandidate1Abstraction>>(result);
            Assert.AreSame(toTest, (result as Variant<Fakes.FakeCandidate1, Fakes.FakeCandidate1Abstraction>).InnerSpecification);
        }

        [Test]
        public void ToString_worksAsExpected()
        {
            // arrange:
            var toTest = No<Fakes.FakeCandidate1>.Specification;

            // act:
            string result = toTest.ToString();

            // assert:
            Assert.AreEqual(string.Format("¬({0})", Any<Fakes.FakeCandidate1>.Specification), result);
        }
    }
}
