//
//  QueryableConstantResolverQA.cs
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
using NUnit.Framework;
using System.Linq.Expressions;
using Rhino.Mocks;
using Epic.Query.Linq.Fakes;
using Challenge00.DDDSample.Cargo;
using System.Collections.Generic;
using Epic.Environment;
using Epic.Fakes;

namespace Epic.Query.Linq.Expressions.Normalization
{
    [TestFixture]
    public class QueryableConstantResolverQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutComposition_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate
            {
                new QueryableConstantResolver(null);
            });
        }

        [Test]
        public void Visit_withoutTheCurrentIQueryProviderInTheContext_dontCatchTheInvalidOperationException()
        {
            // arrange:
            InvalidOperationException exceptionToThrow = new InvalidOperationException();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            context.Expect(c => c.Get<IQueryProvider>()).Throw(exceptionToThrow).Repeat.Once();
            IQueryable queryable = GenerateStrictMock<IQueryable>();
            ConstantExpression constant = Expression.Constant(queryable);
            FakeNormalizer normalizer = new FakeNormalizer();
            QueryableConstantResolver visitor = new QueryableConstantResolver(normalizer);

            // assert:
            Assert.That(delegate { visitor.Visit(constant, context); }, Throws.Exception.SameAs(exceptionToThrow));
        }

        [Test]
        public void Visit_aQueryProducedByADifferentQueryProvider_returnsANewConstantContainingTheResultOfTheExecution()
        {
            // arrange:
            IEnumerable<ICargo> executionResult = Enumerable.Empty<ICargo>();
            IQueryProvider currentQueryProvider = GenerateStrictMock<IQueryProvider>();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            context.Expect(c => c.Get<IQueryProvider>()).Return(currentQueryProvider).Repeat.Once();
            DummyResultExpression dummyExpression = new DummyResultExpression();
            IQueryProvider anotherQueryProvider = GenerateStrictMock<IQueryProvider>();
            anotherQueryProvider.Expect(q => q.Execute(dummyExpression)).Return(executionResult).Repeat.Once();
            IQueryable<ICargo> queryable = GenerateStrictMock<IQueryable<ICargo>>();
            queryable.Expect(q => q.Expression).Return(dummyExpression).Repeat.Once();
            queryable.Expect(q => q.Provider).Return(anotherQueryProvider).Repeat.Once();
            queryable.Expect(q => q.ElementType).Return(typeof(ICargo)).Repeat.Once();
            ConstantExpression expressionToVisit = Expression.Constant(queryable);
            FakeNormalizer normalizer = new FakeNormalizer();
            new QueryableConstantResolver(normalizer);

            // act:
            Expression result = normalizer.Visit(expressionToVisit, context);

            // assert:
            Verify.That(result).IsA<ConstantExpression>()
                .WithA(c => c.Value, that => Is.SameAs(executionResult));
        }

        [Test]
        public void Visit_aRepositoryInAConstantExpression_returnTheConstantItself()
        {
            // arrange:
            IQueryProvider currentQueryProvider = GenerateStrictMock<IQueryProvider>();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            context.Expect(c => c.Get<IQueryProvider>()).Return(currentQueryProvider).Repeat.Once();
            IQueryable<ICargo> queryable = GenerateStrictMock<IQueryable<ICargo>>();
            queryable.Expect(q => q.Provider).Return(currentQueryProvider).Repeat.Once();
            queryable.Expect(q => q.Expression).Return(Expression.Constant(queryable)).Repeat.Once();
            ConstantExpression expressionToVisit = Expression.Constant(queryable);
            FakeNormalizer normalizer = new FakeNormalizer();
            new QueryableConstantResolver(normalizer);

            // act:
            Expression result = normalizer.Visit(expressionToVisit, context);

            // assert:
            Assert.AreSame(expressionToVisit, result);
        }

        [Test]
        public void Visit_aQueryableThatCanBeReplacedWithItsExpression_returnsTheExpression()
        {
            IQueryProvider currentQueryProvider = GenerateStrictMock<IQueryProvider>();
            Expression<Func<IQueryable<ICargo>>> dummyExpression = () => Enumerable.Empty<ICargo>().AsQueryable();
            Expression expectedResult = new DummyResultExpression();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            context.Expect(c => c.Get<IQueryProvider>()).Return(currentQueryProvider).Repeat.Once();
            IQueryable<ICargo> queryable = GenerateStrictMock<IQueryable<ICargo>>();
            queryable.Expect(q => q.Provider).Return(currentQueryProvider).Repeat.Once();
            queryable.Expect(q => q.Expression).Return(dummyExpression).Repeat.Once();
            ConstantExpression expressionToVisit = Expression.Constant(queryable);
            FakeNormalizer normalizer = new FakeNormalizer();
            new QueryableConstantResolver(normalizer);
            FakeVisitor<Expression, LambdaExpression> mockVisitor = GeneratePartialMock<FakeVisitor<Expression, LambdaExpression>>(normalizer);
            mockVisitor.Expect(v => v.CallToVisitor(dummyExpression)).Return(mockVisitor).Repeat.Once();
            mockVisitor.Expect(v => v.Visit(dummyExpression, context)).Return(expectedResult).Repeat.Once();

            // act:
            Expression result = normalizer.Visit(expressionToVisit, context);

            // assert:
            Assert.AreSame(expectedResult, result);
        }
    }
}
