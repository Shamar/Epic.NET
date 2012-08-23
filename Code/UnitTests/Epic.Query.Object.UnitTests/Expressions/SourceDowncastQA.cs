//  
//  SourceDowncastQA.cs
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
    public interface ISpecializedCargo : ICargo {}

    [TestFixture]
    public class SourceDowncastQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutASource_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new SourceDowncast<ICargo, ISpecializedCargo>(null);
            });
        }

        [Test]
        public void Initialize_withASource_works()
        {
            // arrange:
            Expression<IEnumerable<ICargo>> source = GeneratePartialMock<Expression<IEnumerable<ICargo>>>();

            // act:
            SourceDowncast<ICargo, ISpecializedCargo> toTest = new SourceDowncast<ICargo, ISpecializedCargo>(source);

            // assert:
            Assert.AreSame(source, toTest.Source);
        }

        [Test]
        public void Serialization_works()
        {
            // arrange:
            IRepository<ICargo, TrackingId> repository = new Fakes.FakeRepository<ICargo, TrackingId>();
            Source<ICargo, TrackingId> source = new Source<ICargo, TrackingId>(repository);
            SourceDowncast<ICargo, ISpecializedCargo> toSerialize = new SourceDowncast<ICargo, ISpecializedCargo>(source);

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
            SourceDowncast<ICargo, ISpecializedCargo> toSerialize = new SourceDowncast<ICargo, ISpecializedCargo>(source);
            Stream stream = SerializationUtilities.Serialize(toSerialize);

            // act:
            SourceDowncast<ICargo, ISpecializedCargo> deserialized = SerializationUtilities.Deserialize<SourceDowncast<ICargo, ISpecializedCargo>>(stream);

            // assert:
            Assert.IsNotNull (deserialized);
            Assert.IsNotNull (deserialized.Source);
            Assert.IsInstanceOf<Source<ICargo, TrackingId>>(deserialized.Source);

        }

        [Test]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            Expression<IEnumerable<ICargo>> source = GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            SourceDowncast<ICargo, ISpecializedCargo> toTest = new SourceDowncast<ICargo, ISpecializedCargo>(source);
            object expectedResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            IVisitor<object, SourceDowncast<ICargo, ISpecializedCargo>> expressionVisitor = GenerateStrictMock<IVisitor<object, SourceDowncast<ICargo, ISpecializedCargo>>>();
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

