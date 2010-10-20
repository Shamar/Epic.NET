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
	public class RouteSpecification : IRouteSpecification
	{
		private readonly UnLocode _origin;
		private readonly UnLocode _destination;
		private readonly DateTime _arrivalDeadline;
		
		public RouteSpecification (ILocation origin, ILocation destination, DateTime arrivalDeadline)
		{
			if(null == origin || origin is Unknown)
				throw new ArgumentNullException("origin");
			if(null == destination || destination is Unknown)
				throw new ArgumentNullException("destination");
			if(origin.UnLocode.Equals(destination.UnLocode))
				throw new ArgumentException("Origin and destination can't be the same: " + origin.UnLocode);
			if(arrivalDeadline <= DateTime.UtcNow)
				throw new ArgumentException("Arrival deadline can't be in the past: " + arrivalDeadline);
			
			_origin = origin.UnLocode;
			_destination = destination.UnLocode;
			_arrivalDeadline = arrivalDeadline;
		}
	
		#region IRouteSpecification implementation
		public DateTime ArrivalDeadline 
		{
			get 
			{
				return _arrivalDeadline;
			}
		}

		public UnLocode Destination 
		{
			get 
			{
				return _destination;
			}
		}

		public UnLocode Origin 
		{
			get 
			{
				return _origin;
			}
		}
		#endregion

		#region ISpecification[IItinerary] implementation
		public virtual bool IsSatisfiedBy (IItinerary candidate)
		{
			if(null == candidate)
				return false;
			if(!_origin.Equals(candidate.InitialDepartureLocation))
				return false;
			if(!_destination.Equals(candidate.FinalArrivalLocation))
				return false;
			if(_arrivalDeadline < candidate.FinalArrivalDate)
				return false;
			return true;
		}
		#endregion

		#region IEquatable[Challenge00.DDDSample.Shared.ISpecification[IItinerary]] implementation
		public virtual bool Equals (ISpecification<IItinerary> other)
		{
			IRouteSpecification otherRoute = other as IRouteSpecification;
			if(object.ReferenceEquals(otherRoute, null))
				return false;
			if(object.ReferenceEquals(this, other))
				return true;
			if(!_origin.Equals(otherRoute.Origin))
				return false;
			if(!_destination.Equals(otherRoute.Destination))
				return false;
			if(!_arrivalDeadline.Equals(otherRoute.ArrivalDeadline))
				return false;
			return true;
		}
		#endregion
		
		public sealed override bool Equals (object obj)
		{
			return Equals (obj as ISpecification<IItinerary>);
		}
		
		public override int GetHashCode ()
		{
			return _origin.GetHashCode() ^ _destination.GetHashCode();
		}
	}
}

