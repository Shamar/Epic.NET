//  
//  InPortCargo.cs
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
namespace Challenge00.DDDSample.Cargo
{
	[Serializable]
	public class InPortCargo : CargoState
	{
		private readonly UnLocode _lastKnownLocation;
		private readonly DateTime _date;
		private readonly bool _customCleared;
		public InPortCargo (CargoState previousState, ILocation location, DateTime arrivalDate)
			: base(previousState)
		{
			_lastKnownLocation = location.UnLocode;
			_date = arrivalDate;
			_customCleared = false;
		}
		
		protected InPortCargo(InPortCargo previousState, DateTime clearCustomDate)
			: base(previousState)
		{
			_customCleared = true;
		}
		
		protected InPortCargo(CargoState previousState, IRouteSpecification newRoute)
			: base(previousState, newRoute)
		{
		}
		
		protected InPortCargo(CargoState previousState, IItinerary newItinerary)
			: base(previousState, newItinerary)
		{
		}
		
		#region implemented abstract members of Challenge00.DDDSample.Cargo.CargoState
		public override CargoState SpecifyNewRoute (IRouteSpecification routeSpecification)
		{
			if(null == routeSpecification)
				throw new ArgumentNullException("routeSpecification");
			return InPortCargo(this, routeSpecification);
		}
		
		
		public override CargoState AssignToRoute (IItinerary itinerary)
		{
			if(null == itinerary)
				throw new ArgumentNullException("itinerary");
			return InPortCargo(this, itinerary);
		}
		
		public override CargoState Recieve (ILocation location, DateTime date)
		{
			if(null == location)
				throw new ArgumentNullException("location");
			throw new InvalidOperationException("Already recieved.");
		}
		
		
		public override CargoState ClearCustoms (ILocation location, DateTime date)
		{
			if(null == location)
				throw new ArgumentNullException("location");
			if(!_lastKnownLocation.Equals(location.UnLocode))
			{
				string message = string.Format("The cargo is in port at {0}. You can't clear customs in {1}.", _lastKnownLocation, location.UnLocode);
				throw new ArgumentException(message, "location");
			}
			if(date <= this._date)
			{
				string message = string.Format("The cargo arrived in port at {0}. You can't clear customs at {1}.", this._date, date);
				throw new ArgumentException(message, "date");
			}
			if(_customCleared)
				throw new InvalidOperationException("Customs already cleared.");
				                            
			return new InPortCargo(this, date);
		}
		
		
		public override CargoState Claim (ILocation location, DateTime date)
		{
			throw new System.NotImplementedException();
		}
		
		
		public override CargoState LoadOn (Challenge00.DDDSample.Voyage.IVoyage voyage, DateTime date)
		{
			throw new System.NotImplementedException();
		}
		
		
		public override CargoState Unload (Challenge00.DDDSample.Voyage.IVoyage voyage, DateTime date)
		{
			throw new System.NotImplementedException();
		}
		
		
		public override Challenge00.DDDSample.Voyage.VoyageNumber CurrentVoyage 
		{
			get 
			{
				return null;
			}
		}
		
		
		public override UnLocode LastKnownLocation 
		{
			get 
			{
				return _lastKnownLocation;
			}
		}
		
		
		public override TransportStatus TransportStatus 
		{
			get 
			{
				return TransportStatus.InPort;
			}
		}
		
		#endregion
	}
}

