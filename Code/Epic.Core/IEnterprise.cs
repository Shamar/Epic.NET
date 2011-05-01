//  
//  IEnterprise.cs
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
namespace Epic
{
	/// <summary>
	/// Model of the enterprise. It is the domain model entry point.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The Epic target are application modelling enterprise processes.
	/// Thus, this is the entry point for all the domain model boundaries.
	/// </para>
	/// </remarks>
	public interface IEnterprise
	{
		/// <summary>
		/// Name of the enterprise.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		string Name { get; }
		
		/// <summary>
		/// Starts a new working session.
		/// </summary>
		/// <param name='owner'>
		/// Owner of the new session.
		/// </param>
		/// <param name='workingSession'>
		/// The new working session.
		/// </param>
		void StartWorkingSession(IIdentity owner, out IWorkingSession workingSession);
		
		/// <summary>
		/// Acquires an existing working session. 
		/// Used to adopt a working session between different applications/appdomain.
		/// </summary>
		/// <returns>
		/// The working session.
		/// </returns>
		/// <param name='owner'>
		/// The owner.
		/// </param>
		/// <param name='identifier'>
		/// The working session identifier.
		/// </param>
		IWorkingSession AcquireWorkingSession(IIdentity owner, string identifier);
		
		/// <summary>
		/// Ends the working session.
		/// </summary>
		/// <param name='owner'>
		/// The owner.
		/// </param>
		/// <param name='workingSession'>
		/// The working session to end.
		/// </param>
		void EndWorkingSession(IIdentity owner, IWorkingSession workingSession);
	}
}
