//
//  DeferrerBase.cs
//
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
//
//  Copyright (c) 2010-2013 Giacomo Tesio
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
using Epic.Query.Object.Expressions;
using Epic.Math;

namespace Epic.Query.Object
{
    /// <summary>
    /// Base class for deferrers.
    /// </summary>
    public abstract class DeferrerBase : IDeferrer
    {
        /// <summary>
        /// Name of the deferrer (useful for logs and exceptions).
        /// </summary>
        protected readonly string Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Query.Object.DeferrerBase"/> class.
        /// </summary>
        /// <param name='name'>
        /// Name of the deferrer.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        protected DeferrerBase(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            Name = name;
        }

        #region IDeferrer implementation

        /// <summary>
        /// Defer the evaluation of the specified expression.
        /// </summary>
        /// <param name='expression'>
        /// Expression to defer.
        /// </param>
        /// <returns>A <typeparamref name="TDeferred"/> representing a computation that will produce a <typeparamref name="TResult"/>.</returns>
        /// <typeparam name='TDeferred'>
        /// The type of the <see cref="IDeferred{TResult}"/> to create.
        /// </typeparam>
        /// <typeparam name='TResult'>
        /// The result to defer.
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="expression"/> is <see langword="null"/>.</exception>
        /// <exception cref="DeferringException">This deferrer can not build a <typeparamref name='TDeferred'/>
        /// producing a <typeparamref name="TResult"/> on evaluation.</exception>
        public TDeferred Defer<TDeferred, TResult>(Expression<TResult> expression) where TDeferred : IDeferred<TResult>
        {
            IMapping<Expression<TResult>, TDeferred> mapping = this as IMapping<Expression<TResult>, TDeferred>;
            if (null == mapping)
            {
                string message = string.Format("Cannot defer a {0} computing a {1}. No mapping exists in deferrer '{2}'.", typeof(TDeferred), typeof(TResult), Name);
                throw new DeferringException<TDeferred, TResult>(message);
            }
            return mapping.ApplyTo(expression);
        }

        /// <summary>
        /// Evaluate the specified expression.
        /// </summary>
        /// <param name='expression'>
        /// Expression to evaluate.
        /// </param>
        /// <returns>The result of the computation represented by <paramref name="expression"/>.</returns>
        /// <typeparam name='TResult'>
        /// Result of the evaluation of <paramref name="expression"/>.
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="expression"/> is <see langword="null"/>.</exception>
        public TResult Evaluate<TResult>(Expression<TResult> expression)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

