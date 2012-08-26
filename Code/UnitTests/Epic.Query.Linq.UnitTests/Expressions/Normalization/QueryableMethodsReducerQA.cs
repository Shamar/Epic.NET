//  
//  QueryableMethodsReducerQA.cs
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
using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using Epic.Fakes;
using Epic.Query.Linq.Fakes;
using Challenge00.DDDSample.Cargo;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Reflection;

namespace Epic.Query.Linq.Expressions.Normalization
{
    [TestFixture]
    public class QueryableMethodsReducerQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutComposition_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new QueryableMethodsReducer(null);
            });
        }

        private void RegisterEchoVisitorFor<TArgument>(CompositeVisitor<Expression> composition, IVisitContext context, MethodCallExpression callExression, int argumentIndex)
            where TArgument : Expression
        {
            TArgument argumentToReturn = (TArgument)callExression.Arguments[argumentIndex];
            new EchoVisitor<TArgument>(composition, argumentToReturn);
        }

        [Test, TestCaseSource(typeof(Samples), "ReduceableQueryableMethodCallExpressions")]
        public void Visit_aQueryableMethodOverAnExecutedQueryable_returnsAConstantExpressionContainingTheResultingEnumerable(MethodCallExpression expressionToVisit, IEnumerable<string> originalEnumerable, IEnumerable<string> finalEnumerable)
        {
            // arrange:
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            FakeNormalizer composition = new FakeNormalizer();
            new QueryableMethodsReducer(composition);
            FakeVisitor<Expression, MethodCallExpression> mockVisitor = GeneratePartialMock<FakeVisitor<Expression, MethodCallExpression>>(composition);
            //mockVisitor.Expect(v => v.CallToVisitor((MethodCallExpression)expressionToVisit.Arguments[0])).Return(mockVisitor).Repeat.Once();
            mockVisitor.Expect(v => v.Visit((MethodCallExpression)expressionToVisit.Arguments[0], context)).Return(Expression.Constant(originalEnumerable)).Repeat.Once();
            mockVisitor.Expect(v => v.CallToVisitor<MethodCallExpression>(expressionToVisit)).Return(null).Repeat.Any();
            mockVisitor.Expect(v => v.CallToVisitor<Expression>(null)).IgnoreArguments().Return(null).Repeat.Any();
            for(int i = 1; i < expressionToVisit.Arguments.Count; ++i)
            {
                switch(expressionToVisit.Arguments[i].NodeType)
                {
                    case ExpressionType.Quote:
                        RegisterEchoVisitorFor<UnaryExpression>(composition,context,expressionToVisit,i);
                        break;
                    case ExpressionType.Constant:
                        RegisterEchoVisitorFor<ConstantExpression>(composition,context,expressionToVisit,i);
                    break;
                    case ExpressionType.MemberAccess:
                        RegisterEchoVisitorFor<MemberExpression>(composition, context, expressionToVisit, i);
                    break;
                }
            }

            // act:
            Expression result = composition.Visit(expressionToVisit, context);

            // assert:
            Verify.That(result).IsA<ConstantExpression>()
                  .WithA(e => e.Value, 
                         value => Verify.That(value).IsA<IEnumerable<string>>()
                                        .WithA(e => e.Count(), that => Is.EqualTo(finalEnumerable.Count()))
                                        .WithEach<string>(e => e, (c, i) => Assert.AreSame(finalEnumerable.ElementAt(i), c)));
        }

        [Test, TestCaseSource(typeof(Samples), "NotReduceableQueryableMethodCallExpressions")]
        public void Visit_aQueryableMethodOverAnExecutedQueryableThatCanNotBeReduced_returnsAMethodCallExpression(MethodCallExpression expressionToVisit, MethodInfo expectedEnumerableMethod)
        {
            // arrange:
            IEnumerable<string> enumerableToReturn = Enumerable.Empty<string>();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            FakeNormalizer composition = new FakeNormalizer();
            new QueryableMethodsReducer(composition);
            FakeVisitor<Expression, MemberExpression> mockVisitor = GeneratePartialMock<FakeVisitor<Expression, MemberExpression>>(composition);
            mockVisitor.Expect(v => v.CallToVisitor((MemberExpression)expressionToVisit.Arguments[0])).Return(mockVisitor).Repeat.Once();
            mockVisitor.Expect(v => v.Visit((MemberExpression)expressionToVisit.Arguments[0], context)).Return(Expression.Constant(enumerableToReturn)).Repeat.Once();
            mockVisitor.Expect(v => v.CallToVisitor<Expression>(null)).IgnoreArguments().Return(null).Repeat.Any();
            for (int i = 1; i < expressionToVisit.Arguments.Count; ++i)
            {
                switch (expressionToVisit.Arguments[i].NodeType)
                {
                    case ExpressionType.Parameter:
                        RegisterEchoVisitorFor<ParameterExpression>(composition, context, expressionToVisit, i);
                        break;
                    case ExpressionType.Call:
                        RegisterEchoVisitorFor<MethodCallExpression>(composition, context, expressionToVisit, i);
                        break;
                    case ExpressionType.Quote:
                        RegisterEchoVisitorFor<UnaryExpression>(composition, context, expressionToVisit, i);
                        break;
                    case ExpressionType.Constant:
                        RegisterEchoVisitorFor<ConstantExpression>(composition, context, expressionToVisit, i);
                        break;
                    case ExpressionType.MemberAccess:
                        RegisterEchoVisitorFor<MemberExpression>(composition, context, expressionToVisit, i);
                        break;
                    default:
                        Assert.Fail("TEST TO FIX: Unexpected expression of nodeType {0}, as the argument at {1} in the expression: {2}.", expressionToVisit.Arguments[i].NodeType, i, expressionToVisit.ToString());
                        break;
                }
            }

            // act:
            Expression result = composition.Visit(expressionToVisit, context);

            // assert:
            Verify.That(result).IsA<MethodCallExpression>()
                .WithA(e => e.Method, that => Is.SameAs(expectedEnumerableMethod))
                .WithA(e => e.Arguments[0], that => Verify.That(that).IsA<ConstantExpression>().WithA(e => e.Value, value => Is.SameAs(enumerableToReturn)));
        }

        [Test, TestCaseSource(typeof(Samples), "ReduceableQueryableMethodCallExpressions")]
        public void Visit_aQueryableMethodOverAnNotExecutedQueryable_returnsAMethodCallToTheSameMethod(MethodCallExpression expressionToVisit, object dummy1, object dummy2)
        {
            // arrange:
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            FakeNormalizer composition = new FakeNormalizer();
            new QueryableMethodsReducer(composition);
            for (int i = 0; i < expressionToVisit.Arguments.Count; ++i)
            {
                switch (expressionToVisit.Arguments[i].NodeType)
                {
                    case ExpressionType.Parameter:
                        RegisterEchoVisitorFor<ParameterExpression>(composition, context, expressionToVisit, i);
                        break;
                    case ExpressionType.Call:
                        RegisterEchoVisitorFor<MethodCallExpression>(composition, context, expressionToVisit, i);
                        break;
                    case ExpressionType.Quote:
                        RegisterEchoVisitorFor<UnaryExpression>(composition, context, expressionToVisit, i);
                        break;
                    case ExpressionType.Constant:
                        RegisterEchoVisitorFor<ConstantExpression>(composition, context, expressionToVisit, i);
                        break;
                    case ExpressionType.MemberAccess:
                        RegisterEchoVisitorFor<MemberExpression>(composition, context, expressionToVisit, i);
                        break;
                    default:
                        Assert.Fail("TEST TO FIX: Unexpected expression of nodeType {0}, as the argument at {1} in the expression: {2}.", expressionToVisit.Arguments[i].NodeType, i, expressionToVisit.ToString());
                        break;
                }
            }

            // act:
            Expression result = composition.Visit(expressionToVisit, context);

            // assert:
            Verify.That(result).IsA<MethodCallExpression>()
                .WithA(e => e.Method, that => Is.SameAs(expressionToVisit.Method))
                .WithEach(e => e.Arguments, (that, atIndex) => { 
                    Assert.AreSame(that, expressionToVisit.Arguments[atIndex]); 
                });
        }
    }
}

