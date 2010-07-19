//  
//  RouteSpecification.cs
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
using Challenge00.DDDSample.Shared;
using System.Linq;
namespace Challenge00.DDDSample.Cargo
{
	[Serializable]
	public class RouteSpecification : ISpecification<IItinerary>
	{
		private readonly UnLocode _origin;
		private readonly UnLocode _destination;
		private readonly DateTime _arrivalDeadline;
		
		public RouteSpecification (ILocation origin, ILocation destination, DateTime arrivalDeadline)
		{
			if(null == origin || origin is Unknown)
				throw new ArgumentNullException("origin");
			if(null == origin || destination is Unknown)
				throw new ArgumentNullException("origin");
			if(origin.Equals(destination))
				throw new ArgumentException("Origin and destination can't be the same: " + origin.UnLocode);
			if(arrivalDeadline <= DateTime.Now)
				throw new ArgumentException("Arrival deadline can't be in the past: " + arrivalDeadline);
			
			_origin = origin.UnLocode;
			_destination = destination.UnLocode;
			_arrivalDeadline = arrivalDeadline;
		}
	

		#region ISpecification[IItinerary] implementation
		public virtual bool IsSatisfiedBy (IItinerary candidate)
		{
			if(null == candidate)
				return false;
			if(!_origin.Equals(candidate.InitialDepartureLocation))
				return false;
			if(!_destination.Equals(candidate.FinalArrivalLocation))
				return false;
			if(_arrivalDeadline < candidate.Last().UnloadTime)
				return false;
			return true;
		}
		#endregion

		#region IEquatable[Challenge00.DDDSample.Shared.ISpecification[IItinerary]] implementation
		public virtual bool Equals (ISpecification<IItinerary> other)
		{
			RouteSpecification otherRoute = other as RouteSpecification;
			if(null == otherRoute)
				return false;
			if(!_origin.Equals(otherRoute._origin))
				return false;
			if(!_destination.Equals(otherRoute._destination))
				return false;
			if(!_arrivalDeadline.Equals(otherRoute._arrivalDeadline))
				return false;
			return true;
		}
		#endregion
		
		public override bool Equals (object obj)
		{
			return Equals (obj as ISpecification<IItinerary>);
		}
		
		public override int GetHashCode ()
		{
			return _origin.GetHashCode() ^ _destination.GetHashCode();
		}
	}
}

