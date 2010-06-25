//  
//  IVoyage.cs
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
using Challenge00.DDDSample.Location;
using Challenge00.DDDSample.Cargo;
namespace Challenge00.DDDSample.Voyage
{
	/// <summary>
	/// A Voyage.  
	/// </summary>
	public interface IVoyage : IEquatable<IVoyage>
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
		/// True if the voyage is moving toward the next arrival. 
		/// </summary>
		bool IsMoving { get; }
		
		/// <summary>
		/// The voyage is lost.
		/// </summary>
		bool IsLost { get; }
		
		/// <summary>
		/// Move the last knwon location to the next scheduled arrival.
		/// </summary>
		void Arrive();
		
		event EventHandler<VoyageEventArgs> Arrived;
		
		/// <summary>
		/// Start a voyage movement.
		/// </summary>
		void Depart();
		
		event EventHandler<VoyageEventArgs> Departed;
		
		/// <summary>
		/// Mark the voyage as lost. 
		/// </summary>
		/// <param name="lastKnownLocation">
		/// The last known <see cref="ILocation"/>
		/// </param>
		void MarkAsLost(ILocation lastKnownLocation);
		
		event EventHandler<VoyageEventArgs> Losing;
	}
}

