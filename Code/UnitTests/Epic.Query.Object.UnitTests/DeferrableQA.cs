//  
//  DeferrableQA.cs
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
using Challenge00.DDDSample.Cargo;
using System.Collections.Generic;
using Epic.Query.Object.Expressions;
using Rhino.Mocks;


namespace Epic.Query.Object.UnitTests
{
    [TestFixture]
    public class DeferrableQA : RhinoMocksFixtureBase
    {
        [Test]
        public void AsEnumerable_withoutASearch_throwsArgumentNullException()
        {
            // arrange:
            ILimitedSearch<ICargo, TrackingId> search = null;

            // assert:
            Assert.Throws<ArgumentNullException>(delegate { 
                Deferrable.Evaluate<IEnumerable<ICargo>>(search);
            });
        }


        [Test]
        public void AsEnumerable_withASearch_callDeferrerExecuteWithACountExpression()
        {
            // arrange:
            IEnumerable<ICargo> evaluationResult = GenerateStrictMock<IEnumerable<ICargo>>();
            Expression<IEnumerable<ICargo>> expression = GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            IDeferrer deferrer = GenerateStrictMock<IDeferrer>();
            deferrer.Expect(d => d.Evaluate(expression)).Return(evaluationResult).Repeat.Once();
            ISearch<ICargo, TrackingId> search = GenerateStrictMock<ISearch<ICargo, TrackingId>>();
            search.Expect(s => s.Expression).Return(expression).Repeat.Once();
            search.Expect(s => s.Deferrer).Return(deferrer).Repeat.Once();

            // act:
            IEnumerable<ICargo> result = search.Evaluate();

            // assert:
            Assert.AreSame(evaluationResult, result);
        }

    }
}

