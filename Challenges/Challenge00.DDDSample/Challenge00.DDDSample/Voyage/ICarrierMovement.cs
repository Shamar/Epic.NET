//  
//  ICarrierMovement.cs
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
using Challenge00.DDDSample.Location;
namespace Challenge00.DDDSample.Voyage
{
	/// <summary>
	/// A carrier movement is a vessel voyage from one location to another.  
	/// </summary>
	/// <remarks>
	/// <para>
	/// Two carrier movement are equals if they have the same properties' values.
	/// </para>
	/// <para>
	/// To override <see cref="object.GetHashCode()"/> use the exclusive OR between the 
	/// <see cref="ICarrierMovement.ArrivalLocation"/> and the <see cref="ICarrierMovement.DepartureLocation"/>
	/// </para>
	/// </remarks>
	public interface ICarrierMovement : IEquatable<ICarrierMovement>
	{
		/// <summary>
		/// Departure location. 
		/// </summary>
		UnLocode DepartureLocation { get; }
		
		/// <summary>
		/// Arrival location. 
		/// </summary>
		UnLocode ArrivalLocation { get; }
		
		/// <summary>
		/// Time of departure. 
		/// </summary>
		DateTime DepartureTime { get; }
		
		/// <summary>
		/// Time of arrival. 
		/// </summary>
		DateTime ArrivalTime { get; }
		
	}
}
