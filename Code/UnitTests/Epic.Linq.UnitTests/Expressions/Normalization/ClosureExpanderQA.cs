//  
//  ClosureExpanderQA.cs
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
using System.Linq.Expressions;
using Epic.Linq.Fakes;

namespace Epic.Linq.Expressions.Normalization
{
    [TestFixture()]
    public class ClosureExpanderQA
    {
        [Test]
        public void Initialize_withoutComposition_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new ClosureExpander(null);
            });
        }
        
        [Test]
        public void Visit_aLocalVariable_returnsAnExpressionWithTheVariableReplacedWithItsValue()
        {
            // arrange:
            int i = 1;
            Expression<Func<int>> expressionToVisit = () => i;
            FakeNormalizer composition = new FakeNormalizer("test");
            new ClosureExpander(composition);

            // act:
            Expression result = composition.Visit(expressionToVisit, VisitContext.New);

            // assert:
            Verify.That(result).IsA<Expression<Func<int>>>()
                .WithA(e => e.Body, that => Verify.That(that).IsA<ConstantExpression>()
                                                             .WithA(c => c.Value, value => Is.EqualTo(i)));
        }
        
        private readonly int _integerField = 2;
        [Test]
        public void Visit_aLocalField_returnsAnExpressionWithTheVariableReplacedWithItsValue()
        {
            // arrange:
            Expression<Func<int>> expressionToVisit = () => _integerField;
            FakeNormalizer composition = new FakeNormalizer("test");
            new ClosureExpander(composition);

            // act:
            Expression result = composition.Visit(expressionToVisit, VisitContext.New);

            // assert:
            Verify.That(result).IsA<Expression<Func<int>>>()
                .WithA(e => e.Body, that => Verify.That(that).IsA<ConstantExpression>()
                                                             .WithA(c => c.Value, value => Is.EqualTo(_integerField)));
        }
        
        private int IntegerProperty
        {
            get { return _integerField + 1; }
        }
        
        [Test]
        public void Visit_aLocalProperty_returnsAnExpressionWithTheVariableReplacedWithItsValue()
        {
            // arrange:
            Expression<Func<int>> expressionToVisit = () => IntegerProperty;
            FakeNormalizer composition = new FakeNormalizer("test");
            new ClosureExpander(composition);

            // act:
            Expression result = composition.Visit(expressionToVisit, VisitContext.New);

            // assert:
            Verify.That(result).IsA<Expression<Func<int>>>()
                .WithA(e => e.Body, that => Verify.That(that).IsA<ConstantExpression>()
                                                             .WithA(c => c.Value, value => Is.EqualTo(IntegerProperty)));
        }
        
        private int ThrowingProperty
        {
            get { throw new Exception(); }
        }
        
        [Test]
        public void Visit_aPropertyThatCantProvideAValue_dontReplaceTheExpression()
        {
            // arrange:
            Expression<Func<int>> expressionToVisit = () => ThrowingProperty;
            FakeNormalizer composition = new FakeNormalizer("test");
            new ClosureExpander(composition);

            // act:
            Expression result = composition.Visit(expressionToVisit, VisitContext.New);

            // assert:
            Assert.That(result, Is.SameAs(expressionToVisit));
        }

        [Test]
        public void Visit_aStaticProperty_dontReplaceTheExpression()
        {
            // arrange:
            Expression<Func<DateTime>> expressionToVisit = () => DateTime.Now;
            FakeNormalizer composition = new FakeNormalizer("test");
            new ClosureExpander(composition);

            // act:
            Expression result = composition.Visit(expressionToVisit, VisitContext.New);

            // assert:
            Assert.That(result, Is.SameAs(expressionToVisit));
        }

        private static readonly decimal StaticDecimalField = 10m;
        [Test]
        public void Visit_aStaticFieldOfAnExplicitlyNamedClass_dontReplaceTheExpression()
        {
            // arrange:
            Expression<Func<decimal>> expressionToVisit = () => ClosureExpanderQA.StaticDecimalField;
            FakeNormalizer composition = new FakeNormalizer("test");
            new ClosureExpander(composition);

            // act:
            Expression result = composition.Visit(expressionToVisit, VisitContext.New);

            // assert:
            Assert.That(result, Is.SameAs(expressionToVisit));
        }
    }
}

