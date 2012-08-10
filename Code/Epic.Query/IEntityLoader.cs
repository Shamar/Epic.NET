//  
//  IEntityLoader.cs
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

namespace Epic.Query
{
	/// <summary>
	/// A loader for <typeparamref name="TEntity"/>.
	/// </summary>
	/// <typeparam name="TEntity">Type of the entity that the loader can load.</typeparam>
	/// <typeparam name="TIdentity">Type used to identify each <typeparamref name="TEntity"/>.</typeparam>
	public interface IEntityLoader<TEntity, TIdentity>
		where TEntity : class
		where TIdentity : IEquatable<TIdentity>
	{
		/// <summary>
		/// Load the specified entities.
		/// </summary>
		/// <param name='identities'>
		/// Identifiers of the <typeparamref name="TEntity"/> to load.
		/// </param>
		/// <exception cref="Epic.EpicException">One or more <typeparamref name="TEntity"/> can not be loaded.</exception>
		IEnumerable<TEntity> Load(params TIdentity[] identities);
	}
}

