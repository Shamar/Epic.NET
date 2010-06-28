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
	/// State of a Voyage when in port.
	/// </summary>
	[Serializable]
	public class InPortVoyage : VoyageState 
	{
		private readonly int _movementIndex;
		public InPortVoyage (ISchedule schedule, int movementIndex)
			: base(schedule)
		{
			if(movementIndex < 0 || movementIndex >= schedule.MovementsCount)
				throw new ArgumentOutOfRangeException("movementIndex");
			_movementIndex = movementIndex;
		}
		
		#region implemented abstract members of Challenge00.DDDSample.Voyage.VoyageState
		public override VoyageState Arrive ()
		{
			return this;
		}
		
		
		public override VoyageState Depart ()
		{
			return new MovingVoyage(Schedule, _movementIndex);
		}
		
		
		public override VoyageState MarkAsLost (ILocation lastKnownLocation)
		{
			throw new System.NotImplementedException();
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
		
		
		public override bool IsLost 
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
			InPortVoyage voyage = other as InPortVoyage;
			if(null == voyage)
				return false;
			return LastKnownLocation.Equals(voyage.LastKnownLocation) && Schedule.Equals(voyage.Schedule);
		}
		
		#endregion
		
		

	}
}

