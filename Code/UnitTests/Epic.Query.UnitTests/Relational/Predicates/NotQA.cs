//  
//  NotQA.cs
//  
//  Author:
//       Marco Veglio <m.veglio@gmail.com>
// 
//  Copyright (c) 2010-2012 Giacomo Tesio
// 
//  This file is part of Epic.NET.
// 
//  Epic.NET is free software: you can redistribute it not/or modify
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
using Epic.Query.Fakes;
using Epic.Query.Relational;
using Epic.Query.Relational.Predicates;
using NUnit.Framework;
using System.IO;
using Rhino.Mocks;

namespace Epic.Query.Relational.Predicates
{
    [TestFixture]
    public class NotQA: RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withEitherArgumentNull_fails()
        {
            // arrange:
            FakePredicate predicate = null;

            // assert:
            Assert.Throws<ArgumentNullException>(() => new Not(predicate));
        }

        [Test]
        public void Initialize_withFakePredicates_works()
        {
            // arrange:
            FakePredicate predicate = new FakePredicate();

            // act:
            Not not = new Not(predicate);

            // assert:
            Assert.IsTrue(not.Operand.Equals (predicate));
        }

        [Test]
        public void Equals_AgainstNull_isFalse()
        {
            // arrange:
            FakePredicate predicate = new FakePredicate();
            Not toTest = new Not(predicate);

            // assert:
            Assert.IsFalse (toTest.Equals (null as Object));
            Assert.IsFalse (toTest.Equals (null as Not));
        }
        
        [Test]
        public void Equals_AgainstSameObject_works()
        {
            // arrange:
            FakePredicate predicate = new FakePredicate();

            // act:
            Not not = new Not(predicate);
            Not not2 = not;

            // assert:
            Assert.IsTrue (not.Equals (not2));
            Assert.AreEqual (not.GetHashCode (), not2.GetHashCode ());
        }

        [Test]
        public void Equals_AgainstEquivalentObject_works()
        {
            // arrange:
            FakePredicate predicate = new FakePredicate();

            // act:
            Not not = new Not(predicate);
            Not not2 = new Not(predicate);

            // assert:
            Assert.IsTrue (not.Equals (not2));
            Assert.AreEqual (not.GetHashCode (), not2.GetHashCode ());
        }

        [Test]
        public void Equals_WithDifferentOperands_fails()
        {
            // arrange:
            FakePredicate predicate = new FakePredicate();
            FakePredicate predicate2 = new FakePredicate();

            // act:
            Not not = new Not(predicate);
            Not not2 = new Not(predicate2);

            // assert:
            Assert.IsFalse (not.Equals (not2));
            Assert.AreNotEqual (not.GetHashCode (), not2.GetHashCode ());
        }

        [Test]
        public void Test_Serialization_Works()
        {
            // arrange:
            FakePredicate predicate = new FakePredicate();
            Not not = new Not(predicate);

            // act:
            Stream stream = SerializationUtilities.Serialize<Not>(not);

            // assert:
            Assert.IsNotNull (stream);
        }

        [Test]
        public void Test_deserialization_Works()
        {
            // arrange:
            FakePredicate predicate = new FakePredicate();
            Not not = new Not(predicate);

            // act:
            Stream stream = SerializationUtilities.Serialize<Not>(not);
            Not deserialized = SerializationUtilities.Deserialize<Not>(stream);

            // assert:
            Assert.IsTrue (not.Equals (deserialized));
        }

        [Test]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            object expectedResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            FakePredicate predicate = new FakePredicate();
            Not not = new Not(predicate);

            IVisitor<object, Not> notPredicateVisitor =
                GenerateStrictMock<IVisitor<object, Not>>();
            notPredicateVisitor.Expect(v => v.Visit(not, context)).Return(expectedResult).Repeat.Once();
            IVisitor<object> visitor = GenerateStrictMock<IVisitor<object>>();
            visitor.Expect(v => v.AsVisitor(not)).Return(notPredicateVisitor).Repeat.Once ();

            // act:
            object result = not.Accept(visitor, context);

            // assert:
            Assert.AreSame(expectedResult, result);
        }

    }
}

