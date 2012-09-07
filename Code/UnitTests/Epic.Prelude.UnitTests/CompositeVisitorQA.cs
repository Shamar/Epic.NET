//  
//  CompositeVisitorQA.cs
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
using Epic.Fakes;
using System.Linq.Expressions;
using Rhino.Mocks;

namespace Epic
{
    [TestFixture()]
    public class CompositeVisitorQA
    {
        [Test]
        public void Initialize_withoutName_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new FakeCompositeVisitor<int>(null);
            });
            
            Assert.Throws<ArgumentNullException>(delegate {
                new FakeCompositeVisitor<object>(string.Empty);
            });
        }
        
        [Test]
        public void Initialize_withAName_works()
        {
            // arrange:
            string name = "test";
            Expression<Func<string, int>> target = s => s.Length;

            // act:
            CompositeVisitor<object> visitor = new FakeCompositeVisitor<object>(name);

            // assert:
            Assert.IsNotNull(visitor);
            Assert.IsNull (visitor.AsVisitor<Expression>(target));
        }
        
        [Test]
        public void GetFirstVisitor_forAnUnknowType_throwsInvalidOperationException()
        {
            // arrange:
            string name = "test";
            Expression<Func<string, int>> target = s => s.Length;
            CompositeVisitor<object> visitor = new FakeCompositeVisitor<object>(name);

            // assert:
            Assert.Throws<InvalidOperationException>(delegate { 
                visitor.GetFirstVisitor(target);
            }, "No visitor available for the expression {0} (of type: {2}) in the composition '{1}'.", target, name, target.GetType());
        }

        [TestFixture]
        public class CompositeVisitor_VisitorBaseQA
        {
            public class DummyVisitor : CompositeVisitor<string>.VisitorBase
            {
                public DummyVisitor(CompositeVisitor<string> composition)
                    : base(composition)
                {
                }

                public string CallContinueVisit<TExpression>(TExpression expression, IVisitContext context) where TExpression : class
                {
                    return base.ContinueVisit(expression, context);
                }

                public string CallVisitInner<TExpression>(TExpression expression, IVisitContext context) where TExpression : class
                {
                    return base.VisitInner(expression, context);
                }
            }

            [Test]
            public void ContinueVisit_withoutAnyArgument_throwsArgumentNullException()
            {
                // arrange:
                string name = "test";
                CompositeVisitor<string> visitor = new FakeCompositeVisitor<string>(name);
                DummyVisitor toTest = new DummyVisitor(visitor);

                // assert:
                Assert.Throws<ArgumentNullException>(delegate
                {
                    toTest.CallContinueVisit<string>(null, VisitContext.New);
                });
                Assert.Throws<ArgumentNullException>(delegate
                {
                    toTest.CallContinueVisit<string>(string.Empty, null);
                });
            }

            [Test]
            public void VisitInner_withoutAnyArgument_throwsArgumentNullException()
            {
                // arrange:
                string name = "test";
                CompositeVisitor<string> visitor = new FakeCompositeVisitor<string>(name);
                DummyVisitor toTest = new DummyVisitor(visitor);

                // assert:
                Assert.Throws<ArgumentNullException>(delegate
                {
                    toTest.CallVisitInner<string>(null, VisitContext.New);
                });
                Assert.Throws<ArgumentNullException>(delegate
                {
                    toTest.CallVisitInner<string>(string.Empty, null);
                });
            }
        }

        [TestFixture]
        public class CovariantVisitorQA : RhinoMocksFixtureBase
        {
            [Test]
            public void Visit_everyTime_callsTheInnerVisitor()
            {
                // arrange:
                string target = "test";
                IVisitContext context = VisitContext.New;
                string expectedResult = "expected";
                IVisitor<string, string> inner = GenerateStrictMock<IVisitor<string, string>>();
                inner.Expect(v => v.Visit(target, context)).Return(expectedResult).Repeat.Once();
                IVisitor<string, object> toTest = new CompositeVisitor<string>.CovariantVisitor<object, string>(inner);

                // act:
                string result = toTest.Visit(target, context);

                // assert:
                Assert.AreSame(expectedResult, result);
            }

            [Test]
            public void AsVisitor_everyTime_callsTheInnerVisitor()
            {
                // arrange:
                string target = "test";
                IVisitor<string, string> otherVisitor = GenerateStrictMock<IVisitor<string, string>>();
                IVisitor<string, string> inner = GenerateStrictMock<IVisitor<string, string>>();
                inner.Expect(v => v.AsVisitor(target)).Return(otherVisitor).Repeat.Once();
                IVisitor<string, object> toTest = new CompositeVisitor<string>.CovariantVisitor<object, string>(inner);

                // act:
                IVisitor<string, string> result = toTest.AsVisitor(target);

                // assert:
                Assert.AreSame(otherVisitor, result);
            }
        }
    }
}

