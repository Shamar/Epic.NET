//  
//  CompositeVisitorChainQA.cs
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
using Epic.Linq.Expressions.Visit;
using System.Linq.Expressions;
using Rhino.Mocks;
using System.Reflection;
using System.Linq;

namespace Epic.Linq.Expressions.Visit
{
    public class DumbVisitor : VisitorsComposition.VisitorBase
    {
        public DumbVisitor(VisitorsComposition next)
            : base(next)
        {
        }
    }
    
    public class GenericDumbVisitor<TExpression> : VisitorsComposition.VisitorBase, ICompositeVisitor<TExpression>
        where TExpression : Expression
    {
        public GenericDumbVisitor(VisitorsComposition next)
            : base(next)
        {
        }

        #region ICompositeVisitor[TExpression] implementation
        public virtual System.Linq.Expressions.Expression Visit (TExpression target, IVisitState state)
        {
            return target;
        }
        #endregion
    }
    
    [TestFixture]
    public class CompositeVisitorChainQA : RhinoMocksFixtureBase
    {
        [Test]
        public void GetVisitor_withKnownExpression_dontReachEnd()
        {
            // arrange:
            MethodCallExpression expression = Expression.Call(Expression.Constant(this), (MethodInfo)MethodBase.GetCurrentMethod());
            VisitorsComposition chain = new VisitorsComposition();
            GenericDumbVisitor<MethodCallExpression> dumb1 = new GenericDumbVisitor<MethodCallExpression> (chain);
            GenericDumbVisitor<MemberInitExpression> dumb2 = new GenericDumbVisitor<MemberInitExpression>(chain);

            // act:
            ICompositeVisitor<MethodCallExpression> recievedVisitor = dumb2.GetVisitor<MethodCallExpression>(expression);

            // assert:
            Assert.AreSame(dumb1, recievedVisitor);
        }
        
        [Test]
        public void GetVisitor_withUnknownExpression_reachEnd()
        {
            // arrange:
            MethodCallExpression expression = Expression.Call(Expression.Constant(this), (MethodInfo)MethodBase.GetCurrentMethod());
            VisitorsComposition chain = new VisitorsComposition();
            new DumbVisitor(chain);
            DumbVisitor dumb2 = new DumbVisitor(chain);

            // act:
            ICompositeVisitor<MethodCallExpression> recievedVisitor = dumb2.GetVisitor<MethodCallExpression>(expression);

            // assert:
            Assert.IsInstanceOf<UnvisitableExpressionsVisitor>(recievedVisitor);
        }
        
        [Test]
        public void GetVisitor_withUnknownExpressio_toTheChainItself_reachEnd()
        {
            // arrange:
            MethodCallExpression expression = Expression.Call(Expression.Constant(this), (MethodInfo)MethodBase.GetCurrentMethod());
            VisitorsComposition chain = new VisitorsComposition();
            new DumbVisitor(chain);
            new DumbVisitor(chain);

            // act:
            ICompositeVisitor<MethodCallExpression> recievedVisitor = chain.GetVisitor<MethodCallExpression>(expression);

            // assert:
            Assert.IsInstanceOf<UnvisitableExpressionsVisitor>(recievedVisitor);
        }
        
        [Test]
        public void GetVisitor_withKnownExpression_toTheChainItsel_dontReachEnd()
        {
            // arrange:
            MethodCallExpression callExpression = Expression.Call(Expression.Constant(this), (MethodInfo)MethodBase.GetCurrentMethod());
            Expression<Func<int, string>> lambdaExpression = x => x.ToString();
            VisitorsComposition chain = new VisitorsComposition();
            GenericDumbVisitor<MethodCallExpression> dumb1 = new GenericDumbVisitor<MethodCallExpression> (chain);
            GenericDumbVisitor<Expression<Func<int, string>>> dumb2 = new GenericDumbVisitor<Expression<Func<int, string>>>(chain);

            // act:
            ICompositeVisitor<MethodCallExpression> recievedMethodCallVisitor = chain.GetVisitor<MethodCallExpression>(callExpression);
            ICompositeVisitor<Expression<Func<int, string>>> recievedLambdaVisitor = chain.GetVisitor<Expression<Func<int, string>>>(lambdaExpression);

            // assert:
            Assert.AreSame(dumb1, recievedMethodCallVisitor);
            Assert.AreSame(dumb2, recievedLambdaVisitor);
        }
        
        [Test]
        public void Visit_withPrintingVisitor_works()
        {
            // arrange:
            Expression<Func<int, string, int>> expression = (i,s)=> (i + s.Length).ToString().Length;
            VisitorsComposition chain = new VisitorsComposition();
            new PrintingVisitor(chain);
            UnvisitableExpressionAdapter adapter = new UnvisitableExpressionAdapter(expression);

            // act:
            Expression e = adapter.Accept(chain, VisitState.New);
            
            // assert:
            Assert.AreSame(expression, e);
        }
        
        
        [Test]
        public void Visit_TernaryWithPrintingVisitor_works()
        {
            // arrange:
            Expression<Func<int, string, int>> expression = (i,s)=> s == null ? i : s.Length;
            VisitorsComposition chain = new VisitorsComposition();
            new PrintingVisitor(chain);
            UnvisitableExpressionAdapter adapter = new UnvisitableExpressionAdapter(expression);

            // act:
            Expression e = adapter.Accept(chain, VisitState.New);
            
            // assert:
            Assert.AreSame(expression, e);
        }
        
    }
}

