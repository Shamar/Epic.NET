//  
//  ILocation.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010 Giacomo Tesio
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
	/// A location is our model is stops on a journey, such as cargo origin or destination, or carrier movement endpoints. It is uniquely identified by a UN Locode.  
	/// </summary>
	public interface ILocation : IEquatable<ILocation>
	{
		/// <summary>
		/// Actual name of this location, e.g. "Stockholm". 
		/// </summary>
		string Name { get; }
		
		/// <summary>
		/// UN Locode for this location.
		/// </summary>
		UnLocode UnLocode { get; }
	}
}

