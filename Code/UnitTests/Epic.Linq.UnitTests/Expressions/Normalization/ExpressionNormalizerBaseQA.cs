//  
//  DefaultNormalizerQA.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2011 Giacomo Tesio
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
using System.Linq.Expressions;
using Epic.Linq.Fakes;

namespace Epic.Linq.Expressions.Normalization
{
    [TestFixture]
    public class ExpressionNormalizerBaseQA
    {
        [Test]
        public void Initalize_withoutAName_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new FakeNormalizer(null);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                new FakeNormalizer(string.Empty);
            });
        }
        
        [TestCase(0)]
        [TestCase(1)]
        [TestCase("testA")]
        [TestCase("testB")]
        public void Visit_aConstantValue_returnTheConstantItself<T>(T val)
        {
            // arrange:
            ConstantExpression constant = Expression.Constant(val);
            IVisitor<Expression, Expression> visitor = new FakeNormalizer("TEST");

            // act:
            Expression result = visitor.Visit(constant, VisitContext.New);

            // assert:
            Assert.AreSame(constant, result);
        }
    }
}

