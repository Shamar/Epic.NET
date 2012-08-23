//  
//  GreaterQA.cs
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
    public class GreaterQA: RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withEitherArgumentNull_fails()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = null;

            // assert:
            Assert.Throws<ArgumentNullException>(() => new Greater(scalar1, scalar2));
            Assert.Throws<ArgumentNullException>(() => new Greater(scalar2, scalar1));
        }

        [Test]
        public void Initialize_withFakeScalars_operandsOK()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);

            // act:
            Greater greater = new Greater(scalar1, scalar2);

            // assert:
            Assert.IsNotNull (greater);
            Assert.IsTrue (greater.Left.Equals (scalar1));
            Assert.IsTrue (greater.Right.Equals (scalar2));
        }

        [Test]
        public void Equals_AgainstNull_isFalse()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);
            var toTest = new Greater(scalar1, scalar2);
            
            // assert:
            Assert.IsFalse (toTest.Equals (null as Object));
            Assert.IsFalse (toTest.Equals (null as Greater));
        }

        [Test]
        public void Equals_AgainstSameObject_works()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);

            // act:
            Greater greater = new Greater(scalar1, scalar2);
            Greater greater2 = greater;

            // assert:
            Assert.IsTrue (greater.Equals (greater2));
            Assert.AreEqual (greater.GetHashCode (), greater2.GetHashCode ());
        }

        [Test]
        public void Equals_AgainstEquivalentObject_works()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);

            // act:
            Greater greater = new Greater(scalar1, scalar2);
            Greater greater2 = new Greater(scalar1, scalar2);

            // assert:
            Assert.IsTrue (greater.Equals (greater2));
            Assert.AreEqual (greater.GetHashCode (), greater2.GetHashCode ());
        }

        [Test]
        public void Equals_WithDifferentOperands_fails()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar3 = new FakeScalar(ScalarType.Constant);

            // act:
            Greater greater = new Greater(scalar1, scalar2);
            Greater greater2 = new Greater(scalar1, scalar3);

            // assert:
            Assert.IsFalse (greater.Equals (greater2));
            Assert.AreNotEqual (greater.GetHashCode (), greater2.GetHashCode ());
        }

        [Test]
        public void Equals_WithDifferentlyTypedPredicate_fails()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);
            FakeScalarFunction function = new FakeScalarFunction("test");

            // act:
            Greater greater = new Greater(scalar1, function);
            Greater greater2 = new Greater(scalar1, scalar2);

            // assert:
            Assert.IsFalse (greater.Equals (greater2));
            Assert.AreNotEqual (greater.GetHashCode (), greater2.GetHashCode ());
        }

        [Test]
        public void Test_Serialization_Works()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);
            Greater greater = new Greater(scalar1, scalar2);

            // act:
            Stream stream = SerializationUtilities.Serialize<Greater>(greater);

            // assert:
            Assert.IsNotNull (stream);
        }

        [Test]
        public void Test_deserialization_Works()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);
            Greater greater = new Greater(scalar1, scalar2);

            // act:
            Stream stream = SerializationUtilities.Serialize<Greater>(greater);
            Greater deserialized = SerializationUtilities.Deserialize<Greater>(stream);

            // assert:
            // Assert.AreSame(greater, deserialized);
            Assert.IsTrue (greater.Equals (deserialized));
        }

        [Test]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            object expectedResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);
            Greater greater = new Greater(scalar1, scalar2);

            IVisitor<object, Greater> greaterPredicateVisitor =
                GenerateStrictMock<IVisitor<object, Greater>>();
            greaterPredicateVisitor.Expect(v => v.Visit(greater, context)).Return(expectedResult).Repeat.Once();
            IVisitor<object> visitor = GenerateStrictMock<IVisitor<object>>();
            visitor.Expect(v => v.AsVisitor(greater)).Return(greaterPredicateVisitor).Repeat.Once ();

            // act:
            object result = greater.Accept(visitor, context);

            // assert:
            Assert.AreSame(expectedResult, result);
        }

    }
}

