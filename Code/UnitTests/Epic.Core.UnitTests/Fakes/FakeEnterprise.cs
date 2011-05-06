//  
//  FakeEnterprise.cs
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
	public class FakeEnterprise : EnterpriseBase
	{
		public FakeEnterprise ()
			: this("Fake")
		{
		}

		public FakeEnterprise (string name)
			: base(name)
		{
		}
		
		#region implemented abstract members of Epic.Enterprise.EnterpriseBase
		protected override void StartWorkingSession (System.Security.Principal.IPrincipal owner, out WorkingSessionBase workingSession)
		{
			throw new NotImplementedException ();
		}

		protected override WorkingSessionBase AcquireWorkingSessionReal (System.Security.Principal.IPrincipal owner, string identifier)
		{
			throw new NotImplementedException ();
		}

		protected override void BeforeWorkingSessionEnd (System.Security.Principal.IPrincipal owner, WorkingSessionBase workingSession)
		{
			throw new NotImplementedException ();
		}
		#endregion
	}
}

