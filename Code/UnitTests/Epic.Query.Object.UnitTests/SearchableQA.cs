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
using System.Linq;

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

        [Test]
        public void OrderBy_withoutAnyArgument_throwsArgumentNullException()
        {
            // arrange:
            ISearch<ICargo, TrackingId> search = GenerateStrictMock<ISearch<ICargo, TrackingId>>();
            OrderCriterion<ICargo> criterion = GeneratePartialMock<OrderCriterion<ICargo>>();

            // assert:
            Assert.Throws<ArgumentNullException>(delegate { 
                Searchable.OrderBy<ICargo, TrackingId>(null, criterion);
            });
            Assert.Throws<ArgumentNullException>(delegate { 
                Searchable.OrderBy<ICargo, TrackingId>(search, null);
            });
        }

        [Test]
        public void OrderBy_withValidArguments_deferrsANewSearch()
        {
            // arrange:
            OrderCriterion<ICargo> criterion = GeneratePartialMock<OrderCriterion<ICargo>>();
            IOrderedSearch<ICargo, TrackingId> deferResult = GenerateStrictMock<IOrderedSearch<ICargo, TrackingId>>();
            object[] deferArguments = null;
            Expression<IEnumerable<ICargo>> expression = MockRepository.GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            IDeferrer deferrer = MockRepository.GenerateStrictMock<IDeferrer>();
            deferrer.Expect(d => d.Defer<IOrderedSearch<ICargo, TrackingId>, IEnumerable<ICargo>>(null)).IgnoreArguments()
                .WhenCalled(m => deferArguments = m.Arguments)
                .Return(deferResult).Repeat.Once();
            ISearch<ICargo, TrackingId> search = MockRepository.GenerateStrictMock<ISearch<ICargo, TrackingId>>();
            search.Expect(s => s.Expression).Return(expression).Repeat.Once();
            search.Expect(s => s.Deferrer).Return(deferrer).Repeat.Once();

            // act:
            IOrderedSearch<ICargo, TrackingId> result = search.OrderBy(criterion);

            // assert:
            Assert.AreSame(deferResult, result);
            Assert.AreEqual(1, deferArguments.Length);
            Assert.IsInstanceOf<Order<ICargo>>(deferArguments[0]);
            Assert.AreSame(criterion, ((Order<ICargo>)deferArguments[0]).Criterion);
        }


        [Test]
        public void ThenBy_withoutAnyArgument_throwsArgumentNullException()
        {
            // arrange:
            IOrderedSearch<ICargo, TrackingId> search = GenerateStrictMock<IOrderedSearch<ICargo, TrackingId>>();
            OrderCriterion<ICargo> criterion = GeneratePartialMock<OrderCriterion<ICargo>>();

            // assert:
            Assert.Throws<ArgumentNullException>(delegate { 
                Searchable.ThenBy<ICargo, TrackingId>(null, criterion);
            });
            Assert.Throws<ArgumentNullException>(delegate { 
                Searchable.ThenBy<ICargo, TrackingId>(search, null);
            });
        }

        [Test]
        public void ThenBy_withValidArguments_deferrsANewSearch()
        {
            // arrange:
            OrderCriterion<ICargo> initialCriterion = GeneratePartialMock<OrderCriterion<ICargo>>();
            OrderCriterion<ICargo> secondCriterion = GeneratePartialMock<OrderCriterion<ICargo>>();
            IOrderedSearch<ICargo, TrackingId> deferResult = GenerateStrictMock<IOrderedSearch<ICargo, TrackingId>>();
            object[] deferArguments = null;
            Expression<IEnumerable<ICargo>> sourceExpression = MockRepository.GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            Order<ICargo> expression = new Order<ICargo>(sourceExpression, initialCriterion); 
            IDeferrer deferrer = MockRepository.GenerateStrictMock<IDeferrer>();
            deferrer.Expect(d => d.Defer<IOrderedSearch<ICargo, TrackingId>, IEnumerable<ICargo>>(null)).IgnoreArguments()
                .WhenCalled(m => deferArguments = m.Arguments)
                .Return(deferResult).Repeat.Once();
            IOrderedSearch<ICargo, TrackingId> search = MockRepository.GenerateStrictMock<IOrderedSearch<ICargo, TrackingId>>();
            search.Expect(s => s.Expression).Return(expression).Repeat.Once();
            search.Expect(s => s.Deferrer).Return(deferrer).Repeat.Once();

            // act:
            IOrderedSearch<ICargo, TrackingId> result = search.ThenBy(secondCriterion);

            // assert:
            Assert.AreSame(deferResult, result);
            Assert.AreEqual(1, deferArguments.Length);
            Assert.IsInstanceOf<Order<ICargo>>(deferArguments[0]);
            Assert.IsInstanceOf<OrderCriteria<ICargo>>(((Order<ICargo>)deferArguments[0]).Criterion);
            Assert.AreSame(initialCriterion, ((OrderCriteria<ICargo>)((Order<ICargo>)deferArguments[0]).Criterion).ElementAt(0));
            Assert.AreSame(secondCriterion, ((OrderCriteria<ICargo>)((Order<ICargo>)deferArguments[0]).Criterion).ElementAt(1));
        }

        [Test]
        public void Skip_withAOrderedSearch_deferrsANewLimitedSearch()
        {
            // arrange:
            OrderCriterion<ICargo> criterion = GeneratePartialMock<OrderCriterion<ICargo>>();
            ISlicedSearch<ICargo, TrackingId> deferResult = GenerateStrictMock<ISlicedSearch<ICargo, TrackingId>>();
            object[] deferArguments = null;
            Expression<IEnumerable<ICargo>> sourceExpression = MockRepository.GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            Order<ICargo> expression = new Order<ICargo>(sourceExpression, criterion);
            IDeferrer deferrer = MockRepository.GenerateStrictMock<IDeferrer>();
            deferrer.Expect(d => d.Defer<ISlicedSearch<ICargo, TrackingId>, IEnumerable<ICargo>>(null)).IgnoreArguments()
                .WhenCalled(m => deferArguments = m.Arguments)
                .Return(deferResult).Repeat.Once();
            IOrderedSearch<ICargo, TrackingId> search = MockRepository.GenerateStrictMock<IOrderedSearch<ICargo, TrackingId>>();
            search.Expect(s => s.Expression).Return(expression).Repeat.Once();
            search.Expect(s => s.Deferrer).Return(deferrer).Repeat.Once();

            // act:
            ISlicedSearch<ICargo, TrackingId> result = search.Skip(10);

            // assert:
            Assert.AreSame(deferResult, result);
            Assert.AreEqual(1, deferArguments.Length);
            Assert.IsInstanceOf<Slice<ICargo>>(deferArguments[0]);
            Assert.AreSame(expression, ((Slice<ICargo>)deferArguments[0]).Source);
            Assert.AreEqual(uint.MaxValue, ((Slice<ICargo>)deferArguments[0]).TakingAtMost);
            Assert.AreEqual(10, ((Slice<ICargo>)deferArguments[0]).Skipping);
        }

        [Test]
        public void Skip_withALimitedSearch_deferrsANewLimitedSearch()
        {
            // arrange:
            OrderCriterion<ICargo> criterion = GeneratePartialMock<OrderCriterion<ICargo>>();
            ISlicedSearch<ICargo, TrackingId> deferResult = GenerateStrictMock<ISlicedSearch<ICargo, TrackingId>>();
            object[] deferArguments = null;
            Expression<IEnumerable<ICargo>> sourceExpression = MockRepository.GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            Order<ICargo> orderedExpression = new Order<ICargo>(sourceExpression, criterion);
            Slice<ICargo> expression = new Slice<ICargo>(orderedExpression, 10, 5);
            IDeferrer deferrer = MockRepository.GenerateStrictMock<IDeferrer>();
            deferrer.Expect(d => d.Defer<ISlicedSearch<ICargo, TrackingId>, IEnumerable<ICargo>>(null)).IgnoreArguments()
                .WhenCalled(m => deferArguments = m.Arguments)
                .Return(deferResult).Repeat.Once();
            ISlicedSearch<ICargo, TrackingId> search = MockRepository.GenerateStrictMock<ISlicedSearch<ICargo, TrackingId>>();
            search.Expect(s => s.Expression).Return(expression).Repeat.Times(3);
            search.Expect(s => s.Deferrer).Return(deferrer).Repeat.Once();

            // act:
            ISlicedSearch<ICargo, TrackingId> result = search.Skip(7);

            // assert:
            Assert.AreSame(deferResult, result);
            Assert.AreEqual(1, deferArguments.Length);
            Assert.IsInstanceOf<Slice<ICargo>>(deferArguments[0]);
            Assert.AreSame(orderedExpression, ((Slice<ICargo>)deferArguments[0]).Source);
            Assert.AreEqual(5, ((Slice<ICargo>)deferArguments[0]).TakingAtMost);
            Assert.AreEqual(7, ((Slice<ICargo>)deferArguments[0]).Skipping);
        }

        [Test]
        public void Skip_withAnEquivalentLimitedSearch_returnsTheSameInstance()
        {
            // arrange:
            OrderCriterion<ICargo> criterion = GeneratePartialMock<OrderCriterion<ICargo>>();
            object[] deferArguments = null;
            Expression<IEnumerable<ICargo>> sourceExpression = MockRepository.GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            Order<ICargo> orderedExpression = new Order<ICargo>(sourceExpression, criterion);
            Slice<ICargo> expression = new Slice<ICargo>(orderedExpression, 10, 5);
            ISlicedSearch<ICargo, TrackingId> search = MockRepository.GenerateStrictMock<ISlicedSearch<ICargo, TrackingId>>();
            search.Expect(s => s.Expression).Return(expression).Repeat.Times(2);

            // act:
            ISlicedSearch<ICargo, TrackingId> result = search.Skip(10);

            // assert:
            Assert.AreSame(search, result);
            Assert.IsNull(deferArguments);
        }

        [Test]
        public void Skip_withoutASearch_throwsArgumentNullException()
        {
            // arrange:
            IOrderedSearch<ICargo, TrackingId> orderedSearch = null;
            ISlicedSearch<ICargo, TrackingId> limitedSearch = null;

            // assert:
            Assert.Throws<ArgumentNullException>(delegate { 
                Searchable.Skip<ICargo, TrackingId>(orderedSearch, 1);
            });
            Assert.Throws<ArgumentNullException>(delegate { 
                Searchable.Skip<ICargo, TrackingId>(limitedSearch, 1);
            });
        }

        [Test]
        public void Take_withAnEquivalentLimitedSearch_returnsTheSameInstance()
        {
            // arrange:
            OrderCriterion<ICargo> criterion = GeneratePartialMock<OrderCriterion<ICargo>>();
            object[] deferArguments = null;
            Expression<IEnumerable<ICargo>> sourceExpression = MockRepository.GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            Order<ICargo> orderedExpression = new Order<ICargo>(sourceExpression, criterion);
            Slice<ICargo> expression = new Slice<ICargo>(orderedExpression, 10, 5);
            ISlicedSearch<ICargo, TrackingId> search = MockRepository.GenerateStrictMock<ISlicedSearch<ICargo, TrackingId>>();
            search.Expect(s => s.Expression).Return(expression).Repeat.Times(2);

            // act:
            ISlicedSearch<ICargo, TrackingId> result = search.Take(5);

            // assert:
            Assert.AreSame(search, result);
            Assert.IsNull(deferArguments);
        }

        [Test]
        public void Take_withAOrderedSearch_deferrsANewLimitedSearch()
        {
            // arrange:
            OrderCriterion<ICargo> criterion = GeneratePartialMock<OrderCriterion<ICargo>>();
            ISlicedSearch<ICargo, TrackingId> deferResult = GenerateStrictMock<ISlicedSearch<ICargo, TrackingId>>();
            object[] deferArguments = null;
            Expression<IEnumerable<ICargo>> sourceExpression = MockRepository.GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            Order<ICargo> expression = new Order<ICargo>(sourceExpression, criterion);
            IDeferrer deferrer = MockRepository.GenerateStrictMock<IDeferrer>();
            deferrer.Expect(d => d.Defer<ISlicedSearch<ICargo, TrackingId>, IEnumerable<ICargo>>(null)).IgnoreArguments()
                .WhenCalled(m => deferArguments = m.Arguments)
                .Return(deferResult).Repeat.Once();
            IOrderedSearch<ICargo, TrackingId> search = MockRepository.GenerateStrictMock<IOrderedSearch<ICargo, TrackingId>>();
            search.Expect(s => s.Expression).Return(expression).Repeat.Once();
            search.Expect(s => s.Deferrer).Return(deferrer).Repeat.Once();

            // act:
            ISlicedSearch<ICargo, TrackingId> result = search.Take(10);

            // assert:
            Assert.AreSame(deferResult, result);
            Assert.AreEqual(1, deferArguments.Length);
            Assert.IsInstanceOf<Slice<ICargo>>(deferArguments[0]);
            Assert.AreSame(expression, ((Slice<ICargo>)deferArguments[0]).Source);
            Assert.AreEqual(10, ((Slice<ICargo>)deferArguments[0]).TakingAtMost);
            Assert.AreEqual(0, ((Slice<ICargo>)deferArguments[0]).Skipping);
        }

        [Test]
        public void Take_withALimitedSearch_deferrsANewLimitedSearch()
        {
            // arrange:
            OrderCriterion<ICargo> criterion = GeneratePartialMock<OrderCriterion<ICargo>>();
            ISlicedSearch<ICargo, TrackingId> deferResult = GenerateStrictMock<ISlicedSearch<ICargo, TrackingId>>();
            object[] deferArguments = null;
            Expression<IEnumerable<ICargo>> sourceExpression = MockRepository.GeneratePartialMock<Expression<IEnumerable<ICargo>>>();
            Order<ICargo> orderedExpression = new Order<ICargo>(sourceExpression, criterion);
            Slice<ICargo> expression = new Slice<ICargo>(orderedExpression, 10, 5);
            IDeferrer deferrer = MockRepository.GenerateStrictMock<IDeferrer>();
            deferrer.Expect(d => d.Defer<ISlicedSearch<ICargo, TrackingId>, IEnumerable<ICargo>>(null)).IgnoreArguments()
                .WhenCalled(m => deferArguments = m.Arguments)
                .Return(deferResult).Repeat.Once();
            ISlicedSearch<ICargo, TrackingId> search = MockRepository.GenerateStrictMock<ISlicedSearch<ICargo, TrackingId>>();
            search.Expect(s => s.Expression).Return(expression).Repeat.Times(3);
            search.Expect(s => s.Deferrer).Return(deferrer).Repeat.Once();

            // act:
            ISlicedSearch<ICargo, TrackingId> result = search.Take(7);

            // assert:
            Assert.AreSame(deferResult, result);
            Assert.AreEqual(1, deferArguments.Length);
            Assert.IsInstanceOf<Slice<ICargo>>(deferArguments[0]);
            Assert.AreSame(orderedExpression, ((Slice<ICargo>)deferArguments[0]).Source);
            Assert.AreEqual(7, ((Slice<ICargo>)deferArguments[0]).TakingAtMost);
            Assert.AreEqual(10, ((Slice<ICargo>)deferArguments[0]).Skipping);
        }

        [Test]
        public void Take_withoutASearch_throwsArgumentNullException()
        {
            // arrange:
            IOrderedSearch<ICargo, TrackingId> orderedSearch = null;
            ISlicedSearch<ICargo, TrackingId> limitedSearch = null;

            // assert:
            Assert.Throws<ArgumentNullException>(delegate { 
                Searchable.Take<ICargo, TrackingId>(orderedSearch, 1);
            });
            Assert.Throws<ArgumentNullException>(delegate { 
                Searchable.Take<ICargo, TrackingId>(limitedSearch, 1);
            });
        }    
    }
}

