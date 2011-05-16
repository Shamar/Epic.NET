//  
//  RoleBuilderBase.cs
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
	/// Role builders' base class.
	/// </summary>
	/// <typeparam name="TRole">Role builded.</typeparam>
	/// <typeparam name="TImplementation">Concrete role class.</typeparam>
	[Serializable]
	public abstract class RoleBuilderBase<TRole, TImplementation> : RoleBuilder<TRole>
		where TRole : class
		where TImplementation : RoleBase, TRole
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		protected RoleBuilderBase()
			: base()
		{
		}

		#region implemented abstract members of Epic.Enterprise.RoleBuilder[TRoleInterface]
		/// <summary>
		/// Builds the role. Overrides and seals the
		/// <see cref="RoleBuilder{TRole}.BuildRole(IPrincipal)"/> template 
		/// method. Calls <see cref="RoleBuilderBase{TRole, TImplementation}.CreateRoleFor(IPrincipal)"/>.
		/// </summary>
		/// <returns>
		/// The role.
		/// </returns>
		/// <param name='player'>
		/// Role player.
		/// </param>
		protected sealed override TRole BuildRole (IPrincipal player)
		{
			return CreateRoleFor(player);
		}
		#endregion
			
		/// <summary>
		/// Creates the <typeparamref name="TRole"/> instance for <paramref name="player"/>.
		/// </summary>
		/// <returns>
		/// The the <typeparamref name="TRole"/> instance for <paramref name="player"/>.
		/// </returns>
		/// <param name='player'>
		/// Role player.
		/// </param>
		protected abstract TImplementation CreateRoleFor (IPrincipal player);
	}
}

