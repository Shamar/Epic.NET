//  
//  EnterpriseBase.cs
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
using System.Runtime.Serialization;
namespace Epic.Enterprise
{
	[Serializable]
	public abstract class EnterpriseBase : IEnterprise, ISerializable
	{
		private readonly string _name;
		protected EnterpriseBase (string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			_name = name;
		}

		#region IEnterprise implementation
		public string Name 
		{
			get 
			{
				return _name;
			}
		}
		
		public void StartWorkingSession (IPrincipal owner, out IWorkingSession workingSession)
		{
			if(null == owner)
				throw new ArgumentNullException("owner");
			StartWorkingSession(owner, out workingSession);
		}
		
		protected abstract void StartWorkingSession(IPrincipal owner, out WorkingSessionBase workingSession);
		
		public IWorkingSession AcquireWorkingSession (IPrincipal owner, string identifier)
		{
			if(null == owner)
				throw new ArgumentNullException("owner");
			if(string.IsNullOrEmpty(identifier))
				throw new ArgumentNullException("identifier");
			
			return AcquireWorkingSessionReal (owner, identifier);
		}
		
		protected abstract WorkingSessionBase AcquireWorkingSessionReal (IPrincipal owner, string identifier);

		public void EndWorkingSession (IPrincipal owner, IWorkingSession workingSession)
		{
			if(null == owner)
				throw new ArgumentNullException("owner");
			if(null == workingSession)
				throw new ArgumentNullException("workingSession");
			WorkingSessionBase session = workingSession as WorkingSessionBase;
			if(null == session)
				throw new ArgumentException("The working session provided do not extend WorkingSessionBase.", "workingSession");
			
			try
			{
				BeforeEndWorkingSession(owner, session);
				
				session.Dispose();
			}
			catch(ObjectDisposedException disposedExc)
			{
				throw new InvalidOperationException("Working session already ended.", disposedExc);
			}
		}
		
		protected abstract void BeforeEndWorkingSession (IPrincipal owner, WorkingSessionBase workingSession);
		
		#endregion

		#region ISerializable implementation
		void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
		{
			info.SetType(typeof(EnterpriseSerializationHelper));
		}
		#endregion
	}
}

