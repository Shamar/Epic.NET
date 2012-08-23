//  
//  CrossProductQA.cs
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
    public class CrossProductQA: RhinoMocksFixtureBase
    {
        private readonly Relation fakeRelation1 = new Fakes.FakeRelation(RelationType.BaseRelation, "firstRelation");
        private readonly Relation fakeRelation2 = new Fakes.FakeRelation(RelationType.BaseRelation, "secondRelation");

        [Test]
        public void Initialize_WithEitherArgumentNull_Fails()
        {
            // arrange:
            string name = "testName";
 
            // assert:
            Assert.Throws<ArgumentNullException>(() => new CrossProduct(null, fakeRelation2, name));
            Assert.Throws<ArgumentNullException>(() => new CrossProduct(fakeRelation1, null, name));
            Assert.Throws<ArgumentNullException>(() => new CrossProduct(fakeRelation1, fakeRelation2, null));
        }

        [Test]
        public void Initialize_WithFakeArguments_Works()
        {
            // arrange:
            string firstName = "firstName";
            string secondName = "secondName";
 
            // act:
            CrossProduct firstJoin = new CrossProduct(fakeRelation1, fakeRelation2, firstName);
            CrossProduct secondJoin = new CrossProduct(fakeRelation1, fakeRelation2, secondName);

            // assert:
            Assert.IsTrue (firstJoin.LeftRelation.Equals (fakeRelation1));
            Assert.IsTrue (firstJoin.LeftRelation.Equals (secondJoin.LeftRelation));

            Assert.IsTrue (firstJoin.RightRelation.Equals (fakeRelation2));
            Assert.IsTrue (firstJoin.RightRelation.Equals (secondJoin.RightRelation));

            Assert.IsTrue (firstJoin.Name.Equals (firstName));
            Assert.IsTrue (secondJoin.Name.Equals (secondName));
            Assert.IsFalse (firstJoin.Equals (secondJoin));
        }

        [Test]
        public void Equals_ToNull_works()
        {
            // arrange:
            string name = "testName";

            // act:
            CrossProduct innerJoin = new CrossProduct(fakeRelation1, fakeRelation2, name);

            // assert:
            Assert.IsFalse(innerJoin.Equals (null));
        }

        [Test]
        public void Equals_AgainstSameObject_works()
        {
             // arrange:
            string name = "testName";

            // act:
            CrossProduct firstJoin = new CrossProduct(fakeRelation1, fakeRelation2, name);
            CrossProduct secondJoin = firstJoin;

            // assert:
            Assert.IsTrue (firstJoin.Equals(secondJoin));
        }

        [Test]
        public void Equals_AgainstEquivalentObject_works()
        {
            // arrange:
            string name = "testName";
            string otherName = "otherTestName";

            // act:
            CrossProduct firstJoin = new CrossProduct(fakeRelation1, fakeRelation2, name);
            CrossProduct secondJoin = new CrossProduct(fakeRelation1, fakeRelation2, name);
            CrossProduct thirdJoin = new CrossProduct(fakeRelation1, fakeRelation2, otherName);
            Relation relation = secondJoin;

            // assert:
            Assert.IsTrue (firstJoin.Equals (secondJoin));
            Assert.IsTrue (firstJoin.Equals (relation));
            Assert.IsFalse (firstJoin.Equals (thirdJoin));
            Assert.AreEqual (firstJoin.GetHashCode (), secondJoin.GetHashCode ());
        }

        [Test]
        public void Test_Serialization_Works()
        {
            // arrange:
            string name = "testName";
            CrossProduct innerJoin = new CrossProduct(fakeRelation1, fakeRelation2, name);

            // act:
            Stream stream = SerializationUtilities.Serialize<CrossProduct>(innerJoin);

            // assert:
            Assert.IsNotNull (stream);
        }

        [Test]
        public void Test_deserialization_Works()
        {
            // arrange:
            string name = "testName";
            CrossProduct innerJoin = new CrossProduct(fakeRelation1, fakeRelation2, name);

            // act:
            Stream stream = SerializationUtilities.Serialize<CrossProduct>(innerJoin);
            CrossProduct deserialized = SerializationUtilities.Deserialize<CrossProduct>(stream);

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
            CrossProduct innerJoin = new CrossProduct(fakeRelation1, fakeRelation2, name);

            IVisitor<object, CrossProduct> selectionVisitor =
            GenerateStrictMock<IVisitor<object, CrossProduct>>();
            selectionVisitor.Expect(v => v.Visit(innerJoin, context)).Return(expectedResult).Repeat.Once();
            IVisitor<object> visitor = GenerateStrictMock<IVisitor<object>>();
            visitor.Expect(v => v.AsVisitor(innerJoin)).Return(selectionVisitor).Repeat.Once ();

            // act:
            object result = innerJoin.Accept(visitor, context);

            // assert:
            Assert.AreSame(expectedResult, result);
        }

    }
}

