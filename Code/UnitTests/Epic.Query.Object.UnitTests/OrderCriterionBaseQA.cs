//  
//  OrderCriterionBaseQA.cs
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
using NUnit.Framework;
using Challenge00.DDDSample.Cargo;
using System.IO;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using Rhino.Mocks;

namespace Epic.Query.Object.UnitTests
{
    [TestFixture]
    public class OrderCriterionBaseQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_aCriterionOfAWrongType_throwsEpicException()
        {
            // assert:
            Assert.Throws<EpicException>(delegate {
                new Fakes.WrongFakeCriterion<ICargo>();
            });
        }

        [Test]
        public void Initialize_aCriterionWithTheRightType_works()
        {
            // act:
            OrderCriterion<ICargo> criterion = new Fakes.FakeCriterion<ICargo>();

            // assert:
            Assert.IsNotNull(criterion);
        }

        [Test]
        public void Serialization_works()
        {
            // arrange:
            OrderCriterion<ICargo> toSerialize = new Fakes.FakeCriterion<ICargo>(10);

            // act:
            Stream stream = SerializationUtilities.Serialize(toSerialize);

            // assert:
            Assert.IsNotNull (stream);
        }

        [Test]
        public void Deserialization_works()
        {
            // arrange:
            OrderCriterion<ICargo> toSerialize = new Fakes.FakeCriterion<ICargo>(10);
            Stream stream = SerializationUtilities.Serialize(toSerialize);

            // act:
            Fakes.FakeCriterion<ICargo> deserialized = SerializationUtilities.Deserialize<Fakes.FakeCriterion<ICargo>>(stream);

            // assert:
            Assert.IsNotNull (deserialized);
            Assert.AreEqual (10, deserialized.Identity);
        }

        [Test]
        public void Deserialization_withoutSerializationInfo_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new Fakes.FakeCriterion<ICargo>(null, default(StreamingContext));
            });
        }

        [Test]
        public void Serialization_withoutSerializationInfo_throwsArgumentNullException()
        {
            // arrange:
            ISerializable toTest = new Fakes.FakeCriterion<ICargo>();

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                toTest.GetObjectData(null, default(StreamingContext));
            });
        }

        [Test]
        public void Equals_toNull_isFalse()
        {
            // arrange:
            OrderCriterion<ICargo> toTest = new Fakes.FakeCriterion<ICargo>(10);

            // act:
            bool equalsNullCriterion = toTest.Equals((OrderCriterion<ICargo>)null);
            bool equalsNullObject = toTest.Equals((object)null);

            // assert:
            Assert.IsFalse (equalsNullCriterion);
            Assert.IsFalse (equalsNullObject);
        }

        [Test]
        public void Equals_toItself_isTrue()
        {
            // arrange:
            OrderCriterion<ICargo> toTest = new Fakes.FakeCriterion<ICargo>(10);

            // act:
            bool equalsCriterion = toTest.Equals((OrderCriterion<ICargo>)toTest);
            bool equalsObject = toTest.Equals((object)toTest);

            // assert:
            Assert.IsTrue (equalsCriterion);
            Assert.IsTrue (equalsObject);
        }

        [Test]
        public void Equals_toEqualCriterion_isTrue()
        {
            // arrange:
            OrderCriterion<ICargo> toTest = new Fakes.FakeCriterion<ICargo>(10);
            OrderCriterion<ICargo> other = new Fakes.FakeCriterion<ICargo>(10);

            // act:
            bool equalsCriterion = toTest.Equals((OrderCriterion<ICargo>)other);
            bool equalsObject = toTest.Equals((object)other);

            // assert:
            Assert.IsTrue (equalsCriterion);
            Assert.IsTrue (equalsObject);
            Assert.AreEqual(toTest.GetHashCode(), other.GetHashCode());
        }

        [Test]
        public void Equals_toAnotherTypeOfCriterion_isFalse()
        {
            // arrange:
            OrderCriterion<ICargo> toTest = new Fakes.FakeCriterion<ICargo>();
            OrderCriterion<ICargo> other = new Fakes.OtherFakeCriterion<ICargo>();

            // act:
            bool equalsCriterion = toTest.Equals((OrderCriterion<ICargo>)other);
            bool equalsObject = toTest.Equals((object)other);

            // assert:
            Assert.IsFalse (equalsCriterion);
            Assert.IsFalse (equalsObject);
        }

        [Test]
        public void Equals_toAnotherInstanceOfTheSameType_callSafeEquals()
        {
            // arrange:
            Fakes.FakeCriterion<ICargo> toTest = new Fakes.FakeCriterion<ICargo>(3);
            OrderCriterion<ICargo> other = new Fakes.FakeCriterion<ICargo>(4);

            // act:
            bool equalsCriterion = toTest.Equals((OrderCriterion<ICargo>)other);
            bool equalsObject = toTest.Equals((object)other);

            // assert:
            Assert.IsFalse (equalsCriterion);
            Assert.IsFalse (equalsObject);
            Assert.IsTrue(toTest.SafeEqualsCalled);
        }

        [Test]
        public void Chain_toAnotherCriterion_returnsOrderCriteriaInTheRightOrder()
        {
            // arrange:
            Fakes.FakeCriterion<ICargo> first = new Fakes.FakeCriterion<ICargo>(1);
            Fakes.FakeCriterion<ICargo> second = new Fakes.FakeCriterion<ICargo>(2);

            // act:
            OrderCriterion<ICargo> result = first.Chain(second);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OrderCriteria<ICargo>>(result);
            OrderCriteria<ICargo> criteria = (OrderCriteria<ICargo>)result;
            Assert.AreSame(first, criteria.ElementAt(0));
            Assert.AreSame(second, criteria.ElementAt(1));
        }
       
        [Test]
        public void Chain_toNull_throwsArgumentNullException()
        {
            // arrange:
            Fakes.FakeCriterion<ICargo> toTest = new Fakes.FakeCriterion<ICargo>(3);

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                toTest.Chain(null);
            });
        }

        [Test]
        public void Reverse_returnsAReversedInstance()
        {
            // arrange:
            Fakes.FakeCriterion<ICargo> toTest = new Fakes.FakeCriterion<ICargo>(1);

            // act:
            OrderCriterion<ICargo> result = toTest.Reverse();

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ReverseOrder<ICargo>>(result);
        }

        [Test]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            Fakes.FakeCriterion<ICargo> toTest = new Fakes.FakeCriterion<ICargo>(1);
            object expectedResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            IVisitor<object, Fakes.FakeCriterion<ICargo>> expressionVisitor = GenerateStrictMock<IVisitor<object, Fakes.FakeCriterion<ICargo>>>();
            expressionVisitor.Expect(v => v.Visit(toTest, context)).Return(expectedResult).Repeat.Once();
            IVisitor<object> visitor = GenerateStrictMock<IVisitor<object>>();
            visitor.Expect(v => v.AsVisitor(toTest)).Return(expressionVisitor).Repeat.Once ();

            // act:
            object result = toTest.Accept(visitor, context);

            // assert:
            Assert.AreSame(expectedResult, result);
        }

    }
}

