//  
//  ConstantQA.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
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
using NUnit.Framework;
using Epic.Linq.Expressions.Relational;
using System;
using Rhino.Mocks;
using Epic.Linq.Fakes;

namespace Epic.Linq.Expressions.Relational
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
            visitor.Expect (v => v.GetVisitor(testConstant)).Return(constantVisitor).Repeat.Once();

            // act:
            object result = testConstant.Accept(visitor, context);
            // assert:
            Assert.AreSame(expectedResult, result);
        }

    }
}

