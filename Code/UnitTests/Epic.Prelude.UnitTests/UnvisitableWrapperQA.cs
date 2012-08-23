//
//  UnvisitableWrapperQA.cs
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
    public class UnvisitableWrapperQA : RhinoMocksFixtureBase
    {
        private class PrivateInvalidOperationException : InvalidOperationException
        {
        }

        private class PrivateObject : Object
        {
        }

        [Test]
        public void SimulateAccept_onAPublicException_routesToTheRightVisitor ()
        {
            // arrange:
            object visitResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            ArgumentNullException unvisitable = new ArgumentNullException();
            IVisitor<object, ArgumentNullException> visitor = GenerateStrictMock<IVisitor<object, ArgumentNullException>>();
            IVisitor<object> visitorComposition = GenerateStrictMock<IVisitor<object>>();
            visitorComposition.Expect(v => v.GetVisitor<ArgumentNullException>(unvisitable)).Return(visitor).Repeat.Once();
            visitor.Expect(v => v.Visit(unvisitable, context)).Return(visitResult).Repeat.Once();

            // act:
            object result = UnvisitableWrapper<Exception, object>.SimulateAccept(unvisitable, visitorComposition, context);

            // assert:
            Assert.AreSame(visitResult, result);
        }

        [Test]
        public void SimulateAccept_onAPrivateException_routesToTheVisitorOfTheFirstPublicAncestor()
        {
            // arrange:
            object visitResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            PrivateInvalidOperationException unvisitable = new PrivateInvalidOperationException();
            IVisitor<object, InvalidOperationException> visitor = GenerateStrictMock<IVisitor<object, InvalidOperationException>>();
            IVisitor<object> visitorComposition = GenerateStrictMock<IVisitor<object>>();
            visitorComposition.Expect(v => v.GetVisitor<InvalidOperationException>(unvisitable)).Return(visitor).Repeat.Once();
            visitor.Expect(v => v.Visit(unvisitable, context)).Return(visitResult).Repeat.Once();

            // act:
            object result = UnvisitableWrapper<Exception, object>.SimulateAccept(unvisitable, visitorComposition, context);

            // assert:
            Assert.AreSame(visitResult, result);
        }

        [Test]
        public void SimulateAccept_onAPrivateObjectDirectlyDerivedFromObject_throwsArgumentException()
        {
            // arrange:
            object visitResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            object unvisitable = new PrivateObject();
            IVisitor<object> visitorComposition = GenerateStrictMock<IVisitor<object>>();

            // assert:
            Assert.Throws<ArgumentException>(delegate {
                UnvisitableWrapper<object, object>.SimulateAccept(unvisitable, visitorComposition, context);
            });
        }
    }
}

