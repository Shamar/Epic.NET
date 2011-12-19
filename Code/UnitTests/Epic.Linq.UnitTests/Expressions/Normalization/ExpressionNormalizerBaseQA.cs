//  
//  DefaultNormalizerQA.cs
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
using System.Linq.Expressions;
using Epic.Linq.Fakes;
using System.Collections.Generic;
using System.Linq;

namespace Epic.Linq.Expressions.Normalization
{
    [TestFixture]
    public class ExpressionNormalizerBaseQA
    {
        [Test]
        public void Initalize_withoutAName_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new FakeNormalizer(null);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                new FakeNormalizer(string.Empty);
            });
        }
        
        [TestCase(0)]
        [TestCase(1)]
        [TestCase("testA")]
        [TestCase("testB")]
        public void Visit_aConstantValue_returnTheConstantItself<T>(T val)
        {
            // arrange:
            ConstantExpression constant = Expression.Constant(val);
            IVisitor<Expression, Expression> visitor = new FakeNormalizer("TEST");

            // act:
            Expression result = visitor.Visit(constant, VisitContext.New);

            // assert:
            Assert.AreSame(constant, result);
        }

        private static Expression GetExpression<T>(IQueryable<T> queryable)
        {
            return queryable.Expression;
        }

        public static IEnumerable<TestCaseData> ComplexExpressions
        {
            get
            {
                IEnumerable<string> originalStrings = new string[] {
                    "test-A.1", "test-B.1", "test-B.2",
                    "sample-A.2", "sample-B.1", "sample-C.3"
                };
                IQueryable<string> queryableString = originalStrings.AsQueryable().Where(s => true); // this where simulate a deeper tree

                yield return new TestCaseData(
                    GetExpression(queryableString.Where(s => s.StartsWith("test")).OrderBy(s => s.ToCharArray()[s.Length - 1]))
                    );
                yield return new TestCaseData(
                    GetExpression(queryableString.Where(s => s.StartsWith("test")).SelectMany(s => s.ToCharArray()))
                    );
                yield return new TestCaseData(
                    GetExpression(queryableString.Where(s => s.StartsWith("test")).Take(2))
                    );
                yield return new TestCaseData(
                    GetExpression(from s1 in queryableString.Where(s => s.StartsWith("test"))
                                  join s2 in queryableString.Where(s => s.StartsWith("sample")) 
                                    on s1.ToCharArray()[s1.Length - 1] equals s2.ToCharArray()[s2.Length - 1]
                                  select new { Val = s1 + " - " + s2 })
                    );
            }
        }

        [Test, TestCaseSource("ComplexExpressions")]
        public void Visit_aComplexExpression_returnTheComplexExpression(Expression complexExpression)
        {
            // arrange:
            IVisitor<Expression, Expression> visitor = new FakeNormalizer("TEST");

            // act:
            Expression result = visitor.Visit(complexExpression, VisitContext.New);

            // assert:
            Assert.AreSame(complexExpression, result);
        }
    }
}

