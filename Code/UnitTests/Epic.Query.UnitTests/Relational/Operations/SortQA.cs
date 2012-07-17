//  
//  SortQA.cs
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
    public class SortQA:RhinoMocksFixtureBase
    {
        string tableName = "testTable";
        private string attributeName = "attribute";
        Relation table;
 
        [SetUp]
        public void SetUp ()
        {
            table = new Fakes.FakeRelation(RelationType.BaseRelation, tableName);
        }

        [Test]
        public void Initialize_WithEitherArgumentNull_Fails()
        {
            // arrange:
            RelationAttribute attribute = new RelationAttribute(attributeName, table);
            string operationName = "testName";

            // assert:
            Assert.Throws<ArgumentNullException>(() => new Sort(null, attribute, true, operationName));
            Assert.Throws<ArgumentNullException>(() => new Sort(table, null, true, operationName));
            Assert.Throws<ArgumentNullException>(() => new Sort(table, attribute, true, null));
            Assert.Throws<ArgumentNullException>(() => new Sort(table, attribute, true, string.Empty));
        }

        [Test]
        public void Initialize_WithFakeArguments_Works()
        {
            // arrange:
            RelationAttribute attribute = new RelationAttribute(attributeName, table);
            string operationName = "testName";
            string otherOperationName = "otherName";

            // act:
            Sort firstSort = new Sort(table, attribute, true, operationName);
            Sort secondSort = new Sort(table, attribute, true, otherOperationName);

            // assert:
            Assert.IsTrue (firstSort.Relation.Equals (table));
            Assert.IsTrue (firstSort.Relation.Equals (secondSort.Relation));

            Assert.IsTrue (firstSort.Attribute.Equals (attribute));
            Assert.IsTrue (firstSort.Attribute.Equals (secondSort.Attribute));

            Assert.IsTrue (firstSort.Descending);
            Assert.IsTrue (secondSort.Descending);

            Assert.IsTrue (firstSort.Name.Equals (operationName));
            Assert.IsTrue (secondSort.Name.Equals (otherOperationName));
            Assert.IsFalse (firstSort.Equals (secondSort));
        }

        [Test]
        public void Equals_AgainstSameObject_works()
        {
            // arrange:
            RelationAttribute attribute = new RelationAttribute(attributeName, table);
            string operationName = "testName";

            // act:
            Sort firstSort = new Sort(table, attribute, true, operationName);
            Sort secondSort = firstSort;

            // assert:
            Assert.IsTrue (firstSort.Equals (secondSort));
        }

        [Test]
        public void Equals_ToNull_works()
        {
            // arrange:
            RelationAttribute attribute = new RelationAttribute(attributeName, table);
            string operationName = "testName";

            // act:
            Sort firstSort = new Sort(table, attribute, false, operationName);

            // assert:
            Assert.IsFalse (firstSort.Equals (null));
        }

        [Test]
        public void Equals_AgainstEquivalentObject_works()
        {
             // arrange:
            RelationAttribute attribute = new RelationAttribute(attributeName, table);
            string operationName = "testName";
            string otherOperationName = "otherTestName";

            // act:
            Sort firstSort = new Sort(table, attribute, true, operationName);
            Sort secondSort = new Sort(table, attribute, true,  operationName);
            Sort thirdSort = new Sort(table, attribute, true, otherOperationName);
            Relation relation = secondSort;

            // assert:
            Assert.IsTrue (firstSort.Equals (secondSort));
            Assert.AreEqual (firstSort.GetHashCode (), secondSort.GetHashCode ());
            Assert.IsTrue (firstSort.Equals (relation));
            Assert.IsFalse (firstSort.Equals (thirdSort));
        }

        [Test]
        public void Test_Serialization_Works()
        {
            // arrange:
            RelationAttribute attribute = new RelationAttribute(attributeName, table);
            string operationName = "testName";
            Sort selection = new Sort(table, attribute, true, operationName);

            // act:
            Stream stream = SerializationUtilities.Serialize<Sort>(selection);

            // assert:
            Assert.IsNotNull (stream);
        }

        [Test]
        public void Test_deserialization_Works()
        {
            // arrange:
            RelationAttribute attribute = new RelationAttribute(attributeName, table);
            string operationName = "testName";
            Sort selection = new Sort(table, attribute, true, operationName);

            // act:
            Stream stream = SerializationUtilities.Serialize<Sort>(selection);
            Sort deserialized = SerializationUtilities.Deserialize<Sort>(stream);

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
            RelationAttribute attribute = new RelationAttribute(attributeName, table);
            string operationName = "testName";
            Sort selection = new Sort(table, attribute, true, operationName);

            IVisitor<object, Sort> selectionVisitor =
            GenerateStrictMock<IVisitor<object, Sort>>();
            selectionVisitor.Expect(v => v.Visit(selection, context)).Return(expectedResult).Repeat.Once();
            IVisitor<object> visitor = GenerateStrictMock<IVisitor<object>>();
            visitor.Expect(v => v.GetVisitor(selection)).Return(selectionVisitor).Repeat.Once ();

            // act:
            object result = selection.Accept(visitor, context);

            // assert:
            Assert.AreSame(expectedResult, result);
        }

    }
}

