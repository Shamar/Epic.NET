//  
//  UnvisitableExpressionAdapterQA.cs
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

namespace Epic.Linq.Expressions
{
    [TestFixture()]
    public class UnvisitableExpressionAdapterQA
    {
        [Test]
        public void Accept_aVisitor_willStartTheVisit()
        {
            // arrange:
            Expression<Func<int, string, int>> expression = (i,s)=> (i + s.Length).ToString().Length;
            VisitorsComposition chain = new VisitorsComposition();
            new LoggingVisitor(chain, CompositeVisitorChainQA.WriteToConsole);
            UnvisitableExpressionAdapter adapter = new UnvisitableExpressionAdapter(expression);

            // act:
            Expression e = adapter.Accept(chain, VisitState.New);

            // assert:
            Assert.AreSame(expression, e);
        }
    }
}

