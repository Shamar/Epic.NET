//  
//  IVoyage.cs
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
using Challenge00.DDDSample.Cargo;
namespace Challenge00.DDDSample.Voyage
{
	/// <summary>
	/// A Voyage.  
	/// </summary>
	public interface IVoyage
	{
		/// <summary>
		/// Voyage identification number. 
		/// </summary>
		VoyageNumber Number { get; }
		
		/// <summary>
		/// Schedule. 
		/// </summary>
		ISchedule Schedule { get; }

		/// <summary>
		/// Last known location. 
		/// </summary>
		UnLocode LastKnownLocation { get; }
		
		/// <summary>
		/// Next expected location.
		/// </summary>
		UnLocode NextExpectedLocation { get; }
		
		/// <summary>
		/// True if the voyage is moving toward the next arrival. 
		/// </summary>
		bool IsMoving { get; }
		
		/// <summary>
		/// Move the last knwon location to the next scheduled arrival.
		/// </summary>
		/// <param name="location">Location reached.</param>
		/// <exception cref="ArgumentNullException"><paramref name="location"/> is <see langword="null"/>.</exception>
		/// <exception cref="WrongLocationException"><paramref name="location"/> is not the correct location.</exception>
		/// <exception cref="VoyageCompletedException">The voyage has already be completed.</exception>
		void StopOverAt(ILocation location);
		
		event EventHandler<VoyageEventArgs> Stopped;
		
		/// <summary>
		/// Start a voyage movement.
		/// </summary>
		/// <param name="location">Location leave.</param>
		/// <exception cref="ArgumentNullException"><paramref name="location"/> is <see langword="null"/></exception>
		/// <exception cref="WrongLocationException"><paramref name="location"/> is not the correct location.</exception>
		/// <exception cref="VoyageCompletedException">The voyage has already be completed.</exception>
		void DepartFrom(ILocation location);
		
		event EventHandler<VoyageEventArgs> Departed;

		/// <summary>
		/// True if the voyage will stop over at <paramref name="location"/>. 
		/// Will return <see langword="false"/> when either the location is not on the schedule or it was already left.
		/// </summary>
		/// <param name="location">
		/// A <see cref="ILocation"/>.
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="location"/> is <see langword="null"/>.</exception>
		bool WillStopOverAt(ILocation location);
		
	}
}

