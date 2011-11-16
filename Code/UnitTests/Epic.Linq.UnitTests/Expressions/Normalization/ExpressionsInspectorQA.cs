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
using System.Collections.Generic;
using System.Reflection;

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
        public void Visit_anUnaryExpression_returnsAnotherUnaryExpressionWithTheOperandObtainedFromTheComposition(UnaryExpression expressionToVisit, UnaryExpression differentExpression)
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
        public void Visit_aBinaryExpression_returnsAnotherBinaryExpressionWithTheOperandsObtainedFromTheComposition(BinaryExpression expressionToVisit, BinaryExpression differentExpression)
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
        public void Visit_aConditionalExpression_returnsAnotherConditionalExpressionWithThe_Test_ObtainedFromTheComposition()
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
        public void Visit_aConditionalExpression_returnsAnotherConditionalExpressionWithThe_IfTrue_ObtainedFromTheComposition()
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
        public void Visit_aConditionalExpression_returnsAnotherConditionalExpressionWithThe_IfFalse_ObtainedFromTheComposition()
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
        public void Visit_anInvocationExpression_returnsAnotherInvocationExpressionWithThe_Expression_ObtainedFromTheComposition()
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
        public void Visit_anInvocationExpression_returnsAnotherInvocationExpressionWithThe_Arguments_ObtainedFromTheComposition()
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
        public void Visit_aLambdaExpression_returnsAnotherLambdaExpressionWithThe_Body_ObtainedFromTheComposition()
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
        public void Visit_aLambdaExpression_returnsAnotherLambdaExpressionWithThe_Parameters_ObtainedFromTheComposition()
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
        
        [Test]
        public void Visit_aLambdaExpressionWithAVisitorReplacingTheArgumentType_throwsInvalidOperationException()
        {
            Expression<Func<int, int, int>> dummy = (i, j) => i > j ? 1 : 2;
            Expression<Func<int, int, int>> differentExpression = (a, b) => a == b ? 3 : 4;
            LambdaExpression expressionToVisit = dummy;
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Body, context)).Return(expressionToVisit.Body).Repeat.Any();
            interceptor.Expect(v => v.Visit(expressionToVisit.Parameters[0], context)).Return(dummy.Parameters[0]).Repeat.Any();
            interceptor.Expect(v => v.Visit(expressionToVisit.Parameters[1], context)).Return(Expression.Constant(1)).Repeat.Once();

            // assert:
            Assert.Throws<InvalidOperationException>(delegate {
                Expression result = inspector.Visit(expressionToVisit, context);
            });
        }
        
        [Test]
        public void Visit_aListInitExpression_askTheCompositionToVisitTheOperands()
        {
            // arrange:
            ListInitExpression expressionToVisit = Samples.GetNewListInitExpression();
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.NewExpression, context)).Return(expressionToVisit.NewExpression).Repeat.Any();
            interceptor.Expect(v => v.Visit(expressionToVisit.Initializers[0].Arguments[0], context)).Return(expressionToVisit.Initializers[0].Arguments[0]).Repeat.Any();
            interceptor.Expect(v => v.Visit(expressionToVisit.Initializers[0].Arguments[1], context)).Return(expressionToVisit.Initializers[0].Arguments[1]).Repeat.Any();
            interceptor.Expect(v => v.Visit(expressionToVisit.Initializers[1].Arguments[0], context)).Return(expressionToVisit.Initializers[1].Arguments[0]).Repeat.Any();
            interceptor.Expect(v => v.Visit(expressionToVisit.Initializers[1].Arguments[1], context)).Return(expressionToVisit.Initializers[1].Arguments[1]).Repeat.Any();

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.AreSame(expressionToVisit, result);
        }
        
        [Test]
        public void Visit_aListInitExpressionWithAVisitorReturningANull_NewExpression_throwsNotSupportedException()
        {
            // arrange:
            ListInitExpression expressionToVisit = Samples.GetNewListInitExpression();
IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.NewExpression, context)).Return(null).Repeat.Any();

            // assert:
            Assert.Throws<NotSupportedException>(delegate {
                Expression result = inspector.Visit(expressionToVisit, context);
            });
        }

        
        [Test]
        public void Visit_aListInitExpression_returnsAnotherListInitExpressionWithThe_NewExpression_ObtainedFromTheComposition()
        {
            // arrange:
            ListInitExpression[] exprs = Samples.GetTwoDifferentListInitExpressions();
            ListInitExpression expressionToVisit = exprs[0];
            ListInitExpression differentExpression = exprs[1];
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.NewExpression, context)).Return(differentExpression.NewExpression).Repeat.Any();
            interceptor.Expect(v => v.Visit(expressionToVisit.Initializers[0].Arguments[0], context)).Return(expressionToVisit.Initializers[0].Arguments[0]).Repeat.Any();
            interceptor.Expect(v => v.Visit(expressionToVisit.Initializers[0].Arguments[1], context)).Return(expressionToVisit.Initializers[0].Arguments[1]).Repeat.Any();
            interceptor.Expect(v => v.Visit(expressionToVisit.Initializers[1].Arguments[0], context)).Return(expressionToVisit.Initializers[1].Arguments[0]).Repeat.Any();
            interceptor.Expect(v => v.Visit(expressionToVisit.Initializers[1].Arguments[1], context)).Return(expressionToVisit.Initializers[1].Arguments[1]).Repeat.Any();

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ListInitExpression>(result);
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreSame(differentExpression.NewExpression, ((ListInitExpression)result).NewExpression);
            int i = 0;
            foreach(ElementInit init in expressionToVisit.Initializers)
            {
                Assert.AreSame(init, ((ListInitExpression)result).Initializers[i]);
                ++i;
            }
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
        }
        
        [Test]
        public void Visit_aListInitExpression_returnsAnotherListInitExpressionWithThe_Initializers_ObtainedFromTheComposition()
        {
            // arrange:
            ListInitExpression[] exprs = Samples.GetTwoDifferentListInitExpressions();
            ListInitExpression expressionToVisit = exprs[0];
            ListInitExpression differentExpression = exprs[1];
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.NewExpression, context)).Return(expressionToVisit.NewExpression).Repeat.Any();
            interceptor.Expect(v => v.Visit(expressionToVisit.Initializers[0].Arguments[0], context)).Return(differentExpression.Initializers[0].Arguments[0]).Repeat.Any();
            interceptor.Expect(v => v.Visit(expressionToVisit.Initializers[0].Arguments[1], context)).Return(differentExpression.Initializers[0].Arguments[1]).Repeat.Any();
            interceptor.Expect(v => v.Visit(expressionToVisit.Initializers[1].Arguments[0], context)).Return(differentExpression.Initializers[1].Arguments[0]).Repeat.Any();
            interceptor.Expect(v => v.Visit(expressionToVisit.Initializers[1].Arguments[1], context)).Return(differentExpression.Initializers[1].Arguments[1]).Repeat.Any();

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ListInitExpression>(result);
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreSame(expressionToVisit.NewExpression, ((ListInitExpression)result).NewExpression);
            int i = 0;
            foreach(ElementInit init in differentExpression.Initializers)
            {
                Assert.AreSame(expressionToVisit.Initializers[i].AddMethod, ((ListInitExpression)result).Initializers[i].AddMethod);
                int j = 0;
                foreach(Expression argument in init.Arguments)
                {
                    Assert.AreSame(argument, ((ListInitExpression)result).Initializers[i].Arguments[j]);
                    ++j;
                }
                ++i;
            }
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
        }
        
        [Test]
        public void Visit_aMemberExpression_askTheCompositionToVisitTheOperands()
        {
            Expression<Func<string, int>> dummy = s => s.Length;
            MemberExpression expressionToVisit = (MemberExpression)dummy.Body;
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Expression, context)).Return(expressionToVisit.Expression).Repeat.Once();

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.AreSame(expressionToVisit, result);
        }
        
        [Test]
        public void Visit_aMemberExpression_returnsAnotherListInitExpressionWithThe_Expression_ObtainedFromTheComposition()
        {
            // arrange:
            Expression<Func<string, int>> dummy = s => s.Length;
            MemberExpression expressionToVisit = (MemberExpression)dummy.Body;
            Expression newExpression = Expression.Constant("test");
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Expression, context)).Return(newExpression).Repeat.Once();

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<MemberExpression>(result);
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreSame(newExpression, ((MemberExpression)result).Expression);
            Assert.AreSame(expressionToVisit.Member, ((MemberExpression)result).Member);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
        }
        
        [Test]
        public void Visit_aMemberInitExpression_askTheCompositionToVisitTheOperands()
        {
            // arrange:
            Expression<Func<string, ClassToTestMemberBindings>> dummy = s => new ClassToTestMemberBindings() { Name = s, Father = { Name = "Father" }, Children = { new ClassToTestMemberBindings(), new ClassToTestMemberBindings() } };
            MemberInitExpression expressionToVisit = (MemberInitExpression)dummy.Body;
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.NewExpression, context)).Return(expressionToVisit.NewExpression).Repeat.Once();
            foreach(MemberBinding binding in expressionToVisit.Bindings)
            {
                switch(binding.BindingType)
                {
                    case MemberBindingType.Assignment:
                        interceptor.Expect(v => v.Visit(((MemberAssignment)binding).Expression, context)).Return(((MemberAssignment)binding).Expression).Repeat.Once();
                    break;
                    case MemberBindingType.ListBinding:
                        foreach(ElementInit init in ((MemberListBinding)binding).Initializers)
                        {
                            foreach(Expression argument in init.Arguments)
                            {
                                interceptor.Expect(v => v.Visit(argument, context)).Return(argument).Repeat.Once();
                            }
                        }
                    break;
                    case MemberBindingType.MemberBinding:
                        MemberMemberBinding memberBinding = (MemberMemberBinding)binding;
                        foreach(MemberAssignment assignment in memberBinding.Bindings) // test only assignment now...
                        {
                            interceptor.Expect(v => v.Visit(assignment.Expression, context)).Return(assignment.Expression).Repeat.Once();
                        }
                    break;
                }
            }

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.AreSame(expressionToVisit, result);
        }
        
        [Test]
        public void Visit_aMemberInitExpression_returnsAnotherMemberInitExpressionWithThe_NewExpression_ObtainedFromTheComposition()
        {
            // arrange:
            Expression<Func<string, ClassToTestMemberBindings>> dummy = s => new ClassToTestMemberBindings() { Name = s, Father = { Name = "Father" }, Children = { new ClassToTestMemberBindings(), new ClassToTestMemberBindings() } };
            Expression<Func<string, ClassToTestMemberBindings>> dummy2 = s => new ClassToTestMemberBindings(s) { Name = "NewName", Father = { Name = "NewFather" }, Children = { new ClassToTestMemberBindings(s + "1"), new ClassToTestMemberBindings(s + "2") } };
            MemberInitExpression expressionToVisit = (MemberInitExpression)dummy.Body;
            MemberInitExpression differentExpression = (MemberInitExpression)dummy2.Body;
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.NewExpression, context)).Return(differentExpression.NewExpression).Repeat.Once();
            foreach(MemberBinding binding in expressionToVisit.Bindings)
            {
                switch(binding.BindingType)
                {
                    case MemberBindingType.Assignment:
                        interceptor.Expect(v => v.Visit(((MemberAssignment)binding).Expression, context)).Return(((MemberAssignment)binding).Expression).Repeat.Once();
                    break;
                    case MemberBindingType.ListBinding:
                        foreach(ElementInit init in ((MemberListBinding)binding).Initializers)
                        {
                            foreach(Expression argument in init.Arguments)
                            {
                                interceptor.Expect(v => v.Visit(argument, context)).Return(argument).Repeat.Once();
                            }
                        }
                    break;
                    case MemberBindingType.MemberBinding:
                        MemberMemberBinding memberBinding = (MemberMemberBinding)binding;
                        foreach(MemberAssignment assignment in memberBinding.Bindings) // test only assignment now...
                        {
                            interceptor.Expect(v => v.Visit(assignment.Expression, context)).Return(assignment.Expression).Repeat.Once();
                        }
                    break;
                }
            }

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<MemberInitExpression>(result);
            MemberInitExpression typedResult = (MemberInitExpression)result;
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreSame(differentExpression.NewExpression, typedResult.NewExpression);
            int b = 0;
            foreach(MemberBinding binding in expressionToVisit.Bindings)
            {
                switch(binding.BindingType)
                {
                    case MemberBindingType.Assignment:
                        Assert.AreSame(((MemberAssignment)binding).Expression, ((MemberAssignment)typedResult.Bindings[b]).Expression);
                    break;
                    case MemberBindingType.ListBinding:
                        int initI = 0;
                        foreach(ElementInit init in ((MemberListBinding)binding).Initializers)
                        {
                            int a = 0;
                            foreach(Expression argument in init.Arguments)
                            {
                                Assert.AreSame(argument, ((MemberListBinding)typedResult.Bindings[b]).Initializers[initI].Arguments[a]);
                                ++a;
                            }
                            ++initI;
                        }
                    break;
                    case MemberBindingType.MemberBinding:
                        MemberMemberBinding memberBinding = (MemberMemberBinding)binding;
                        int mb = 0;
                        foreach(MemberAssignment assignment in memberBinding.Bindings) // test only assignment now...
                        {
                            Assert.AreSame(assignment.Expression, ((MemberAssignment)((MemberMemberBinding)typedResult.Bindings[b]).Bindings[mb]).Expression);
                            ++mb;
                        }
                    break;
                }
                ++b;
            }
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
        }
        
        [Test]
        public void Visit_aMemberInitExpression_returnsAnotherMemberInitExpressionWithThe_Bindings_ObtainedFromTheComposition()
        {
            // arrange:
            Expression<Func<string, ClassToTestMemberBindings>> dummy = s => new ClassToTestMemberBindings() { Name = s, Father = { Name = "Father" }, Children = { new ClassToTestMemberBindings(), new ClassToTestMemberBindings() } };
            Expression<Func<string, ClassToTestMemberBindings>> dummy2 = s => new ClassToTestMemberBindings(s) { Name = "NewName", Father = { Name = "NewFather" }, Children = { new ClassToTestMemberBindings(s + "1"), new ClassToTestMemberBindings(s + "2") } };
            MemberInitExpression expressionToVisit = (MemberInitExpression)dummy.Body;
            MemberInitExpression differentExpression = (MemberInitExpression)dummy2.Body;
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.NewExpression, context)).Return(expressionToVisit.NewExpression).Repeat.Once();
            int b = 0;
            foreach(MemberBinding binding in expressionToVisit.Bindings)
            {
                switch(binding.BindingType)
                {
                    case MemberBindingType.Assignment:
                        MemberAssignment differentAssignent = (MemberAssignment)differentExpression.Bindings[b];
                        interceptor.Expect(v => v.Visit(((MemberAssignment)binding).Expression, context)).Return(differentAssignent.Expression).Repeat.Once();
                    break;
                    case MemberBindingType.ListBinding:
                        int initI = 0;
                        foreach(ElementInit init in ((MemberListBinding)binding).Initializers)
                        {
                            int a = 0;
                            foreach(Expression argument in init.Arguments)
                            {
                                Expression differentArgument = ((MemberListBinding)differentExpression.Bindings[b]).Initializers[initI].Arguments[a];
                                interceptor.Expect(v => v.Visit(argument, context)).Return(differentArgument).Repeat.Once();
                                ++a;
                            }
                            ++initI;
                        }
                    break;
                    case MemberBindingType.MemberBinding:
                        MemberMemberBinding memberBinding = (MemberMemberBinding)binding;
                        int memberAssignmentIndex = 0;
                        foreach(MemberAssignment assignment in memberBinding.Bindings) // test only assignment now...
                        {
                            MemberAssignment differentAssignment = (MemberAssignment)((MemberMemberBinding)differentExpression.Bindings[b]).Bindings[memberAssignmentIndex];
                            interceptor.Expect(v => v.Visit(assignment.Expression, context)).Return(differentAssignment.Expression).Repeat.Once();
                            ++memberAssignmentIndex;
                        }
                    break;
                }
                ++b;
            }

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<MemberInitExpression>(result);
            MemberInitExpression typedResult = (MemberInitExpression)result;
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreSame(expressionToVisit.NewExpression, typedResult.NewExpression);
            b = 0;
            foreach(MemberBinding binding in differentExpression.Bindings)
            {
                switch(binding.BindingType)
                {
                    case MemberBindingType.Assignment:
                        Assert.AreSame(((MemberAssignment)binding).Expression, ((MemberAssignment)typedResult.Bindings[b]).Expression);
                    break;
                    case MemberBindingType.ListBinding:
                        int initI = 0;
                        foreach(ElementInit init in ((MemberListBinding)binding).Initializers)
                        {
                            int a = 0;
                            foreach(Expression argument in init.Arguments)
                            {
                                Assert.AreSame(argument, ((MemberListBinding)typedResult.Bindings[b]).Initializers[initI].Arguments[a]);
                                ++a;
                            }
                            ++initI;
                        }
                    break;
                    case MemberBindingType.MemberBinding:
                        MemberMemberBinding memberBinding = (MemberMemberBinding)binding;
                        int mb = 0;
                        foreach(MemberAssignment assignment in memberBinding.Bindings) // test only assignment now...
                        {
                            Assert.AreSame(assignment.Expression, ((MemberAssignment)((MemberMemberBinding)typedResult.Bindings[b]).Bindings[mb]).Expression);
                            ++mb;
                        }
                    break;
                }
                ++b;
            }
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
        }
  
        [Test]
        public void Visit_aMemberInitExpressionWithAVisitorReplacingTheArgumentType_throwsNotSupportedException()
        {
            // arrange:
            Expression<Func<string, ClassToTestMemberBindings>> dummy = s => new ClassToTestMemberBindings() { Name = s, Father = { Name = "Father" }, Children = { new ClassToTestMemberBindings(), new ClassToTestMemberBindings() } };
            MemberInitExpression expressionToVisit = (MemberInitExpression)dummy.Body;
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.NewExpression, context)).Return(null).Repeat.Once();

            // assert:
            Assert.Throws<NotSupportedException>(delegate {
                Expression result = inspector.Visit(expressionToVisit, context);
            });
        }
        
        [Test]
        public void Visit_aMethodCallExpression_askTheCompositionToVisitTheOperands()
        {
            // arrange:
            Expression<Func<string, string>> dummy = s => s.Substring(1);
            MethodCallExpression expressionToVisit = (MethodCallExpression)dummy.Body;
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Object, context)).Return(expressionToVisit.Object).Repeat.Once();
            foreach(Expression argument in expressionToVisit.Arguments)
            {
                interceptor.Expect(v => v.Visit(argument, context)).Return(argument).Repeat.Once();
            }

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.AreSame(expressionToVisit, result);
        }
        
        [Test]
        public void Visit_aMethodCallExpression_returnsAnotherMethodCallExpressionWithThe_Object_ObtainedFromTheComposition()
        {
            // arrange:
            Expression<Func<string, string>> dummy = s => s.Substring(1);
            Expression<Func<int, string>> dummy2 = i => i.ToString().Substring(3);
            MethodCallExpression expressionToVisit = (MethodCallExpression)dummy.Body;
            MethodCallExpression differentExpression = (MethodCallExpression)dummy2.Body;
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Object, context)).Return(differentExpression.Object).Repeat.Once();
            foreach(Expression argument in expressionToVisit.Arguments)
            {
                interceptor.Expect(v => v.Visit(argument, context)).Return(argument).Repeat.Once();
            }

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<MethodCallExpression>(result);
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreSame(differentExpression.Object, ((MethodCallExpression)result).Object);
            int j = 0;
            foreach(Expression argument in expressionToVisit.Arguments)
            {
                Assert.AreSame(argument, ((MethodCallExpression)result).Arguments[j]);
                ++j;
            }
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
        }
        
        
        [Test]
        public void Visit_aMethodCallExpression_returnsAnotherMethodCallExpressionWithThe_Arguments_ObtainedFromTheComposition()
        {
            // arrange:
            Expression<Func<string, string>> dummy = s => s.Substring(1);
            Expression<Func<int, string>> dummy2 = i => i.ToString().Substring(3);
            MethodCallExpression expressionToVisit = (MethodCallExpression)dummy.Body;
            MethodCallExpression differentExpression = (MethodCallExpression)dummy2.Body;
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Object, context)).Return(expressionToVisit.Object).Repeat.Once();
            int j = 0;
            foreach(Expression argument in expressionToVisit.Arguments)
            {
                interceptor.Expect(v => v.Visit(argument, context)).Return(differentExpression.Arguments[j]).Repeat.Once();
                ++j;
            }

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<MethodCallExpression>(result);
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreSame(expressionToVisit.Object, ((MethodCallExpression)result).Object);
            j = 0;
            foreach(Expression argument in differentExpression.Arguments)
            {
                Assert.AreSame(argument, ((MethodCallExpression)result).Arguments[j]);
                ++j;
            }
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
        }
        
        [Test]
        public void Visit_aNewExpressionWithoutMembers_askTheCompositionToVisitTheOperands()
        {
            // arrange:
            NewExpression expressionToVisit = Samples.GetNewExpressionWithMembers(1, "test");
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            foreach(Expression argument in expressionToVisit.Arguments)
            {
                interceptor.Expect(v => v.Visit(argument, context)).Return(argument).Repeat.Once();
            }

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.AreSame(expressionToVisit, result);
        }
        
        [Test]
        public void Visit_aNewExpressionWithoutMembers_returnsAnotherNewExpressionWithThe_Arguments_ObtainedFromTheComposition()
        {
            // arrange:
            NewExpression expressionToVisit = Samples.GetNewExpressionWithoutMembers(1, "test1");
            NewExpression differentExpression = Samples.GetNewExpressionWithoutMembers(2, "test2");
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            int i = 0;
            foreach(Expression argument in expressionToVisit.Arguments)
            {
                interceptor.Expect(v => v.Visit(argument, context)).Return(differentExpression.Arguments[i]).Repeat.Once();
                ++i;
            }

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NewExpression>(result);
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            i = 0;
            foreach(Expression argument in differentExpression.Arguments)
            {
                Assert.AreSame(argument, ((NewExpression)result).Arguments[i]);
                ++i;
            }
            Assert.AreSame(expressionToVisit.Type, result.Type);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
        }
        
        
        [Test]
        public void Visit_aNewExpressionWithMembers_askTheCompositionToVisitTheOperands()
        {
            // arrange:
            NewExpression expressionToVisit = Samples.GetNewExpressionWithMembers(1, "test");
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            foreach(Expression argument in expressionToVisit.Arguments)
            {
                interceptor.Expect(v => v.Visit(argument, context)).Return(argument).Repeat.Once();
            }

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.AreSame(expressionToVisit, result);
        }

        [Test]
        public void Visit_aNewExpressionWithMembers_returnsAnotherNewExpressionWithThe_Arguments_ObtainedFromTheComposition()
        {
            // arrange:
            NewExpression expressionToVisit = Samples.GetNewExpressionWithMembers(1, "test1");
            NewExpression differentExpression = Samples.GetNewExpressionWithMembers(2, "test2");
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            int i = 0;
            foreach (Expression argument in expressionToVisit.Arguments)
            {
                interceptor.Expect(v => v.Visit(argument, context)).Return(differentExpression.Arguments[i]).Repeat.Once();
                ++i;
            }

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NewExpression>(result);
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            i = 0;
            foreach (Expression argument in differentExpression.Arguments)
            {
                Assert.AreSame(argument, ((NewExpression)result).Arguments[i]);
                ++i;
            }
            i = 0;
            foreach (MemberInfo member in expressionToVisit.Members)
            {
                Assert.AreSame(member, ((NewExpression)result).Members[i]);
                ++i;
            }
            Assert.AreSame(expressionToVisit.Type, result.Type);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
        }
        
        [Test]
        public void Visit_aNewExpressionWithMembersOfDifferentTypes_returnsAnotherNewExpressionWithThe_Arguments_ObtainedFromTheComposition()
        {
            // arrange:
            NewExpression expressionToVisit = Samples.GetNewExpressionWithMembers<int, object>(1, new object());
            NewExpression differentExpression = Samples.GetNewExpressionWithMembers<int, string>(2, "test2");
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            int i = 0;
            foreach (Expression argument in expressionToVisit.Arguments)
            {
                interceptor.Expect(v => v.Visit(argument, context)).Return(differentExpression.Arguments[i]).Repeat.Once();
                ++i;
            }

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NewExpression>(result);
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            i = 0;
            foreach (Expression argument in differentExpression.Arguments)
            {
                if(((NewExpression)result).Arguments[i].NodeType == ExpressionType.Convert)
                {
                    Assert.AreSame(argument, ((UnaryExpression)((NewExpression)result).Arguments[i]).Operand);
                }
                else
                {
                    Assert.AreSame(argument, ((NewExpression)result).Arguments[i]);
                }
                ++i;
            }
            i = 0;
            foreach (MemberInfo member in expressionToVisit.Members)
            {
                Assert.AreSame(member, ((NewExpression)result).Members[i]);
                ++i;
            }
            Assert.AreSame(expressionToVisit.Type, result.Type);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
        }
        
        [Test]
        public void Visit_aTypeBinaryExpression_askTheCompositionToVisitTheOperands()
        {
            // arrange:
            TypeBinaryExpression expressionToVisit = Samples.GetNewTypeBinaryExpression("test");
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Expression, context)).Return(expressionToVisit.Expression).Repeat.Once();

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.AreSame(expressionToVisit, result);
        }
        
        [Test]
        public void Visit_aTypeBinaryExpression_returnsAnotherTypeBinaryExpressionWithThe_Expression_ObtainedFromTheComposition()
        {
            // arrange:
            TypeBinaryExpression expressionToVisit = Samples.GetNewTypeBinaryExpression("test");
            Expression newOperand = new UnknownExpression();
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Expression, context)).Return(newOperand).Repeat.Once();

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<TypeBinaryExpression>(result);
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreSame(expressionToVisit.TypeOperand, ((TypeBinaryExpression)result).TypeOperand);
            Assert.AreSame(expressionToVisit.Type, result.Type);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
        }
        
        [Test]
        public void Visit_aNewArrayExpression_askTheCompositionToVisitTheOperands()
        {
            // arrange:
            Expression<Func<object[]>> dummy = () => new object[] { new object(), "test" };
            NewArrayExpression expressionToVisit = (NewArrayExpression)dummy.Body;
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            foreach(Expression argument in expressionToVisit.Expressions)
            {
                interceptor.Expect(v => v.Visit(argument, context)).Return(argument).Repeat.Once();
            }

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.AreSame(expressionToVisit, result);
        }
        
        [Test]
        public void Visit_aNewArrayExpression_returnsAnotherNewArrayExpressionWithThe_Expressions_ObtainedFromTheComposition()
        {
            // arrange:
            Expression<Func<object[]>> dummy = () => new object[] { new object(), "test" };
            Expression<Func<object[]>> dummy2 = () => new object[] { new ClassWithFieldAndProperty(), "test2" };
            NewArrayExpression expressionToVisit = (NewArrayExpression)dummy.Body;
            NewArrayExpression differentExpression = (NewArrayExpression)dummy2.Body;
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            int i = 0;
            foreach(Expression argument in expressionToVisit.Expressions)
            {
                interceptor.Expect(v => v.Visit(argument, context)).Return(differentExpression.Expressions[i]).Repeat.Once();
                ++i;
            }

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NewArrayExpression>(result);
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            i = 0;
            foreach(Expression argument in differentExpression.Expressions)
            {
                Assert.AreSame(argument, ((NewArrayExpression)result).Expressions[i]);
                ++i;
            }
            Assert.AreSame(expressionToVisit.Type, result.Type);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
        }
        
        [Test]
        public void Visit_aNewArrayExpressionWithArrayBounds_askTheCompositionToVisitTheOperands()
        {
            // arrange:
            Expression<Func<object>> dummy = () => new string[1,2];
            NewArrayExpression expressionToVisit = (NewArrayExpression)dummy.Body;
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            foreach(Expression argument in expressionToVisit.Expressions)
            {
                interceptor.Expect(v => v.Visit(argument, context)).Return(argument).Repeat.Once();
            }

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.AreSame(expressionToVisit, result);
        }
        
        [Test]
        public void Visit_aNewArrayExpressionWithArrayBounds_returnsAnotherNewArrayExpressionWithThe_Expressions_ObtainedFromTheComposition()
        {
            // arrange:
            Expression<Func<object>> dummy = () => new string[1,2];
            Expression<Func<object>> dummy2 = () => new string[3,4];
            NewArrayExpression expressionToVisit = (NewArrayExpression)dummy.Body;
            NewArrayExpression differentExpression = (NewArrayExpression)dummy2.Body;
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            int i = 0;
            foreach(Expression argument in expressionToVisit.Expressions)
            {
                interceptor.Expect(v => v.Visit(argument, context)).Return(differentExpression.Expressions[i]).Repeat.Once();
                ++i;
            }

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NewArrayExpression>(result);
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            i = 0;
            foreach(Expression argument in differentExpression.Expressions)
            {
                Assert.AreSame(argument, ((NewArrayExpression)result).Expressions[i]);
                ++i;
            }
            Assert.AreSame(expressionToVisit.Type, result.Type);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
        }
    }
}

