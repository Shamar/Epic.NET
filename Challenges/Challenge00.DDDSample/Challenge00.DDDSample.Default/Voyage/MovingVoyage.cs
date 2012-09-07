//  
//  MovingVoyage.cs
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
	[Serializable]
	public class MovingVoyage : VoyageState 
	{
		private readonly int _movementIndex;
		public MovingVoyage (VoyageNumber number, ISchedule schedule, int movementIndex)
			: base(number, schedule)
		{
			if(movementIndex < 0 || movementIndex >= schedule.MovementsCount)
				throw new ArgumentOutOfRangeException("movementIndex");
			_movementIndex = movementIndex;
		}
		
		#region implemented abstract members of Challenge00.DDDSample.Voyage.VoyageState
		
		public override VoyageState StopOverAt (ILocation location)
		{
			if(NextExpectedLocation.Equals(location.UnLocode))
			{
				if(_movementIndex < Schedule.MovementsCount - 1)
					return new StoppedVoyage(Number, Schedule, _movementIndex + 1);
				else
					return new CompletedVoyage(Number, Schedule);
			}
			string message = string.Format("The voyage should stop over at {0}.", NextExpectedLocation);
			throw new WrongLocationException("location", message, NextExpectedLocation, location.UnLocode);
		}
		
		public override VoyageState DepartFrom (ILocation location)
		{
			if(LastKnownLocation.Equals(location.UnLocode))
				return this;
			string message = string.Format("The voyage departed from {0}.", LastKnownLocation);
			throw new WrongLocationException("location", message, LastKnownLocation, location.UnLocode);
		}
	
		public override UnLocode LastKnownLocation 
		{
			get 
			{
				return Schedule[_movementIndex].DepartureLocation;
			}
		}
		
		public override UnLocode NextExpectedLocation 
		{
			get 
			{
				return Schedule[_movementIndex].ArrivalLocation;
			}
		}
		
		public override bool IsMoving 
		{
			get 
			{
				return true;
			}
		}
		
		public override bool WillStopOverAt (ILocation location)
		{
			UnLocode locationCode = location.UnLocode;
			for(int i = _movementIndex; i < Schedule.MovementsCount; ++i)
			{
				if(locationCode.Equals(Schedule[i].ArrivalLocation))
					return true;
			}
			return false;
		}		
		
		public override bool Equals (VoyageState other)
		{
			if(object.ReferenceEquals(this, other))
				return true;
			MovingVoyage voyage = other as MovingVoyage;
			if(null == voyage)
				return false;
			return 	Number.Equals(voyage.Number) &&
					_movementIndex == voyage._movementIndex && 
					Schedule.Equals(voyage.Schedule);
		}
		
		#endregion

	}
}

