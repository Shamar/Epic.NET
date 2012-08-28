//  
//  IDeferrer.cs
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
using Epic.Specifications;
using System.Collections.Generic;
using Epic.Query.Object.Expressions;


namespace Epic.Query.Object
{
    /// <summary>
    /// Defines methods to create and execute queries that are described by an <see cref="IDeferred{TResult}"/> object.
    /// </summary>
    public interface IDeferrer
    {
        /// <summary>
        /// Defer the evaluation of the specified expression.
        /// </summary>
        /// <param name='expression'>
        /// Expression to defer.
        /// </param>
        /// <typeparam name='TDeferred'>
        /// The type of the <see cref="IDeferred{TResult}"/> to create.
        /// </typeparam>
        /// <typeparam name='TResult'>
        /// The result to defer.
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="expression"/> is <see langword="null"/>.</exception>
        /// <exception cref="DeferringException">This deferrer can not build a <typeparamref name='TDeferred'/>
        /// producing a <typeparamref name="TResult"/> on evaluation.</exception>
        TDeferred Defer<TDeferred, TResult>(Expression<TResult> expression)
            where TDeferred : IDeferred<TResult>;

        /// <summary>
        /// Evaluate the specified expression.
        /// </summary>
        /// <param name='expression'>
        /// Expression to evaluate.
        /// </param>
        /// <typeparam name='TResult'>
        /// Result of the evaluation of <paramref name="expression"/>.
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="expression"/> is <see langword="null"/>.</exception>
        TResult Evaluate<TResult>(Expression<TResult> expression);
    }
}

