//  
//  ExpressionsInspectorQA.cs
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
using Epic.Query.Linq.Fakes;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Reflection;

namespace Epic.Query.Linq.Expressions.Normalization
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
            ExpressionsInspector inspector = new ExpressionsInspector(composition);
            CompositeVisitor<Expression>.VisitorBase mockable = GeneratePartialMock<MockableVisitor<Expression>>(composition);
            mockableInterceptor = mockable as IVisitor<Expression, Expression>;
            return inspector;
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
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
            Verify.That(result).IsA<UnaryExpression>()
                .WithA(unary => unary.Operand, that => Is.SameAs(differentExpression.Operand));
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

            // act:
            Expression result = inspector.Visit(expressionToVisit, context);

            // assert:
            Assert.IsNotNull(result);
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
            Verify.That(result).IsA<BinaryExpression>()
                .WithA(e => e.Left, that => Is.SameAs(differentExpression.Left))
                .WithA(e => e.Right, that => Is.SameAs(differentExpression.Right))
                .WithA(e => e.Conversion, that => Is.SameAs(differentExpression.Conversion));
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
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
            Verify.That(result).IsA<ConditionalExpression>()
                .WithA(e => e.Test, that => Is.SameAs(differentExpression.Test))
                .WithA(e => e.IfTrue, that => Is.SameAs(expressionToVisit.IfTrue))
                .WithA(e => e.IfFalse, that => Is.SameAs(expressionToVisit.IfFalse));
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
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
            Verify.That(result).IsA<ConditionalExpression>()
                .WithA(e => e.Test, that => Is.SameAs(expressionToVisit.Test))
                .WithA(e => e.IfTrue, that => Is.SameAs(differentExpression.IfTrue))
                .WithA(e => e.IfFalse, that => Is.SameAs(expressionToVisit.IfFalse));
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
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
            Verify.That(result).IsA<ConditionalExpression>()
                .WithA(e => e.Test, that => Is.SameAs(expressionToVisit.Test))
                .WithA(e => e.IfTrue, that => Is.SameAs(expressionToVisit.IfTrue))
                .WithA(e => e.IfFalse, that => Is.SameAs(differentExpression.IfFalse));
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
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
            Verify.That(result).IsA<InvocationExpression>()
                .WithA(e => e.Expression, that => Is.SameAs(differentExpression.Expression))
                .WithEach(e => e.Arguments, (that, atIndex) => Is.SameAs(expressionToVisit.Arguments[atIndex]));
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
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
            Verify.That(result).IsA<InvocationExpression>()
                .WithA(e => e.Expression, that => Is.SameAs(expressionToVisit.Expression))
                .WithEach(e => e.Arguments, (that, atIndex) => Is.SameAs(differentExpression.Arguments[atIndex]))
                .WithA(e => e.Arguments[1], that => Is.SameAs(expressionToVisit.Arguments[1]));
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
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
            Verify.That(result).IsA<LambdaExpression>()
                .WithA(e => e.Body, that => Is.SameAs(differentExpression.Body))
                .WithEach(e => e.Parameters, (that, atIndex) => Is.SameAs(expressionToVisit.Parameters[atIndex]));
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
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
            Verify.That(result).IsA<LambdaExpression>()
                .WithA(e => e.Body, that => Is.SameAs(expressionToVisit.Body))
                .WithEach(e => e.Parameters, (that, atIndex) => Is.SameAs(differentExpression.Parameters[atIndex]));
        }
        
        [Test]
        public void Visit_aLambdaExpressionWithAVisitorReplacingTheArgumentType_throwsInvalidOperationException()
        {
            Expression<Func<int, int, int>> dummy = (i, j) => i > j ? 1 : 2;
            LambdaExpression expressionToVisit = dummy;
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.Body, context)).Return(expressionToVisit.Body).Repeat.Any();
            interceptor.Expect(v => v.Visit(expressionToVisit.Parameters[0], context)).Return(dummy.Parameters[0]).Repeat.Any();
            interceptor.Expect(v => v.Visit(expressionToVisit.Parameters[1], context)).Return(Expression.Constant(1)).Repeat.Once();

            // assert:
            Assert.Throws<InvalidOperationException>(delegate {
                inspector.Visit(expressionToVisit, context);
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
                inspector.Visit(expressionToVisit, context);
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
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
            Verify.That(result).IsA<ListInitExpression>()
                .WithA(e => e.NewExpression, that => Is.SameAs(differentExpression.NewExpression))
                .WithEach(e => e.Initializers, (that, atIndex) => Is.SameAs(expressionToVisit.Initializers[atIndex]));
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
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
            Verify.That(result).IsA<ListInitExpression>()
                .WithA(e => e.NewExpression, that => Is.SameAs(expressionToVisit.NewExpression))
                .WithEach(e => e.Initializers, (that, atIndex) => Assert.AreSame(expressionToVisit.Initializers[atIndex].AddMethod, that.AddMethod))
                .WithEach(e => e.Initializers, (initializer, initializerIndex) =>
                    Verify.That(initializer).IsA<ElementInit>()
                            .WithEach(e2 => e2.Arguments, (argument, argumentIndex) => Is.SameAs(differentExpression.Initializers[initializerIndex].Arguments[argumentIndex]))
                        );
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
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
            Verify.That(result).IsA<MemberExpression>()
                .WithA(e => e.Expression, that => Is.SameAs(newExpression))
                .WithA(e => e.Member, that => Is.SameAs(expressionToVisit.Member));
        }
        
        [Test]
        public void Visit_aMemberInitExpression_askTheCompositionToVisitTheOperands()
        {
            // arrange:
            MemberInitExpression expressionToVisit = Samples.GetNewMemberInitExpression();
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
            MemberInitExpression[] exps = Samples.GetTwoDifferentMemberInitExpression();
            MemberInitExpression expressionToVisit = exps[0];
            MemberInitExpression differentExpression = exps[1];
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
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
            Verify.That(result).IsA<MemberInitExpression>()
                .WithA(e => e.NewExpression, that => Is.SameAs(differentExpression.NewExpression))
                .WithEach(e => e.Bindings, 
                          when => when.BindingType == MemberBindingType.Assignment, 
                          (binding, bindingIndex) => Verify.That(binding).IsA<MemberAssignment>()
                                                           .WithA(assignment => assignment.Expression, 
                                                                  that => Is.SameAs(((MemberAssignment)expressionToVisit.Bindings[bindingIndex]).Expression)))
                .WithEach(e => e.Bindings, 
                          when => when.BindingType == MemberBindingType.ListBinding, 
                          (binding, bindingIndex) => Verify.That(binding).IsA<MemberListBinding>()
                                                           .WithEach(list => list.Initializers, 
                                                                     (initializer, intializerIndex) => 
                                                                          Verify.That(initializer).WithEach(i => i.Arguments, 
                                                                                      (argument, a) => Is.SameAs(((MemberListBinding)expressionToVisit.Bindings[bindingIndex]).Initializers[intializerIndex].Arguments[a]) )))
                .WithEach(e => e.Bindings,
                          when => when.BindingType == MemberBindingType.MemberBinding,
                          (binding, bindingIndex) => Verify.That(binding).IsA<MemberMemberBinding>()
                              .WithEach(e2 => e2.Bindings, (assignment, a1) => 
                                  Verify.That(assignment).IsA<MemberAssignment>()
                                    .WithA(a => a.Expression, 
                                           that => Is.SameAs(((MemberAssignment)((MemberMemberBinding)expressionToVisit.Bindings[bindingIndex]).Bindings[a1]).Expression))
                                 ));
        }
        
        [Test]
        public void Visit_aMemberInitExpression_returnsAnotherMemberInitExpressionWithThe_Bindings_ObtainedFromTheComposition()
        {
            // arrange:
            MemberInitExpression[] exps = Samples.GetTwoDifferentMemberInitExpression();
            MemberInitExpression expressionToVisit = exps[0];
            MemberInitExpression differentExpression = exps[1];
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
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
            Verify.That(result).IsA<MemberInitExpression>()
                .WithA(e => e.NewExpression, that => Is.SameAs(expressionToVisit.NewExpression))
                .WithEach(e => e.Bindings,
                          when => when.BindingType == MemberBindingType.Assignment,
                          (binding, bindingIndex) => Verify.That(binding).IsA<MemberAssignment>()
                                                           .WithA(assignment => assignment.Expression,
                                                                  that => Is.SameAs(((MemberAssignment)differentExpression.Bindings[bindingIndex]).Expression)))
                .WithEach(e => e.Bindings,
                          when => when.BindingType == MemberBindingType.ListBinding,
                          (binding, bindingIndex) => Verify.That(binding).IsA<MemberListBinding>()
                                                           .WithEach(list => list.Initializers,
                                                                     (initializer, intializerIndex) =>
                                                                          Verify.That(initializer).WithEach(i => i.Arguments,
                                                                                      (argument, a) => Is.SameAs(((MemberListBinding)differentExpression.Bindings[bindingIndex]).Initializers[intializerIndex].Arguments[a]))))
                .WithEach(e => e.Bindings,
                          when => when.BindingType == MemberBindingType.MemberBinding,
                          (binding, bindingIndex) => Verify.That(binding).IsA<MemberMemberBinding>()
                              .WithEach(e2 => e2.Bindings, (assignment, a1) =>
                                  Verify.That(assignment).IsA<MemberAssignment>()
                                    .WithA(a => a.Expression,
                                           that => Is.SameAs(((MemberAssignment)((MemberMemberBinding)differentExpression.Bindings[bindingIndex]).Bindings[a1]).Expression))
                                 ));
        }
  
        [Test]
        public void Visit_aMemberInitExpressionWithAVisitorReplacingTheArgumentType_throwsNotSupportedException()
        {
            // arrange:
            MemberInitExpression expressionToVisit = Samples.GetNewMemberInitExpression();
            IVisitor<Expression, Expression> interceptor = null;
            IVisitContext context = VisitContext.New;
            ExpressionsInspector inspector = BuildCompositionWithMockableInterceptor(out interceptor);
            interceptor.Expect(v => v.Visit(expressionToVisit.NewExpression, context)).Return(null).Repeat.Once();

            // assert:
            Assert.Throws<NotSupportedException>(delegate {
                inspector.Visit(expressionToVisit, context);
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
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
            Verify.That(result).IsA<MethodCallExpression>()
                .WithA(e => e.Object, that => Is.SameAs(differentExpression.Object))
                .WithEach(e => e.Arguments, (that, atIndex) => Is.SameAs(expressionToVisit.Arguments[atIndex]));
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
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
            Verify.That(result).IsA<MethodCallExpression>()
                .WithA(e => e.Object, that => Is.SameAs(expressionToVisit.Object))
                .WithEach(e => e.Arguments, (that, atIndex) => Is.SameAs(differentExpression.Arguments[atIndex]));
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
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreSame(expressionToVisit.Type, result.Type);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
            Verify.That(result).IsA<NewExpression>()
                .WithEach(e => e.Arguments, (that, atIndex) => Is.SameAs(differentExpression.Arguments[atIndex]));
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
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreSame(expressionToVisit.Type, result.Type);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);

            Verify.That(result).IsA<NewExpression>()
                .WithEach(e => e.Arguments, (that, atIndex) => Is.SameAs(differentExpression.Arguments[atIndex]))
                .WithEach(e => e.Members, (that, atIndex) => Is.SameAs(expressionToVisit.Members[atIndex]));
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
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreSame(expressionToVisit.Type, result.Type);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
            Verify.That(result).IsA<NewExpression>()
                .WithEach(e => e.Members, (that, atIndex) => Is.SameAs(expressionToVisit.Members[atIndex]))
                .WithEach(e => e.Arguments, when => when.NodeType == ExpressionType.Convert,
                          (argument, atIndex) => Verify.That(argument).IsA<UnaryExpression>()
                                                       .WithA(u => u.Operand, that => Is.SameAs(differentExpression.Arguments[atIndex])))
                .WithEach(e => e.Arguments, when => when.NodeType != ExpressionType.Convert,
                          (argument, atIndex) => Is.SameAs(differentExpression.Arguments[atIndex]));
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
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreSame(expressionToVisit.Type, result.Type);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
            Verify.That(result).IsA<TypeBinaryExpression>()
                .WithA(e => e.TypeOperand, that => Is.SameAs(expressionToVisit.TypeOperand))
                .WithA(e => e.Expression, that => Is.SameAs(newOperand));
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
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreSame(expressionToVisit.Type, result.Type);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
            Verify.That(result).IsA<NewArrayExpression>()
                .WithEach(e => e.Expressions, (that, atIndex) => Is.SameAs(differentExpression.Expressions[atIndex]));
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
            Assert.AreNotSame(expressionToVisit, result);
            Assert.AreNotSame(differentExpression, result);
            Assert.AreSame(expressionToVisit.Type, result.Type);
            Assert.AreEqual(expressionToVisit.NodeType, result.NodeType);
            Verify.That(result).IsA<NewArrayExpression>()
                .WithEach(e => e.Expressions, (that, atIndex) => Is.SameAs(differentExpression.Expressions[atIndex]));
        }
    }
}

