//  
//  PredicateExtensionsQA.cs
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

namespace Epic.Linq.Expressions.Relational.Predicates
{
    [TestFixture]
    public class PredicateExtensionsQA: RhinoMocksFixtureBase
    {
        [Test]
        public void GreaterMethod_FromScalarObjects_ReturnsPredicate()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);

            // act:
            Greater<FakeScalar, FakeScalar> greater = scalar1.Greater(scalar2);

            // assert:
            Assert.IsTrue (greater.Left.Equals (scalar1));
            Assert.IsTrue (greater.Right.Equals (scalar2));
        }

        [Test]
        public void LessMethod_FromScalarObjects_ReturnsPredicate()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);

            // act:
            Less<FakeScalar, FakeScalar> less = scalar1.Less(scalar2);

            // assert:
            Assert.IsTrue (less.Left.Equals (scalar1));
            Assert.IsTrue (less.Right.Equals (scalar2));
        }

        [Test]
        public void EqualMethod_FromScalarObjects_ReturnsPredicate()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);

            // act:
            Equal<FakeScalar, FakeScalar> equal = scalar1.Equal(scalar2);

            // assert:
            Assert.IsTrue (equal.Left.Equals (scalar1));
            Assert.IsTrue (equal.Right.Equals (scalar2));

        }

        [Test]
        public void AndMethod_FromPredicateObjects_ReturnsPredicate()
        {
            // arrange:
            FakePredicate predicate1 = new FakePredicate();
            FakePredicate predicate2 = new FakePredicate();

            // act:
            And<FakePredicate, FakePredicate> and = predicate1.And(predicate2);

            // assert:
            Assert.IsTrue (and.Left.Equals (predicate1));
            Assert.IsTrue (and.Right.Equals (predicate2));
        }

        [Test]
        public void OrMethod_FromPredicateObjects_ReturnsPredicate()
        {
            // arrange:
            FakePredicate predicate1 = new FakePredicate();
            FakePredicate predicate2 = new FakePredicate();

            // act:
            Or<FakePredicate, FakePredicate> or = predicate1.Or(predicate2);

            // assert:
            Assert.IsTrue (or.Left.Equals (predicate1));
            Assert.IsTrue (or.Right.Equals (predicate2));
        }

        [Test]
        public void NotMethod_FromPredicateObject_ReturnsPredicate()
        {
            // arrange:
            FakePredicate predicate = new FakePredicate();

            // act:
            Not<FakePredicate> not = predicate.Not();

            // assert:
            Assert.IsTrue (not.Operand.Equals (predicate));

        }

        [Test]
        public void ExtensionMethod_TestConcatenation_Works()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Function);
            FakeScalar scalar3 = new FakeScalar(ScalarType.Attribute);

            // act:

            Predicate composition = scalar1.Greater (scalar2).And (scalar1.Less (scalar3)).Not ();

            Assert.IsAssignableFrom(
                typeof(Not<And<Greater<FakeScalar, FakeScalar>, Less<FakeScalar, FakeScalar>>>),
                composition);

        }
    }
}

