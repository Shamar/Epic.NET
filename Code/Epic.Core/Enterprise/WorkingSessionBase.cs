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
		private readonly IPrincipal _owner;
		private bool _disposed = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="Epic.Enterprise.WorkingSessionBase"/> class.
		/// </summary>
		/// <param name='identifier'>
		/// Session identifier (must be unique in the enterprise application).
		/// </param>
		/// <param name="owner">
		/// Working session's owner.
		/// </param>
		/// <exception cref='ArgumentNullException'>
		/// Is thrown when <paramref name="identifier"/> is <see langword="null" /> 
		/// or <see cref="string.Empty"/>.
		/// </exception>
		/// <exception cref='ArgumentNullException'>
		/// Is thrown when <paramref name="owner"/> is <see langword="null" />.
		/// </exception>
		protected WorkingSessionBase(string identifier, IPrincipal owner)
		{
			if(string.IsNullOrEmpty(identifier))
				throw new ArgumentNullException("identifier");
			if(null == owner)
				throw new ArgumentNullException("owner");
			_identifier = identifier;
			_owner = owner;
		}
		
		/// <summary>
		/// Working session's owner.
		/// </summary>
		/// <value>
		/// The owner.
		/// </value>
		protected IPrincipal Owner
		{
			get
			{
				return _owner;
			}
		}
		
		protected void throwAfterDisposition()
		{
			if(_disposed)
				throw new ObjectDisposedException(_identifier);
		}

		#region abstract template method
		
		/// <summary>
		/// Template method for disposition. Called just before the disposition follow on the achieved roles.
		/// </summary>
		protected abstract void BeforeDispose();
		
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
		/// Indicates whether the owner can achieve the specified role.
		/// </summary>
		/// <returns>
		/// <value>true</value> if the session owner can achieve the role, <value>false</value> otherwise.
		/// </returns>
		/// <typeparam name='TRole'>
		/// The type of the role to achieve.
		/// </typeparam>
		public bool CanAchieve<TRole> () where TRole : class
		{
			throwAfterDisposition();
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
			throwAfterDisposition();
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
			throwAfterDisposition();
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
		public void Dispose ()
		{
			if(!_disposed)
			{
				_disposed = true;
				BeforeDispose();
				foreach(RoleRef roleRef in _roles.Values)
				{
					roleRef.Dispose();
				}
				_roles.Clear();
			}
		}
		#endregion
	}
}

