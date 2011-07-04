//  
//  RepositoryBase.cs
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
using System.Collections;

namespace Epic.Linq
{
	[Serializable]
	public abstract class RepositoryBase<TEntity, TIdentity> : IRepository<TEntity, TIdentity>, IQueryable<TEntity>, IQueryable, IEnumerable<TEntity>, IEnumerable
		where TEntity : class
		where TIdentity : IEquatable<TIdentity>
	{
        private readonly string _providerName;
            
		[NonSerialized]
		private System.Linq.Expressions.Expression _expression;
            
        [NonSerialized]
        private IQueryProvider _provider;
		
        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Linq.RepositoryBase`2"/> class.
        /// </summary>
        /// <param name='providerName'>
        /// Provider name in the Environment.
        /// </param>
		protected RepositoryBase (string providerName)
		{
            if(string.IsNullOrEmpty(providerName))
                throw new ArgumentNullException("providerName");
            _providerName = providerName;
		}

		#region IRepository[TEntity,TIdentity] implementation
		public TEntity this[TIdentity identity] {
			get {
				throw new NotImplementedException ();
			}
		}
		#endregion

		#region IQueryable implementation
		/// <summary>
		/// Gets the expression tree that is associated with the instance of IQueryable.
		/// </summary>
		/// <value>
		/// The expression.
		/// </value>
		public System.Linq.Expressions.Expression Expression {
			get 
            {
				if(null == _expression)
				{
					_expression = System.Linq.Expressions.Expression.Constant(this);
				}
				return _expression;
			}
		}
		
		/// <summary>
		/// Gets the type of the element(s) that are returned when the expression tree associated with this instance of IQueryable is executed.
		/// </summary>
		/// <value>
		/// The type of the element.
		/// </value>
		public Type ElementType {
			get {
				return typeof(TEntity);
			}
		}
		
		/// <summary>
		/// Gets the query provider that is associated with this data source.
		/// </summary>
		/// <value>
		/// The provider.
		/// </value>
		public IQueryProvider Provider {
			get {
                if(null == _provider)
                {
                    _provider = Application.Environment.Get<IQueryProvider>(new InstanceName<IQueryProvider>(_providerName));
                }
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
		public IEnumerator<TEntity> GetEnumerator ()
		{
			return Provider.Execute<IEnumerable<TEntity>>(Expression).GetEnumerator();
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

