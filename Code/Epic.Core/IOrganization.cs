//  
//  IOrganization.cs
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
using System.Security.Principal;
namespace Epic
{
	/// <summary>
	/// Model of the organization. It is the domain model entry point.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The Epic target are enterprise modelling organization processes.
	/// Thus, this is the entry point for all the domain model boundaries.
	/// </para>
	/// </remarks>
	public interface IOrganization
	{
		/// <summary>
		/// Name of the organization.
		/// </summary>
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
		/// <exception cref="ArgumentNullException"><paramref name="owner"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException"><paramref name="owner"/> can not create a 
		/// new <see cref="IWorkingSession"/>.</exception>
		void StartWorkingSession(IPrincipal owner, out IWorkingSession workingSession);
		
		/// <summary>
		/// Acquires an existing working session. 
		/// Used to adopt a working session between different enterprises/appdomain.
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
		/// <exception cref="ArgumentNullException"><paramref name="owner"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="identifier"/> is <see langword="null"/> 
		/// or empty.</exception>
		/// <exception cref="InvalidOperationException"><paramref name="owner"/> can not acquire 
		/// the <see cref="IWorkingSession"/> identified by <paramref name="identifier"/>.</exception>
		IWorkingSession AcquireWorkingSession(IPrincipal owner, string identifier);
		
		/// <summary>
		/// Ends the working session.
		/// </summary>
		/// <param name='owner'>
		/// The owner.
		/// </param>
		/// <param name='workingSession'>
		/// The working session to end.
		/// </param>
		/// <exception cref="ArgumentNullException">Either <paramref name="owner"/> or 
		/// <paramref name="workingSession"/> are <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException"><paramref name="owner"/> can not end 
		/// <paramref name="workingSession"/>.</exception>
		void EndWorkingSession(IPrincipal owner, IWorkingSession workingSession);
	}
}

