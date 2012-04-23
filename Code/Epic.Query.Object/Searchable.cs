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
namespace Epic.Query.Object
{
    public static class Searchable
    {
        public static int Count<TEntity, TIdentity>(this IQuery<TEntity, TIdentity> query)
            where TEntity : class
            where TIdentity : IEquatable<TIdentity>
        {
            if(null == query)
                throw new ArgumentNullException("query");
            return query.Provider.Count(query.Specification);
        }
    }
}

