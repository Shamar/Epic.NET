//
//  Queryable.cs
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
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace Epic.Query.Linq
{
	/// <summary>
	/// Queryable class.
	/// </summary>
	/// <exception cref='ArgumentNullException'>
	/// Is thrown when an argument passed to a method is invalid because it is <see langword="null" /> .
	/// </exception>
	/// <typeparam name="T">The type of the data in the source.</typeparam>
	internal sealed class Queryable<T> : IQueryable<T>, IQueryable, IEnumerable<T>, IEnumerable, IOrderedQueryable<T>, IOrderedQueryable
	{
		private readonly IQueryProvider _provider;
		private readonly System.Linq.Expressions.Expression _expression;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Queryable{T}"/> class.
		/// </summary>
		/// <param name='provider'>
		/// Provider.
		/// </param>
		/// <param name='expression'>
		/// Expression.
		/// </param>
		/// <exception cref='ArgumentNullException'>
		/// Is thrown when an argument passed to a method is invalid because it is <see langword="null" /> .
		/// </exception>
		public Queryable (IQueryProvider provider, System.Linq.Expressions.Expression expression)
		{
			if(null == provider)
				throw new ArgumentNullException("provider");
			if(null == expression)
				throw new ArgumentNullException("expression");
			_provider = provider;
			_expression = expression;
		}

		#region IQueryable implementation
		/// <summary>
		/// Gets the expression tree that is associated with the instance of IQueryable.
		/// </summary>
		public System.Linq.Expressions.Expression Expression {
			get {
				return _expression;
			}
		}
		
		/// <summary>
		/// Gets the type of the element(s) that are returned when the expression tree associated with this instance of IQueryable is executed.
		/// </summary>
		public Type ElementType {
			get {
				return typeof(T);
			}
		}
		
		/// <summary>
		/// Gets the query provider that is associated with this data source.
		/// </summary>
		public IQueryProvider Provider {
			get {
				return _provider;
			}
		}
		#endregion

		#region IEnumerable[T] implementation
		/// <summary>
		/// Returns an enumerator that iterates through a collection of <typeparamref name="T"/>.
		/// </summary>
		/// <returns>
		/// The enumerator.
		/// </returns>
		public IEnumerator<T> GetEnumerator ()
		{
			return _provider.Execute<IEnumerable<T>>(_expression).GetEnumerator();
		}
		#endregion

		#region IEnumerable implementation
		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// The enumerator.
		/// </returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}
		#endregion
	}
}

