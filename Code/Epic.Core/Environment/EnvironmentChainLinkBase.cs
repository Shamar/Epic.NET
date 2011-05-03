//  
//  EnvironmentChainLink.cs
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
namespace Epic.Environment
{
	/// <summary>
	/// Base class of a single link in an <see cref="EnvironmentsChain"/>.
	/// </summary>
	public abstract class EnvironmentChainLinkBase : IEnvironment
	{
		/// <summary>
		/// Indicates whether the environment knows about a specific tool or information.
		/// When this method returns <value>false</value>, the related call to
		/// <see cref="EnvironmentChainLinkBase.Get{TObject}(string)"/> will throw <see cref="KeyNotFoundException"/>.
		/// </summary>
		/// <typeparam name="TObject">
		/// Type of the tool or information required.
		/// </typeparam>
		/// <param name="name">
		/// An identifier for the needed object.
		/// </param>
		/// <returns>
		/// <value>true</value> if the environment knows how to get the required <typeparamref name="TObject"/>.
		/// </returns>
		public abstract bool Knows<TObject> (string name);
		
		#region IEnvironment implementation
		/// <summary>
		/// Provide a tool or information from the environment.
		/// </summary>
		/// <param name="name">
		/// An identifier for the needed object.
		/// </param>
		/// <returns>
		/// The required object.
		/// </returns>
		/// <exception cref="KeyNotFoundException">The provided <paramref name="name"/> do not match any property of the environment.</exception>
		public abstract TObject Get<TObject> (string name);
		#endregion
	}
}

