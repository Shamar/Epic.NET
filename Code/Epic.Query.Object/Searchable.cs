//  
//  Searchable.cs
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
using Epic.Query.Object.Expressions;


namespace Epic.Query.Object
{
    /// <summary>
    /// Provides a set of static methods for querying data structures that implement <see cref="IDeferred{TResult}"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The set of methods declared in the Searchable class provides an implementation 
    /// of the standard query operators for querying deferred query objects
    /// that implement any interface derived from <see cref="IDeferred{TResult}"/>. 
    /// The standard query operators are general purpose methods that follow the 
    /// <see href="http://martinfowler.com/eaaCatalog/queryObject.html">Query Object pattern</see>
    /// and enable you to express filter and projection operations over domain entities.
    /// </para>
    /// <para>
    /// The majority of the methods in this class are defined as extension methods that
    /// extend the <see cref="ISearch{TEntity, TIdentity}"/> type and its specializations. 
    /// This means they can be called like an instance method on any object that 
    /// implements <see cref="ISearch{TEntity, TIdentity}"/>. 
    /// These methods that extend <see cref="IDeferred{TResult}"/> specializations 
    /// (like <see cref="ISearch{TEntity, TIdentity}"/>, <see cref="IOrderedSearch{TEntity, TIdentity}"/>
    /// and so on) do not perform any querying directly. 
    /// Instead, their functionality is to build an 
    /// <see cref="Expression{TResult}"/> object, which is an expression tree that 
    /// represents the cumulative request. The methods then pass the new expression 
    /// tree to either the <see cref="IDeferrer.Evaluate{TResult}"/> method or the 
    /// <see cref="IDeferrer.Defer{TDeferred, TResult}"/>  method of the input 
    /// <see cref="IDeferred{TResult}"/>. 
    /// The method that is called depends on whether the <see cref="Searchable"/> 
    /// method returns an actual value, in which case 
    /// <see cref="IDeferrer.Evaluate{TResult}"/> is called, or has deferred results, 
    /// in which case <see cref="IDeferrer.Defer{TDeferred, TResult}"/> is called.
    /// </para>
    /// <para>
    /// The actual query execution on the target data is performed when the actual value
    /// is needed. For example <see cref="Searchable.Count{TItem}"/> 
    /// actually execute the count and <see cref="Searchable.Identify{TEntity, TIdentity}"/> 
    /// retrieves and returns the identities of the required entities.
    /// </para>
    /// </remarks>
    public static class Searchable
    {
        /// <summary>
        /// Evaluate the deferred search, returning the identities of the results.
        /// </summary>
        /// <returns>
        /// An enumerable of the <typeparamref name="TIdentity"/> resulting from the search.
        /// </returns>
        /// <param name='search'>
        /// A search for <typeparamref name="TEntity"/>.
        /// </param>
        /// <typeparam name='TEntity'>
        /// Type of the searched entities.
        /// </typeparam>
        /// <typeparam name='TIdentity'>
        /// Type of the identity of <typeparamref name="TEntity"/>s
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="search"/> 
        /// is <see langword="null"/>.</exception>
        public static IEnumerable<TIdentity> Identify<TEntity, TIdentity>(this ISearch<TEntity, TIdentity> search)
            where TEntity : class
            where TIdentity : IEquatable<TIdentity>
        {
            if(null == search)
                throw new ArgumentNullException("search");
            IDeferred<IEnumerable<TEntity>> deferred = search;
            return search.Deferrer.Evaluate(new Identification<TEntity, TIdentity>(deferred.Expression));
        }

        /// <summary>
        /// Count the results of the specified search.
        /// </summary>
        /// <param name='search'>
        /// A search for <typeparamref name="TItem"/>.
        /// </param>
        /// <typeparam name='TItem'>
        /// The type of the results.
        /// </typeparam>
        /// <returns>
        /// The number of results that the specified search will return.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="search"/> 
        /// is <see langword="null"/>.</exception>
        public static uint Count<TItem>(this IDeferred<IEnumerable<TItem>> search)
        {
            if(null == search)
                throw new ArgumentNullException("search");
            return search.Deferrer.Evaluate(new Count<TItem>(search.Expression));
        }

