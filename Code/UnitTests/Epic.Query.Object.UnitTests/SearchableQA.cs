//  
//  SearchableQA.cs
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
using NUnit.Framework;
using System;
using Rhino.Mocks;
using Challenge00.DDDSample.Cargo;
using Epic.Query.Object.Expressions;
using System.Collections.Generic;

namespace Epic.Query.Object.UnitTests
{
    [TestFixture]
    public class SearchableQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Count_withoutASearch_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate { 
                Searchable.Count<ICargo>(null);
            });
        }

        [Test]
        public void Count_withASearch_callDeferrerExecuteWithACountExpression()
        {
            // arrange:
            uint countToReturn = 10;
            object[] evaluationArguments = null;
            Expression<IEnumerable<ICargo>> expression = MockRepository.GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            IDeferrer deferrer = MockRepository.GenerateStrictMock<IDeferrer>();
            deferrer.Expect(d => d.Evaluate(null as Expression<uint>)).IgnoreArguments()
                .WhenCalled(m => evaluationArguments = m.Arguments)
                .Return(countToReturn).Repeat.Once();
            ISearch<ICargo, TrackingId> search = MockRepository.GenerateStrictMock<ISearch<ICargo, TrackingId>>();
            search.Expect(s => s.Expression).Return(expression).Repeat.Once();
            search.Expect(s => s.Deferrer).Return(deferrer).Repeat.Once();

            // act:
            uint result = search.Count();

            // assert:
            Assert.AreEqual(countToReturn, result);
            Assert.AreEqual(1, evaluationArguments.Length);
            Assert.IsInstanceOf<Count<ICargo>>(evaluationArguments[0]);
            Assert.AreSame(expression, ((Count<ICargo>)evaluationArguments[0]).Source);
        }

        [Test]
        public void AsEnumerable_withoutASearch_throwsArgumentNullException()
        {
            // arrange:
            ILimitedSearch<ICargo, TrackingId> search = null;

            // assert:
            Assert.Throws<ArgumentNullException>(delegate { 
                Searchable.AsEnumerable(search);
            });
        }


        [Test]
        public void AsEnumerable_withASearch_callDeferrerExecuteWithACountExpression()
        {
            // arrange:
            IEnumerable<ICargo> evaluationResult = GenerateStrictMock<IEnumerable<ICargo>>();
            Expression<IEnumerable<ICargo>> expression = MockRepository.GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            IDeferrer deferrer = MockRepository.GenerateStrictMock<IDeferrer>();
            deferrer.Expect(d => d.Evaluate(expression)).Return(evaluationResult).Repeat.Once();
            ISearch<ICargo, TrackingId> search = MockRepository.GenerateStrictMock<ISearch<ICargo, TrackingId>>();
            search.Expect(s => s.Expression).Return(expression).Repeat.Once();
            search.Expect(s => s.Deferrer).Return(deferrer).Repeat.Once();

            // act:
            IEnumerable<ICargo> result = search.AsEnumerable();

            // assert:
            Assert.AreSame(evaluationResult, result);
        }

        [Test]
        public void Identify_withoutASearch_throwsArgumentNullException()
        {
            // arrange:
            IOrderedSearch<ICargo, TrackingId> search = null;

            // assert:
            Assert.Throws<ArgumentNullException>(delegate { 
                Searchable.Identify(search);
            });
        }

        [Test]
        public void Identify_withASearch_callDeferrerExecuteWithACountExpression()
        {
            // arrange:
            IEnumerable<TrackingId> evaluationResult = GenerateStrictMock<IEnumerable<TrackingId>>();
            object[] evaluationArguments = null;
            Expression<IEnumerable<ICargo>> expression = MockRepository.GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            IDeferrer deferrer = MockRepository.GenerateStrictMock<IDeferrer>();
            deferrer.Expect(d => d.Evaluate(null as Expression<IEnumerable<TrackingId>>)).IgnoreArguments()
                .WhenCalled(m => evaluationArguments = m.Arguments)
                .Return(evaluationResult).Repeat.Once();
            ISearch<ICargo, TrackingId> search = MockRepository.GenerateStrictMock<ISearch<ICargo, TrackingId>>();
            search.Expect(s => s.Expression).Return(expression).Repeat.Once();
            search.Expect(s => s.Deferrer).Return(deferrer).Repeat.Once();

            // act:
            IEnumerable<TrackingId> result = search.Identify();

            // assert:
            Assert.AreSame(evaluationResult, result);
            Assert.AreEqual(1, evaluationArguments.Length);
            Assert.IsInstanceOf<Identification<ICargo, TrackingId>>(evaluationArguments[0]);
            Assert.AreSame(expression, ((Identification<ICargo, TrackingId>)evaluationArguments[0]).Entities);
        }
    }
}

