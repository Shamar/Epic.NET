//  
//  InitialVoyageState.cs
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
	/// <summary>
	/// State of a Voyage when stopped off.
	/// </summary>
	[Serializable]
	public class StoppedVoyage : VoyageState 
	{
		private readonly int _movementIndex;
		public StoppedVoyage (ISchedule schedule, int movementIndex)
			: base(schedule)
		{
			if(movementIndex < 0 || movementIndex >= schedule.MovementsCount)
				throw new ArgumentOutOfRangeException("movementIndex");
			_movementIndex = movementIndex;
		}
		
		#region implemented abstract members of Challenge00.DDDSample.Voyage.VoyageState
		public override VoyageState StopOverAt (ILocation location)
		{
			if(LastKnownLocation.Equals(location.UnLocode))
				return this;
			string message = string.Format("The voyage stopped over at {0}.", LastKnownLocation);
			throw new ArgumentException(message, "location");
		}
		
		
		public override VoyageState DepartFrom (ILocation location)
		{
			return new MovingVoyage(Schedule, _movementIndex);
		}
		
		public override UnLocode LastKnownLocation 
		{
			get 
			{
				return Schedule[_movementIndex].DepartureLocation;
			}
		}
		
		
		public override bool IsMoving 
		{
			get 
			{
				return false;
			}
		}
		
		#endregion
		
		#region implemented abstract members of Challenge00.DDDSample.Voyage.VoyageState
		
		public override bool Equals (VoyageState other)
		{
			if(object.ReferenceEquals(this, other))
				return true;
			StoppedVoyage voyage = other as StoppedVoyage;
			if(null == voyage)
				return false;
			return LastKnownLocation.Equals(voyage.LastKnownLocation) && Schedule.Equals(voyage.Schedule);
		}
		
		#endregion
		
		

	}
}

