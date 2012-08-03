//  
//  EnvironmentBase.cs
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
using System.Runtime.Serialization;
using System.Security.Permissions;
namespace Epic.Environment
{
	/// <summary>
	/// Base class for enterprises environments. It handle serialization.
	/// </summary>
	[Serializable]
	public abstract class EnvironmentBase : IEnvironment, ISerializable
	{
		#region IEnvironment implementation
		
        /// <summary>
        /// Provide a tool or information from the enterprise environment.
        /// </summary>
        /// <param name="name">
        /// An identifier for the needed object.
        /// </param>
        /// <returns>
        /// The required object.
        /// </returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">The provided <paramref name="name"/> do not match any property of the environment.</exception>
        public abstract TObject Get<TObject> (InstanceName<TObject> name);

		#endregion
		
		#region ISerializable implementation
		
		[SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
		void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
		{
			info.SetType(typeof(EnvironmentSerializationHelper));
		}
		
		#endregion
		
		
	}
}

