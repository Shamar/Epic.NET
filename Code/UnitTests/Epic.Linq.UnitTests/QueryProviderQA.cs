//  
//  QueryProviderQA.cs
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
using NUnit.Framework;
using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;

namespace Epic.Linq.UnitTests
{
    [TestFixture]
    public class QueryProviderQA
    {
        [Test]
        public void Initialize_withoutName_throwArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
               new QueryProvider(null);
            });
            Assert.Throws<ArgumentNullException>(delegate {
               new QueryProvider(string.Empty);
            });
        }
        
        [TestCase("Default")]
        [TestCase("Custom")]
        [TestCase("Test1")]
        public void Initialize_withAName_works(string name)
        {
            // act:
            QueryProvider provider = new QueryProvider(name);

            // assert:
            Assert.AreEqual(name, provider.Name);
        }
        
        [Test]
        public void CreateQuery_withGenericArgument_returnCorrectQueryable()
        {
            // arrange:
            Expression<Func<string, int>> expression = (string s) => s.Length;
            IQueryProvider provider = new QueryProvider("test");

            // act:
            IQueryable<string> query = provider.CreateQuery<string>(expression);

            // assert:
            Assert.IsNotNull(query);
            Assert.IsInstanceOf<Queryable<string>>(query);
            Assert.AreSame(expression, query.Expression);
            Assert.AreSame(provider, query.Provider);
        }
        
        [Test]
        public void CreateQuery_withoutGenericArgument_returnCorrectQueryable()
        {
            // arrange:
            Expression expression = Expression.NewArrayInit (typeof (int));
            IQueryProvider provider = new QueryProvider("test");

            // act:
            IQueryable query = provider.CreateQuery(expression);

            // assert:
            Assert.IsNotNull(query);
            Assert.IsInstanceOf<Queryable<int>>(query);
            Assert.AreSame(expression, query.Expression);
            Assert.AreSame(provider, query.Provider);
        }
        
        [Test]
        public void CreateQuery_withExpressionNotProducingEnumerable_throwsArgumentException()
        {
            // arrange:
            Expression expression = Expression.Constant(0);
            IQueryProvider provider = new QueryProvider("test");

            // assert:
            Assert.Throws<ArgumentException>(delegate{provider.CreateQuery(expression);});
        }

    }
}

