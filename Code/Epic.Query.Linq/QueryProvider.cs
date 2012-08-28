//  
//  QueryProvider.cs
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

namespace Epic.Query.Linq
{
	/// <summary>
	/// Query provider.
	/// </summary>
	/// <exception cref='ArgumentNullException'>
	/// Is thrown when an argument passed to a method is invalid because it is <see langword="null" /> .
	/// </exception>
	/// <exception cref='ArgumentException'>
	/// Is thrown when an argument passed to a method is invalid.
	/// </exception>
	internal sealed class QueryProvider : IQueryProvider
	{
		private readonly string _name;
        

		/// <summary>
		/// Initializes a new instance of the <see cref="Epic.Query.Linq.QueryProvider"/> class.
		/// </summary>
		/// <param name='name'>
		/// Name of the query provider. Used to get tools from the <see cref="Epic.IEnvironment"/>.
		/// </param>
		/// <exception cref='ArgumentNullException'>
		/// Is thrown when an argument passed to a method is invalid because it is <see langword="null" /> .
		/// </exception>
		public QueryProvider (string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			_name = name;
		}

        /// <summary>
        /// Returns the name of the provider.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }
     
        #region IQueryProvider implementation
		/// <summary>
		/// Constructs an IQueryable object that can evaluate the query represented by a specified expression tree.
		/// </summary>
		/// <remarks>
		/// Several of the standard query operator methods defined in Queryable, such as <see cref="Queryable.OfType{TResult}"/> 
        /// and <see cref="Queryable.Cast{TResult}"/>, call this method. They pass it a MethodCallExpression that represents a LINQ query.
		/// </remarks>
		/// <returns>
		/// The query.
		/// </returns>
		/// <param name='expression'>
		/// An expression tree that represents a LINQ query.
		/// </param>
		/// <exception cref='ArgumentException'>
        /// Is thrown when <paramref name="expression"/> is not assignable from <see cref="System.Collections.Generic.IEnumerable{T}"/>.
		/// </exception>
		public IQueryable CreateQuery (System.Linq.Expressions.Expression expression)
		{
			Type element = null;
			if(! Reflection.TryGetItemTypeOfEnumerable(expression.Type, out element))
			{
				throw new ArgumentException ("The expression provided does not produce an IEnumerable.", "expression");
			}
			return (IQueryable) Activator.CreateInstance(typeof(Queryable<>).MakeGenericType(element), new object[] {this, expression});
		}

		public object Execute (System.Linq.Expressions.Expression expression)
		{
			throw new NotImplementedException ();
		}
		
		/// <summary>
		/// Creates the query.
		/// </summary>
		/// <returns>
		/// The query.
		/// </returns>
		/// <param name='expression'>
		/// An expression tree that represents a LINQ query.
		/// </param>
		/// <typeparam name='TElement'>
		/// The element's type.
		/// </typeparam>
		IQueryable<TElement> IQueryProvider.CreateQuery<TElement> (System.Linq.Expressions.Expression expression)
		{
			return new Queryable<TElement>(this, expression);
		}

		TResult IQueryProvider.Execute<TResult> (System.Linq.Expressions.Expression expression)
		{
			throw new NotImplementedException ();
		}
		#endregion
	}
}

