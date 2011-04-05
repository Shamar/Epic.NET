//  
//  Unknown.cs
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
namespace Challenge00.DDDSample.Location
{
	/// <summary>
	/// Unknown location. 
	/// </summary>
	[Serializable]
	public sealed class Unknown : ILocation
	{
		private static Unknown _location = new Unknown();
		
		public static ILocation Location
		{
			get
			{
				return _location;
			}
		}
		
		private Unknown ()
		{
		}
		
		#region ILocation implementation
		public string Name {
			get {
				return "Unknown";
			}
		}
		
		
		public UnLocode UnLocode {
			get {
				return null;
			}
		}
		
		#endregion
	}
}

