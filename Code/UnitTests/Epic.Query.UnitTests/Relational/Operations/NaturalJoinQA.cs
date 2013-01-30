//  
//  NaturalJoinQA.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2013 Giacomo Tesio
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
    public class NaturalJoinQA : RhinoMocksFixtureBase
    {
        private readonly RelationalExpression fakeRelation1 = new Fakes.FakeRelation(RelationType.BaseRelation, "firstRelation");
        private readonly RelationalExpression fakeRelation2 = new Fakes.FakeRelation(RelationType.BaseRelation, "secondRelation");
        private readonly Predicate predicate = new Fakes.FakePredicate();

        [Test]
        public void Initialize_WithEitherArgumentNull_Fails()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(() => new NaturalJoin(null, fakeRelation2));
            Assert.Throws<ArgumentNullException>(() => new NaturalJoin(fakeRelation1, null));
        }

        [Test]
        public void Initialize_WithFakeArguments_Works()
        {
            // act:
            NaturalJoin toTest = new NaturalJoin(fakeRelation1, fakeRelation2);

            // assert:
            Assert.IsTrue (toTest.LeftRelation.Equals (fakeRelation1));
            Assert.IsTrue (toTest.RightRelation.Equals (fakeRelation2));
        }

        [Test]
        public void Equals_ToNull_works()
        {
            // act:
            NaturalJoin toTest = new NaturalJoin(fakeRelation1, fakeRelation2);

            // assert:
            Assert.IsFalse(toTest.Equals (null));
        }

        [Test]
        public void Equals_AgainstSameObject_works()
        {
           // act:
            NaturalJoin firstJoin = new NaturalJoin(fakeRelation1, fakeRelation2);
            NaturalJoin secondJoin = firstJoin;

            // assert:
            Assert.IsTrue (firstJoin.Equals(secondJoin));
        }

        [Test]
        public void Equals_AgainstEquivalentObject_works()
        {
            // act:
            NaturalJoin firstJoin = new NaturalJoin(fakeRelation1, fakeRelation2);
            NaturalJoin secondJoin = new NaturalJoin(fakeRelation1, fakeRelation2);
            RelationalExpression relation = secondJoin;

            // assert:
            Assert.IsTrue (firstJoin.LeftRelation.Equals (secondJoin.LeftRelation));
            Assert.IsTrue (firstJoin.RightRelation.Equals (secondJoin.RightRelation));
            Assert.IsTrue (firstJoin.Equals (secondJoin));
            Assert.IsTrue (firstJoin.Equals (relation));
            Assert.AreEqual (firstJoin.GetHashCode (), secondJoin.GetHashCode ());
        }

        [Test]
        public void Test_Serialization_Works()
        {
            // arrange:
            NaturalJoin toTest = new NaturalJoin(fakeRelation1, fakeRelation2);

            // act:
            Stream stream = SerializationUtilities.Serialize<NaturalJoin>(toTest);

            // assert:
            Assert.IsNotNull (stream);
        }

        [Test]
        public void Test_deserialization_Works()
        {
            // arrange:
            NaturalJoin toTest = new NaturalJoin(fakeRelation1, fakeRelation2);

            // act:
            Stream stream = SerializationUtilities.Serialize<NaturalJoin>(toTest);
            NaturalJoin deserialized = SerializationUtilities.Deserialize<NaturalJoin>(stream);

            // assert:
            // Assert.AreSame(and, deserialized);
            Assert.IsTrue (toTest.Equals (deserialized));
        }

        [Test]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            object expectedResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            NaturalJoin toTest = new NaturalJoin(fakeRelation1, fakeRelation2);

            IVisitor<object, NaturalJoin> selectionVisitor = GenerateStrictMock<IVisitor<object, NaturalJoin>>();
            selectionVisitor.Expect(v => v.Visit(toTest, context)).Return(expectedResult).Repeat.Once();
            IVisitor<object> visitor = GenerateStrictMock<IVisitor<object>>();
            visitor.Expect(v => v.AsVisitor(toTest)).Return(selectionVisitor).Repeat.Once ();

            // act:
            object result = toTest.Accept(visitor, context);

            // assert:
            Assert.AreSame(expectedResult, result);
        }

    }
}

