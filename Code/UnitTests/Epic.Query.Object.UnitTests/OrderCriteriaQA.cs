//  
//  OrderCriteriaQA.cs
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
    public class OrderCriteriaQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutAnyArgument_throwsArgumentNullException()
        {
            // arrange:
            OrderCriterion<ICargo> other = new Fakes.FakeCriterion<ICargo>();

            // assert:
            Assert.Throws<ArgumentNullException>(delegate { 
                new OrderCriteria<ICargo>(null, other);
            });
            Assert.Throws<ArgumentNullException>(delegate { 
                new OrderCriteria<ICargo>(other, null);
            });
        }

        [Test]
        public void Initialize_withTwoCriterion_works()
        {
            // arrange:
            OrderCriterion<ICargo> first = new Fakes.FakeCriterion<ICargo>();
            OrderCriterion<ICargo> second = new Fakes.FakeCriterion<ICargo>();


            // act:
            OrderCriteria<ICargo> toTest = new OrderCriteria<ICargo>(first, second);

            // assert:
            Assert.AreSame(first, toTest.ElementAt(0));
            Assert.AreSame(second, toTest.ElementAt(1));
        }

        [Test]
        public void Initialize_withACriterionAndASetOfCriteria_works()
        {
            // arrange:
            OrderCriterion<ICargo> first = new Fakes.FakeCriterion<ICargo>();
            OrderCriterion<ICargo> second = new Fakes.FakeCriterion<ICargo>();
            OrderCriterion<ICargo> third = new Fakes.FakeCriterion<ICargo>();
            OrderCriteria<ICargo> setOfCriteria = new OrderCriteria<ICargo>(second, third);

            // act:
            OrderCriteria<ICargo> toTest = new OrderCriteria<ICargo>(first, setOfCriteria);

            // assert:
            Assert.AreSame(first, toTest.ElementAt(0));
            Assert.AreSame(second, toTest.ElementAt(1));
            Assert.AreSame(third, toTest.ElementAt(2));
        }

        [Test]
        public void Initialize_withASetOfCriteriaAndACriterion_works()
        {
            // arrange:
            OrderCriterion<ICargo> first = new Fakes.FakeCriterion<ICargo>();
            OrderCriterion<ICargo> second = new Fakes.FakeCriterion<ICargo>();
            OrderCriterion<ICargo> third = new Fakes.FakeCriterion<ICargo>();
            OrderCriteria<ICargo> setOfCriteria = new OrderCriteria<ICargo>(first, second);

            // act:
            OrderCriteria<ICargo> toTest = new OrderCriteria<ICargo>(setOfCriteria, third);

            // assert:
            Assert.AreSame(first, toTest.ElementAt(0));
            Assert.AreSame(second, toTest.ElementAt(1));
            Assert.AreSame(third, toTest.ElementAt(2));
        }

        [Test]
        public void Serialization_works()
        {
            // arrange:
            OrderCriterion<ICargo> first = new Fakes.FakeCriterion<ICargo>();
            OrderCriterion<ICargo> second = new Fakes.FakeCriterion<ICargo>();
            OrderCriteria<ICargo> toSerialize = new OrderCriteria<ICargo>(first, second);

            // act:
            Stream stream = SerializationUtilities.Serialize(toSerialize);

            // assert:
            Assert.IsNotNull (stream);
        }

        [Test]
        public void Deserialization_works()
        {
            // arrange:
            OrderCriterion<ICargo> first = new Fakes.FakeCriterion<ICargo>(7);
            OrderCriterion<ICargo> second = new Fakes.FakeCriterion<ICargo>(10);
            OrderCriteria<ICargo> toSerialize = new OrderCriteria<ICargo>(first, second);
            Stream stream = SerializationUtilities.Serialize(toSerialize);

            // act:
            OrderCriteria<ICargo> deserialized = SerializationUtilities.Deserialize<OrderCriteria<ICargo>>(stream);

            // assert:
            Assert.IsNotNull (deserialized);
            Assert.AreEqual (first, deserialized.ElementAt(0));
            Assert.AreEqual (second, deserialized.ElementAt(1));
        }

        [Test]
        public void Serialization_withoutSerializationInfo_throwsArgumentNullException()
        {
            // arrange:
            OrderCriterion<ICargo> first = new Fakes.FakeCriterion<ICargo>(7);
            OrderCriterion<ICargo> second = new Fakes.FakeCriterion<ICargo>(10);
            ISerializable toTest = new OrderCriteria<ICargo>(first, second);

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                toTest.GetObjectData(null, default(StreamingContext));
            });
        }

        [Test]
        public void Chain_withAnotherCriterion_works()
        {
            // arrange:
            OrderCriterion<ICargo> first = new Fakes.FakeCriterion<ICargo>();
            OrderCriterion<ICargo> second = new Fakes.FakeCriterion<ICargo>();
            OrderCriterion<ICargo> third = new Fakes.FakeCriterion<ICargo>();
            OrderCriteria<ICargo> toTest = new OrderCriteria<ICargo>(first, second);

            // act:
            OrderCriterion<ICargo> result = toTest.Chain(third);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OrderCriteria<ICargo>>(result);
            OrderCriteria<ICargo> newCriteria = (OrderCriteria<ICargo>)result;
            Assert.AreSame(first, newCriteria.ElementAt(0));
            Assert.AreSame(second, newCriteria.ElementAt(1));
            Assert.AreSame(third, newCriteria.ElementAt(2));
        }

        [Test]
        public void Chain_withoutAnotherCriterion_works()
        {
            // arrange:
            OrderCriterion<ICargo> first = new Fakes.FakeCriterion<ICargo>();
            OrderCriterion<ICargo> second = new Fakes.FakeCriterion<ICargo>();
            OrderCriteria<ICargo> toTest = new OrderCriteria<ICargo>(first, second);

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                toTest.Chain(null);
            });
        }

        [Test]
        public void GetEnumerator_works()
        {
            // arrange:
            OrderCriterion<ICargo> first = new Fakes.FakeCriterion<ICargo>(1);
            OrderCriterion<ICargo> second = new Fakes.FakeCriterion<ICargo>(2);
            OrderCriterion<ICargo>[] expected = new OrderCriterion<ICargo>[] { first, second };
            OrderCriteria<ICargo> toTest = new OrderCriteria<ICargo>(first, second);

            // assert:
            int i = 0;
            foreach(OrderCriterion<ICargo> o in toTest)
            {
                Assert.AreSame(expected[i], o);
                ++i;
            }
            i = 0;
            foreach(object o in (toTest as System.Collections.IEnumerable))
            {
                Assert.AreSame(expected[i], o);
                ++i;
            }
        }

        [Test]
        public void Reverse_returnsCriteriaInReversedOrder()
        {
            // arrange:
            OrderCriterion<ICargo> first = new Fakes.FakeCriterion<ICargo>(1);
            OrderCriterion<ICargo> second = new Fakes.FakeCriterion<ICargo>(2);
            OrderCriterion<ICargo>[] expected = new OrderCriterion<ICargo>[] { second, first };
            OrderCriteria<ICargo> toTest = new OrderCriteria<ICargo>(first, second);

            // act:
            OrderCriterion<ICargo> result = toTest.Reverse();

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OrderCriteria<ICargo>>(result);
            OrderCriteria<ICargo> newCriteria = (OrderCriteria<ICargo>)result;
            int i = 0;
            foreach(OrderCriterion<ICargo> o in newCriteria)
            {
                Assert.AreSame(expected[i], o);
                ++i;
            }
        }

        [Test]
        public void Equals_withDifferentNumberOfCriteria_isFalse()
        {
            // arrange:
            OrderCriterion<ICargo> first = new Fakes.FakeCriterion<ICargo>(1);
            OrderCriterion<ICargo> second = new Fakes.FakeCriterion<ICargo>(2);
            OrderCriterion<ICargo> third = new Fakes.FakeCriterion<ICargo>(3);
            OrderCriteria<ICargo> toTest = new OrderCriteria<ICargo>(first, second);
            OrderCriteria<ICargo> other = new OrderCriteria<ICargo>(toTest, third);

            // act:
            bool result = toTest.Equals(other);

            // assert:
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_toSameCriteriaInSameOrder_isTrue()
        {
            // arrange:
            OrderCriterion<ICargo> first = new Fakes.FakeCriterion<ICargo>(1);
            OrderCriterion<ICargo> second = new Fakes.FakeCriterion<ICargo>(2);
            OrderCriteria<ICargo> toTest = new OrderCriteria<ICargo>(first, second);
            OrderCriteria<ICargo> other = new OrderCriteria<ICargo>(first, second);

            // act:
            bool result = toTest.Equals(other);

            // assert:
            Assert.IsTrue(result);
        }

        [Test]
        public void Equals_toSameCriteriaInDifferentOrder_isFalse()
        {
            // arrange:
            OrderCriterion<ICargo> first = new Fakes.FakeCriterion<ICargo>(1);
            OrderCriterion<ICargo> second = new Fakes.FakeCriterion<ICargo>(2);
            OrderCriteria<ICargo> toTest = new OrderCriteria<ICargo>(first, second);
            OrderCriteria<ICargo> other = new OrderCriteria<ICargo>(second, first);

            // act:
            bool result = toTest.Equals(other);

            // assert:
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_withDifferentCriteria_isFalse()
        {
            // arrange:
            OrderCriterion<ICargo> first = new Fakes.FakeCriterion<ICargo>(1);
            OrderCriterion<ICargo> second = new Fakes.FakeCriterion<ICargo>(2);
            OrderCriterion<ICargo> third = new Fakes.FakeCriterion<ICargo>(3);
            OrderCriteria<ICargo> toTest = new OrderCriteria<ICargo>(first, second);
            OrderCriteria<ICargo> other = new OrderCriteria<ICargo>(first, third);

            // act:
            bool result = toTest.Equals(other);

            // assert:
            Assert.IsFalse(result);
        }

        [Test]
        public void Compare_wheneverReachANonZero_returnsThatValue()
        {
            // arrange:
            List<int> called = new List<int>();
            OrderCriterion<ICargo> first = new Fakes.FakeCriterion<ICargo>(1, (x, y) => {called.Add(1); return 0;});
            OrderCriterion<ICargo> second = new Fakes.FakeCriterion<ICargo>(2, (x, y) => {called.Add(2); return -1;});
            OrderCriterion<ICargo> third = new Fakes.FakeCriterion<ICargo>(3, (x, y) => {Assert.Fail("This criterion should not be used during comparison."); return 1;});
            OrderCriteria<ICargo> toTest = new OrderCriteria<ICargo>(first, second);
            toTest = new OrderCriteria<ICargo>(toTest, third);
            ICargo a = GenerateStrictMock<ICargo>();
            ICargo b = GenerateStrictMock<ICargo>();

            // act:
            int result = toTest.Compare(a, b);

            // assert:
            Assert.AreEqual(-1, result);
            Assert.Contains(1, called);
            Assert.Contains(2, called);
        }

        [Test]
        public void Compare_wheneverDontReachANonZero_returnsZero()
        {
            // arrange:
            List<int> called = new List<int>();
            OrderCriterion<ICargo> first = new Fakes.FakeCriterion<ICargo>(1, (x, y) => {called.Add(1); return 0;});
            OrderCriterion<ICargo> second = new Fakes.FakeCriterion<ICargo>(2, (x, y) => {called.Add(2); return 0;});
            OrderCriterion<ICargo> third = new Fakes.FakeCriterion<ICargo>(3, (x, y) => {called.Add(3); return 0;});
            OrderCriteria<ICargo> toTest = new OrderCriteria<ICargo>(first, second);
            toTest = new OrderCriteria<ICargo>(toTest, third);
            ICargo a = GenerateStrictMock<ICargo>();
            ICargo b = GenerateStrictMock<ICargo>();

            // act:
            int result = toTest.Compare(a, b);

            // assert:
            Assert.AreEqual(0, result);
            Assert.Contains(1, called);
            Assert.Contains(2, called);
            Assert.Contains(3, called);
        }

        
        [Test]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            OrderCriterion<ICargo> first = new Fakes.FakeCriterion<ICargo>(1);
            OrderCriterion<ICargo> second = new Fakes.FakeCriterion<ICargo>(2);
            OrderCriteria<ICargo> toTest = new OrderCriteria<ICargo>(first, second);
            object expectedResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            IVisitor<object, OrderCriteria<ICargo>> expressionVisitor = GenerateStrictMock<IVisitor<object, OrderCriteria<ICargo>>>();
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

