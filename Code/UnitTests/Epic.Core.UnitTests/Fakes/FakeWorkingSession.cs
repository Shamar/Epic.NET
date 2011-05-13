//  
//  FakeWorkingSession.cs
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
using Epic.Enterprise;

namespace Epic.Fakes
{
	[Serializable]
	public class FakeWorkingSession : WorkingSessionBase
	{
		public FakeWorkingSession ()
			: base("FakeWorkingSession")
		{
		}

		public FakeWorkingSession (string identifier)
			: base(identifier)
		{
		}
		
		#region templates for tests
		
		public virtual bool CallAllowNewOwner (System.Security.Principal.IPrincipal newOwner, out System.Security.Principal.IPrincipal ownerToAssign)
		{
			ownerToAssign = null;
			return false;
		}

		public virtual bool CallIsAllowed<TRole> () where TRole : class
		{
			return false;
		}

		public virtual RoleBuilderBase<TRole> CallGetRoleBuilder<TRole> () where TRole : class
		{
			return null;
		}

		#endregion templates for tests

		#region implemented abstract members of Epic.Enterprise.WorkingSessionBase
		protected override bool AllowNewOwner (System.Security.Principal.IPrincipal newOwner, out System.Security.Principal.IPrincipal ownerToAssign)
		{
			return CallAllowNewOwner(newOwner, out ownerToAssign);
		}

		protected override bool IsAllowed<TRole> ()
		{
			return CallIsAllowed<TRole>();
		}

		protected override RoleBuilderBase<TRole> GetRoleBuilder<TRole> ()
		{
			return CallGetRoleBuilder<TRole>();
		}
		#endregion
	}
}

