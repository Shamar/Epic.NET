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
    public static class Searchable
    {
        public static IEnumerable<TEntity> AsEnumerable<TEntity, TIdentity>(this ISearch<TEntity, TIdentity> search)
            where TEntity : class
            where TIdentity : IEquatable<TIdentity>
        {
            if(null == search)
                throw new ArgumentNullException("search");
            return search.Deferrer.Evaluate(search.Expression);
        }

        public static uint Count<TEntity, TIdentity>(this ISearch<TEntity, TIdentity> search)
            where TEntity : class
            where TIdentity : IEquatable<TIdentity>
        {
            if(null == search)
                throw new ArgumentNullException("search");
            return search.Deferrer.Evaluate(new Count<TEntity>(search.Expression));
        }

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

        public static ILimitedSearch<TEntity, TIdentity> Take<TEntity, TIdentity>(this IOrderedSearch<TEntity, TIdentity> search, uint count)
            where TEntity : class
            where TIdentity : IEquatable<TIdentity>
        {
            if(null == search)
                throw new ArgumentNullException("search");
            
            return search.Deferrer.Defer<ILimitedSearch<TEntity, TIdentity>, IEnumerable<TEntity>>(new Limits<TEntity>(search.Expression, count));
        }

        public static ILimitedSearch<TEntity, TIdentity> After<TEntity, TIdentity>(this ILimitedSearch<TEntity, TIdentity> search, uint count)
            where TEntity : class
            where TIdentity : IEquatable<TIdentity>
        {
            if(null == search)
                throw new ArgumentNullException("search");
            if(count == search.Expression.Offset)
                return search;
            return search.Deferrer.Defer<ILimitedSearch<TEntity, TIdentity>, IEnumerable<TEntity>>(new Limits<TEntity>(search.Expression.Source, search.Expression.Limit, count));
        }    
    }
}

