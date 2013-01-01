//  
//  IRepository.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2013 Giacomo Tesio
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

namespace Epic
{
	/// <summary>
	/// Common interface of repositories.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This is the base interface for repositories, in Epic.
	/// </para>
	/// </remarks>
	/// <typeparam name="TEntity">Accessible entity.</typeparam>
	/// <typeparam name="TIdentity"><typeparamref name="TEntity"/>'s identifier.</typeparam>
	public interface IRepository<TEntity, TIdentity>
		where TEntity : class
		where TIdentity : IEquatable<TIdentity>
	{
		/// <summary>
		/// Gets the <typeparamref name="TEntity"/> identified by <paramref name="identity"/>.
		/// </summary>
		/// <param name='identity'>
		/// <typeparamref name="TEntity"/>'s identifier.
		/// </param>
		TEntity this[TIdentity identity] { get; }
	}
}

