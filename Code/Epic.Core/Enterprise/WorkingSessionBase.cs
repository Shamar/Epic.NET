//  
//  WorkingSessionBase.cs
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
using System.Collections.Generic;
namespace Epic.Enterprise
{
	/// <summary>
	/// Base class for working sessions.
	/// </summary>
	[Serializable]
	public abstract class WorkingSessionBase : IWorkingSession, IDisposable
	{
		private readonly string _identifier;
		private readonly Dictionary<Type, object> _roles = new Dictionary<Type, object>();
		private string _owner;

		protected WorkingSessionBase(string identifier)
		{
			if(string.IsNullOrEmpty(identifier))
				throw new ArgumentNullException("identifier");
			_identifier = identifier;
		}
		
		#region IWorkingSession implementation

		public void AssignTo (IPrincipal owner)
		{
			if(null == owner)
				throw new ArgumentNullException("owner");
			if(_roles.Count > 0)
			{
				string messageTpl = "Can not assign the working session {0} to {1}, since it belong to {2} who is still achieving {3} roles ({4}).";
				List<string> roleList = new List<string>(); 
				foreach(Type rT in _roles.Keys)
				{
					roleList.Add(rT.FullName);
				}
				string message = string.Format(messageTpl, _identifier, owner.Identity.Name, "", roleList.Count, string.Join(", ", roleList.ToArray()));
				throw new InvalidOperationException(message);
			}
			
			try
			{
				UpdateCurrentOwner(owner, out _owner);
			}
			catch(Exception e)
			{
				string message = string.Format("Can not assign the working session {0} to {1}.", _identifier, owner.Identity.Name);
				throw new InvalidOperationException(message, e);
			}
		}
		
		protected abstract void UpdateCurrentOwner(IPrincipal newOwner, out string ownerIdentifier);
		
		private void RaiseOwnerChanged(Events.ChangeEventArgs<string> args)
		{
			if(null == args)
				throw new ArgumentNullException("args");
			EventHandler<Events.ChangeEventArgs<string>> handler = OwnerChanged;
			if(null != handler)
				handler(this, args);
		}

		public event EventHandler<Events.ChangeEventArgs<string>> OwnerChanged;
		
		public bool CouldAchieve<TRole> () where TRole : class
		{
			throw new NotImplementedException ();
		}

		public void Achieve<TRole> (out TRole role) where TRole : class
		{
			throw new NotImplementedException ();
		}

		public void Leave<TRole> (ref TRole role) where TRole : class
		{
			throw new NotImplementedException ();
		}

		public string Identifier 
		{
			get 
			{
				return _identifier;
			}
		}

		public string Owner 
		{
			get 
			{
				return _owner;
			}
		}
		
		#endregion
		
		#region IDisposable implementation
		public virtual void Dispose ()
		{
			throw new NotImplementedException ();
		}
		#endregion
	}
}

