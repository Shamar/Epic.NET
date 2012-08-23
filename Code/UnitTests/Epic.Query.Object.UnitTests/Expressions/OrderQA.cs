//  
//  OrderQA.cs
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
    public class OrderQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutASource_throwsArgumentNullException()
        {
            // arrange:
            Expression<IEnumerable<ICargo>> source = GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            OrderCriterion<ICargo> criterion = new Fakes.FakeCriterion<ICargo>();
            
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new Order<ICargo>(null, criterion);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                new Order<ICargo>(source, null);
            });
        }

        [Test]
        public void Initialize_withASource_works()
        {
            // arrange:
            Expression<IEnumerable<ICargo>> source = GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            OrderCriterion<ICargo> criterion = new Fakes.FakeCriterion<ICargo>();

            // act:
            Order<ICargo> toTest = new Order<ICargo>(source, criterion);

            // assert:
            Assert.AreSame(source, toTest.Source);
            Assert.AreSame(criterion, toTest.Criterion);
        }

        [Test]
        public void Serialization_works()
        {
            // arrange:
            IRepository<ICargo, TrackingId> repository = new Fakes.FakeRepository<ICargo, TrackingId>();
            Source<ICargo, TrackingId> source = new Source<ICargo, TrackingId>(repository);
            OrderCriterion<ICargo> criterion = new Fakes.FakeCriterion<ICargo>();
            Order<ICargo> toSerialize = new Order<ICargo>(source, criterion);

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
            Order<ICargo> toSerialize = new Order<ICargo>(source, criterion);
            Stream stream = SerializationUtilities.Serialize(toSerialize);

            // act:
            Order<ICargo> deserialized = SerializationUtilities.Deserialize<Order<ICargo>>(stream);

            // assert:
            Assert.IsNotNull (deserialized);
            Assert.IsNotNull (deserialized.Source);
            Assert.IsInstanceOf<Source<ICargo, TrackingId>>(deserialized.Source);
            Assert.IsInstanceOf<Fakes.FakeCriterion<ICargo>>(deserialized.Criterion);
        }

        [Test]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            Expression<IEnumerable<ICargo>> source = GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            OrderCriterion<ICargo> criterion = new Fakes.FakeCriterion<ICargo>();
            Order<ICargo> toTest = new Order<ICargo>(source, criterion);
            object expectedResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            IVisitor<object, Order<ICargo>> expressionVisitor = GenerateStrictMock<IVisitor<object, Order<ICargo>>>();
            expressionVisitor.Expect(v => v.Visit(toTest, context)).Return(expectedResult).Repeat.Once();
            IVisitor<object> visitor = GenerateStrictMock<IVisitor<object>>();
            visitor.Expect(v => v.AsVisitor(toTest)).Return(expressionVisitor).Repeat.Once ();

            // act:
            object result = toTest.Accept(visitor, context);

            // assert:
            Assert.AreSame(expectedResult, result);
        }
        
        [Test]
        public void ThenBy_withoutACriterion_throwsArgumentNullException()
        {
            // arrange:
            Expression<IEnumerable<ICargo>> source = GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            OrderCriterion<ICargo> criterion = new Fakes.FakeCriterion<ICargo>();
            Order<ICargo> toTest = new Order<ICargo>(source, criterion);

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                toTest.ThanBy(null);
            });
        }

        [Test]
        public void ThenBy_withAnotherCriterion_returnsANewOrder()
        {
            // arrange:
            Expression<IEnumerable<ICargo>> source = GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            OrderCriterion<ICargo> criterion = new Fakes.FakeCriterion<ICargo>();
            OrderCriterion<ICargo> otherCriterion = new Fakes.FakeCriterion<ICargo>();
            Order<ICargo> toTest = new Order<ICargo>(source, criterion);

            // assert:
            Order<ICargo> result = toTest.ThanBy(otherCriterion);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Criterion);
            Assert.IsInstanceOf<OrderCriteria<ICargo>>(result.Criterion);
        }
    }
}

