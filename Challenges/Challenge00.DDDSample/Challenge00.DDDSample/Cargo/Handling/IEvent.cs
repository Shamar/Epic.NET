//  
//  Event.cs
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
using Challenge00.DDDSample.Voyage;
namespace Challenge00.DDDSample.Cargo.Handling
{
	/// <summary>
	/// Single Handling event
	/// </summary>
	public interface IEvent
	{
		/// <summary>
		/// Cargo which this handling event is concerned.
		/// </summary>
		TrackingId Cargo { get; }
		
		/// <summary>
		/// Date when action represented by the event was completed.
		/// </summary>
		DateTime Date { get; }
		
		/// <summary>
		/// Type of the event.
		/// </summary>
		EventType Type { get; }
		
		/// <summary>
		/// Location where event occured.
		/// </summary>
		UnLocode Location { get; }
		
		/// <summary>
		/// Related voyage (on load and unload events)
		/// </summary>
		VoyageNumber Voyage { get; }
	}
}

