//  
//  CompositeVisitorBaseQA.cs
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
using System;
using System.Linq.Expressions;
using Epic.Fakes;
using Rhino.Mocks;

namespace Epic
{
    [TestFixture()]
    public class CompositeVisitorBaseQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutName_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new FakeCompositeVisitor<int, Expression>(null);
            });
            
            Assert.Throws<ArgumentNullException>(delegate {
                new FakeCompositeVisitor<object, Expression>(string.Empty);
            });
        }
        
        [Test]
        public void Initialize_withAName_works()
        {
            // arrange:
            string name = "test";

            // act:
            IVisitor<object, Expression> visitor = new FakeCompositeVisitor<object, Expression>(name);

            // assert:
            Assert.IsNotNull(visitor);
        }
        
        [Test]
        public void Visit_withoutAnyArgument_throwsArgumentNullException()
        {
            // arrange:
            string name = "test";
            Expression expression = Expression.Constant(1);
            IVisitor<object, Expression> visitor = new FakeCompositeVisitor<object, Expression>(name);

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                visitor.Visit(null, VisitContext.New);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                visitor.Visit(expression, null);
            });
        }
        
        [Test]
        public void Visit_withValidArguments_callInitializeVisitContext()
        {
            // arrange:
            string name = "test";
            object expectedResult = new object();
            IVisitContext initialContext = GenerateStrictMock<IVisitContext>();
            IVisitContext initializedContext = GenerateStrictMock<IVisitContext>();
            ConstantExpression expression = Expression.Constant(1);
            FakeCompositeVisitor<object, Expression> composition = GeneratePartialMock<FakeCompositeVisitor<object, Expression>>(name);
            composition.Expect(c => c.CallInitializeVisitContext(expression, initialContext)).Return(initializedContext).Repeat.Once();
            FakeVisitor<object, ConstantExpression> registered = GeneratePartialMock<FakeVisitor<object, ConstantExpression>>(composition);
            registered.Expect(v => v.CallToVisitor(expression)).Return(registered).Repeat.Once();
            registered.Expect(v => v.Visit(expression, initializedContext)).Return(expectedResult).Repeat.Once();

            // act:
            object result = composition.Visit(expression, initialContext);


            // assert:
            Assert.AreSame(expectedResult, result);
        }
    }
}

