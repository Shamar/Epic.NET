//  
//  AndQA.cs
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
using Epic.Query.Fakes;
using Epic.Query.Relational;
using Epic.Query.Relational.Predicates;
using NUnit.Framework;
using System.IO;
using Rhino.Mocks;

namespace Epic.Query.Relational.Predicates
{
    [TestFixture]
    public class AndQA: RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withEitherArgumentNull_fails()
        {
            // arrange:
            FakePredicate pred1 = new FakePredicate();
            FakePredicate pred2 = null;

            // assert:
            Assert.Throws<ArgumentNullException>(() => new And(pred1, pred2));
            Assert.Throws<ArgumentNullException>(() => new And(pred2, pred1));
        }

        [Test]
        public void Initialize_withFakePredicates_works()
        {
            // arrange:
            FakePredicate pred1 = new FakePredicate();
            FakePredicate pred2 = new FakePredicate();

            // act:
            And and = new And(pred1, pred2);

            // assert:
            Assert.IsTrue(and.Left.Equals (pred1));
            Assert.IsTrue(and.Right.Equals (pred2));
        }

        [Test]
        public void Equals_AgainstNull_isFalse()
        {
            // arrange:
            FakePredicate predicate1 = new FakePredicate();
            FakePredicate predicate2 = new FakePredicate();
            And and = new And(predicate1, predicate2);

            // assert:
            Assert.IsFalse (and.Equals (null as Object));
            Assert.IsFalse (and.Equals (null as And));
        }

        [Test]
        public void Equals_AgainstSameObject_works()
        {
            // arrange:
            FakePredicate predicate1 = new FakePredicate();
            FakePredicate predicate2 = new FakePredicate();

            // act:
            And and = new And(predicate1, predicate2);
            And and2 = and;

            // assert:
            Assert.IsTrue (and.Equals (and2));
            Assert.AreEqual (and.GetHashCode (), and2.GetHashCode ());
        }

        [Test]
        public void Equals_AgainstEquivalentObject_works()
        {
            // arrange:
            FakePredicate predicate1 = new FakePredicate();
            FakePredicate predicate2 = new FakePredicate();

            // act:
            And and = new And(predicate1, predicate2);
            And and2 = new And(predicate1, predicate2);

            // assert:
            Assert.IsTrue (and.Equals (and2));
            Assert.AreEqual (and.GetHashCode (), and2.GetHashCode ());
        }

        [Test]
        public void Equals_WithDifferentOperands_fails()
        {
            // arrange:
            FakePredicate predicate1 = new FakePredicate();
            FakePredicate predicate2 = new FakePredicate();
            FakePredicate predicate3 = new FakePredicate();

            // act:
            And and = new And(predicate1, predicate2);
            And and2 = new And(predicate1, predicate3);

            // assert:
            Assert.IsFalse (and.Equals (and2));
            Assert.AreNotEqual (and.GetHashCode (), and2.GetHashCode ());
        }

        [Test]
        public void Test_Serialization_Works()
        {
            // arrange:
            FakePredicate predicate1 = new FakePredicate();
            FakePredicate predicate2 = new FakePredicate();
            And and = new And(predicate1, predicate2);

            // act:
            Stream stream = SerializationUtilities.Serialize<And>(and);

            // assert:
            Assert.IsNotNull (stream);
        }

        [Test]
        public void Test_deserialization_Works()
        {
            // arrange:
            FakePredicate predicate1 = new FakePredicate();
            FakePredicate predicate2 = new FakePredicate();
            And and = new And(predicate1, predicate2);

            // act:
            Stream stream = SerializationUtilities.Serialize<And>(and);
            And deserialized = SerializationUtilities.Deserialize<And>(stream);

            // assert:
            // Assert.AreSame(and, deserialized);
            Assert.IsTrue (and.Equals (deserialized));
        }

        [Test]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            object expectedResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            FakePredicate predicate1 = new FakePredicate();
            FakePredicate predicate2 = new FakePredicate();
            And and = new And(predicate1, predicate2);

            IVisitor<object, And> andPredicateVisitor =
                GenerateStrictMock<IVisitor<object, And>>();
            andPredicateVisitor.Expect(v => v.Visit(and, context)).Return(expectedResult).Repeat.Once();
            IVisitor<object> visitor = GenerateStrictMock<IVisitor<object>>();
            visitor.Expect(v => v.AsVisitor(and)).Return(andPredicateVisitor).Repeat.Once ();

            // act:
            object result = and.Accept(visitor, context);

            // assert:
            Assert.AreSame(expectedResult, result);
        }

    }
}

