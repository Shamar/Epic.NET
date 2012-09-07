//  
//  RenameQA.cs
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
using NUnit.Framework;
using System.IO;
using Rhino.Mocks;


namespace Epic.Query.Relational.Operations
{
    [TestFixture]
    public class RenameQA: RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_WithEitherArgumentNull_Fails()
        {
            // arrange:
            string tableName = "testTable";
            RelationalExpression table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
            string newRelationName = "testTableRenamed";

            // assert:
            Assert.Throws<ArgumentNullException>(() => new Rename(table, null));
            Assert.Throws<ArgumentNullException>(() => new Rename(null, newRelationName));
            Assert.Throws<ArgumentNullException>(() => new Rename(null, null));
        }

        [Test]
        public void Initialize_WithFakeArguments_Works()
        {
            // arrange:
            string tableName = "testTable";
            RelationalExpression table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
            string newRelationName = "testTableRenamed";

            // act:
            Rename firstRename = new Rename(table, newRelationName);

            // assert:
            Assert.IsTrue (firstRename.Relation.Equals (table));
            Assert.IsTrue (firstRename.Name.Equals (newRelationName));
        }

        [Test]
        public void Equals_AgainstSameObject_works()
        {
            // arrange:
            string tableName = "testTable";
            RelationalExpression table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
            string newRelationName = "testTableRenamed";

            // act:
            Rename firstRename = new Rename(table, newRelationName);
            Rename secondRename = firstRename;

            // assert:
            Assert.IsTrue (firstRename.Equals (secondRename));
        }

        [Test]
        public void Equals_AgainstEquivalentObject_works()
        {
            // arrange:
            string tableName = "testTable";
            RelationalExpression table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
            string newRelationName = "testTableRenamed";

            // act:
            Rename firstRename = new Rename(table, newRelationName);
            Rename secondRename = new Rename(table, newRelationName);
            RelationalExpression relation = secondRename;

            // assert:
            Assert.IsTrue (firstRename.Equals (secondRename));
            Assert.IsTrue (firstRename.Equals (relation));
            Assert.IsTrue (firstRename.Name.Equals (newRelationName));
            Assert.IsTrue (secondRename.Name.Equals (newRelationName));
            Assert.AreEqual (firstRename.GetHashCode (), secondRename.GetHashCode ());
        }

        [Test]
        public void Equals_ToNull_works()
        {
            // arrange:
            string tableName = "testTable";
            RelationalExpression table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
            string newRelationName = "testTableRenamed";

            // act:
            Rename toTest = new Rename(table, newRelationName);

            // assert:
            Assert.IsFalse (toTest.Equals (null));
        }

        [Test]
        public void Test_Serialization_Works()
        {
            // arrange:
            string tableName = "testTable";
            RelationalExpression table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
            string newRelationName = "testTableRenamed";
            Rename selection = new Rename(table, newRelationName);

            // act:
            Stream stream = SerializationUtilities.Serialize<Rename>(selection);

            // assert:
            Assert.IsNotNull (stream);
        }

        [Test]
        public void Test_deserialization_Works()
        {
            // arrange:
            string tableName = "testTable";
            RelationalExpression table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
            string newRelationName = "testTableRenamed";
            Rename selection = new Rename(table, newRelationName);

            // act:
            Stream stream = SerializationUtilities.Serialize<Rename>(selection);
            Rename deserialized = SerializationUtilities.Deserialize<Rename>(stream);

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
            string newRelationName = "testTableRenamed";
            Rename selection = new Rename(table, newRelationName);

            IVisitor<object, Rename> selectionVisitor =
            GenerateStrictMock<IVisitor<object, Rename>>();
            selectionVisitor.Expect(v => v.Visit(selection, context)).Return(expectedResult).Repeat.Once();
            IVisitor<object> visitor = GenerateStrictMock<IVisitor<object>>();
            visitor.Expect(v => v.AsVisitor(selection)).Return(selectionVisitor).Repeat.Once ();

            // act:
            object result = selection.Accept(visitor, context);

            // assert:
            Assert.AreSame(expectedResult, result);
        }
    }
}

