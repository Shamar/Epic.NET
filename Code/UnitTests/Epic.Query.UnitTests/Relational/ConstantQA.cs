//  
//  ConstantQA.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
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
using NUnit.Framework;
using Epic.Query.Relational;
using System;
using Rhino.Mocks;
using Epic.Query.Fakes;

namespace Epic.Query.Relational
{
    [TestFixture()]
    public class ConstantQA: RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutValue_works()
        {
            // act:
            Constant<string> cString = new Constant<string>(null);
                        
            // assert:
            Assert.IsNotNull(cString);
            Assert.IsNull(cString.Value);
            Assert.AreEqual (cString.Type, ScalarType.Constant);
        }
        
        [Test]
        public void Initialize_withIntegerValue_works()
        {
            // arrange:
            int value = 10;
            Constant<int> cInt = new Constant<int>(value);

            // assert:
            Assert.IsNotNull (cInt);
            Assert.AreEqual (cInt.Value, value);
            Assert.AreEqual (cInt.Type, ScalarType.Constant);
        }
        
        [Test]
        public void InitializeTwoConstants_withIntegerValue_sameHash()
        {
            // arrange:
            int value = 10;
            Constant<int> cInt = new Constant<int>(value);     
            Constant<int> cInt2 = new Constant<int>(value);
            Object obj = cInt2;
            
            // assert:
            Assert.AreEqual(cInt.GetHashCode(), value.GetHashCode());
            Assert.IsTrue(cInt.Equals(cInt2));
            Assert.IsTrue (cInt.Equals (obj));
            Assert.IsTrue (obj.Equals (cInt));
        }
        
        [Test]
        public void Compare_withNullObject_returnsFalse()
        {
            // arrange:
            Constant<string> cString = new Constant<string>("test");
            Constant<string> cNull = null;
            
            // assert:
            Assert.IsFalse (cString.Equals (cNull));
        }
        
        [Test]
        public void Compare_withOtherTypeConstant_returnsFalse()
        {
            // arrange:
            Constant<string> cString = new Constant<string>("test");
            Constant<int> cInt = new Constant<int>(10);
            Object obj = cInt;

            // assert:
            Assert.IsFalse (cString.Equals(cInt));
            Assert.IsFalse (cString.Equals (obj));
            Assert.IsFalse (obj.Equals (cString));
        }

        [Test]
        public void Compare_throughScalarPointer_returnsTrue()
        {
            // arrange:
            Constant<string> cString = new Constant<string>("test");
            Scalar cScalar = new Constant<string>("test");
            Object obj = cScalar;

            // assert:
            Assert.IsTrue (cString.Equals(cScalar));
            Assert.IsTrue (cScalar.Equals(cString));
            Assert.IsTrue (cString.Equals (obj));
            Assert.IsTrue (obj.Equals (cString));
        }

        [Test]
        public void Compare_withOtherTypeConstantThroughScalarPointer_returnsFalse()
        {
            // arrange:
            Constant<string> cString = new Constant<string>("test");
            Scalar scalar = new Constant<int>(10);
            Object obj = cString;

            // assert:
            Assert.IsFalse (cString.Equals(scalar));
            Assert.IsFalse (scalar.Equals (cString));
            Assert.IsFalse (scalar.Equals (obj));
            Assert.IsTrue (obj.Equals (cString));
            Assert.IsTrue (cString.Equals (obj));
        }

        [Test]
        public void GetHashCode_fromNullConstant_returnsZero()
        {
            // arrange:
            Constant<string> cString = new Constant<string>(null);

            // act:
            int hash = cString.GetHashCode ();
            
            // assert:
            Assert.AreEqual (hash, 0);
        }
        
        [Test]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            object expectedResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            Constant<string> testConstant = new Constant<string> ("test");
            IVisitor<object, Constant<string>> constantVisitor = GenerateStrictMock<IVisitor<object, Constant<string>>>();
            constantVisitor.Expect(v => v.Visit(testConstant, context)).Return(expectedResult).Repeat.Once();
            
            IVisitor<object> visitor = GenerateStrictMock<IVisitor<object>>();
            visitor.Expect (v => v.AsVisitor(testConstant)).Return(constantVisitor).Repeat.Once();

            // act:
            object result = testConstant.Accept(visitor, context);
            // assert:
            Assert.AreSame(expectedResult, result);
        }

    }
}

