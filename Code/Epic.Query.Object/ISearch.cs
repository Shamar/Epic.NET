//  
//  ISearch.cs
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
using System.Collections.Generic;
using Epic.Specifications;
using Epic.Query.Object.Expressions;


namespace Epic.Query.Object
{
    /// <summary>
    /// Reppresent a search for a <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity of interest.</typeparam>
    /// <typeparam name="TIdentity">Type of the identity of <typeparamref name="TEntity"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// As a <see cref="IDeferred{TResult}"/>, this interface can be used to obtain a result
    /// with some extension method (see for example <see cref="Deferrable.Evaluate{TResult}"/> 
    /// and <see cref="Searchable.Identify{TEntity, TIdentity}"/>) but it can be used
    /// to build other deferreds to specialize the search.
    /// </para>
    /// </remarks>
    /// <seealso cref="IOrderedSearch{TEntity, TIdentity}"/>
    /// <seealso cref="ISlicedSearch{TEntity, TIdentity}"/>
    public interface ISearch<TEntity, TIdentity> : IDeferred<IEnumerable<TEntity>>
        where TEntity : class
        where TIdentity : IEquatable<TIdentity>
    {
        /// <summary>
        /// Selection expression.
        /// </summary>
        new Expression<IEnumerable<TEntity>> Expression { get; }
    }
}

