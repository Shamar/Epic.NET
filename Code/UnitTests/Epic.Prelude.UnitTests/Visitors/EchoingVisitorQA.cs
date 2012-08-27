//
//  EchoingVisitorQA.cs
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
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Epic.Visitors
{
    [TestFixture]
    public class EchoingVisitorQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutComposition_throwsArgumentNullException ()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new EchoingVisitor<string>(null);
            });
        }

        [Test]
        public void Visit_aTarget_returnsTheTargetItself()
        {
            // arrange:
            string target = "result";
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            FakeCompositeVisitor<string, string> optimizer = new FakeCompositeVisitor<string,string>("test");
            IVisitor<string, string> visitor = new EchoingVisitor<string>(optimizer);

            // act:
            string result = visitor.Visit(target, context);

            // assert:
            Assert.AreSame(target, result);
        }
    }
}

