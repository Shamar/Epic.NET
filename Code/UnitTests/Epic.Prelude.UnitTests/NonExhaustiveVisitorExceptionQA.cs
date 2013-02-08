//
//  NonExhaustiveVisitorExceptionQA.cs
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
using System.Collections.Generic;

namespace Epic
{
    [TestFixture()]
    public class NonExhaustiveVisitorExceptionQA : EpicExceptionQABase<NonExhaustiveVisitorException<string>>
    {
        #region implemented abstract members of EpicExceptionQABase        
        protected override IEnumerable<NonExhaustiveVisitorException<string>> ExceptionsToSerialize
        {
            get
            {
                yield return new NonExhaustiveVisitorException<string>("composition", "unexpectedExpression");
            }
        }
        #endregion

        [Test]
        public void Initialize_withArguments_works()
        {
            // arrange:
            string expression = "unexpectedExpression";
            string compositionName = "composition";

            // act:
            var toTest = new NonExhaustiveVisitorException<string>("composition", "unexpectedExpression");

            // assert:
            Assert.AreEqual(typeof(string), toTest.ExpressionType);
            Assert.AreSame(expression, toTest.UnknownExpression);
            Assert.AreSame(compositionName, toTest.VisitorCompositionName);
        }
    }
}

