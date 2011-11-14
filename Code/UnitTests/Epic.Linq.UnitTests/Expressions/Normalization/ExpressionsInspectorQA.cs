//  
//  ExpressionsInspectorQA.cs
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
using System;
using System.Linq.Expressions;
using Epic.Linq.Fakes;
using Rhino.Mocks;

namespace Epic.Linq.Expressions.Normalization
{
    [TestFixture()]
    public class ExpressionsInspectorQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutComposition_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new ExpressionsInspector(null);
            });
        }
        
        private ExpressionsInspector BuildCompositionWithMockableInterceptor(out IVisitor<Expression, Expression> mockableInterceptor)
        {
            FakeCompositeVisitor<Expression, Expression> composition = new FakeCompositeVisitor<Expression, Expression>("TEST");
            CompositeVisitor<Expression>.VisitorBase mockable = GeneratePartialMock<CompositeVisitor<Expression>.VisitorBase, IVisitor<Expression, Expression>>(composition);
            ExpressionsInspector inspector = new ExpressionsInspector(composition);
            mockableInterceptor = mockable as IVisitor<Expression, Expression>;
            return inspector;
        }
        
        [Test]
        public void Visit_aConstantExpression_returnTheConstantRecieved()
        {
            // arrange:
            ConstantExpression expressionToVisit = Expression.Constant(1);
            IVisitor<Expression, Expression> interceptor = null;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);

            // act:
            Expression result = inspector.Visit(expressionToVisit, VisitContext.New);

            // assert:
            Assert.AreSame(expressionToVisit, result);
        }
        
        [Test]
        public void Visit_aParameterExpression_returnTheParameterRecieved()
        {
            // arrange:
            ParameterExpression expressionToVisit = Expression.Parameter(typeof(int), "p");
            IVisitor<Expression, Expression> interceptor = null;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);

            // act:
            Expression result = inspector.Visit(expressionToVisit, VisitContext.New);

            // assert:
            Assert.AreSame(expressionToVisit, result);
        }
        
        [Test, TestCaseSource(typeof(Samples), "UnaryExpressions")]
        public void Visit_anUnaryExpression_askTheCompositionToVisitTheOperand(UnaryExpression expressionToVisit)
        {
            // arrange:
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Operand, context)).Return(expressionToVisit.Operand).Repeat.Once();

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.AreSame(expressionToVisit, result);
        }
        
        [Test, TestCaseSource(typeof(Samples), "DifferentUnaryExpressionsFromTheSameFactory")]
        public void Visit_anUnaryExpression_returnANewUnaryExpressionWithTheOperandObtainedFromTheComposition(UnaryExpression expressionToVisit, UnaryExpression differentExpression)
        {
            // arrange:
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Operand, context)).Return(differentExpression.Operand).Repeat.Once();

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<UnaryExpression>(result);
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreSame(differentExpression.Operand, ((UnaryExpression)result).Operand);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
        }
        
        [Test, TestCaseSource(typeof(Samples), "BinaryExpressions")]
        public void Visit_aBinaryExpression_askTheCompositionToVisitTheOperands(BinaryExpression expressionToVisit)
        {
            // arrange:
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Left, context)).Return(expressionToVisit.Left).Repeat.Once();
            interceptor.Expect(v => v.Visit(expressionToVisit.Right, context)).Return(expressionToVisit.Right).Repeat.Once();
            interceptor.Expect(v => v.Visit(expressionToVisit.Conversion, context)).Return(expressionToVisit.Conversion).Repeat.Once();

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.AreSame(expressionToVisit, result);
        }
        
        [Test, TestCaseSource(typeof(Samples), "DifferentBinaryExpressionsFromTheSameFactory")]
        public void Visit_aBinaryExpression_returnANewBinaryExpressionWithTheOperandsObtainedFromTheComposition(BinaryExpression expressionToVisit, BinaryExpression differentExpression)
        {
            // arrange:
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Left, context)).Return(differentExpression.Left).Repeat.Once();
            interceptor.Expect(v => v.Visit(expressionToVisit.Right, context)).Return(differentExpression.Right).Repeat.Once();
            interceptor.Expect(v => v.Visit(expressionToVisit.Conversion, context)).Return(differentExpression.Conversion).Repeat.Once();

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BinaryExpression>(result);
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreSame(differentExpression.Left, ((BinaryExpression)result).Left);
            Assert.AreSame(differentExpression.Right, ((BinaryExpression)result).Right);
            Assert.AreSame(differentExpression.Conversion, ((BinaryExpression)result).Conversion);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
        }
        
        [Test]
        public void Visit_aConditionalExpression_askTheCompositionToVisitTheOperands()
        {
            // arrange:
            Expression<Func<int, int>> dummy = i => i > 0 ? 1 : 2;
            ConditionalExpression expressionToVisit = (ConditionalExpression)dummy.Body;
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Test, context)).Return(expressionToVisit.Test).Repeat.Once();
            interceptor.Expect(v => v.Visit(expressionToVisit.IfTrue, context)).Return(expressionToVisit.IfTrue).Repeat.Once();
            interceptor.Expect(v => v.Visit(expressionToVisit.IfFalse, context)).Return(expressionToVisit.IfFalse).Repeat.Once();

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.AreSame(expressionToVisit, result);
        }
  
        [Test]
        public void Visit_aConditionalExpression_returnANewConditionalExpressionWithThe_Test_ObtainedFromTheComposition()
        {
            // arrange:
            Expression<Func<int, int>> dummy = i => i > 0 ? 1 : 2;
            Expression<Func<int, int>> dummy2 = i => i > 3 ? 4 : 5;
            ConditionalExpression expressionToVisit = (ConditionalExpression)dummy.Body;
            ConditionalExpression differentExpression = (ConditionalExpression)dummy2.Body;
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Test, context)).Return(differentExpression.Test).Repeat.Once();
            interceptor.Expect(v => v.Visit(expressionToVisit.IfTrue, context)).Return(expressionToVisit.IfTrue).Repeat.Once();
            interceptor.Expect(v => v.Visit(expressionToVisit.IfFalse, context)).Return(expressionToVisit.IfFalse).Repeat.Once();

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ConditionalExpression>(result);
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreSame(differentExpression.Test, ((ConditionalExpression)result).Test);
            Assert.AreSame(expressionToVisit.IfTrue, ((ConditionalExpression)result).IfTrue);
            Assert.AreSame(expressionToVisit.IfFalse, ((ConditionalExpression)result).IfFalse);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
        }
        
        [Test]
        public void Visit_aConditionalExpression_returnANewConditionalExpressionWithThe_IfTrue_ObtainedFromTheComposition()
        {
            // arrange:
            Expression<Func<int, int>> dummy = i => i > 0 ? 1 : 2;
            Expression<Func<int, int>> dummy2 = i => i > 3 ? 4 : 5;
            ConditionalExpression expressionToVisit = (ConditionalExpression)dummy.Body;
            ConditionalExpression differentExpression = (ConditionalExpression)dummy2.Body;
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Test, context)).Return(expressionToVisit.Test).Repeat.Once();
            interceptor.Expect(v => v.Visit(expressionToVisit.IfTrue, context)).Return(differentExpression.IfTrue).Repeat.Once();
            interceptor.Expect(v => v.Visit(expressionToVisit.IfFalse, context)).Return(expressionToVisit.IfFalse).Repeat.Once();

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ConditionalExpression>(result);
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreSame(expressionToVisit.Test, ((ConditionalExpression)result).Test);
            Assert.AreSame(differentExpression.IfTrue, ((ConditionalExpression)result).IfTrue);
            Assert.AreSame(expressionToVisit.IfFalse, ((ConditionalExpression)result).IfFalse);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
        }
        
  
        [Test]
        public void Visit_aConditionalExpression_returnANewConditionalExpressionWithThe_IfFalse_ObtainedFromTheComposition()
        {
            // arrange:
            Expression<Func<int, int>> dummy = i => i > 0 ? 1 : 2;
            Expression<Func<int, int>> dummy2 = i => i > 3 ? 4 : 5;
            ConditionalExpression expressionToVisit = (ConditionalExpression)dummy.Body;
            ConditionalExpression differentExpression = (ConditionalExpression)dummy2.Body;
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Test, context)).Return(expressionToVisit.Test).Repeat.Once();
            interceptor.Expect(v => v.Visit(expressionToVisit.IfTrue, context)).Return(expressionToVisit.IfTrue).Repeat.Once();
            interceptor.Expect(v => v.Visit(expressionToVisit.IfFalse, context)).Return(differentExpression.IfFalse).Repeat.Once();

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ConditionalExpression>(result);
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreSame(expressionToVisit.Test, ((ConditionalExpression)result).Test);
            Assert.AreSame(expressionToVisit.IfTrue, ((ConditionalExpression)result).IfTrue);
            Assert.AreSame(differentExpression.IfFalse, ((ConditionalExpression)result).IfFalse);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
        }
        
        [Test]
        public void Visit_anInvocationExpression_askTheCompositionToVisitTheOperands()
        {
            // arrange:
            Expression<Func<int, int, int>> dummy = (i, j) => i > j ? 1 : 2;
            Expression[] arguments = new Expression[] { Expression.Constant(5), Expression.Constant(2) };
            InvocationExpression expressionToVisit = Expression.Invoke(dummy, arguments);
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Expression, context)).Return(expressionToVisit.Expression).Repeat.Once();
            interceptor.Expect(v => v.Visit(arguments[0], context)).Return(arguments[0]).Repeat.Once();
            interceptor.Expect(v => v.Visit(arguments[1], context)).Return(arguments[1]).Repeat.Once();

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.AreSame(expressionToVisit, result);
        }
        
        [Test]
        public void Visit_anInvocationExpression_returnANewInvocationExpressionWithThe_Expression_ObtainedFromTheComposition()
        {
            // arrange:
            Expression<Func<int, int, int>> dummy = (i, j) => i > j ? 1 : 2;
            Expression<Func<int, int, int>> dummy2 = (a, b) => a == b ? 3 : 4;
            Expression[] arguments = new Expression[] { Expression.Constant(5), Expression.Constant(2) };
            InvocationExpression expressionToVisit = Expression.Invoke(dummy, arguments);
            InvocationExpression differentExpression = Expression.Invoke(dummy2, arguments);
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Expression, context)).Return(differentExpression.Expression).Repeat.Once();
            interceptor.Expect(v => v.Visit(arguments[0], context)).Return(arguments[0]).Repeat.Once();
            interceptor.Expect(v => v.Visit(arguments[1], context)).Return(arguments[1]).Repeat.Once();

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<InvocationExpression>(result);
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreSame(differentExpression.Expression, ((InvocationExpression)result).Expression);
            Assert.AreSame(expressionToVisit.Arguments[0], ((InvocationExpression)result).Arguments[0]);
            Assert.AreSame(expressionToVisit.Arguments[1], ((InvocationExpression)result).Arguments[1]);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
        }
        
        [Test]
        public void Visit_anInvocationExpression_returnANewInvocationExpressionWithThe_Arguments_ObtainedFromTheComposition()
        {
            // arrange:
            Expression<Func<int, int, int>> dummy = (i, j) => i > j ? 1 : 2;
            Expression common = Expression.Constant(2);
            Expression[] arguments = new Expression[] { Expression.Constant(5), common };
            Expression[] arguments2 = new Expression[] { Expression.Constant(6), common };
            InvocationExpression expressionToVisit = Expression.Invoke(dummy, arguments);
            InvocationExpression differentExpression = Expression.Invoke(dummy, arguments2);
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Expression, context)).Return(expressionToVisit.Expression).Repeat.Once();
            interceptor.Expect(v => v.Visit(arguments[0], context)).Return(arguments2[0]).Repeat.Once();
            interceptor.Expect(v => v.Visit(arguments[1], context)).Return(arguments[1]).Repeat.Once();

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<InvocationExpression>(result);
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreSame(expressionToVisit.Expression, ((InvocationExpression)result).Expression);
            Assert.AreSame(differentExpression.Arguments[0], ((InvocationExpression)result).Arguments[0]);
            Assert.AreSame(differentExpression.Arguments[1], ((InvocationExpression)result).Arguments[1]);
            Assert.AreSame(expressionToVisit.Arguments[1], ((InvocationExpression)result).Arguments[1]);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
        }
        
        [Test]
        public void Visit_aLambdaExpression_askTheCompositionToVisitTheOperands()
        {
            Expression<Func<int, int, int>> dummy = (i, j) => i > j ? 1 : 2;
            LambdaExpression expressionToVisit = dummy;
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Body, context)).Return(expressionToVisit.Body).Repeat.Once();
            interceptor.Expect(v => v.Visit(expressionToVisit.Parameters[0], context)).Return(expressionToVisit.Parameters[0]).Repeat.Once();
            interceptor.Expect(v => v.Visit(expressionToVisit.Parameters[1], context)).Return(expressionToVisit.Parameters[1]).Repeat.Once();

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.AreSame(expressionToVisit, result);
        }
        
        [Test]
        public void Visit_aLambdaExpression_returnANewLambdaExpressionWithThe_Body_ObtainedFromTheComposition()
        {
            Expression<Func<int, int, int>> dummy = (i, j) => i > j ? 1 : 2;
            Expression<Func<int, int, int>> differentExpression = (a, b) => a == b ? 3 : 4;
            LambdaExpression expressionToVisit = dummy;
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Body, context)).Return(differentExpression.Body).Repeat.Once();
            interceptor.Expect(v => v.Visit(expressionToVisit.Parameters[0], context)).Return(expressionToVisit.Parameters[0]).Repeat.Once();
            interceptor.Expect(v => v.Visit(expressionToVisit.Parameters[1], context)).Return(expressionToVisit.Parameters[1]).Repeat.Once();

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<LambdaExpression>(result);
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreSame(differentExpression.Body, ((LambdaExpression)result).Body);
            Assert.AreSame(expressionToVisit.Parameters[0], ((LambdaExpression)result).Parameters[0]);
            Assert.AreSame(expressionToVisit.Parameters[1], ((LambdaExpression)result).Parameters[1]);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
        }
        
        [Test]
        public void Visit_aLambdaExpression_returnANewLambdaExpressionWithThe_Parameters_ObtainedFromTheComposition()
        {
            Expression<Func<int, int, int>> dummy = (i, j) => i > j ? 1 : 2;
            Expression<Func<int, int, int>> differentExpression = (a, b) => a == b ? 3 : 4;
            LambdaExpression expressionToVisit = dummy;
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Body, context)).Return(expressionToVisit.Body).Repeat.Once();
            interceptor.Expect(v => v.Visit(expressionToVisit.Parameters[0], context)).Return(differentExpression.Parameters[0]).Repeat.Once();
            interceptor.Expect(v => v.Visit(expressionToVisit.Parameters[1], context)).Return(differentExpression.Parameters[1]).Repeat.Once();

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<LambdaExpression>(result);
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreSame(expressionToVisit.Body, ((LambdaExpression)result).Body);
            Assert.AreSame(differentExpression.Parameters[0], ((LambdaExpression)result).Parameters[0]);
            Assert.AreSame(differentExpression.Parameters[1], ((LambdaExpression)result).Parameters[1]);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
        }
    }
}

