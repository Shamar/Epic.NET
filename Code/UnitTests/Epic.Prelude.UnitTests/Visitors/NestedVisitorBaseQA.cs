//
//  NestedVisitorBaseQA.cs
//
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
//
//  Copyright (c) 2010-2013 Giacomo Tesio
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
using Rhino.Mocks;
using NUnit.Framework;

namespace Epic.Visitors
{
    [TestFixture]
    public class NestedVisitorBaseQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withAnOuterVisitor_works()
        {
            // arrange:
            var composition = new Fakes.FakeCompositeVisitor<object>("test");
            Fakes.FakeVisitor<object, string> outer = GeneratePartialMock<Fakes.FakeVisitor<object, string>>(composition);

            // act:
            Fakes.FakeVisitor<object, string>.FakeNested nested = new Epic.Fakes.FakeVisitor<object, string>.FakeNested(outer);
        }

        [Test]
        public void Initialize_withoutAnOuterVisitor_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate { new Epic.Fakes.FakeVisitor<object, string>.FakeNested(null); });
        }

        [Test]
        public void AsVisitor_callAsVisitorOnTheComposition()
        {
            // arrange:
            string target = "target";
            Fakes.FakeCompositeVisitor<object> composition = GeneratePartialMock<Fakes.FakeCompositeVisitor<object>>("test");
            Fakes.FakeVisitor<object, string> outer = GeneratePartialMock<Fakes.FakeVisitor<object, string>>(composition);
            IVisitor<object> nested = new Epic.Fakes.FakeVisitor<object, string>.FakeNested(outer);

            // act:
            IVisitor<object, string> result = nested.AsVisitor<string>(target);

            // assert:
            Assert.AreSame(outer, result);
        }

        [Test]
        public void Visit_callTheProtectedVisit()
        {
            // arrange:
            string target = "target";
            object expectedResult = new object();
            VisitContext context = VisitContext.New;
            Fakes.FakeCompositeVisitor<object> composition = GeneratePartialMock<Fakes.FakeCompositeVisitor<object>>("test");
            Fakes.FakeVisitor<object, string> outer = GeneratePartialMock<Fakes.FakeVisitor<object, string>>(composition);
            Fakes.FakeVisitor<object, string>.FakeNested nested = GeneratePartialMock<Epic.Fakes.FakeVisitor<object, string>.FakeNested>(outer);
            nested.Expect(v => v.CallToVisit(target, context, outer)).Return(expectedResult).Repeat.Once();
            
            // act:
            object result = (nested as IVisitor<object, string>).Visit(target, context);
            
            // assert:
            Assert.AreSame(expectedResult, result);
        }
    }
}

