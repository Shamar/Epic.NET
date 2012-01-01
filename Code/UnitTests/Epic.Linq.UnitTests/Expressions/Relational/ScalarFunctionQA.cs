//  
//  ScalarFunctionQA.cs
//  
//  Author:
//       Marco Veglio <m.veglio@gmail.com>
// 
//  Copyright (c) 2010-2011 Giacomo Tesio
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
using Epic.Linq.Fakes;
using Rhino.Mocks;

namespace Epic.Linq.Expressions.Relational
{
    [TestFixture]
    public class ScalarFunctionQA: RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withEmptyString_Fails()
        {
            // arrange:
            string name = string.Empty;

            // assert:
            Assert.Throws<ArgumentNullException>(delegate { new FakeScalarFunction(name); } );
        }

        [Test]
        public void Initialize_withNullString_Fails()
        {
            // arrange:
            string name = null;

            // assert:
            Assert.Throws<ArgumentNullException>(delegate { new FakeScalarFunction(name); } );
        }

        [TestCase("test")]
        public void Initialize_withAName_works(string name)
        {
            // act:
            FakeScalarFunction function = new FakeScalarFunction(name);

            // assert:
            Assert.AreEqual(name, function.Name);
            Assert.AreEqual(ScalarType.Function, function.Type);
        }

        [TestCase("test")]
        public void FunctionEquals_toScalarFunction_isEvaluated(string name)
        {
            // arrange:
            ScalarFunction func1 = new FakeScalarFunction(name);
            ScalarFunction func2 = new FakeScalarFunction(name);

            // act:
            bool result = func1.Equals(func2);

            // assert:
            // here the FakeScalarFunction.Equals is called, so the result depends on the implementation
            Assert.IsTrue(result);
        }

        [TestCase("test")]
        public void FunctionEquals_toOtherScalarType_isFalse(string name)
        {
            // arrange:
            ScalarFunction func1 = new FakeScalarFunction(name);
            Scalar scalar = new FakeScalar(ScalarType.Constant);
            // act:
            bool result1 = func1.Equals(scalar);
            // assert:
            // here the FakeScalarFunction.Equals is called, so the result depends on the implementation
            Assert.IsFalse(result1);
        }

        [TestCase("test")]
        public void FunctionEquals_FromObjectReferenceToEqualInstance_isTrue(string name)
        {
            // arrange:
            ScalarFunction func1 = new FakeScalarFunction(name);
            ScalarFunction func2 = new FakeScalarFunction(name);
            Scalar scalar = func2;
            object obj = func2;

            // act:
            bool result1 = func1.Equals (func2);
            bool result2 = func1.Equals (scalar);
            bool result3 = func1.Equals (obj);
            bool result4 = scalar.Equals(func1);
            bool result5 = obj.Equals (func1);

            // assert:
            Assert.IsTrue (result1);
            Assert.IsTrue (result2);
            Assert.IsTrue (result3);
            Assert.IsTrue (result4);
            Assert.IsTrue (result5);


        }

        [TestCase("test")]
        public void FunctionEquals_withOtherReference_isFalse(string name)
        {
            // arrange:
            ScalarFunction func1 = new FakeScalarFunction(name);
            Scalar scalar = new FakeScalar(ScalarType.Function);
            object obj = scalar;

            // act:
            bool result1 = func1.Equals(scalar);
            bool result2 = func1.Equals(obj);
            bool result3 = obj.Equals (func1);
            bool result4 = scalar.Equals(func1);

            // assert:
            Assert.IsFalse (result1);
            Assert.IsFalse (result2);
            Assert.IsFalse (result3);
            Assert.IsFalse (result4);
        }

    }
}