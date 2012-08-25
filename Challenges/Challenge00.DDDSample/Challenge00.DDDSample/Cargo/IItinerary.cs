//  
//  IItinerary.cs
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
using System.Collections.Generic;
namespace Challenge00.DDDSample.Cargo
{
	/// <summary>
	/// An itinerary, the plan for a cargo.
	/// </summary>
	public interface IItinerary : IEquatable<IItinerary>, IEnumerable<ILeg>
	{
		/// <summary>
		/// The location of first departure according to this itinerary. 
		/// </summary>
		UnLocode InitialDepartureLocation { get; }
		
		/// <summary>
		/// The location of final destination according to this itinerary. 
		/// </summary>
		UnLocode FinalArrivalLocation { get; }
		
		/// <summary>
		/// Date when cargo arrives at final destination.
		/// </summary>
		DateTime FinalArrivalDate { get; }
		
		/// <summary>
		/// Create a new itinerary by appending a new <paramref name="leg"/>.
		/// </summary>
		/// <param name="leg">
		/// A <see cref="ILeg"/> to append.
		/// </param>
		/// <returns>
		/// A new <see cref="IItinerary"/>
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="leg"/> is <see langword="null"/>.</exception>
		/// <exception cref="WrongLocationException">The <see cref="ILeg.LoadLocation"/> do not match the last leg 
		/// <see cref="UnloadLocation"/>.</exception>
		IItinerary Append(ILeg leg);
		
		/// <summary>
		/// Create a new itinerary by replacing a segment. 
		/// </summary>
		/// <param name="legs">
		/// The new legs.
		/// </param>
		/// <returns>
		/// A <see cref="IItinerary"/>
		/// </returns>
		/// <exception cref="ArgumentNullException">Any argument is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">The first load location of <paramref name="legs"/> do not belong to this itinerary.</exception>
		/// <exception cref="ArgumentException">The first load time of <paramref name="legs"/> is before the corrisponding unload location in the itinerary.</exception>
		/// <exception cref="ArgumentException">The first and the last legs in <paramref name="legs"/> 
		/// do not match the previous legs requirements.</exception>
		IItinerary ReplaceSegment(IItinerary legs);
	}
}

