//  
//  SourceQA.cs
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
    public class SourceQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutASource_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new Source<ICargo,TrackingId>(null);
            });
        }

        [Test]
        public void Initialize_withASource_works()
        {
            // arrange:
            IRepository<ICargo, TrackingId> repository = GenerateStrictMock<IRepository<ICargo, TrackingId>>();

            // act:
            Source<ICargo, TrackingId> toTest = new Source<ICargo, TrackingId>(repository);

            // assert:
            Assert.AreSame(repository, toTest.Repository);
        }

        [Test]
        public void Serialization_works()
        {
            // arrange:
            IRepository<ICargo, TrackingId> repository = new Fakes.FakeRepository<ICargo, TrackingId>();
            Source<ICargo, TrackingId> toSerialize = new Source<ICargo, TrackingId>(repository);
 
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
            Source<ICargo, TrackingId> toSerialize = new Source<ICargo, TrackingId>(repository);
            Stream stream = SerializationUtilities.Serialize(toSerialize);

            // act:
            Source<ICargo, TrackingId> deserialized = SerializationUtilities.Deserialize<Source<ICargo, TrackingId>>(stream);

            // assert:
            Assert.IsNotNull (deserialized);
            Assert.IsNotNull (deserialized.Repository);
            Assert.IsInstanceOf<Fakes.FakeRepository<ICargo, TrackingId>>(deserialized.Repository);

        }

        [Test]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            IRepository<ICargo, TrackingId> repository = new Fakes.FakeRepository<ICargo, TrackingId>();
            Source<ICargo, TrackingId> toTest = new Source<ICargo, TrackingId>(repository);
            object expectedResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            IVisitor<object, Source<ICargo, TrackingId>> expressionVisitor = GenerateStrictMock<IVisitor<object, Source<ICargo, TrackingId>>>();
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

