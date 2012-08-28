//
//  IIdentityMap.cs
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

namespace Epic.Collections
{
    /// <summary>
    /// Represent an Identity Map.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity of interest.</typeparam>
    /// <typeparam name="TIdentity">Type of the identity of <typeparamref name="TEntity"/>.</typeparam>
    public interface IIdentityMap<TEntity, TIdentity> :  IMap<TIdentity, TEntity>, IDisposable
        where TEntity : class
        where TIdentity : IEquatable<TIdentity>
    {
        /// <summary>
        /// Register in the map the specified entity.
        /// </summary>
        /// <param name='entity'>
        /// Entity to register.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="entity"/> has already been registered.</exception>
        void Register(TEntity entity);

        /// <summary>
        /// Determines if the map already knows the specified entity.
        /// </summary>
        /// <param name='entity'>
        /// Identity of the entity of interest.
        /// </param>
        /// <returns><c>true</c>if the map knows the specified entity, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <see langword="null"/>.</exception>
        bool Knows(TIdentity entity);
    }
}

