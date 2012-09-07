//  
//  Less.cs
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
    public class LessQA: RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withEitherArgumentNull_fails()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = null;

            // assert:
            Assert.Throws<ArgumentNullException>(() => new Less(scalar1, scalar2));
            Assert.Throws<ArgumentNullException>(() => new Less(scalar2, scalar1));
        }

        [Test]
        public void Initialize_withFakeScalars_operandsOK()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);

            // act:
            Less less = new Less(scalar1, scalar2);

            // assert:
            Assert.IsNotNull (less);
            Assert.IsTrue (less.Left.Equals (scalar1));
            Assert.IsTrue (less.Right.Equals (scalar2));
        }

        [Test]
        public void Equals_AgainstSameObject_works()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);

            // act:
            Less less = new Less(scalar1, scalar2);
            Less less2 = less;

            // assert:
            Assert.IsTrue (less.Equals (less2));
            Assert.AreEqual (less.GetHashCode (), less2.GetHashCode ());
        }

        [Test]
        public void Equals_AgainstNull_isFalse()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);
            var toTest = new Less(scalar1, scalar2);

            // assert:
            Assert.IsFalse (toTest.Equals (null as Object));
            Assert.IsFalse (toTest.Equals (null as Less));
        }
        
        [Test]
        public void Equals_AgainstEquivalentObject_works()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);

            // act:
            Less less = new Less(scalar1, scalar2);
            Less less2 = new Less(scalar1, scalar2);

            // assert:
            Assert.IsTrue (less.Equals (less2));
            Assert.AreEqual (less.GetHashCode (), less2.GetHashCode ());
        }

        [Test]
        public void Equals_WithDifferentOperands_fails()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar3 = new FakeScalar(ScalarType.Constant);

            // act:
            Less less = new Less(scalar1, scalar2);
            Less less2 = new Less(scalar1, scalar3);

            // assert:
            Assert.IsFalse (less.Equals (less2));
            Assert.AreNotEqual (less.GetHashCode (), less2.GetHashCode ());
        }

        [Test]
        public void Equals_WithDifferentlyTypedPredicate_fails()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);
            FakeScalarFunction function = new FakeScalarFunction("test");

            // act:
            Less less = new Less(scalar1, function);
            Less less2 = new Less(scalar1, scalar2);

            // assert:
            Assert.IsFalse (less.Equals (less2));
            Assert.AreNotEqual (less.GetHashCode (), less2.GetHashCode ());
        }

        [Test]
        public void Test_Serialization_Works()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);
            Less less = new Less(scalar1, scalar2);

            // act:
            Stream stream = SerializationUtilities.Serialize<Less>(less);

            // assert:
            Assert.IsNotNull (stream);
        }

        [Test]
        public void Test_deserialization_Works()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);
            Less less = new Less(scalar1, scalar2);

            // act:
            Stream stream = SerializationUtilities.Serialize<Less>(less);
            Less deserialized = SerializationUtilities.Deserialize<Less>(stream);

            // assert:
            // Assert.AreSame(less, deserialized);
            Assert.IsTrue (less.Equals (deserialized));
        }

        [Test]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            object expectedResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);
            Less less = new Less(scalar1, scalar2);

            IVisitor<object, Less> lessPredicateVisitor =
                GenerateStrictMock<IVisitor<object, Less>>();
            lessPredicateVisitor.Expect(v => v.Visit(less, context)).Return(expectedResult).Repeat.Once();
            IVisitor<object> visitor = GenerateStrictMock<IVisitor<object>>();
            visitor.Expect(v => v.AsVisitor(less)).Return(lessPredicateVisitor).Repeat.Once ();

            // act:
            object result = less.Accept(visitor, context);

            // assert:
            Assert.AreSame(expectedResult, result);
        }

    }

}

