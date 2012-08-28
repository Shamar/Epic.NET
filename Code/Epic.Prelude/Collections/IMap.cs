//
//  IMap.cs
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
    /// A map that provide access to <typeparamref name="TValue"/> 
    /// given a <typeparamref name="TKey"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the map.</typeparam>
    /// <typeparam name="TValue">The type of values in the map.</typeparam>
    public interface IMap<TKey, TValue>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets the <typeparamref name="TValue"/> with the specified key.
        /// </summary>
        /// <param name='key'>
        /// The key of the element to get.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException{TKey}">The property is retrieved and <paramref name="key"/> is not found.</exception>
        TValue this[TKey key] { get; }
    }
}

