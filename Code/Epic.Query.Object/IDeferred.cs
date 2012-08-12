//  
//  IDeferred.cs
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
using Epic.Query.Object.Expressions;

namespace Epic.Query.Object
{
    /// <summary>
    /// Represents a description of a required <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TResult">
    /// Type of the result required.
    /// </typeparam>
    /// <remarks>
    /// <para>
    /// A deferred can be seen as a value which has not 
    /// been computed yet. Its <see cref="IDeferred{TResult}.Expression"/> 
    /// declaratively describes the qualities of the 
    /// <typeparamref name="TResult"/>. Such expression is defined by
    /// the <see cref="IDeferrer.Defer{TDeferred, TResult}"/> method of the 
    /// <see cref="IDeferred{TResult}.Deferrer"/>.
    /// </para>
    /// <para>
    /// Different extension methods can be defined to produce different
    /// kind of deferreds. For example, some common search operations are
    /// defined in the <see cref="Searchable"/> static class. Such extentions
    /// will create new deferreds extending <see cref="IDeferred{TResult}"/>
    /// (like <see cref="ISearch{TEntity, TIdentity}"/>, 
    /// <see cref="IOrderedSearch{TEntity, TIdentity}"/> and
    /// <see cref="ISlicedSearch{TEntity, TIdentity}"/>) using specialized
    /// <see cref="Expressions.Expression{TResult}"/> for their specific purposes.
    /// </para>
    /// <para>
    /// The <see cref="IDeferred{TResult}"/> can be evaluated when 
    /// the <typeparamref name="TResult"/> is actually needed, through
    /// the <see cref="Deferrable.Evaluate{TResult}"/> method. 
    /// </para>
    /// </remarks>
    public interface IDeferred<TResult>
    {
        /// <summary>
        /// Deferred expression.
        /// </summary>
        Expression<TResult> Expression { get; }

        /// <summary>
        /// Deferrer that created and can evaluated <see cref="IDeferred{TResult}.Expression"/>.
        /// </summary>
        IDeferrer Deferrer { get; }
    }
}

