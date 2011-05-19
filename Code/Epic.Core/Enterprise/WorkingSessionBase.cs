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

		/// <summary>
		/// Initializes a new instance of the <see cref="Epic.Enterprise.WorkingSessionBase"/> class.
		/// </summary>
		/// <param name='identifier'>
		/// Session identifier (must be unique in the enterprise application).
		/// </param>
		/// <exception cref='ArgumentNullException'>
		/// Is thrown when <paramref name="identifier"/> is <see langword="null" /> 
		/// or <see cref="string.Empty"/>.
		/// </exception>
		protected WorkingSessionBase(string identifier)
		{
			if(string.IsNullOrEmpty(identifier))
				throw new ArgumentNullException("identifier");
			_identifier = identifier;
		}
		
		/// <summary>
		/// Gets the current owner. <see langword="null" /> when the session has never be assigned to a owner.
		/// </summary>
		/// <value>
		/// The current owner.
		/// </value>
		/// <seealso cref="WorkingSessionBase.AssignTo(IPrincipal)"/>
		protected IPrincipal Owner
		{
			get
			{
				return _owner;
			}
		}
		
		#region abstract template method
		
		/// <summary>
		/// Indicates whether the current working session can be assigned 
		/// to <paramref name="newOwner"/>.
		/// </summary>
		/// <returns>
		/// <value>true</value> if the assignment is allowed, <value>false</value> otherwise.
		/// </returns>
		/// <param name='newOwner'>
		/// The new owner.
		/// </param>
		/// <param name='ownerToAssign'>
		/// If the assigment is allowed, the owner to assign (usually <paramref name="newOwner"/>).
		/// </param>
		protected abstract bool AllowNewOwner(IPrincipal newOwner, out IPrincipal ownerToAssign);
		
		/// <summary>
		/// Determines whether the owner of the working session is allowed to achieve 
		/// <typeparam name="TRole"/>.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the owner of the working session is allowed to achieve the role; 
		/// otherwise, <c>false</c>.
		/// </returns>
		/// <typeparam name='TRole'>
		/// Role of interest.
		/// </typeparam>
		protected abstract bool IsAllowed<TRole>() where TRole : class;
		
		/// <summary>
		/// Gets the role builder for <typeparamref name="TRole"/>.
		/// </summary>
		/// <returns>
		/// The role builder.
		/// </returns>
		/// <typeparam name='TRole'>
		/// Role of interest.
		/// </typeparam>
		protected abstract RoleBuilder<TRole> GetRoleBuilder<TRole>() where TRole : class;
		
		#endregion abstract template method
		
		/// <summary>
		/// Raises the owner changed event.
		/// </summary>
		/// <param name='oldOwner'>
		/// Old owner.
		/// </param>
		/// <seealso cref="WorkingSessionBase.AssignTo(IPrincipal)"/>
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
		
		/// <summary>
		/// Build a <typeparamref name="TRole"/> instance.
		/// </summary>
		/// <typeparam name='TRole'>
		/// Role of interest.
		/// </typeparam>
		private TRole Build<TRole>() where TRole : class
		{
			RoleBuilder<TRole> builder = GetRoleBuilder<TRole>();
			return builder.Build(_owner);
		}

		#region IWorkingSession implementation
		
		/// <summary>
		/// Assigns the working session to <paramref name="owner"/>.
		/// </summary>
		/// <param name='owner'>
		/// New owner.
		/// </param>
		/// <exception cref='ArgumentNullException'>
		/// Is thrown when <param name='owner'> is <see langword="null" />.
		/// </exception>
		/// <exception cref='InvalidOperationException'>
		/// Is thrown when the assignment cannot be performed.
		/// </exception>
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
			bool allowed = false;
			try
			{
				allowed = this.AllowNewOwner(owner, out _owner);
				if(allowed)
				{
					RaiseOwnerChanged(oldOwner);
				}
			}
			catch(Exception e)
			{
				if(e is InvalidOperationException)
					throw e;
				string message = string.Format("Can not assign the working session {0} to {1}.", _identifier, owner.Identity.Name);
				throw new InvalidOperationException(message, e);
			}
			if(! allowed)
			{
				string message = string.Format("Can not assign the working session {0} to {1}. Operation not allowed.", _identifier, owner.Identity.Name);
				throw new InvalidOperationException(message);
			}
		}
		
		/// <summary>
		/// Occurs when the owner change.
		/// </summary>
		public event EventHandler<Events.ChangeEventArgs<string>> OwnerChanged;
		
		/// <summary>
		/// Indicates whether the owner can achieve the specified role.
		/// </summary>
		/// <returns>
		/// <value>true</value> if the session owner can achieve the role, <value>false</value> otherwise.
		/// </returns>
		/// <typeparam name='TRole'>
		/// The type of the role to achieve.
		/// </typeparam>
		public bool CouldAchieve<TRole> () where TRole : class
		{
			Type roleType = typeof(TRole);
			if(_roles.ContainsKey(roleType))
				return true;
			return IsAllowed<TRole>();
		}

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
		public void Achieve<TRole> (out TRole role) where TRole : class
		{
			Type roleType = typeof(TRole);
			RoleRef roleRef = null;
			if(!_roles.TryGetValue(roleType, out roleRef))
			{
				if(!IsAllowed<TRole>())
				{
					string message = string.Format("The owner of the working session ({0}) can not achieve the {1} role.", ((IWorkingSession)this).Owner, roleType.FullName);
					throw new InvalidOperationException(message);
				}
				TRole newRole = Build<TRole>();
				roleRef = new RoleRef(newRole as RoleBase);
				_roles[roleType] = roleRef;
			}
			roleRef.Increase();
			role = roleRef.Role as TRole;
		}

		/// <summary>
		/// Leave the specified role. 
		/// After calling this method, your role will be disposed 
		/// and the reference to <paramref name="role"/> will be set to <value>null</value>.
		/// </summary>
		/// <param name='role'>
		/// User's role to be disposed.
		/// </param>
		/// <typeparam name='TRole'>
		/// The type of the role to leave.
		/// </typeparam>
		/// <exception cref="ArgumentNullException">The <paramref name="role"/> is <value>null</value>.</exception>
		/// <exception cref="ArgumentException">The <paramref name="role"/> is unknown to the session.</exception>
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

		/// <summary>
		/// Unique identifier of the working session.
		/// </summary>
		/// <value>
		/// A unpredictable identifier, unique in the whole enterprise.
		/// </value>
		public string Identifier 
		{
			get 
			{
				return _identifier;
			}
		}

		/// <summary>
		/// Owner of the working session. Will be <see cref="string.Empty"/> when anonymous.
		/// </summary>
		/// <value>
		/// Owner of the working session. Will be <see cref="string.Empty"/> when anonymous.
		/// </value>
		string IWorkingSession.Owner 
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
		/// <summary>
		/// Releases all resource used by the <see cref="Epic.Enterprise.WorkingSessionBase"/> object.
		/// </summary>
		/// <remarks>
		/// Call <see cref="Dispose"/> when you are finished using the <see cref="Epic.Enterprise.WorkingSessionBase"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="Epic.Enterprise.WorkingSessionBase"/> in an unusable state.
		/// After calling <see cref="Dispose"/>, you must release all references to the
		/// <see cref="Epic.Enterprise.WorkingSessionBase"/> so the garbage collector can reclaim the memory that the
		/// <see cref="Epic.Enterprise.WorkingSessionBase"/> was occupying.
		/// </remarks>
		// TODO: introduce template method for dispose()
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