        /// <summary>
        /// Sorts the entities according to a <paramref name="criterion"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="IOrderedSearch{TEntity, TIdentity}"/> whose elements are 
        /// sorted according to the <paramref name="criterion"/> provided.
        /// </returns>
        /// <param name='search'>
        /// A search for <typeparamref name="TEntity"/>.
        /// </param>
        /// <param name='criterion'>
        /// Order criterion.
        /// </param>
        /// <typeparam name='TEntity'>
        /// The type of the entities searched by <paramref name="search"/>.
        /// </typeparam>
        /// <typeparam name='TIdentity'>
        /// The type of the identities of the entities searched by <paramref name="search"/>.
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="search"/> or 
        /// <paramref name="criterion"/> is <see langword="null"/>.</exception>
        public static IOrderedSearch<TEntity, TIdentity> OrderBy<TEntity, TIdentity>(this ISearch<TEntity, TIdentity> search, OrderCriterion<TEntity> criterion)
            where TEntity : class
            where TIdentity : IEquatable<TIdentity>
        {
            if(null == search)
                throw new ArgumentNullException("search");
            if(null == criterion)
                throw new ArgumentNullException("criterion");
            return search.Deferrer.Defer<IOrderedSearch<TEntity, TIdentity>, IEnumerable<TEntity>>(new Order<TEntity>(search.Expression, criterion));
        }

        /// <summary>
        /// Performs a subsequent ordering of the entities in a 
        /// <see cref="IOrderedSearch{TEntity, TIdentity}"/> according to <paramref name="criterion"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="IOrderedSearch{TEntity, TIdentity}"/> whose entities are 
        /// sorted according to the <paramref name="criterion"/> provided.
        /// </returns>
        /// <param name='search'>
        /// An <see cref="IOrderedSearch{TEntity, TIdentity}"/> that contains entities
        /// to sort.
        /// </param>
        /// <param name='criterion'>
        /// Order criterion.
        /// </param>
        /// <typeparam name='TEntity'>
        /// The type of the entities searched by <paramref name="search"/>.
        /// </typeparam>
        /// <typeparam name='TIdentity'>
        /// The type of the identities of the entities searched by <paramref name="search"/>.
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="search"/> or 
        /// <paramref name="criterion"/> is <see langword="null"/>.</exception>
        public static IOrderedSearch<TEntity, TIdentity> ThenBy<TEntity, TIdentity>(this IOrderedSearch<TEntity, TIdentity> search, OrderCriterion<TEntity> criterion)
            where TEntity : class
            where TIdentity : IEquatable<TIdentity>
        {
            if(null == search)
                throw new ArgumentNullException("search");
            if(null == criterion)
                throw new ArgumentNullException("criterion");
            return search.Deferrer.Defer<IOrderedSearch<TEntity, TIdentity>, IEnumerable<TEntity>>(search.Expression.ThanBy(criterion));
        }

        /// <summary>
        /// Returns a specified number of results from a search.
        /// </summary>
        /// <param name='search'>
        /// A search for <typeparamref name="TEntity"/>.
        /// </param>
        /// <param name='count'>
        /// The number of results to return.
        /// </param>
        /// <returns>
        /// An <see cref="ISlicedSearch{TEntity, TIdentity}"/> that returns
        /// at most <paramref name="count"/> results.
        /// </returns>
        /// <typeparam name='TEntity'>
        /// The type of the entities searched by <paramref name="search"/>.
        /// </typeparam>
        /// <typeparam name='TIdentity'>
        /// The type of the identities of the entities searched by <paramref name="search"/>.
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="search"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is <c>0</c>.</exception>
        public static ISlicedSearch<TEntity, TIdentity> Take<TEntity, TIdentity>(this IOrderedSearch<TEntity, TIdentity> search, uint count)
            where TEntity : class
            where TIdentity : IEquatable<TIdentity>
        {
            if(null == search)
                throw new ArgumentNullException("search");
            
            return search.Deferrer.Defer<ISlicedSearch<TEntity, TIdentity>, IEnumerable<TEntity>>(new Slice<TEntity>(search.Expression, count));
        }

