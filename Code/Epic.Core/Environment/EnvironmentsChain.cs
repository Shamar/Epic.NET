//  
//  EnvironmentsChain.cs
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
using System.Collections.Generic;
namespace Epic.Environment
{
	/// <summary>
	/// Chainable environment.
	/// </summary>
	/// <remarks>
	/// This environment implement a chain of responsibility with <see cref="EnvironmentChainLinkBase"/>
	/// asking to each element in the chain if it know a specific information or tool.
	/// </remarks>
	[Serializable]
	public sealed class EnvironmentsChain : EnvironmentBase
	{
		private readonly EnvironmentChainLinkBase[] _environments;
		
		/// <summary>
		/// Chain constructor.
		/// </summary>
		/// <param name="environmentsToChain">
		/// Chain links (that can't be changed after construction).
		/// The order of the items is relevant since the first that know an information will be asked for.
		/// </param>
		/// <seealso cref="EnvironmentChainLinkBase"/>
		public EnvironmentsChain (params EnvironmentChainLinkBase[] environmentsToChain)
		{
			if(null == environmentsToChain)
				throw new ArgumentNullException("environmentsToChain");
			if(environmentsToChain.Length == 0)
				throw new ArgumentNullException("environmentsToChain", "You must provide at least one chain link.");
			_environments = new EnvironmentChainLinkBase[environmentsToChain.Length];
			Array.Copy(environmentsToChain, _environments, environmentsToChain.Length);
		}

		#region implemented abstract members of Epic.Environment.EnvironmentBase
		
		/// <summary>
		/// Provide a tool or information from the chained environments.
		/// </summary>
		/// <param name="name">
		/// An identifier for the needed object.
		/// </param>
		/// <typeparam name="TObject">Type of the required tool or information.</typeparam>
		/// <returns>
		/// The <typeparamref name="TObject"/> returned from the first chain link that knows it.
		/// </returns>
		public override TObject Get<TObject> (InstanceName<TObject> name)
		{
			int lenght = _environments.Length;
			for(int i = 0; i < lenght; ++i)
			{
				EnvironmentChainLinkBase environment = _environments[i];
				TObject result;
				if(environment.TryGet<TObject>(name, out result))
					return result;
			}
			string message = string.Format("Can not find any {0} named {1} in the environment.", typeof(TObject).FullName, name);
			throw new KeyNotFoundException(message);
		}
		
		#endregion
	}
}

