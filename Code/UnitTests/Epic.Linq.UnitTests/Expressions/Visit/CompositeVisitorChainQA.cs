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

namespace Epic.Linq.Expressions.Visit
{
    public class DumbVisitor : CompositeVisitorBase
    {
        public DumbVisitor(ICompositeVisitor next)
            : base(next)
        {
        }
    }
    
    public class GenericDumbVisitor<TExpression> : CompositeVisitorBase, ICompositeVisitor<TExpression>
        where TExpression : Expression
    {
        public GenericDumbVisitor(ICompositeVisitor next)
            : base(next)
        {
        }

        #region ICompositeVisitor[TExpression] implementation
        public virtual System.Linq.Expressions.Expression Visit (TExpression target)
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
            ICompositeVisitor<MethodCallExpression> endVisitor = GenerateStrictMock<ICompositeVisitor<MethodCallExpression>>();
            CompositeVisitorChain chain = new CompositeVisitorChain(endVisitor);
            GenericDumbVisitor<MethodCallExpression> dumb1 = new GenericDumbVisitor<MethodCallExpression> (chain);
            chain.Append(dumb1);
            GenericDumbVisitor<MemberInitExpression> dumb2 = new GenericDumbVisitor<MemberInitExpression>(chain);
            chain.Append(dumb2);

            // act:
            ICompositeVisitor<MethodCallExpression> recievedVisitor = dumb2.GetVisitor<MethodCallExpression>();

            // assert:
            Assert.AreSame(dumb1, recievedVisitor);
        }
        
        [Test]
        public void GetVisitor_withUnknownExpression_reachEnd()
        {
            // arrange:
            ICompositeVisitor<MethodCallExpression> endVisitor = GenerateStrictMock<ICompositeVisitor<MethodCallExpression>>();
            endVisitor.Expect(v => v.GetVisitor<MethodCallExpression>()).Return(endVisitor).Repeat.Once();
            CompositeVisitorChain chain = new CompositeVisitorChain(endVisitor);
            DumbVisitor dumb1 = new DumbVisitor(chain);
            chain.Append(dumb1);
            DumbVisitor dumb2 = new DumbVisitor(chain);
            chain.Append(dumb2);

            // act:
            ICompositeVisitor<MethodCallExpression> recievedVisitor = dumb2.GetVisitor<MethodCallExpression>();

            // assert:
            Assert.AreSame(endVisitor, recievedVisitor);
        }
    }
}

