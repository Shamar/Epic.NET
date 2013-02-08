//  
//  DeferredEvaluationExceptionQA.cs
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
using NUnit.Framework;
using System;
using Challenge00.DDDSample.Cargo;
using Epic.Query.Object.Expressions;
using System.Collections.Generic;

namespace Epic.Query.Object.UnitTests
{
    [TestFixture()]
    public class DeferredEvaluationExceptionQA : EpicExceptionQABase<DeferredEvaluationException<string>>
    {
        [Test]
        public void Initialize_withoutInnerException_works()
        {
            // arrange:
            Expression<string> unexpectedExpression = new Fakes.FakeExpression<string>("unexpectedExpression");
            string message = "Message.";

            // act:
            DeferredEvaluationException<string> toTest = new DeferredEvaluationException<string>(unexpectedExpression, message);

            // assert:
            Assert.AreSame(unexpectedExpression, toTest.UnevaluatedExpression);
            Assert.AreSame(unexpectedExpression, (toTest as DeferredEvaluationException).UnevaluatedExpression);
            Assert.AreSame(typeof(string), (toTest as DeferredEvaluationException).ResultType);
            Assert.AreSame(message, toTest.Message);
        }

        #region implemented abstract members of EpicExceptionQABase

        protected override IEnumerable<DeferredEvaluationException<string>> ExceptionsToSerialize
        {
            get
            {
                string innerMessage = "Test inner.";
                string message = "Test message.";
                EquatableException inner = new EquatableException(innerMessage);
                yield return new DeferredEvaluationException<string>(new Fakes.FakeExpression<string>("unexpectedExpression"), message, inner);
            }
        }

        #endregion
    }
}

