//  
//  ExpressionForwarderQA.cs
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
using System.Collections.Generic;
using Rhino.Mocks;
using Epic.Fakes;
using Epic.Linq.Fakes;

namespace Epic.Linq.Expressions.Normalization
{
    [TestFixture()]
    public class ExpressionForwarderQA : RhinoMocksFixtureBase
    {
        private IVisitor<Expression, Expression> BuildCompositionWithMockableInterceptor(out IDerivedExpressionsVisitor mockableInterceptor)
        {
            FakeCompositeVisitor<Expression, Expression> composition = new FakeCompositeVisitor<Expression, Expression>("TEST");
            new ExpressionForwarder(composition);
            CompositeVisitor<Expression>.VisitorBase mockable = GeneratePartialMock<CompositeVisitor<Expression>.VisitorBase, IDerivedExpressionsVisitor>(composition);
            mockableInterceptor = mockable as IDerivedExpressionsVisitor;
            return composition;
        }

        
        [Test]
        public void Initialize_withoutComposition_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new ExpressionForwarder(null);
            });
        }
        
        [Test]
        public void Visit_aNullExpression_returnsNull()
        {
            // arrange:
            FakeCompositeVisitor<Expression, Expression> composition = new FakeCompositeVisitor<Expression, Expression>("TEST");
            IVisitor<Expression, Expression> visitor = new ExpressionForwarder(composition);

            // act:
            Expression result = visitor.Visit(null, VisitContext.New);

            // assert:
            Assert.IsNull(result);
        }
        
        [Test, TestCaseSource(typeof(Samples), "UnaryExpressions")]
        public void Visit_anUnaryExpression_callTheRightVisitor(Expression expression)
        {
            // arrange:
            IVisitContext context = VisitContext.New;
            IDerivedExpressionsVisitor mockableInterceptor = null;
            UnaryExpression unaryExpression = (UnaryExpression)expression;
            IVisitor<Expression, Expression> visitor = BuildCompositionWithMockableInterceptor(out mockableInterceptor);
            mockableInterceptor.Expect(v => v.Visit(unaryExpression, context)).Return(unaryExpression).Repeat.Once();

            // act:
            Expression visitResult = visitor.Visit(expression, context);

            // assert:
            Assert.AreSame(expression, visitResult);
        }
  
        [Test, TestCaseSource(typeof(Samples), "BinaryExpressions")]
        public void Visit_anBinaryExpression_callTheRightVisitor(Expression expression)
        {
            // arrange:
            IVisitContext context = VisitContext.New;
            IDerivedExpressionsVisitor mockableInterceptor = null;
            BinaryExpression binaryExpression = (BinaryExpression)expression;
            IVisitor<Expression, Expression> visitor = BuildCompositionWithMockableInterceptor(out mockableInterceptor);
            mockableInterceptor.Expect(v => v.Visit(binaryExpression, context)).Return(binaryExpression).Repeat.Once();

            // act:
            Expression visitResult = visitor.Visit(expression, context);

            // assert:
            Assert.AreSame(expression, visitResult);
        }
  
        [Test]
        public void Visit_aConditionalExpression_callTheRightVisitor()
        {
            // arrange:
            Expression<Func<int, int>> dumbExp = i => i > 0 ? 1 : -1;
            ConditionalExpression typedExpression = dumbExp.Body as ConditionalExpression;
            Expression expression = typedExpression;
            IVisitContext context = VisitContext.New;
            IDerivedExpressionsVisitor mockableInterceptor = null;
            IVisitor<Expression, Expression> visitor = BuildCompositionWithMockableInterceptor(out mockableInterceptor);
            mockableInterceptor.Expect(v => v.Visit(typedExpression, context)).Return(typedExpression).Repeat.Once();
            

            // act:
            Expression visitResult = visitor.Visit(expression, context);

            // assert:
            Assert.IsNotNull(typedExpression);
            Assert.AreSame(expression, visitResult);
        }
        
        [Test]
        public void Visit_aConstantExpression_callTheRightVisitor()
        {
            // arrange:
            ConstantExpression typedExpression = Expression.Constant(1);
            Expression expression = typedExpression;
            IVisitContext context = VisitContext.New;
            IDerivedExpressionsVisitor mockableInterceptor = null;
            IVisitor<Expression, Expression> visitor = BuildCompositionWithMockableInterceptor(out mockableInterceptor);
            mockableInterceptor.Expect(v => v.Visit(typedExpression, context)).Return(typedExpression).Repeat.Once();
            

            // act:
            Expression visitResult = visitor.Visit(expression, context);

            // assert:
            Assert.AreSame(expression, visitResult);
        }
        
        [Test]
        public void Visit_anInvocationExpression_callTheRightVisitor()
        {
            // arrange:
            Expression<Func<int, int>> dumbExp = i => i > 0 ? 1 : -1;
            InvocationExpression typedExpression = Expression.Invoke(dumbExp, new Expression[] { Expression.Constant(1) });
            Expression expression = typedExpression;
            IVisitContext context = VisitContext.New;
            IDerivedExpressionsVisitor mockableInterceptor = null;
            IVisitor<Expression, Expression> visitor = BuildCompositionWithMockableInterceptor(out mockableInterceptor);
            mockableInterceptor.Expect(v => v.Visit(typedExpression, context)).Return(typedExpression).Repeat.Once();
            

            // act:
            Expression visitResult = visitor.Visit(expression, context);

            // assert:
            Assert.AreSame(expression, visitResult);
        }
        
        [Test]
        public void Visit_aLambdaExpression_callTheRightVisitor()
        {
            // arrange:
            Expression<Func<int, int>> dumbExp = i => i > 0 ? 1 : -1;
            LambdaExpression typedExpression = dumbExp;
            Expression expression = typedExpression;
            IVisitContext context = VisitContext.New;
            IDerivedExpressionsVisitor mockableInterceptor = null;
            IVisitor<Expression, Expression> visitor = BuildCompositionWithMockableInterceptor(out mockableInterceptor);
            mockableInterceptor.Expect(v => v.Visit(typedExpression, context)).Return(typedExpression).Repeat.Once();
            

            // act:
            Expression visitResult = visitor.Visit(expression, context);

            // assert:
            Assert.AreSame(expression, visitResult);
        }
        
        
        [Test]
        public void Visit_aMemberExpression_callTheRightVisitor()
        {
            // arrange:
            Expression<Func<string, int>> dumbExp = s => s.Length;
            MemberExpression typedExpression = dumbExp.Body as MemberExpression;
            Expression expression = typedExpression;
            IVisitContext context = VisitContext.New;
            IDerivedExpressionsVisitor mockableInterceptor = null;
            IVisitor<Expression, Expression> visitor = BuildCompositionWithMockableInterceptor(out mockableInterceptor);
            mockableInterceptor.Expect(v => v.Visit(typedExpression, context)).Return(typedExpression).Repeat.Once();
            

            // act:
            Expression visitResult = visitor.Visit(expression, context);

            // assert:
            Assert.IsNotNull(typedExpression);
            Assert.AreSame(expression, visitResult);
        }
        
        [Test]
        public void Visit_aMethodCallExpression_callTheRightVisitor()
        {
            // arrange:
            Expression<Func<string, string>> dumbExp = s => s.Substring(1);
            MethodCallExpression typedExpression = dumbExp.Body as MethodCallExpression;
            Expression expression = typedExpression;
            IVisitContext context = VisitContext.New;
            IDerivedExpressionsVisitor mockableInterceptor = null;
            IVisitor<Expression, Expression> visitor = BuildCompositionWithMockableInterceptor(out mockableInterceptor);
            mockableInterceptor.Expect(v => v.Visit(typedExpression, context)).Return(typedExpression).Repeat.Once();
            

            // act:
            Expression visitResult = visitor.Visit(expression, context);

            // assert:
            Assert.IsNotNull(typedExpression);
            Assert.AreSame(expression, visitResult);
        }
        
        [Test]
        public void Visit_aNewExpression_callTheRightVisitor()
        {
            // arrange:
            Expression<Func<object>> dumbExp = () => new object();
            NewExpression typedExpression = dumbExp.Body as NewExpression;
            Expression expression = typedExpression;
            IVisitContext context = VisitContext.New;
            IDerivedExpressionsVisitor mockableInterceptor = null;
            IVisitor<Expression, Expression> visitor = BuildCompositionWithMockableInterceptor(out mockableInterceptor);
            mockableInterceptor.Expect(v => v.Visit(typedExpression, context)).Return(typedExpression).Repeat.Once();
            

            // act:
            Expression visitResult = visitor.Visit(expression, context);

            // assert:
            Assert.IsNotNull(typedExpression);
            Assert.AreSame(expression, visitResult);
        }
        
        [Test]
        public void Visit_aNewArrayExpression_callTheRightVisitor()
        {
            // arrange:
            Expression<Func<object[]>> dumbExp = () => new object[0] { };
            NewArrayExpression typedExpression = dumbExp.Body as NewArrayExpression;
            Expression expression = typedExpression;
            IVisitContext context = VisitContext.New;
            IDerivedExpressionsVisitor mockableInterceptor = null;
            IVisitor<Expression, Expression> visitor = BuildCompositionWithMockableInterceptor(out mockableInterceptor);
            mockableInterceptor.Expect(v => v.Visit(typedExpression, context)).Return(typedExpression).Repeat.Once();
            

            // act:
            Expression visitResult = visitor.Visit(expression, context);

            // assert:
            Assert.IsNotNull(typedExpression);
            Assert.AreSame(expression, visitResult);
        }
         
        [Test]
        public void Visit_aMemberInitExpression_callTheRightVisitor()
        {
            // arrange:
            Expression<Func<object>> dumbExp = () => new ClassWithFieldAndProperty { Field = 1, Property = "test" };
            MemberInitExpression typedExpression = dumbExp.Body as MemberInitExpression;
            Expression expression = typedExpression;
            IVisitContext context = VisitContext.New;
            IDerivedExpressionsVisitor mockableInterceptor = null;
            IVisitor<Expression, Expression> visitor = BuildCompositionWithMockableInterceptor(out mockableInterceptor);
            mockableInterceptor.Expect(v => v.Visit(typedExpression, context)).Return(typedExpression).Repeat.Once();
            

            // act:
            Expression visitResult = visitor.Visit(expression, context);

            // assert:
            Assert.IsNotNull(typedExpression);
            Assert.AreSame(expression, visitResult);
        }
        
        [Test]
        public void Visit_aListInitExpression_callTheRightVisitor()
        {
            // arrange:
            ListInitExpression typedExpression = Samples.GetNewListInitExpression();
            Expression expression = typedExpression;
            IVisitContext context = VisitContext.New;
            IDerivedExpressionsVisitor mockableInterceptor = null;
            IVisitor<Expression, Expression> visitor = BuildCompositionWithMockableInterceptor(out mockableInterceptor);
            mockableInterceptor.Expect(v => v.Visit(typedExpression, context)).Return(typedExpression).Repeat.Once();
            

            // act:
            Expression visitResult = visitor.Visit(expression, context);

            // assert:
            Assert.IsNotNull(typedExpression);
            Assert.AreSame(expression, visitResult);
        }
       
                
        [Test]
        public void Visit_aParameterExpression_callTheRightVisitor()
        {
            // arrange:
            ParameterExpression typedExpression = Expression.Parameter(typeof(int), "i");
            Expression expression = typedExpression;
            IVisitContext context = VisitContext.New;
            IDerivedExpressionsVisitor mockableInterceptor = null;
            IVisitor<Expression, Expression> visitor = BuildCompositionWithMockableInterceptor(out mockableInterceptor);
            mockableInterceptor.Expect(v => v.Visit(typedExpression, context)).Return(typedExpression).Repeat.Once();
            

            // act:
            Expression visitResult = visitor.Visit(expression, context);

            // assert:
            Assert.AreSame(expression, visitResult);
        }
        
        [Test]
        public void Visit_aTypeBinaryExpression_callTheRightVisitor()
        {
            // arrange:
            TypeBinaryExpression typedExpression = Samples.GetNewTypeBinaryExpression<int>(1);
            Expression expression = typedExpression;
            IVisitContext context = VisitContext.New;
            IDerivedExpressionsVisitor mockableInterceptor = null;
            IVisitor<Expression, Expression> visitor = BuildCompositionWithMockableInterceptor(out mockableInterceptor);
            mockableInterceptor.Expect(v => v.Visit(typedExpression, context)).Return(typedExpression).Repeat.Once();
            
            // act:
            Expression visitResult = visitor.Visit(expression, context);

            // assert:
            Assert.AreSame(expression, visitResult);
        }
        
        [Test]
        public void Visit_anUnknownExpression_throwsArgumentException()
        {
            // arrange:
            UnknownExpression typedExpression = new UnknownExpression();
            Expression expression = typedExpression;
            IVisitContext context = VisitContext.New;
            IDerivedExpressionsVisitor mockableInterceptor = null;
            IVisitor<Expression, Expression> visitor = BuildCompositionWithMockableInterceptor(out mockableInterceptor);
            Expression visitResult = null;

            // assert:
            Assert.Throws<ArgumentException>(delegate {
                visitResult = visitor.Visit(expression, context);
            });
            Assert.IsNull(visitResult);
        }
    }
}

