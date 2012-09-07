//  
//  RoleBase.cs
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

namespace Epic.Organization
{
	/// <summary>
	/// Base class for users' roles.
	/// </summary>
	[Serializable]
	public abstract class RoleBase : IDisposable
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Organization.RoleBase"/> class.
        /// </summary>
		protected RoleBase ()
		{
		}

		#region IDisposable implementation
        /// <summary>
        /// Releases the responsibilities reppresented by the <see cref="Epic.Organization.RoleBase"/> object.
        /// </summary>
        /// <remarks>
        /// <see cref="WorkingSessionBase"/> will call <see cref="Dispose"/> when 
        /// the implemented role has been left with <see cref="IWorkingSession.Leave{TRole}"/> 
        /// as many time it has been achieved with <see cref="IWorkingSession.Achieve{TRole}"/> .
        /// </remarks>
		public abstract void Dispose ();
		#endregion
	}
}

