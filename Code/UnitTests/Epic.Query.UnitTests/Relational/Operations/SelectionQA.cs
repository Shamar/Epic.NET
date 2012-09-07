//  
//  SelectionQA.cs
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
using Epic.Query.Relational.Predicates;
using System.IO;
using Rhino.Mocks;


namespace Epic.Query.Relational.Operations
{
    [TestFixture]
    public class SelectionQA:RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_WithEitherArgumentNull_Fails()
        {
            // arrange:
            string tableName = "testTable";
            RelationalExpression table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
            Predicate predicate = new Fakes.FakePredicate();

            // assert:
            Assert.Throws<ArgumentNullException>(() => new Selection(table, null));
            Assert.Throws<ArgumentNullException>(() => new Selection(null, predicate));
            Assert.Throws<ArgumentNullException>(() => new Selection(null, null));
        }

        [Test]
        public void Initialize_WithFakeArguments_Works()
        {
            // arrange:
            string tableName = "testTable";
            RelationalExpression table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
            Predicate predicate = new Fakes.FakePredicate();

            // act:
            Selection firstSelection = new Selection(table, predicate);

            // assert:
            Assert.IsTrue (firstSelection.Relation.Equals (table));
            Assert.IsTrue (firstSelection.Condition.Equals (predicate));
        }

        [Test]
        public void Equals_AgainstSameObject_works()
        {
            // arrange:
            string tableName = "testTable";
            RelationalExpression table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
            Predicate predicate = new Fakes.FakePredicate();

            // act:
            Selection firstSelection = new Selection(table, predicate);
            Selection secondSelection = firstSelection;

            // assert:
            Assert.IsTrue (firstSelection.Equals (secondSelection));
        }

        [Test]
        public void Equals_ToNull_works()
        {
            // arrange:
            string tableName = "testTable";
            RelationalExpression table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
            Predicate predicate = new Fakes.FakePredicate();

            // act:
            Selection firstSelection = new Selection(table, predicate);

            // assert:
            Assert.IsFalse (firstSelection.Equals (null));
        }

        [Test]
        public void Equals_AgainstEquivalentObject_works()
        {
            // arrange:
            string tableName = "testTable";
            RelationalExpression table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
            Predicate predicate = new Fakes.FakePredicate();

            // act:
            Selection firstSelection = new Selection(table, predicate);
            Selection secondSelection = new Selection(table, predicate);
            RelationalExpression relation = secondSelection;

            // assert:
            Assert.IsTrue (firstSelection.Relation.Equals (secondSelection.Relation));
            Assert.IsTrue (firstSelection.Condition.Equals (secondSelection.Condition));
            Assert.IsTrue (firstSelection.Equals (secondSelection));
            Assert.IsTrue (firstSelection.Equals (relation));
            Assert.AreEqual (firstSelection.GetHashCode (), secondSelection.GetHashCode ());

        }

        [Test]
        public void Test_Serialization_Works()
        {
            // arrange:
            string tableName = "testTable";
            RelationalExpression table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
            Predicate predicate = new Fakes.FakePredicate();
            Selection selection = new Selection(table, predicate);

            // act:
            Stream stream = SerializationUtilities.Serialize<Selection>(selection);

            // assert:
            Assert.IsNotNull (stream);
        }

        [Test]
        public void Test_deserialization_Works()
        {
            // arrange:
            string tableName = "testTable";
            RelationalExpression table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
            Predicate predicate = new Fakes.FakePredicate();
            Selection selection = new Selection(table, predicate);

            // act:
            Stream stream = SerializationUtilities.Serialize<Selection>(selection);
            Selection deserialized = SerializationUtilities.Deserialize<Selection>(stream);

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
            Predicate predicate = new Fakes.FakePredicate();
            Selection selection = new Selection(table, predicate);

            IVisitor<object, Selection> selectionVisitor =
            GenerateStrictMock<IVisitor<object, Selection>>();
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

