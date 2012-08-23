//  
//  AcceptCallerQA.cs
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
using Rhino.Mocks;

namespace Epic
{
    [TestFixture]
    public class AcceptCallerQA : RhinoMocksFixtureBase
    {
        public class PublicUnvisitable
        {
        }

        [Test]
        public void CallAccept_onAVisitableType_callsTheIVisitableAcceptMethod()
        {
            // arrange:
            object visitResult = new object();
            IVisitable visitable = GenerateStrictMock<IVisitable>();
            IVisitor<object, IVisitable> visitor = GenerateStrictMock<IVisitor<object, IVisitable>>();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            visitable.Expect(v => v.Accept(visitor, context)).Return(visitResult).Repeat.Once();

            // act:
            object result = AcceptCaller<IVisitable, object>.CallAccept(visitable, visitor, context);

            // assert:
            Assert.AreSame(visitResult, result);
        }

        [Test]
        public void CallAccept_onAPublicUnvisitableType_routeToTheRightVisistor()
        {
            // arrange:
            object visitResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            PublicUnvisitable unvisitable = new PublicUnvisitable();
            IVisitor<object, PublicUnvisitable> visitor = GenerateStrictMock<IVisitor<object, PublicUnvisitable>>();
            IVisitor<object> visitorComposition = GenerateStrictMock<IVisitor<object>>();
            visitorComposition.Expect(v => v.AsVisitor<PublicUnvisitable>(unvisitable)).Return(visitor).Repeat.Once();
            visitor.Expect(v => v.Visit(unvisitable, context)).Return(visitResult).Repeat.Once();

            // act:
            object result = AcceptCaller<PublicUnvisitable, object>.CallAccept(unvisitable, visitorComposition, context);

            // assert:
            Assert.AreSame(visitResult, result);
        }
    }
}
