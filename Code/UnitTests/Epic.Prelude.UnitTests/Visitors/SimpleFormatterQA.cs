//
//  SimpleFormatterQA.cs
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
using Rhino.Mocks;

namespace Epic.Visitors
{
    [TestFixture]
    public class SimpleFormatterQA
    {
        [Test]
        public void Initialize_withoutAnyArgument_throwsArgumentNullException()
        {
            // arrange:
            FakeCompositeVisitor<string> composition = new FakeCompositeVisitor<string>("test");

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new SimpleFormatter<Exception>(null, e => e.Message);
            });
            Assert.Throws<ArgumentNullException>(delegate
            {
                new SimpleFormatter<Exception>(composition, null);
            });
            Assert.Throws<ArgumentNullException>(delegate
            {
                new SimpleFormatter<Exception>(composition, e => e.Message, null);
            });
        }

        [Test]
        public void Visit_anObjectOfTheRightType_returnsTheExpectedResult()
        {
            // arrange:
            string expectedResult = "Translated message.";
            Exception toVisit = new InvalidOperationException("Test message.");
            FakeCompositeVisitor<string, Exception> composition = new FakeCompositeVisitor<string, Exception>("test");
            var translation = MockRepository.GenerateMock<Func<Exception, string>>();
            translation.Expect(e => e(toVisit)).Return(expectedResult).Repeat.Once();
            new SimpleFormatter<Exception>(composition, translation);

            // act:
            string result = toVisit.Accept(composition, VisitContext.New);

            // assert:
            Assert.AreSame(expectedResult, result);
        }


        [Test]
        public void Visit_anObjectThatDoesntSatisfyTheAcceptRule_returnsTheResultFromTheUnconstrainedOne()
        {
            // arrange:
            string expectedResult = "Unconstrained test message.";
            Exception toVisit = new InvalidOperationException("Test message.");
            FakeCompositeVisitor<string, Exception> composition = new FakeCompositeVisitor<string, Exception>("test");
            var translation = MockRepository.GenerateMock<Func<Exception, string>>();
            translation.Expect(e => e(toVisit)).Return(expectedResult).Repeat.Once();
            new SimpleFormatter<Exception>(composition, translation);
            var ignoredTranslation = MockRepository.GenerateMock<Func<Exception, string>>();
            var acceptRule = MockRepository.GenerateMock<Func<Exception, bool>>();
            acceptRule.Expect(r => r(toVisit)).Return(false).Repeat.Once();
            new SimpleFormatter<Exception>(composition, ignoredTranslation, acceptRule);

            // act:
            string result = toVisit.Accept(composition, VisitContext.New);

            // assert:
            Assert.AreSame(expectedResult, result);
        }

    }
}
