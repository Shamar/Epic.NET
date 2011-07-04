//  
//  ReflectionQA.cs
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
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Epic.Linq.UnitTests
{
    public interface ITestEnumerable<T> : IOrderedEnumerable<T>
    {
    }
    
    public interface IClosedTestEnumerable : ITestEnumerable<string>
    {
    }
    
    [TestFixture()]
    public class ReflectionQA
    {
        [Test]
        public void TryGetItemTypeOfEnumerable_withoutEnumerableType_throwsArgumentNullException()
        {
            // arrange:
            Type elementType = null;

            // assert:
            Assert.Throws<ArgumentNullException>(delegate { Reflection.TryGetItemTypeOfEnumerable(null, out elementType); });
        }
        
        [TestCase(typeof(IEnumerable<string>), typeof(string))]
        [TestCase(typeof(IEnumerable<int>), typeof(int))]
        [TestCase(typeof(IEnumerable<IEnumerable<string>>), typeof(IEnumerable<string>))]
        [TestCase(typeof(IEnumerable), typeof(object))]
        [TestCase(typeof(int[]), typeof(int))]
        [TestCase(typeof(IQueryable<string>), typeof(string))]
        [TestCase(typeof(IQueryable<int>), typeof(int))]
        [TestCase(typeof(IQueryable<IEnumerable<string>>), typeof(IEnumerable<string>))]
        [TestCase(typeof(IQueryable<IQueryable<string>>), typeof(IQueryable<string>))]
        [TestCase(typeof(ITestEnumerable<DateTime>), typeof(DateTime))]
        [TestCase(typeof(IClosedTestEnumerable), typeof(string))]
        public void TryGetItemTypeOfEnumerable_withEnumerableTypes_returnTheExpectedItem(Type enumerable, Type expectedItemType)
        {
            // arrange:
            Type itemType = null;

            // act:
            bool found = Reflection.TryGetItemTypeOfEnumerable(enumerable, out itemType);

            // assert:
            Assert.IsTrue(found);
            Assert.AreEqual(expectedItemType, itemType);
        }
        
        [TestCase(typeof(int))]
        [TestCase(typeof(DateTime))]
        [TestCase(typeof(object))]
        public void TryGetItemTypeOfEnumerable_withNotEnumerableTypes_returnFalseWithoutSettingTheElementType(Type notEnumerable)
        {
            // arrange:
            Type itemType = null;

            // act:
            bool found = Reflection.TryGetItemTypeOfEnumerable(notEnumerable, out itemType);

            // assert:
            Assert.IsFalse(found);
            Assert.IsNull(itemType);
        }
    }
}

