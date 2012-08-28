//  
//  Enterprise.cs
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
using System.Configuration;
namespace Epic
{
	/// <summary>
	///	Enterprise entry point. It MUST be initialized once.
	/// </summary>
	public static class Enterprise
	{
		private static EnterpriseBase _enterprise = new Uninitialized();
		
		/// <summary>
		/// Initialize the enterprise entry point.
		/// </summary>
		/// <param name="enterprise">
		/// A <see cref="EnterpriseBase"/> to handle request.
		/// </param>
		/// <exception cref="ArgumentNullException">When <paramref name="enterprise"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">When called more then once.</exception>
		public static void Initialize(EnterpriseBase enterprise)
		{
			if(null == enterprise)
				throw new ArgumentNullException("enterprise");
			if(!(_enterprise is Uninitialized))
				throw new InvalidOperationException("Already initialized.");
			_enterprise = enterprise;
		}
		
		/// <summary>
		/// Reset the enterprise entry point (for testing purpose). 
		/// </summary>
		internal static void Reset()
		{
			_enterprise = new Uninitialized();
		}
		
		/// <summary>
		/// The enterprise name.
		/// </summary>
		public static string Name
		{
			get 
			{
				return _enterprise.Name;
			}
		}

		/// <summary>
		/// Enterprise's <see cref="IEnvironment"/>.
		/// </summary>
		public static IEnvironment Environment
		{
			get 
			{
				return _enterprise.Environment;
			}
		}


		/// <summary>
		/// Organization that use the domain model.
		/// </summary>
		public static IOrganization Organization
		{
			get 
			{
				return _enterprise.Organization;
			}
		}
		
		/// <summary>
		/// Uninitialized enterprise: throws InvalidOperationException.
		/// </summary>
		sealed class Uninitialized : EnterpriseBase
		{
			public Uninitialized()
				: base("Uninitialized")
			{
			}
			
			#region implemented abstract members of Epic.EnterpriseBase
			
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

