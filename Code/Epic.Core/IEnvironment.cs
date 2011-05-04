//  
//  IEnvironment.cs
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
	/// Environment of the running application.
	/// It will provide informations useful for the application, but not belonging to the domain.
	/// Obviously, no domain class should depend on either the environment or the tools 
	/// it provide: the domain model have to be coded, not configured.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The environment that host an application provide a set of configured tools, options, thread values and so on.
	/// This mean that it must behave the same among all instances of any specific application deployed.
	/// </para>
	/// <para>
	/// For example it could provide appsettings values, the arguments used when starting the application, 
	/// the current thread culture, the logging services and so on.
	/// </para>
	/// <para>
	/// Such abstraction is particularly useful when we need to test a software component in isolation.
	/// </para>
	/// </remarks>
	public interface IEnvironment
	{
		/// <summary>
		/// Provide a tool or information from the application environment.
		/// </summary>
		/// <param name="name">
		/// An identifier for the needed object.
		/// </param>
		/// <returns>
		/// The required object.
		/// </returns>
		/// <exception cref="KeyNotFoundException">The provided <paramref name="name"/> do not match any property of the environment.</exception>
		TObject Get<TObject>(string name);
	}
}

