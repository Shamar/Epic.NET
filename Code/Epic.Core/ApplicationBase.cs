//  
//  ApplicationBase.cs
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
namespace Epic
{
	/// <summary>
	/// Application base class, to be injected in <see cref="Application"/> during initialization.
	/// </summary>
	/// <remarks>
	/// It MUST be implemented by a class tailored to the application and enterprise that will use the domain.
	/// </remarks>
	public abstract class ApplicationBase
	{
		private string _name;
		
		/// <summary>
		/// Constructor 
		/// </summary>
		/// <param name="name">
		/// The application name.
		/// </param>
		protected ApplicationBase (string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			_name = name;
		}
		
		/// <summary>
		/// Application name.
		/// </summary>
		public string Name 
		{ 
			get { return _name; }
		}
		
		/// <summary>
		/// Application's <see cref="IEnvironment"/>.
		/// </summary>
		public IEnvironment Environment { get { return RetrieveEnvironmentInstance(); } }
		
		/// <summary>
		/// Retrieves the environment instance to be returned from 
		/// the <see cref="ApplicationBase.Environment"/> property.
		/// </summary>
		/// <returns>
		/// The environment instance.
		/// </returns>
		protected abstract Epic.Environment.EnvironmentBase RetrieveEnvironmentInstance();
		
		/// <summary>
		/// Enterprise that use the domain model.
		/// </summary>
		public abstract IEnterprise Enterprise { get; }
	}
}

