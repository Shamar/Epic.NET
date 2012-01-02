//  
//  FakeOrganization.cs
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
using Epic.Organization;

namespace Epic.Fakes
{
	[Serializable]
	public class FakeOrganization : OrganizationBase
	{
		public FakeOrganization ()
			: this("Fake")
		{
		}

		public FakeOrganization (string name)
			: base(name)
		{
			WorkingSessionBase workingSession = null;
			CallStartWorkingSession(null, out workingSession);
			CallAcquireWorkingSessionReal(null, null);
			CallBeforeWorkingSessionEnd(null, null);
		}
		
		#region templates for tests
		
		public virtual void CallStartWorkingSession (System.Security.Principal.IPrincipal owner, out WorkingSessionBase workingSession)
		{
			workingSession = null;
		}

		public virtual WorkingSessionBase CallAcquireWorkingSessionReal (System.Security.Principal.IPrincipal owner, string identifier)
		{
			return null;
		}

		public virtual void CallBeforeWorkingSessionEnd (System.Security.Principal.IPrincipal owner, WorkingSessionBase workingSession)
		{
		}
		
		#endregion templates for tests
		
		#region implemented abstract members of Epic.Organization.OrganizationBase
		protected override void StartWorkingSession (System.Security.Principal.IPrincipal owner, out WorkingSessionBase workingSession)
		{
			CallStartWorkingSession(owner, out workingSession);
		}

		protected override WorkingSessionBase AcquireWorkingSessionReal (System.Security.Principal.IPrincipal owner, string identifier)
		{
			return CallAcquireWorkingSessionReal(owner, identifier);
		}

		protected override void BeforeWorkingSessionEnd (System.Security.Principal.IPrincipal owner, WorkingSessionBase workingSession)
		{
			CallBeforeWorkingSessionEnd(owner, workingSession);
		}
		#endregion
	}
}

