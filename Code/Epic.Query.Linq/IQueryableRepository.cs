//  
//  IQueryableRepository.cs
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
    /// Common interface of repositories.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the base interface for repositories, in Epic.Query.Linq.
    /// </para>
    /// <para>
    /// It express the only feature that all repository should expose: queryability.
    /// </para>
    /// </remarks>
    /// <typeparam name="TEntity">Accessible entity.</typeparam>
    /// <typeparam name="TIdentity"><typeparamref name="TEntity"/>'s identifier.</typeparam>
    public interface IQueryableRepository<TEntity, TIdentity> : IRepository<TEntity, TIdentity>, IQueryable<TEntity>
        where TEntity : class
        where TIdentity : IEquatable<TIdentity>
    {
    }
}

