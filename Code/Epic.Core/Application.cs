//  
//  Application.cs
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
using System.Configuration;
namespace Epic
{
	/// <summary>
	///	Application entry point. It MUST be initialized once.
	/// </summary>
	public static class Application
	{
		private static ApplicationBase _application = new Uninitialized();
		
		/// <summary>
		/// Initialize the application entry point.
		/// </summary>
		/// <param name="application">
		/// A <see cref="ApplicationBase"/> to handle request.
		/// </param>
		/// <exception cref="ArgumentNullException">When <paramref name="application"/> is <value>null</value>.</exception>
		/// <exception cref="InvalidOperationException">When called more then once.</exception>
		public static void Initialize(ApplicationBase application)
		{
			if(null == application)
				throw new ArgumentNullException("application");
			if(!(_application is Uninitialized))
				throw new InvalidOperationException("Already initialized.");
			_application = application;
		}
		
		/// <summary>
		/// Reset the application entry point (for testing purpose). 
		/// </summary>
		internal static void Reset()
		{
			_application = new Uninitialized();
		}
		
		/// <summary>
		/// The application name.
		/// </summary>
		public static string Name
		{
			get 
			{
				return _application.Name;
			}
		}

		/// <summary>
		/// Application's <see cref="IEnvironment"/>.
		/// </summary>
		public static IEnvironment Environment
		{
			get 
			{
				return _application.Environment;
			}
		}


		/// <summary>
		/// Organization that use the domain model.
		/// </summary>
		public static IOrganization Organization
		{
			get 
			{
				return _application.Organization;
			}
		}
		
		/// <summary>
		/// Uninitialized application: throws InvalidOperationException.
		/// </summary>
		sealed class Uninitialized : ApplicationBase
		{
			public Uninitialized()
				: base("Uninitialized")
			{
			}
			
			#region implemented abstract members of Epic.ApplicationBase
			
			protected override Epic.Environment.EnvironmentBase RetrieveEnvironmentInstance()
			{
				throw new InvalidOperationException("Initialization required.");
			}
			
			public override IOrganization Organization 
			{
				get 
				{
					throw new InvalidOperationException("Initialization required.");
				}
			}
			
			#endregion
		}
	}
}

