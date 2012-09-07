//  
//  ReverseOrderQA.cs
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
using Challenge00.DDDSample.Cargo;
using System.IO;
using System.Runtime.Serialization;
using System.Linq;
using Rhino.Mocks;

namespace Epic.Query.Object.UnitTests
{
    [TestFixture()]
    public class ReverseOrderQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutACriterionToReverse_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new ReverseOrder<ICargo>(null);
            });
        }

        [Test]
        public void Initialize_withACriterionToReverse_works()
        {
            // arrange:
            OrderCriterion<ICargo> other = new Fakes.FakeCriterion<ICargo>();

            // act:
            ReverseOrder<ICargo> reverse = new ReverseOrder<ICargo>(other);

            // assert:
            Assert.AreSame(other, reverse.Reversed);
        }

        [Test]
        public void Serialization_works()
        {
            // arrange:
            OrderCriterion<ICargo> other = new Fakes.FakeCriterion<ICargo>();
            ReverseOrder<ICargo> toSerialize = new ReverseOrder<ICargo>(other);

            // act:
            Stream stream = SerializationUtilities.Serialize(toSerialize);

            // assert:
            Assert.IsNotNull (stream);
        }

        [Test]
        public void Deserialization_works()
        {
            // arrange:
            OrderCriterion<ICargo> other = new Fakes.FakeCriterion<ICargo>(10);
            ReverseOrder<ICargo> toSerialize = new ReverseOrder<ICargo>(other);
            Stream stream = SerializationUtilities.Serialize(toSerialize);

            // act:
            ReverseOrder<ICargo> deserialized = SerializationUtilities.Deserialize<ReverseOrder<ICargo>>(stream);

            // assert:
            Assert.IsNotNull (deserialized);
            Assert.AreEqual(other, deserialized.Reversed);
        }

        [Test]
        public void Serialization_withoutSerializationInfo_throwsArgumentNullException()
        {
            // arrange:
            OrderCriterion<ICargo> other = new Fakes.FakeCriterion<ICargo>(10);
            ISerializable toTest = new ReverseOrder<ICargo>(other);

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                toTest.GetObjectData(null, default(StreamingContext));
            });
        }

        [Test]
        public void Reverse_returnsTheOrigininalCriterion()
        {
            // arrange:
            OrderCriterion<ICargo> other = new Fakes.FakeCriterion<ICargo>();
            OrderCriterion<ICargo> toTest = new ReverseOrder<ICargo>(other);

            // act:
            OrderCriterion<ICargo> result = toTest.Reverse();

            // assert:
            Assert.AreSame(other, result);
        }

        [Test]
        public void Equals_toAReverseOrderReversingAnEqualCriterion_isTrue()
        {
            // arrange:
            OrderCriterion<ICargo> reversed1 = new Fakes.FakeCriterion<ICargo>(10);
            OrderCriterion<ICargo> reversed2 = new Fakes.FakeCriterion<ICargo>(10);
            Assert.IsTrue(reversed1.Equals(reversed2) && reversed2.Equals(reversed1)); // just to be safe.
            OrderCriterion<ICargo> toTest = new ReverseOrder<ICargo>(reversed1);
            OrderCriterion<ICargo> other = new ReverseOrder<ICargo>(reversed2);

            // act:
            bool result = toTest.Equals(other);

            // assert:
            Assert.IsTrue(result);
        }

        [Test]
        public void Equals_toAReverseOrderReversingADifferentCriterion_isFalse()
        {
            // arrange:
            OrderCriterion<ICargo> reversed1 = new Fakes.FakeCriterion<ICargo>(10);
            OrderCriterion<ICargo> reversed2 = new Fakes.FakeCriterion<ICargo>(1);
            Assert.IsFalse(reversed1.Equals(reversed2) && reversed2.Equals(reversed1)); // just to be safe.
            OrderCriterion<ICargo> toTest = new ReverseOrder<ICargo>(reversed1);
            OrderCriterion<ICargo> other = new ReverseOrder<ICargo>(reversed2);

            // act:
            bool result = toTest.Equals(other);

            // assert:
            Assert.IsFalse(result);
        }

        [Test]
        public void Chain_withAnotherCriterion_returnsAnOrderCriteriaWithTheOriginalAsFirst()
        {
            // arrange:
            OrderCriterion<ICargo> other = new Fakes.FakeCriterion<ICargo>(5);
            OrderCriterion<ICargo> reversed = new Fakes.FakeCriterion<ICargo>(10);
            OrderCriterion<ICargo> toTest = new ReverseOrder<ICargo>(reversed);

            // act:
            OrderCriterion<ICargo> chain = toTest.Chain(other);

            // assert:
            Assert.IsNotNull(chain);
            Assert.IsInstanceOf<OrderCriteria<ICargo>>(chain);
            OrderCriteria<ICargo> criteria = (OrderCriteria<ICargo>)chain;
            Assert.AreEqual (toTest, criteria.ElementAt(0));
            Assert.AreEqual (other, criteria.ElementAt(1));
        }

        [Test]
        public void Chain_withoutAnotherCriterion_throwsArgumentNullException()
        {
            // arrange:
            OrderCriterion<ICargo> reversed = new Fakes.FakeCriterion<ICargo>(10);
            OrderCriterion<ICargo> toTest = new ReverseOrder<ICargo>(reversed);

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                toTest.Chain(null);
            });
        }

        [Test]
        public void Compare_withArguments_callTheReversedInvertingThem()
        {
            // arrange:
            int expectedResult = 1;
            ICargo xRecieved = null;
            ICargo yRecieved = null;
            ICargo x = GenerateStrictMock<ICargo>();
            ICargo y = GenerateStrictMock<ICargo>();
            OrderCriterion<ICargo> reversed = new Fakes.FakeCriterion<ICargo>(10, (c1, c2) => {
                xRecieved = c1;
                yRecieved = c2;
                return expectedResult;
            });
            OrderCriterion<ICargo> toTest = new ReverseOrder<ICargo>(reversed);

            // act:
            int comparisonResult = toTest.Compare(x, y);

            // assert:
            Assert.AreEqual(expectedResult, comparisonResult);
            Assert.AreSame(y, xRecieved);
            Assert.AreSame(x, yRecieved);
        }
                
        [Test]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            OrderCriterion<ICargo> reversed = new Fakes.FakeCriterion<ICargo>(10);
            ReverseOrder<ICargo> toTest = new ReverseOrder<ICargo>(reversed);
            object expectedResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            IVisitor<object, ReverseOrder<ICargo>> expressionVisitor = GenerateStrictMock<IVisitor<object, ReverseOrder<ICargo>>>();
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

