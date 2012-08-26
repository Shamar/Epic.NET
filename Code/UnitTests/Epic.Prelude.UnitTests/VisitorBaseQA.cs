//  
//  VisitorBaseQA.cs
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
using NUnit.Framework;
using Epic.Fakes;
using System.Linq.Expressions;
using Rhino.Mocks;

namespace Epic
{
    [TestFixture]
    public class VisitorBaseQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutComposition_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate { 
                new FakeVisitor<string, string>(null);
            });
        }
        
        [Test]
        public void Initialize_withACompositionAlreadyContainingAnAnalogueVisitor_alwaysReturnTheLastRegisteredOne()
        {
            // arrange:
            ConstantExpression expression = Expression.Constant(0);
            CompositeVisitor<int> composition = new FakeCompositeVisitor<int>("test");
            FakeVisitor<int, ConstantExpression> firstRegistered = GeneratePartialMock<FakeVisitor<int, ConstantExpression>>(composition);
            FakeVisitor<int, ConstantExpression> lastRegistered = GeneratePartialMock<FakeVisitor<int, ConstantExpression>>(composition);
            lastRegistered.Expect(v => v.CallToVisitor(expression)).Return(lastRegistered).Repeat.Times(3);

            // act:
            IVisitor<int, ConstantExpression> returnedFromFirstVisitor = (firstRegistered as IVisitor<int>).AsVisitor(expression);
            IVisitor<int, ConstantExpression> returnedFromLastVisitor = (lastRegistered as IVisitor<int>).AsVisitor(expression);
            IVisitor<int, ConstantExpression> returnedFromComposition = composition.GetFirstVisitor(expression);

            // assert:
            Assert.AreSame(lastRegistered, returnedFromFirstVisitor);
            Assert.AreSame(lastRegistered, returnedFromLastVisitor);
            Assert.AreSame(lastRegistered, returnedFromComposition);
        }
        
        [Test]
        public void Initialize_withAComposition_registerTheVisitorInTheComposition()
        {
            // arrange:
            ConstantExpression expression = Expression.Constant(0);
            CompositeVisitor<int> composition = new FakeCompositeVisitor<int>("test");

            // act:
            IVisitor<int, ConstantExpression> visitor = new FakeVisitor<int, ConstantExpression>(composition);
            IVisitor<int, ConstantExpression> returnedVisitor = composition.GetFirstVisitor<ConstantExpression>(expression);

            // assert:
            Assert.AreSame(visitor, returnedVisitor);
        }
       
        
        [Test]
        public void AsVisitor_withAnExpressionThatCouldBeHandled_callToVisitorAndReturnTheVisitorItself()
        {
            // arrange:
            ConstantExpression expression = Expression.Constant(0);
            CompositeVisitor<int> composition = new FakeCompositeVisitor<int>("test");
            FakeVisitor<int, ConstantExpression> visitor = GeneratePartialMock<FakeVisitor<int, ConstantExpression>>(composition);
            visitor.Expect(v => v.CallToVisitor(expression)).Return(visitor).Repeat.Twice ();

            // act:
            IVisitor<int, ConstantExpression> returnedFromVisitor = (visitor as IVisitor<int>).AsVisitor(expression);
            IVisitor<int, ConstantExpression> returnedFromComposition = composition.GetFirstVisitor(expression);

            // assert:
            Assert.AreSame(visitor, returnedFromVisitor);
            Assert.AreSame(visitor, returnedFromComposition);
        }
        
        [Test]
        public void ContinueVisit_withAnExpressionThatCouldBeHandled_dontCallToVisitorAndReturnTheNextVisitorMatchingVisitor()
        {
            // arrange:
            ConstantExpression expression = Expression.Constant(0);
            IVisitContext context = VisitContext.New;
            int resultReturnedFromTheVisitorThatWillContinue = 1;
            CompositeVisitor<int> composition = new FakeCompositeVisitor<int>("test");
            FakeVisitor<int, ConstantExpression> visitorThatWillContinue = GeneratePartialMock<FakeVisitor<int, ConstantExpression>>(composition);
            visitorThatWillContinue.Expect(v => v.CallToVisitor(expression)).Return(visitorThatWillContinue).Repeat.Once ();
            visitorThatWillContinue.Expect(v => v.Visit(expression, context)).Return(resultReturnedFromTheVisitorThatWillContinue).Repeat.Once();
            FakeVisitor<int, ConstantExpression> visitor = new FakeVisitor<int, ConstantExpression>(composition);

            // act:
            int visitResult = visitor.CallContinueVisit(expression, context);

            // assert:
            Assert.AreEqual(resultReturnedFromTheVisitorThatWillContinue, visitResult);
        }
        
        [Test]
        public void VisitInner_withAnExpressionThatCouldBeHandled_callToVisitorAndReturnTheVisitorItself()
        {
            // arrange:
            ConstantExpression expression = Expression.Constant(0);
            IVisitContext context = VisitContext.New;
            int resultReturnedFromTheVisitorThatWillContinue = 1;
            CompositeVisitor<int> composition = new FakeCompositeVisitor<int>("test");
            FakeVisitor<int, ConstantExpression> visitorNotReached = new FakeVisitor<int, ConstantExpression>(composition);
            FakeVisitor<int, ConstantExpression> visitor = GeneratePartialMock<FakeVisitor<int, ConstantExpression>>(composition);
            visitor.Expect(v => v.CallToVisitor(expression)).Return(visitor).Repeat.Once ();
            visitor.Expect(v => v.Visit(expression, context)).Return(resultReturnedFromTheVisitorThatWillContinue).Repeat.Once();

            // act:
            int visitResult = visitor.CallVisitInner(expression, context);

            // assert:
            Assert.AreEqual(resultReturnedFromTheVisitorThatWillContinue, visitResult);
            Assert.IsNotNull(visitorNotReached);
        }
        
        [Test]
        public void VisitInner_withAnExpressionThatCanNotBeHandled_callToVisitorButReturnTheFirstValidVisitor()
        {
            // arrange:
            ConstantExpression expression = Expression.Constant(0);
            IVisitContext context = VisitContext.New;
            int resultReturnedFromTheVisitorThatWillContinue = 1;
            CompositeVisitor<int> composition = new FakeCompositeVisitor<int>("test");
            FakeVisitor<int, ConstantExpression> visitorNotReached = new FakeVisitor<int, ConstantExpression>(composition);
            FakeVisitor<int, ConstantExpression> constantVisitor = GeneratePartialMock<FakeVisitor<int, ConstantExpression>>(composition);
            constantVisitor.Expect(v => v.CallToVisitor(expression)).Return(constantVisitor).Repeat.Once ();
            constantVisitor.Expect(v => v.Visit(expression, context)).Return(resultReturnedFromTheVisitorThatWillContinue).Repeat.Once();
            
            FakeVisitor<int, MethodCallExpression> visitor = GeneratePartialMock<FakeVisitor<int, MethodCallExpression>>(composition);
            visitor.Expect(v => v.CallToVisitor(expression)).Return(null).Repeat.Once ();

            // act:
            int visitResult = visitor.CallVisitInner<ConstantExpression>(expression, context);

            // assert:
            Assert.AreEqual(resultReturnedFromTheVisitorThatWillContinue, visitResult);
            Assert.IsNotNull(visitorNotReached);
        }
    }
}

