//  
//  IDelivery.cs
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
using Challenge00.DDDSample.Voyage;
using Challenge00.DDDSample.Location;
namespace Challenge00.DDDSample.Cargo
{
	/// <summary>
	/// The actual transportation of the cargo, as opposed to the 
	/// customer requirement (<see cref="IRouteSpecification"/>) and the plan (<see cref="IItinerary"/>). 
	/// </summary>
	public interface IDelivery : IEquatable<IDelivery>
	{
		/// <summary>
		/// When this delivery was calculated. 
		/// </summary>
		DateTime CalculationDate { get; }
		
		/// <summary>
		/// Current voyage or <see langword="null"/> if 
		/// the delivery history is empty 
		/// </summary>
		VoyageNumber CurrentVoyage { get; }
		
		/// <summary>
		/// Estimated time of arrival.
		/// </summary>
		DateTime? EstimatedTimeOfArrival { get; }
		
		/// <summary>
		/// True if the cargo has been unloaded at the final destination. 
		/// </summary>
		bool IsUnloadedAtDestination { get; }
		
		/// <summary>
		/// Last known location of the cargo, or <see langword="null"/> if 
		/// the delivery history is empty 
		/// </summary>
		UnLocode LastKnownLocation { get; }
		
		/// <summary>
		/// Transport status 
		/// </summary>
		TransportStatus TransportStatus { get; }
		
		/// <summary>
		/// Routing status. 
		/// </summary>
		RoutingStatus RoutingStatus { get; }
		
	}
}

