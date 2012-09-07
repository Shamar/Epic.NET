//  
//  ProjectionQA.cs
//  
//  Author:
//       Marco Veglio <m.veglio@gmail.com>
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
using System.Collections.Generic;
using NUnit.Framework;
using Epic.Query.Relational.Predicates;
using System.IO;
using Rhino.Mocks;
using System.Linq;


namespace Epic.Query.Relational.Operations
{
    [TestFixture]
    public class ProjectionQA:RhinoMocksFixtureBase
    {
        private readonly RelationalExpression fakeRelation = new Fakes.FakeRelation(RelationType.BaseRelation, "test");
        private string attributeName = "attribute";

        [Test]
        public void Initialize_WithEitherArgumentNull_Fails()
        {
            // arrange:
            string tableName = "testTable";
            RelationalExpression table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
            IEnumerable<RelationAttribute> attributes = new [] { new RelationAttribute(attributeName, fakeRelation) };

            // assert:
            Assert.Throws<ArgumentNullException>(() => new Projection(table, null));
            Assert.Throws<ArgumentNullException>(() => new Projection(null, attributes));
            Assert.Throws<ArgumentNullException>(() => new Projection(null, null));
        }

        [Test]
        public void Initialize_WithFakeArguments_Works()
        {
            // arrange:
            string tableName = "testTable";
            RelationalExpression table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
            IEnumerable<RelationAttribute> attributes = new [] { new RelationAttribute(attributeName, fakeRelation) };

            // act:
            Projection firstProjection = new Projection(table, attributes);

            // assert:
            Assert.IsTrue (firstProjection.Relation.Equals (table));
            Assert.IsTrue (firstProjection.Attributes.SequenceEqual (attributes));
        }

        [Test]
        public void Equals_ToNull_works()
        {
            // arrange:
            string tableName = "testTable";
            RelationalExpression table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
            IEnumerable<RelationAttribute> attributes = new [] { new RelationAttribute(attributeName, fakeRelation) };

            // act:
            Projection firstProjection = new Projection(table, attributes);

            // assert:
            Assert.IsFalse(firstProjection.Equals (null));
        }

        [Test]
        public void Equals_AgainstSameObject_works()
        {
            // arrange:
            string tableName = "testTable";
            RelationalExpression table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
            IEnumerable<RelationAttribute> attributes = new [] { new RelationAttribute(attributeName, fakeRelation) };

            // act:
            Projection firstProjection = new Projection(table, attributes);
            Projection secondProjection = firstProjection;

            // assert:
            Assert.IsTrue (firstProjection.Equals (secondProjection));
        }

        [Test]
        public void Equals_AgainstEquivalentObject_works()
        {
            // arrange:
            string tableName = "testTable";
            RelationalExpression table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
            IEnumerable<RelationAttribute> attributes = new [] { new RelationAttribute(attributeName, fakeRelation) };

            // act:
            Projection firstProjection = new Projection(table, attributes);
            Projection secondProjection = new Projection(table, attributes);
            RelationalExpression relation = secondProjection;

            // assert:
            Assert.IsTrue (firstProjection.Equals (secondProjection));
            Assert.IsTrue (firstProjection.Equals (relation));
            Assert.AreEqual (firstProjection.GetHashCode (), secondProjection.GetHashCode ());
        }

        [Test]
        public void Test_Serialization_Works()
        {
            // arrange:
            string tableName = "testTable";
            RelationalExpression table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
            IEnumerable<RelationAttribute> attributes = new [] { new RelationAttribute(attributeName, fakeRelation) };
            Projection selection = new Projection(table, attributes);

            // act:
            Stream stream = SerializationUtilities.Serialize<Projection>(selection);

            // assert:
            Assert.IsNotNull (stream);
        }

        [Test]
        public void Test_deserialization_Works()
        {
            // arrange:
            string tableName = "testTable";
            RelationalExpression table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
            IEnumerable<RelationAttribute> attributes = new [] { new RelationAttribute(attributeName, fakeRelation) };
            Projection selection = new Projection(table, attributes);

            // act:
            Stream stream = SerializationUtilities.Serialize<Projection>(selection);
            Projection deserialized = SerializationUtilities.Deserialize<Projection>(stream);

            // assert:
            // Assert.AreSame(and, deserialized);
            Assert.IsTrue (selection.Equals (deserialized));
        }

        [Test]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            object expectedResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            string tableName = "testTable";
            RelationalExpression table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
            IEnumerable<RelationAttribute> attributes = new [] { new RelationAttribute(attributeName, fakeRelation) };
            Projection projection = new Projection(table, attributes);

            IVisitor<object, Projection> selectionVisitor =
            GenerateStrictMock<IVisitor<object, Projection>>();
            selectionVisitor.Expect(v => v.Visit(projection, context)).Return(expectedResult).Repeat.Once();
            IVisitor<object> visitor = GenerateStrictMock<IVisitor<object>>();
            visitor.Expect(v => v.AsVisitor(projection)).Return(selectionVisitor).Repeat.Once ();

            // act:
            object result = projection.Accept(visitor, context);

            // assert:
            Assert.AreSame(expectedResult, result);
        }

    }
}

