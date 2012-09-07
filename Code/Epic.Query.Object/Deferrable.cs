//  
//  Deferrable.cs
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
namespace Epic.Query.Object
{
    /// <summary>
    /// Provides an extension method for evaluating deferreds results.
    /// </summary>
    public static class Deferrable
    {
        /// <summary>
        /// Evaluate the deferred, returning the results.
        /// </summary>
        /// <returns>
        /// A <typeparamref name="TResult"/> resulting from the deferral.
        /// </returns>
        /// <param name='deferred'>
        /// A deferred <typeparamref name="TResult"/>.
        /// </param>
        /// <typeparam name='TResult'>
        /// The type of the results.
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="deferred"/> 
        /// is <see langword="null"/>.</exception>
        public static TResult Evaluate<TResult>(this IDeferred<TResult> deferred)
        {
            if(null == deferred)
                throw new ArgumentNullException("deferred");
            return deferred.Deferrer.Evaluate(deferred.Expression);
        }
    }
}