        /// <summary>
        /// Bypasses a specified number of results in a search and then returns the remaining elements.
        /// </summary>
        /// <param name='search'>
        /// A search for <typeparamref name="TEntity"/>.
        /// </param>
        /// <param name='count'>
        /// The number of elements to skip before returning the remaining results.
        /// </param>
        /// <returns>
        /// An <see cref="ISlicedSearch{TEntity, TIdentity}"/> that skip
        /// <paramref name="count"/> results.
        /// </returns>
        /// <typeparam name='TEntity'>
        /// The type of the entities searched by <paramref name="search"/>.
        /// </typeparam>
        /// <typeparam name='TIdentity'>
        /// The type of the identities of the entities searched by <paramref name="search"/>.
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="search"/> is <see langword="null"/>.</exception>
        public static ISlicedSearch<TEntity, TIdentity> Skip<TEntity, TIdentity>(this IOrderedSearch<TEntity, TIdentity> search, uint count)
            where TEntity : class
            where TIdentity : IEquatable<TIdentity>
        {
            if(null == search)
                throw new ArgumentNullException("search");
            
            return search.Deferrer.Defer<ISlicedSearch<TEntity, TIdentity>, IEnumerable<TEntity>>(new Slice<TEntity>(count, search.Expression));
        }

        /// <summary>
        /// Returns a specified number of results from a search.
        /// </summary>
        /// <param name='search'>
        /// A search for <typeparamref name="TEntity"/>.
        /// </param>
        /// <param name='count'>
        /// The number of results to return.
        /// </param>
        /// <returns>
        /// An <see cref="ISlicedSearch{TEntity, TIdentity}"/> that returns
        /// at most <paramref name="count"/> results.
        /// </returns>
        /// <typeparam name='TEntity'>
        /// The type of the entities searched by <paramref name="search"/>.
        /// </typeparam>
        /// <typeparam name='TIdentity'>
        /// The type of the identities of the entities searched by <paramref name="search"/>.
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="search"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is <c>0</c>.</exception>
        public static ISlicedSearch<TEntity, TIdentity> Take<TEntity, TIdentity>(this ISlicedSearch<TEntity, TIdentity> search, uint count)
            where TEntity : class
            where TIdentity : IEquatable<TIdentity>
        {
            if(null == search)
                throw new ArgumentNullException("search");
            if(count == search.Expression.TakingAtMost)
                return search;
            return search.Deferrer.Defer<ISlicedSearch<TEntity, TIdentity>, IEnumerable<TEntity>>(new Slice<TEntity>(search.Expression.Source, search.Expression.Skipping, count));
        }    

        /// <summary>
        /// Bypasses a specified number of results in a search and then returns the remaining elements.
        /// </summary>
        /// <param name='search'>
        /// A search for <typeparamref name="TEntity"/>.
        /// </param>
        /// <param name='count'>
        /// The number of elements to skip before returning the remaining results.
        /// </param>
        /// <returns>
        /// An <see cref="ISlicedSearch{TEntity, TIdentity}"/> that skip
        /// <paramref name="count"/> results.
        /// </returns>
        /// <typeparam name='TEntity'>
        /// The type of the entities searched by <paramref name="search"/>.
        /// </typeparam>
        /// <typeparam name='TIdentity'>
        /// The type of the identities of the entities searched by <paramref name="search"/>.
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="search"/> is <see langword="null"/>.</exception>
        public static ISlicedSearch<TEntity, TIdentity> Skip<TEntity, TIdentity>(this ISlicedSearch<TEntity, TIdentity> search, uint count)
            where TEntity : class
            where TIdentity : IEquatable<TIdentity>
        {
            if(null == search)
                throw new ArgumentNullException("search");
            if(count == search.Expression.Skipping)
                return search;
            return search.Deferrer.Defer<ISlicedSearch<TEntity, TIdentity>, IEnumerable<TEntity>>(new Slice<TEntity>(search.Expression.Source, count, search.Expression.TakingAtMost));
        }    
    }
}

