//  
//  IWorkingSession.cs
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
using System.Security.Principal;
using Epic.Events;
namespace Epic
{
	/// <summary>
	/// A user working session. It can be created or disposed by <see cref="IOrganization"/>
	/// </summary>
	public interface IWorkingSession
	{
		/// <summary>
		/// Unique identifier of the working session.
		/// </summary>
		/// <remarks>
		/// This must be an unpredictable identifier, unique in the whole organization.
        /// </remarks>
		string Identifier { get; }
		
		/// <summary>
		/// Owner of the working session. Will be <see cref="string.Empty"/> when anonymous.
		/// </summary>
		string Owner { get; }
		
		/// <summary>
		/// Indicates whether the owner can achieve the specified role.
		/// </summary>
		/// <returns>
		/// <see langword="true"/> if the session owner can achieve the role, <see langword="false"/> otherwise.
		/// </returns>
		/// <typeparam name='TRole'>
		/// The type of the role to achieve.
		/// </typeparam>
		bool CanAchieve<TRole>() where TRole : class;
		
		/// <summary>
		/// Achieve the specified role.
		/// </summary>
		/// <param name='role'>
		/// User's role, entry point to a specific context boundary in the domain.
		/// </param>
		/// <typeparam name='TRole'>
		/// The type of the role to achieve.
		/// </typeparam>
		/// <exception cref="InvalidOperationException">The <see cref="IWorkingSession.Owner"/> can not achieve 
		/// the required <typeparamref name="TRole"/>.</exception>
		void Achieve<TRole>(out TRole role) where TRole : class;
		
		/// <summary>
		/// Leave the specified role. 
		/// After calling this method, your role will be disposed 
		/// and the reference to <paramref name="role"/> will be set to <see langword="null"/>.
		/// </summary>
		/// <param name='role'>
		/// User's role to be disposed.
		/// </param>
		/// <typeparam name='TRole'>
		/// The type of the role to leave.
		/// </typeparam>
		/// <exception cref="ArgumentNullException">The <paramref name="role"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">The <paramref name="role"/> is unknown to the session.</exception>
		void Leave<TRole>(ref TRole role) where TRole : class;
	}
}

