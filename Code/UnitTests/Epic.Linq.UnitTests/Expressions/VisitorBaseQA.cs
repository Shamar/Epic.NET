//  
//  VisitorBaseQA.cs
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
using System;
using NUnit.Framework;
using Epic.Linq.Fakes;
using System.Linq.Expressions;
using Rhino.Mocks;

namespace Epic.Linq.Expressions
{
    [TestFixture]
    public class VisitorBaseQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutComposition_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate { 
                new FakeVisitor<string, int>(null);
            });
        }
        
        [Test]
        public void Initialize_twiceWithTheSameComposition_throwsArgumentException()
        {
            // arrange:
            CompositeVisitor<int> composition = new FakeCompositeVisitor<int>("test");
            new FakeVisitor<int, ConstantExpression>(composition);

            // assert:
            Assert.Throws<ArgumentException>(delegate {
                new FakeVisitor<int, ConstantExpression>(composition);
            });
        }
        
        [Test]
        public void Initialize_withAComposition_registerTheVisitorInTheComposition()
        {
            // arrange:
            ConstantExpression expression = Expression.Constant(0);
            CompositeVisitor<int> composition = new FakeCompositeVisitor<int>("test");

            // act:
            IVisitor<int, ConstantExpression> visitor = new FakeVisitor<int, ConstantExpression>(composition);
            IVisitor<int, ConstantExpression> returnedVisitor = composition.GetFirstVisitor<ConstantExpression>(expression);

            // assert:
            Assert.AreSame(visitor, returnedVisitor);
        }
        
        [Test]
        public void GetVisitor_withAnExpressionThatCouldBeHandled_callAsVisitorAndReturnTheVisitorItself()
        {
            // arrange:
            ConstantExpression expression = Expression.Constant(0);
            CompositeVisitor<int> composition = new FakeCompositeVisitor<int>("test");
            FakeVisitor<int, ConstantExpression> visitor = GeneratePartialMock<FakeVisitor<int, ConstantExpression>>(composition);
            visitor.Expect(v => v.CallAsVisitor(expression)).Return(visitor).Repeat.Twice ();

            // act:
            IVisitor<int, ConstantExpression> returnedFromVisitor = visitor.GetVisitor(expression);
            IVisitor<int, ConstantExpression> returnedFromComposition = visitor.GetVisitor(expression);

            // assert:
            Assert.AreSame(visitor, returnedFromVisitor);
            Assert.AreSame(visitor, returnedFromComposition);
        }
    }
}

