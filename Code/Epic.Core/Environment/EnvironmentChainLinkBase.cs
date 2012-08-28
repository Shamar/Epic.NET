//  
//  EnvironmentChainLink.cs
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
namespace Epic.Environment
{
	/// <summary>
	/// Base class of a single link in an <see cref="EnvironmentsChain"/>.
	/// </summary>
	public abstract class EnvironmentChainLinkBase
	{
		/// <summary>
		/// Try to gets the information associated with the specified name.
		/// </summary>
		/// <param name="name">
		/// An identifier for the needed object.
		/// </param>
		/// <param name="information">
		/// When this method returns, contains the information associated with the
	    /// specified key, if the key is found; otherwise, the default value for the
	    /// type of the value parameter. This parameter is passed uninitialized.
		/// </param>
		/// <returns>
		/// <see langword="true"/> if the chain link knows how to provide the required 
		/// <paramref name="information"/>, <see langword="false"/> otherwise.
		/// </returns>
		public abstract bool TryGet<TObject> (InstanceName<TObject> name, out TObject information);
	}
}

