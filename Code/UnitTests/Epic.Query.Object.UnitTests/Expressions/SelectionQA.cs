//  
//  SelectionQA.cs
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
using Epic.Specifications;

namespace Epic.Query.Object.UnitTests.Expressions
{
    [TestFixture]
    public class SelectionQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutAnyArgument_throwsArgumentNullException()
        {
            // arrange:
            ISpecification<ICargo> specification = GenerateStrictMock<ISpecification<ICargo>>();
            Expression<IEnumerable<ICargo>> source = GeneratePartialMock<Expression<IEnumerable<ICargo>>>();

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new Selection<ICargo>(null, specification);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                new Selection<ICargo>(source, null);
            });
        }

        [Test]
        public void Initialize_withValidArguments_works()
        {
            // arrange:
            ISpecification<ICargo> specification = GenerateStrictMock<ISpecification<ICargo>>();
            Expression<IEnumerable<ICargo>> source = GeneratePartialMock<Expression<IEnumerable<ICargo>>>();

            // act:
            Selection<ICargo> toTest = new Selection<ICargo>(source, specification);

            // assert:
            Assert.AreSame(source, toTest.Source);
            Assert.AreSame(specification, toTest.Specification);
        }

        [Test]
        public void Serialization_works()
        {
            // arrange:
            IRepository<ICargo, TrackingId> repository = new Fakes.FakeRepository<ICargo, TrackingId>();
            Source<ICargo, TrackingId> source = new Source<ICargo, TrackingId>(repository);
            ISpecification<ICargo> specification = new Fakes.FakeSpecification<ICargo>();
            Selection<ICargo> toSerialize = new Selection<ICargo>(source, specification);

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
            ISpecification<ICargo> specification = new Fakes.FakeSpecification<ICargo>();
            Selection<ICargo> toSerialize = new Selection<ICargo>(source, specification);
            Stream stream = SerializationUtilities.Serialize(toSerialize);

            // act:
            Selection<ICargo> deserialized = SerializationUtilities.Deserialize<Selection<ICargo>>(stream);

            // assert:
            Assert.IsNotNull (deserialized);
            Assert.IsNotNull (deserialized.Source);
            Assert.IsInstanceOf<Source<ICargo, TrackingId>>(deserialized.Source);
            Assert.IsNotNull (deserialized.Specification);
            Assert.IsInstanceOf<Fakes.FakeSpecification<ICargo>>(deserialized.Specification);

        }

        [Test]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            Expression<IEnumerable<ICargo>> source = GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            ISpecification<ICargo> specification = new Fakes.FakeSpecification<ICargo>();
            Selection<ICargo> toTest = new Selection<ICargo>(source, specification);
            object expectedResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            IVisitor<object, Selection<ICargo>> expressionVisitor = GenerateStrictMock<IVisitor<object, Selection<ICargo>>>();
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

