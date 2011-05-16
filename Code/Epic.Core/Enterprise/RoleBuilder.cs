//  
//  RoleBuilder.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2011 Giacomo Tesio
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

namespace Epic.Enterprise
{
	/// <summary>
	/// Role builder public contract. Enforce <typeparamref name="TRole"/> to be a pure interface.
	/// </summary>
	/// <typeparam name="TRole">Type (interface) of role that the builder can build.</typeparam>
	/// <exception cref='InvalidOperationException'>
	/// Is thrown on type initialization if <typeparamref name="TRole"/> is not an interface.
	/// </exception>
	/// <seealso cref="RoleBuilderBase{TRole, TImplementation}"/>
	[Serializable]
	public abstract class RoleBuilder<TRole> where TRole : class
	{
		/// <summary>
		/// Initializes a role builder class type.
		/// </summary>
		/// <exception cref='InvalidOperationException'>
		/// <typeparamref name="TRole"/> is not an interface.
		/// </exception>
		static RoleBuilder()
		{
			if(!typeof(TRole).IsInterface)
			{
				string message = string.Format("Can not initialize a RoleBuilder<{0}> since {0} is not an interface.", typeof(TRole).FullName);
				throw new InvalidOperationException(message);
			}
		}
		
		/// <summary>
		/// Constructor. Internal keep the implementation binded to 
		/// <see cref="RoleBuilderBase{TRoleInterface, TRoleImplementation}"/>.
		/// </summary>
		internal RoleBuilder()
		{
		}
		
		/// <summary>
		/// Build the <typeparamref name="TRole"/> for <paramref name="player"/>. Template method.
		/// </summary>
		/// <param name='player'>
		/// Role player. 
		/// </param>
		/// <seealso cref="RoleBuilder{TRole}.BuildRole(IPrincipal)"/>
		public TRole Build(IPrincipal player)
		{
			TRole newRole = BuildRole(player);
			return newRole;
		}
		
		/// <summary>
		/// Build the <typeparamref name="TRole"/> for <paramref name="player"/>.
		/// </summary>
		/// <returns>
		/// The role. Must implement <typeparamref name="TRole"/>.
		/// </returns>
		/// <param name='player'>
		/// Role player.
		/// </param>
		protected abstract TRole BuildRole(IPrincipal player);
	}
}

