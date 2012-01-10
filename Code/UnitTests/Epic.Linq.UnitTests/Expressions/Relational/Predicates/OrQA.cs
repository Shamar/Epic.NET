//  
//  OrQA.cs
//  
//  Author:
//       Marco Veglio <m.veglio@gmail.com>
// 
//  Copyright (c) 2010-2012 Giacomo Tesio
// 
//  This file is part of Epic.NET.
// 
//  Epic.NET is free software: you can redistribute it or/or modify
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
using Epic.Linq.Fakes;
using Epic.Linq.Expressions.Relational;
using Epic.Linq.Expressions.Relational.Predicates;
using NUnit.Framework;
using System.IO;
using Rhino.Mocks;

namespace Epic.Linq.Expressions.Relational.Predicates
{
    [TestFixture]
    public class OrQA: RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withEitherArgumentNull_fails()
        {
            // arrange:
            FakePredicate pred1 = new FakePredicate();
            FakePredicate pred2 = null;

            // assert:
            Assert.Throws<ArgumentNullException>(() => new Or<FakePredicate, FakePredicate>(pred1, pred2));
            Assert.Throws<ArgumentNullException>(() => new Or<FakePredicate, FakePredicate>(pred2, pred1));
        }

        [Test]
        public void Initialize_withFakePredicates_works()
        {
            // arrange:
            FakePredicate pred1 = new FakePredicate();
            FakePredicate pred2 = new FakePredicate();

            // act:
            Or<FakePredicate, FakePredicate> or = new Or<FakePredicate, FakePredicate>(pred1, pred2);

            // assert:
            Assert.IsTrue(or.Left.Equals (pred1));
            Assert.IsTrue(or.Right.Equals (pred2));
        }

        [Test]
        public void TwoPredicates_withDifferentGenericTypes_areNotEqual()
        {
            // arrange:
            FakePredicate pred1 = new FakePredicate();
            FakePredicate pred2 = new FakePredicate();

            // act:
            Or<FakePredicate, FakePredicate> or = new Or<FakePredicate, FakePredicate>(pred1, pred2);
            Or<Predicate, Predicate> or2 = new Or<Predicate, Predicate>(pred1, pred2);

            // assert:
            Assert.IsFalse(or.Equals (or2));
            Assert.AreNotEqual (or.GetHashCode (), or2.GetHashCode ());

        }

        [Test]
        public void Equals_AgainstSameObject_works()
        {
            // arrange:
            FakePredicate predicate1 = new FakePredicate();
            FakePredicate predicate2 = new FakePredicate();

            // act:
            Or<FakePredicate, FakePredicate> or = new Or<FakePredicate, FakePredicate>(predicate1, predicate2);
            Or<FakePredicate, FakePredicate> or2 = or;

            // assert:
            Assert.IsTrue (or.Equals (or2));
            Assert.AreEqual (or.GetHashCode (), or2.GetHashCode ());
        }

        [Test]
        public void Equals_AgainstEquivalentObject_works()
        {
            // arrange:
            FakePredicate predicate1 = new FakePredicate();
            FakePredicate predicate2 = new FakePredicate();

            // act:
            Or<FakePredicate, FakePredicate> or = new Or<FakePredicate, FakePredicate>(predicate1, predicate2);
            Or<FakePredicate, FakePredicate> or2 = new Or<FakePredicate, FakePredicate>(predicate1, predicate2);

            // assert:
            Assert.IsTrue (or.Equals (or2));
            Assert.AreEqual (or.GetHashCode (), or2.GetHashCode ());
        }

        [Test]
        public void Equals_WithDifferentOperors_fails()
        {
            // arrange:
            FakePredicate predicate1 = new FakePredicate();
            FakePredicate predicate2 = new FakePredicate();
            FakePredicate predicate3 = new FakePredicate();

            // act:
            Or<FakePredicate, FakePredicate> or = new Or<FakePredicate, FakePredicate>(predicate1, predicate2);
            Or<FakePredicate, FakePredicate> or2 = new Or<FakePredicate, FakePredicate>(predicate1, predicate3);

            // assert:
            Assert.IsFalse (or.Equals (or2));
            Assert.AreNotEqual (or.GetHashCode (), or2.GetHashCode ());
        }

        [Test]
        public void Test_Serialization_Works()
        {
            // arrange:
            FakePredicate predicate1 = new FakePredicate();
            FakePredicate predicate2 = new FakePredicate();
            Or<FakePredicate, FakePredicate> or = new Or<FakePredicate, FakePredicate>(predicate1, predicate2);

            // act:
            Stream stream = TestUtilities.Serialize<Or<FakePredicate,FakePredicate>>(or);

            // assert:
            Assert.IsNotNull (stream);
        }

        [Test]
        public void Test_deserialization_Works()
        {
            // arrange:
            FakePredicate predicate1 = new FakePredicate();
            FakePredicate predicate2 = new FakePredicate();
            Or<FakePredicate, FakePredicate> or = new Or<FakePredicate, FakePredicate>(predicate1, predicate2);

            // act:
            Stream stream = TestUtilities.Serialize<Or<FakePredicate,FakePredicate>>(or);
            Or<FakePredicate, FakePredicate> deserialized = TestUtilities.Deserialize<Or<FakePredicate, FakePredicate>>(stream);

            // assert:
            // Assert.AreSame(or, deserialized);
            Assert.IsTrue (or.Equals (deserialized));
        }

        [Test]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            object expectedResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            FakePredicate predicate1 = new FakePredicate();
            FakePredicate predicate2 = new FakePredicate();
            Or<FakePredicate, FakePredicate> or = new Or<FakePredicate, FakePredicate>(predicate1, predicate2);

            IVisitor<object, Or<FakePredicate, FakePredicate>> orPredicateVisitor =
                GenerateStrictMock<IVisitor<object, Or<FakePredicate, FakePredicate>>>();
            orPredicateVisitor.Expect(v => v.Visit(or, context)).Return(expectedResult).Repeat.Once();
            IVisitor<object> visitor = GenerateStrictMock<IVisitor<object>>();
            visitor.Expect(v => v.GetVisitor(or)).Return(orPredicateVisitor).Repeat.Once ();

            // act:
            object result = or.Accept(visitor, context);

            // assert:
            Assert.AreSame(expectedResult, result);
        }
    }
}

