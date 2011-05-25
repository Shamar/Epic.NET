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
using System.Security.Principal;
using System.Collections.Generic;
using System.Reflection;

namespace Epic.Fakes
{
	[Serializable]
	public class FakeWorkingSession : WorkingSessionBase
	{
		private static IPrincipal CreateFakePrincipal()
		{
			return new GenericPrincipal(new GenericIdentity("FakeUser"), new string[0]);
		}
		public FakeWorkingSession ()
			: base("FakeWorkingSession", CreateFakePrincipal())
		{
		}

		public FakeWorkingSession (string identifier, IPrincipal owner)
			: base(identifier, owner)
		{
			CallBeforeDispose ();
			CallIsAllowed<object> ();
			CallGetRoleBuilder<object> ();
		}
		
		#region templates for tests
		
		public new IPrincipal Owner
		{
			get
			{
				return base.Owner;
			}
		}

		public virtual void CallBeforeDispose ()
		{
		}
		
		public virtual bool CallIsAllowed<TRole> () where TRole : class
		{
			return false;
		}

		public virtual RoleBuilder<TRole> CallGetRoleBuilder<TRole> () where TRole : class
		{
			return null;
		}

		#endregion templates for tests

		#region implemented abstract members of Epic.Enterprise.WorkingSessionBase


		protected override bool IsAllowed<TRole> ()
		{
			return CallIsAllowed<TRole>();
		}

		protected override RoleBuilder<TRole> GetRoleBuilder<TRole> ()
		{
			return CallGetRoleBuilder<TRole>();
		}

		protected override void BeforeDispose ()
		{
			CallBeforeDispose();
		}
		#endregion
	}
	
	[Serializable]
	public class FakeSerializableWorkingSession : WorkingSessionBase
	{
		private static IPrincipal CreateFakePrincipal()
		{
			return new GenericPrincipal(new GenericIdentity("FakeUser"), new string[0]);
		}
		public FakeSerializableWorkingSession ()
			: base("FakeWorkingSession", CreateFakePrincipal())
		{
			IsAllowed<object> ();
			GetRoleBuilder<object> ();
			BeforeDispose ();
		}
		
		private readonly List<string> _calledMethods = new List<string>();
		public IEnumerable<string> CalledMethods
		{
			get
			{
				return _calledMethods;
			}
		}
		
		#region implemented abstract members of Epic.Enterprise.WorkingSessionBase
		protected override bool IsAllowed<TRole> ()
		{
			_calledMethods.Add(MethodBase.GetCurrentMethod().Name);
			switch(typeof(TRole).Name)
			{
				case "IFakeRole":
					return true;
				default:
					return false;
			}
		}
		protected override RoleBuilder<TRole> GetRoleBuilder<TRole> ()
		{
			_calledMethods.Add(MethodBase.GetCurrentMethod().Name);
			switch(typeof(TRole).Name)
			{
				case "IFakeRole":
					return new FakeRoleBuilder<IFakeRole, FakeRole>() as RoleBuilder<TRole>;
				default:
					return null;
			}
		}

		protected override void BeforeDispose ()
		{
		}
		#endregion
	}
}

