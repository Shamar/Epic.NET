//  
//  VisitableBaseQA.cs
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
using Epic.Fakes;
using Rhino.Mocks;
using NUnit.Framework;

namespace Epic
{

    
    public class VisitableBaseQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Accept_withoutVisitor_throwsArgumentNullException()
        {
            // arrange:
            VisitableBase visitable = new FakeVisitable<int>();

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                visitable.Accept<FakeVisitable<int>>(null, VisitContext.New);
            });
        }
        
        [Test]
        public void Accept_withoutContext_throwsArgumentNullException()
        {
            // arrange:
            IVisitor<string> visitor = GenerateStrictMock<IVisitor<string>>();
            VisitableBase visitable = new FakeVisitable<int>();

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                visitable.Accept(visitor, null);
            });
        }
        
        [Test]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            string expectedResult = "DONE";
            IVisitContext context = VisitContext.New;
            FakeVisitable<int> visitable = new FakeVisitable<int>();
            IVisitor<string, FakeVisitable<int>> visitorToUse = GenerateStrictMock<IVisitor<string, FakeVisitable<int>>>();
            visitorToUse.Expect(v => v.Visit(visitable, context)).Return(expectedResult).Repeat.Once();
            IVisitor<string> visitor = GenerateStrictMock<IVisitor<string>>();
            visitor.Expect(v => v.AsVisitor<FakeVisitable<int>>(visitable)).Return(visitorToUse).Repeat.Once();

            // act:
            string visitResult = visitable.Accept(visitor, context);

            // assert:
            Assert.AreEqual(expectedResult, visitResult);
        }
        
        [Test]
        public void AcceptMe_withWrongInstance_throwsArgumentOutOfRangeException()
        {
            // arrange:
            VisitableBase mockVisitable = GeneratePartialMock<FakeVisitable<int>>();
            IVisitor<string> visitor = GenerateStrictMock<IVisitor<string>>();
            FakeVisitable<int> visitable = new FakeVisitable<int>();

            // assert:
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                visitable.FoolAcceptMe(mockVisitable, visitor, VisitContext.New);
            });
        }
        
        [Test]
        public void AcceptMe_withWronglyDerivedType_throwsInvalidOperationException()
        {
            // arrange:
            IVisitor<string> visitor = GenerateStrictMock<IVisitor<string>>();
            FakeVisitable<int> visitable = new DerivedFakeVisitable<int>();

            // assert:
            Assert.Throws<InvalidOperationException>(delegate {
                visitable.Accept(visitor, VisitContext.New);
            });
        }
        
        
    }
}

