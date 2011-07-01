//  
//  Queryable.cs
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
using System.Linq;
using System.Collections.Generic;

namespace Epic.Linq
{
	public sealed class Queryable<T> : IQueryable<T>, IQueryable, IEnumerable<T>, IEnumerable, IOrderedQueryable<T>, IOrderedQueryable
	{
		private readonly IQueryProvider _provider;
		private readonly Expression _expression;
		public Queryable (IQueryProvider provider, Expression expression)
		{
			if(null == provider)
				throw new ArgumentNullException("provider");
			if(null == expression)
				throw new ArgumentNullException("expression");
			_provider = provider;
			_expression = expression;
		}

		#region IQueryable implementation
		public System.Linq.Expressions.Expression Expression {
			get {
				return _expression;
			}
		}

		public Type ElementType {
			get {
				return typeof(T);
			}
		}

		public IQueryProvider Provider {
			get {
				return _provider;
			}
		}
		#endregion

		#region IEnumerable[T] implementation
		public IEnumerator<T> GetEnumerator ()
		{
			return _provider.Execute<IEnumerable<T>>(_expression).GetEnumerator();
		}
		#endregion

		#region IEnumerable implementation
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}
		#endregion
	}
}

