//  
//  CompletedVoyage.cs
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
	public class CompletedVoyage : VoyageState
	{
		public CompletedVoyage (VoyageNumber number, ISchedule schedule)
			: base(number, schedule)
		{
		}
		
		#region implemented abstract members of Challenge00.DDDSample.Voyage.VoyageState
		public override VoyageState StopOverAt (ILocation location)
		{
			string message = string.Format("The voyage {0} has been completed.", Number);
			throw new VoyageCompletedException(Number, message);
		}
		
		
		public override VoyageState DepartFrom (ILocation location)
		{
			string message = string.Format("The voyage {0} has been completed.", Number);
			throw new VoyageCompletedException(Number, message);
		}
		
		public override UnLocode LastKnownLocation 
		{
			get 
			{
				return Schedule[Schedule.MovementsCount - 1].ArrivalLocation;
			}
		}
		
		public override UnLocode NextExpectedLocation 
		{
			get 
			{
				return Schedule[Schedule.MovementsCount - 1].ArrivalLocation;
			}
		}
		
		public override bool IsMoving {
			get {
				return false;
			}
		}
		
		public override bool WillStopOverAt (ILocation location)
		{
			return false;
		}
		
		public override bool Equals (VoyageState other)
		{
			if(object.ReferenceEquals(this, other))
				return true;
			CompletedVoyage voyage = other as CompletedVoyage;
			if(null == voyage)
				return false;
			return Number.Equals(voyage.Number) && Schedule.Equals(voyage.Schedule);

		}
		
		#endregion

	}
}

