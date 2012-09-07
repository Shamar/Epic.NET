//  
//  ISchedule.cs
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
using System.Collections.Generic;
namespace Challenge00.DDDSample.Voyage
{
	/// <summary>
	/// A voyage schedule.  
	/// </summary>
	/// <remarks>
	/// <para>
	/// Two schedule are equals if they contains the same <see cref="ICarrierMovement"/>.
	/// </para>
	/// <para>
	/// To override <see cref="object.GetHashCode()"/> use the exclusive OR between the 
	/// the fist and the last <see cref="ICarrierMovement"/>.
	/// </para>
	/// </remarks>
	public interface ISchedule : IEquatable<ISchedule>, IEnumerable<ICarrierMovement>
	{
		/// <summary>
		/// Create a new schedule by appending a carrier movement. 
		/// </summary>
		/// <param name="movement">
		/// A <see cref="ICarrierMovement"/> to append
		/// </param>
		/// <returns>
		/// A new <see cref="ISchedule"/>
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="movement"/> is <see langword="null"/>.</exception>
		/// <exception cref="Location.WrongLocationException"><paramref name="movement"/>'s departure does not match the scheduled arrival.</exception>
		ISchedule Append(ICarrierMovement movement);
		
		/// <summary>
		/// Number of movements.
		/// </summary>
		int MovementsCount { get; }
		
		/// <summary>
		/// Return the <see cref="ICarrierMovement"/> at <paramref name="index"/>.
		/// </summary>
		/// <param name="index">
		/// The movement index in the schedule sequence. 
		/// </param>
		/// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is not valid.</exception>
		ICarrierMovement this[int index] { get; }
	}
}

