//  
//  QueryableQA.cs
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
using Rhino.Mocks;
using System.Linq;
using System.Linq.Expressions;
using Epic.Query.Linq;
using System.Collections.Generic;
using System.Collections;

namespace Epic.Query.Linq
{
	[TestFixture]
	public class QueryableQA : RhinoMocksFixtureBase
	{
		[Test]
		public void Ctor_withoutArguments_throwsArgumentNullException ()
		{
			// arrange:
			IQueryProvider provider = GenerateStrictMock<IQueryProvider>();
			Expression expression = Expression.Constant(1);
			
			// assert:
			Assert.Throws<ArgumentNullException>(delegate {
				new Queryable<string>(null, expression);
			});
			Assert.Throws<ArgumentNullException>(delegate {
				new Queryable<string>(provider, null);
			});
		}
		
		[Test]
		public void Ctor_withArguments_works()
		{
			// arrange:
			IQueryProvider provider = GenerateStrictMock<IQueryProvider>();
			Expression expression = Expression.Constant(1);

			// act:
			IQueryable<string> queryable = new Queryable<string>(provider, expression);

			// assert:
			Assert.IsNotNull(queryable);
			Assert.AreSame(expression, queryable.Expression);
			Assert.AreSame(provider, queryable.Provider);
			Assert.AreEqual(typeof(string), queryable.ElementType);
		}
		
		[Test]
		public void GetEnumerator_fromGenericEnumerable_callProviderExecute()
		{
			// arrange:
			string[] strings = new string[] { "A", "B", "C" };
			Expression expression = Expression.Constant(1);
			IQueryProvider provider = GenerateStrictMock<IQueryProvider>();
			provider.Expect(p => p.Execute<IEnumerable<string>>(expression)).Return(strings).Repeat.Once();
			IQueryable<string> queryable = new Queryable<string>(provider, expression);

			// assert:
			int i = 0;
			foreach(string s in queryable)
			{
				Assert.AreSame(s, strings[i++]);
			}
		}
		
		[Test]
		public void GetEnumerator_fromEnumerable_callProviderExecute()
		{
			// arrange:
			string[] strings = new string[] { "A", "B", "C" };
			Expression expression = Expression.Constant(1);
			IQueryProvider provider = GenerateStrictMock<IQueryProvider>();
			provider.Expect(p => p.Execute<IEnumerable<string>>(expression)).Return(strings).Repeat.Once();
			IQueryable<string> queryable = new Queryable<string>(provider, expression);
			IEnumerable source = queryable;

			// assert:
			int i = 0;
			foreach(object s in source)
			{
				Assert.AreSame(s, strings[i++]);
			}
		}

	}
}

