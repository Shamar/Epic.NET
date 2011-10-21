//  
//  ExpressionForwarderQA.cs
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
using System.Collections.Generic;
using Rhino.Mocks;
using Epic.Linq.Fakes;

namespace Epic.Linq.Expressions.Normalization
{
    public interface IDerivedExpressionsVisitor : IVisitor<Expression, UnaryExpression>, 
        IVisitor<Expression, BinaryExpression>, 
        IVisitor<Expression, ConditionalExpression>,
        IVisitor<Expression, ConstantExpression>,
        IVisitor<Expression, InvocationExpression>,
        IVisitor<Expression, LambdaExpression>,
        IVisitor<Expression, MemberExpression>,
        IVisitor<Expression, MethodCallExpression>,
        IVisitor<Expression, NewExpression>,
        IVisitor<Expression, NewArrayExpression>,
        IVisitor<Expression, MemberInitExpression>,
        IVisitor<Expression, ListInitExpression>,
        IVisitor<Expression, ParameterExpression>,
        IVisitor<Expression, TypeBinaryExpression>
    {
    }
    
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
        
        [Test, TestCaseSource("UnaryExpressions")]
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
  
        [Test, TestCaseSource("BinaryExpressions")]
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

        
        #region data sources
        
        public static IEnumerable<Expression> BinaryExpressions
        {
            get
            {
                yield return Expression.Add(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.AddChecked(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.Divide(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.Modulo(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.Multiply(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.MultiplyChecked(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.Power(Expression.Constant(2.0), Expression.Constant(2.0));
                yield return Expression.Subtract(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.SubtractChecked(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.And(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.Or(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.ExclusiveOr(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.LeftShift(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.RightShift(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.AndAlso(Expression.Constant(true), Expression.Constant(false));
                yield return Expression.OrElse(Expression.Constant(true), Expression.Constant(false));
                yield return Expression.Equal(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.NotEqual(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.GreaterThanOrEqual(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.GreaterThan(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.LessThan(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.LessThanOrEqual(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.Coalesce(Expression.Parameter(typeof(string), "p"), Expression.Constant("test"));
                yield return Expression.ArrayIndex(Expression.Constant(new int[1]), Expression.Constant(0));
            }
        }
        
        public static IEnumerable<Expression> UnaryExpressions
        {
            get
            {
                yield return Expression.ArrayLength(Expression.Constant(new int[0])); 
                yield return Expression.Convert(Expression.Constant(1), typeof(uint));
                yield return Expression.ConvertChecked(Expression.Constant(1), typeof(uint));
                yield return Expression.Negate(Expression.Constant(1));
                yield return Expression.NegateChecked(Expression.Constant(1));
                yield return Expression.Not(Expression.Constant(true));
                Expression<Func<int, bool>> toQuote = i => i > 0;
                yield return Expression.Quote(toQuote);
                yield return Expression.TypeAs(Expression.Constant(new object()), typeof(string));
                yield return Expression.UnaryPlus(Expression.Constant(1));
            }
        }
        
        #endregion data sources
    }
}

