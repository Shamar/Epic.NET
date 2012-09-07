//  
//  VoyageEventArgs.cs
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
	/// Event arguments for voyage executions.
	/// </summary>
	[Serializable]
	public class VoyageEventArgs : EventArgs
	{
		/// <summary>
		/// Previous location.
		/// </summary>
		public readonly UnLocode PreviousLocation;

		/// <summary>
		/// Destination location.
		/// </summary>
		public readonly UnLocode DestinationLocation;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="previousLocation">
		/// A <see cref="ILocation"/>
		/// </param>
		/// <param name="destinationLocation">
		/// A <see cref="ILocation"/>
		/// </param>
		/// <exception cref="ArgumentNullException">Any argument is <see langword="null"/>.</exception>
		public VoyageEventArgs (UnLocode previousLocation, UnLocode destinationLocation)
		{
			if (null == previousLocation)
				throw new ArgumentNullException ("previousLocation");
			if (null == destinationLocation)
				throw new ArgumentNullException ("destinationLocation");
			PreviousLocation = previousLocation;
			DestinationLocation = destinationLocation;
		}
	}
}

