//  
//  InnerJoinQA.cs
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
using System.Collections.Generic;
using Epic.Query.Relational.Predicates;
using System.IO;
using Rhino.Mocks;


namespace Epic.Query.Relational.Operations
{
    [TestFixture]
    public class InnerJoinQA: RhinoMocksFixtureBase
    {
        private readonly Relation fakeRelation1 = new Fakes.FakeRelation(RelationType.BaseRelation, "firstRelation");
        private readonly Relation fakeRelation2 = new Fakes.FakeRelation(RelationType.BaseRelation, "secondRelation");
        private readonly Predicate predicate = new Fakes.FakePredicate();

        [Test]
        public void Initialize_WithEitherArgumentNull_Fails()
        {
            // arrange:
            string name = "testName";
 
            // assert:
            Assert.Throws<ArgumentNullException>(() => new InnerJoin(null, fakeRelation2, predicate));
            Assert.Throws<ArgumentNullException>(() => new InnerJoin(fakeRelation1, null, predicate));
            Assert.Throws<ArgumentNullException>(() => new InnerJoin(fakeRelation1, fakeRelation2, null));
            Assert.Throws<ArgumentNullException>(() => new InnerJoin(null, fakeRelation2, predicate, name));
            Assert.Throws<ArgumentNullException>(() => new InnerJoin(fakeRelation1, null, predicate, name));
            Assert.Throws<ArgumentNullException>(() => new InnerJoin(fakeRelation1, fakeRelation2, null, name));
            Assert.Throws<ArgumentNullException>(() => new InnerJoin(fakeRelation1, fakeRelation2, predicate, null));
        }

        [Test]
        public void Initialize_WithFakeArguments_Works()
        {
            // arrange:
            string name = "testName";
 
            // act:
            InnerJoin firstJoin = new InnerJoin(fakeRelation1, fakeRelation2, predicate);
            InnerJoin secondJoin = new InnerJoin(fakeRelation1, fakeRelation2, predicate, name);

            // assert:
            Assert.IsTrue (firstJoin.LeftRelation.Equals (fakeRelation1));
            Assert.IsTrue (firstJoin.LeftRelation.Equals (secondJoin.LeftRelation));

            Assert.IsTrue (firstJoin.RightRelation.Equals (fakeRelation2));
            Assert.IsTrue (firstJoin.RightRelation.Equals (secondJoin.RightRelation));

            Assert.IsTrue (firstJoin.Predicate.Equals (predicate));
            Assert.IsTrue (firstJoin.Predicate.Equals (secondJoin.Predicate));

            Assert.IsTrue (secondJoin.Name.Equals (name));
            Assert.IsTrue (firstJoin.Equals (secondJoin));
        }

        [Test]
        public void Equals_ToNull_works()
        {
            // arrange:
            string name = "testName";

            // act:
            InnerJoin innerJoin = new InnerJoin(fakeRelation1, fakeRelation2, predicate, name);

            // assert:
            Assert.IsFalse(innerJoin.Equals (null));
        }

        [Test]
        public void Equals_AgainstSameObject_works()
        {
             // arrange:
            string name = "testName";

            // act:
            InnerJoin firstJoin = new InnerJoin(fakeRelation1, fakeRelation2, predicate, name);
            InnerJoin secondJoin = firstJoin;

            // assert:
            Assert.IsTrue (firstJoin.Equals(secondJoin));
        }

        [Test]
        public void Equals_AgainstEquivalentObject_works()
        {
            // arrange:
            string name = "testName";

            // act:
            InnerJoin firstJoin = new InnerJoin(fakeRelation1, fakeRelation2, predicate, name);
            InnerJoin secondJoin = new InnerJoin(fakeRelation1, fakeRelation2, predicate, name);
            Relation relation = secondJoin;

            // assert:
            Assert.IsTrue (firstJoin.Equals (secondJoin));
            Assert.IsTrue (firstJoin.Equals (relation));
            Assert.AreEqual (firstJoin.GetHashCode (), secondJoin.GetHashCode ());
        }

        [Test]
        public void Test_Serialization_Works()
        {
            // arrange:
            string name = "testName";
            InnerJoin innerJoin = new InnerJoin(fakeRelation1, fakeRelation2, predicate, name);

            // act:
            Stream stream = SerializationUtilities.Serialize<InnerJoin>(innerJoin);

            // assert:
            Assert.IsNotNull (stream);
        }

        [Test]
        public void Test_deserialization_Works()
        {
            // arrange:
            string name = "testName";
            InnerJoin innerJoin = new InnerJoin(fakeRelation1, fakeRelation2, predicate, name);

            // act:
            Stream stream = SerializationUtilities.Serialize<InnerJoin>(innerJoin);
            InnerJoin deserialized = SerializationUtilities.Deserialize<InnerJoin>(stream);

            // assert:
            // Assert.AreSame(and, deserialized);
            Assert.IsTrue (innerJoin.Equals (deserialized));
        }

        [Test]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            object expectedResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            string name = "testName";
            InnerJoin innerJoin = new InnerJoin(fakeRelation1, fakeRelation2, predicate, name);

            IVisitor<object, InnerJoin> selectionVisitor =
            GenerateStrictMock<IVisitor<object, InnerJoin>>();
            selectionVisitor.Expect(v => v.Visit(innerJoin, context)).Return(expectedResult).Repeat.Once();
            IVisitor<object> visitor = GenerateStrictMock<IVisitor<object>>();
            visitor.Expect(v => v.GetVisitor(innerJoin)).Return(selectionVisitor).Repeat.Once ();

            // act:
            object result = innerJoin.Accept(visitor, context);

            // assert:
            Assert.AreSame(expectedResult, result);
        }

    }
}

