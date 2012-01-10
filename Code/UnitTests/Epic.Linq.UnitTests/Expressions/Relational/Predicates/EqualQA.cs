//  
//  EqualQA.cs
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
using Epic.Linq.Fakes;
using Epic.Linq.Expressions.Relational;
using Epic.Linq.Expressions.Relational.Predicates;
using NUnit.Framework;
using System.IO;
using Rhino.Mocks;

namespace Epic.Linq.Expressions.Relational.Predicates
{
    [TestFixture]
    public class EqualQA: RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withEitherArgumentNull_fails()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = null;

            // assert:
            Assert.Throws<ArgumentNullException>(() => new Equal<FakeScalar, FakeScalar>(scalar1, scalar2));
            Assert.Throws<ArgumentNullException>(() => new Equal<FakeScalar, FakeScalar>(scalar2, scalar1));
        }

        [Test]
        public void Initialize_withFakeScalars_operandsOK()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);

            // act:
            Equal<FakeScalar, FakeScalar> equal = new Equal<FakeScalar, FakeScalar>(scalar1, scalar2);

            // assert:
            Assert.IsNotNull (equal);
            Assert.IsTrue (equal.Left.Equals (scalar1));
            Assert.IsTrue (equal.Right.Equals (scalar2));
        }

        [Test]
        public void TwoPredicates_withDifferentGenericTypes_areNotEqual()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);

            // act:
            Equal<FakeScalar, FakeScalar> equal = new Equal<FakeScalar, FakeScalar>(scalar1, scalar2);
            Equal<Scalar, Scalar> equal2 = new Equal<Scalar, Scalar>(scalar1, scalar2);

            // assert:
            Assert.IsFalse (equal.Equals (equal2));
            Assert.AreNotEqual (equal.GetHashCode (), equal2.GetHashCode ());
        }


        [Test]
        public void Equals_AgainstSameObject_works()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);

            // act:
            Equal<FakeScalar, FakeScalar> equal = new Equal<FakeScalar, FakeScalar>(scalar1, scalar2);
            Equal<FakeScalar, FakeScalar> equal2 = equal;

            // assert:
            Assert.IsTrue (equal.Equals (equal2));
            Assert.AreEqual (equal.GetHashCode (), equal2.GetHashCode ());
        }

        [Test]
        public void Equals_AgainstEquivalentObject_works()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);

            // act:
            Equal<FakeScalar, FakeScalar> equal = new Equal<FakeScalar, FakeScalar>(scalar1, scalar2);
            Equal<FakeScalar, FakeScalar> equal2 = new Equal<FakeScalar, FakeScalar>(scalar1, scalar2);

            // assert:
            Assert.IsTrue (equal.Equals (equal2));
            Assert.AreEqual (equal.GetHashCode (), equal2.GetHashCode ());
        }

        [Test]
        public void Equals_WithDifferentOperands_fails()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar3 = new FakeScalar(ScalarType.Constant);

            // act:
            Equal<FakeScalar, FakeScalar> equal = new Equal<FakeScalar, FakeScalar>(scalar1, scalar2);
            Equal<FakeScalar, FakeScalar> equal2 = new Equal<FakeScalar, FakeScalar>(scalar1, scalar3);

            // assert:
            Assert.IsFalse (equal.Equals (equal2));
            Assert.AreNotEqual (equal.GetHashCode (), equal2.GetHashCode ());
        }

        [Test]
        public void Equals_WithDifferentlyTypedPredicate_fails()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);
            FakeScalarFunction function = new FakeScalarFunction("test");

            // act:
            Equal<FakeScalar, FakeScalarFunction> equal = new Equal<FakeScalar, FakeScalarFunction>(scalar1, function);
            Equal<FakeScalar, FakeScalar> equal2 = new Equal<FakeScalar, FakeScalar>(scalar1, scalar2);

            // assert:
            Assert.IsFalse (equal.Equals (equal2));
            Assert.AreNotEqual (equal.GetHashCode (), equal2.GetHashCode ());
        }

        [Test]
        public void Test_Serialization_Works()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);
            Equal<FakeScalar, FakeScalar> equal = new Equal<FakeScalar, FakeScalar>(scalar1, scalar2);

            // act:
            Stream stream = TestUtilities.Serialize<Equal<FakeScalar,FakeScalar>>(equal);

            // assert:
            Assert.IsNotNull (stream);
        }

        [Test]
        public void Test_deserialization_Works()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);
            Equal<FakeScalar, FakeScalar> equal = new Equal<FakeScalar, FakeScalar>(scalar1, scalar2);

            // act:
            Stream stream = TestUtilities.Serialize<Equal<FakeScalar,FakeScalar>>(equal);
            Equal<FakeScalar, FakeScalar> deserialized = TestUtilities.Deserialize<Equal<FakeScalar, FakeScalar>>(stream);

            // assert:
            // Assert.AreSame(equal, deserialized);
            Assert.IsTrue (equal.Equals (deserialized));
        }

        [Test]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            object expectedResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);
            Equal<FakeScalar, FakeScalar> equal = new Equal<FakeScalar, FakeScalar>(scalar1, scalar2);

            IVisitor<object, Equal<FakeScalar, FakeScalar>> equalPredicateVisitor =
                GenerateStrictMock<IVisitor<object, Equal<FakeScalar, FakeScalar>>>();
            equalPredicateVisitor.Expect(v => v.Visit(equal, context)).Return(expectedResult).Repeat.Once();
            IVisitor<object> visitor = GenerateStrictMock<IVisitor<object>>();
            visitor.Expect(v => v.GetVisitor(equal)).Return(equalPredicateVisitor).Repeat.Once ();

            // act:
            object result = equal.Accept(visitor, context);

            // assert:
            Assert.AreSame(expectedResult, result);
        }

    }
}

