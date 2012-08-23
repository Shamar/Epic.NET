//  
//  SliceQA.cs
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
using Epic.Query.Object.Expressions;
using Challenge00.DDDSample.Cargo;
using System.IO;
using System.Collections.Generic;
using Rhino.Mocks;

namespace Epic.Query.Object.UnitTests.Expressions
{
    [TestFixture]
    public class SliceQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutASource_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new Slice<ICargo>(null, 10);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                new Slice<ICargo>(10, null);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                new Slice<ICargo>(null, 7, 5);
            });
        }

        [Test]
        public void Initialize_withASource_works()
        {
            // arrange:
            Expression<IEnumerable<ICargo>> source = GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            OrderCriterion<ICargo> criterion = new Fakes.FakeCriterion<ICargo>();
            Order<ICargo> order = new Order<ICargo>(source, criterion);

            // act:
            Slice<ICargo> toTest = new Slice<ICargo>(order, 7, 10);

            // assert:
            Assert.AreSame(order, toTest.Source);
            Assert.AreEqual(7, toTest.Skipping);
            Assert.AreEqual(10, toTest.TakingAtMost);
        }

        [Test]
        public void Serialization_works()
        {
            // arrange:
            IRepository<ICargo, TrackingId> repository = new Fakes.FakeRepository<ICargo, TrackingId>();
            Source<ICargo, TrackingId> source = new Source<ICargo, TrackingId>(repository);
            OrderCriterion<ICargo> criterion = new Fakes.FakeCriterion<ICargo>();
            Order<ICargo> order = new Order<ICargo>(source, criterion);
            Slice<ICargo> toSerialize = new Slice<ICargo>(order, 7, 10);

            // act:
            Stream stream = SerializationUtilities.Serialize(toSerialize);

            // assert:
            Assert.IsNotNull (stream);
        }

        [Test]
        public void Deserialization_works()
        {
            // arrange:
            IRepository<ICargo, TrackingId> repository = new Fakes.FakeRepository<ICargo, TrackingId>();
            Source<ICargo, TrackingId> source = new Source<ICargo, TrackingId>(repository);
            OrderCriterion<ICargo> criterion = new Fakes.FakeCriterion<ICargo>();
            Order<ICargo> order = new Order<ICargo>(source, criterion);
            Slice<ICargo> toSerialize = new Slice<ICargo>(order, 7, 10);
            Stream stream = SerializationUtilities.Serialize(toSerialize);

            // act:
            Slice<ICargo> deserialized = SerializationUtilities.Deserialize<Slice<ICargo>>(stream);

            // assert:
            Assert.IsNotNull (deserialized);
            Assert.IsNotNull (deserialized.Source);
            Assert.IsInstanceOf<Order<ICargo>>(deserialized.Source);
            Assert.AreEqual(7, deserialized.Skipping);
            Assert.AreEqual(10, deserialized.TakingAtMost);
        }

        [Test]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            Expression<IEnumerable<ICargo>> source = GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            OrderCriterion<ICargo> criterion = new Fakes.FakeCriterion<ICargo>();
            Order<ICargo> order = new Order<ICargo>(source, criterion);
            Slice<ICargo> toTest = new Slice<ICargo>(order, 7, 10);
            object expectedResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            IVisitor<object, Slice<ICargo>> expressionVisitor = GenerateStrictMock<IVisitor<object, Slice<ICargo>>>();
            expressionVisitor.Expect(v => v.Visit(toTest, context)).Return(expectedResult).Repeat.Once();
            IVisitor<object> visitor = GenerateStrictMock<IVisitor<object>>();
            visitor.Expect(v => v.AsVisitor(toTest)).Return(expressionVisitor).Repeat.Once ();

            // act:
            object result = toTest.Accept(visitor, context);

            // assert:
            Assert.AreSame(expectedResult, result);
        }

        [Test]
        public void Initialize_takingZeroItems_throwsArgumentOutOfRangeException()
        {
            // arrange:
            Expression<IEnumerable<ICargo>> source = GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            OrderCriterion<ICargo> criterion = new Fakes.FakeCriterion<ICargo>();
            Order<ICargo> order = new Order<ICargo>(source, criterion);

            // assert:
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                new Slice<ICargo>(order, 10, 0);
            });
        }

        [Test]
        public void Initialize_bypassingMaxValueItems_throwsArgumentOutOfRangeException()
        {
            // arrange:
            Expression<IEnumerable<ICargo>> source = GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            OrderCriterion<ICargo> criterion = new Fakes.FakeCriterion<ICargo>();
            Order<ICargo> order = new Order<ICargo>(source, criterion);

            // assert:
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                new Slice<ICargo>(order, uint.MaxValue, 7);
            });
        }
    }
}

