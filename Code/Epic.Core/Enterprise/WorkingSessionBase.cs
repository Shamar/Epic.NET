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
		private readonly Dictionary<Type, RoleRef> _roles = new Dictionary<Type, RoleRef>();
		private IPrincipal _owner;

		protected WorkingSessionBase(string identifier)
		{
			if(string.IsNullOrEmpty(identifier))
				throw new ArgumentNullException("identifier");
			_identifier = identifier;
		}
		
		protected IPrincipal CurrentOwner
		{
			get
			{
				return _owner;
			}
		}
		
		protected abstract bool AllowNewOwner(IPrincipal newOwner, out IPrincipal ownerToAssign);
		
		private void RaiseOwnerChanged(IPrincipal oldOwner)
		{
			string oldName; 
			string newName;
			if(null == oldOwner)
				oldName = string.Empty;
			else
				oldName = oldOwner.Identity.Name;
			if(null == _owner)
				newName = string.Empty;
			else
				newName = _owner.Identity.Name;
			Events.ChangeEventArgs<string> args = new Events.ChangeEventArgs<string>(oldName, newName);
			EventHandler<Events.ChangeEventArgs<string>> handler = OwnerChanged;
			if(null != handler)
				handler(this, args); // TODO: evaluate whether aggregate exceptions
		}
		
		protected abstract bool IsAllowed<TRole>() where TRole : class;
		
		protected abstract RoleBuilderBase<TRole> GetRoleBuilder<TRole>() where TRole : class;
		
		private TRole Build<TRole>() where TRole : class
		{
			RoleBuilderBase<TRole> builder = GetRoleBuilder<TRole>();
			return builder.Build(_owner);
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
				string message = string.Format(messageTpl, _identifier, owner.Identity.Name, _owner.Identity.Name, roleList.Count, string.Join(", ", roleList.ToArray()));
				throw new InvalidOperationException(message);
			}
			
			IPrincipal oldOwner = _owner;
			try
			{
				if(this.AllowNewOwner(owner, out _owner))
				{
					RaiseOwnerChanged(oldOwner);
				}
				else
				{
					string message = string.Format("Can not assign the working session {0} to {1}. Operation not allowed.", _identifier, owner.Identity.Name);
					throw new InvalidOperationException(message);
				}
			}
			catch(Exception e)
			{
				if(e is InvalidOperationException)
					throw e;
				string message = string.Format("Can not assign the working session {0} to {1}.", _identifier, owner.Identity.Name);
				throw new InvalidOperationException(message, e);
			}
		}
		
		public event EventHandler<Events.ChangeEventArgs<string>> OwnerChanged;
		
		public bool CouldAchieve<TRole> () where TRole : class
		{
			Type roleType = typeof(TRole);
			if(_roles.ContainsKey(roleType))
				return true;
			return IsAllowed<TRole>();
		}

		public void Achieve<TRole> (out TRole role) where TRole : class
		{
			Type roleType = typeof(TRole);
			RoleRef roleRef = null;
			if(!_roles.TryGetValue(roleType, out roleRef))
			{
				if(!IsAllowed<TRole>())
				{
					string message = string.Format("The owner of the working session ({0}) can not achieve the {1} role.", this.Owner, roleType.FullName);
					throw new InvalidOperationException(message);
				}
				TRole newRole = Build<TRole>();
				roleRef = new RoleRef(newRole as RoleBase);
				_roles[roleType] = roleRef;
			}
			roleRef.Increase();
			role = roleRef.Role as TRole;
		}

		public void Leave<TRole> (ref TRole role) where TRole : class
		{
			if(null == role)
				throw new ArgumentNullException("role");
			Type roleType = typeof(TRole);
			RoleRef roleRef = null;
			if(!_roles.TryGetValue(roleType, out roleRef))
			{
				string message = string.Format("Unknown role type {0}.", roleType.FullName);
				throw new ArgumentException(message, "role");
			}
			if(! object.ReferenceEquals(role, roleRef.Role))
			{
				string message = string.Format("Unknown role {0}.", roleType.FullName);
				throw new ArgumentException(message, "role");
			}
			if(roleRef.Decrease() == 0)
			{
				_roles.Remove(roleType);
				roleRef.Dispose();
			}
			role = null;
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
				if(null == _owner)
					return string.Empty;
				return _owner.Identity.Name;
			}
		}
		
		#endregion
		
		#region IDisposable implementation
		public virtual void Dispose ()
		{
			foreach(RoleRef roleRef in _roles.Values)
			{
				roleRef.Dispose();
			}
			_roles.Clear();
			_owner = null;
		}
		#endregion
	}
}

