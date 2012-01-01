//  
//  PredicateExtensionsQA.cs
//  
//  Author:
//       Marco <${AuthorEmail}>
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
using Epic.Linq.Fakes;
using Epic.Linq.Expressions.Relational;
using Epic.Linq.Expressions.Relational.Predicates;
using NUnit.Framework;

namespace Epic.Linq.Expressions.Relational.Predicates
{
    [TestFixture]
    public class PredicateExtensionsQA: RhinoMocksFixtureBase
    {
        [Test]
        public void AndMethod_FromScalarObjects_ReturnsPredicate()
        {
            // arrange:
            FakeScalar scalar1 = new FakeScalar(ScalarType.Constant);
            FakeScalar scalar2 = new FakeScalar(ScalarType.Constant);

            // act:
            Greater<FakeScalar, FakeScalar> greater = scalar1.Greater(scalar2);

            // assert:
            Assert.IsTrue (greater.Left.Equals (scalar1));
            Assert.IsTrue (greater.Right.Equals (scalar2));
        }
    }
}

