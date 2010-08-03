//  
//  VoyageState.cs
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
namespace Challenge00.DDDSample.Voyage
{
	[Serializable]
	public abstract class VoyageState : IEquatable<VoyageState>
	{
		public readonly ISchedule Schedule;
		
		public readonly VoyageNumber Number;
		
		protected VoyageState(VoyageNumber number, ISchedule schedule)
		{
			if(null == schedule)
				throw new ArgumentNullException("schedule");
			if(null == number)
				throw new ArgumentNullException("number");
			Schedule = schedule;
			Number = number;
		}
		
		public abstract VoyageState StopOverAt (ILocation location);
		
		public abstract VoyageState DepartFrom (ILocation location);
		
		public abstract UnLocode LastKnownLocation { get; }

		public abstract UnLocode NextExpectedLocation { get; }

		public abstract bool IsMoving { get; }
		
		public abstract bool WillStopOverAt(ILocation location);

		#region IEquatable[VoyageState] implementation
		
		public abstract bool Equals (VoyageState other);
		
		#endregion
		
		public sealed override bool Equals (object obj)
		{
			return Equals(obj as VoyageState);
		}
		
		public sealed override int GetHashCode ()
		{
			return this.GetType().GetHashCode() ^ LastKnownLocation.GetHashCode();
		}
	}
}

