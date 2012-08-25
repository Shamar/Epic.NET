//  
//  NewCargo.cs
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
namespace Challenge00.DDDSample.Cargo
{
	[Serializable]
	public class NewCargo : CargoState
	{
		public NewCargo (TrackingId identifier, IRouteSpecification routeSpecification)
			: base(identifier, routeSpecification)
		{
		}
		
		protected NewCargo(CargoState previousState, IRouteSpecification newRoute)
			: base(previousState, newRoute)
		{
		}
		
		protected NewCargo(CargoState previousState, IItinerary newItinerary)
			: base(previousState, newItinerary)
		{
		}
		
		#region implemented abstract members of Challenge00.DDDSample.Cargo.CargoState
		public override CargoState SpecifyNewRoute (IRouteSpecification routeSpecification)
		{
			if(null == routeSpecification)
				throw new ArgumentNullException("routeSpecification");
			if(this.RouteSpecification.Equals(routeSpecification))
				return this;
			return new NewCargo(this, routeSpecification);
		}
		
		
		public override CargoState AssignToRoute (IItinerary itinerary)
		{
			if(null == itinerary)
				throw new ArgumentNullException("itinerary");
			if(itinerary.Equals(this.Itinerary))
				return this;
			return new NewCargo(this, itinerary);
		}
		
		
		public override CargoState Recieve (Challenge00.DDDSample.Location.ILocation location, DateTime date)
		{
			if(null == location)
				throw new ArgumentNullException("location");
			if(RoutingStatus.Routed != this.RoutingStatus)
			{
				throw new RoutingException(Identifier, this.RoutingStatus);
			}
			if(!this.Itinerary.InitialDepartureLocation.Equals(location.UnLocode))
			{
				string message = string.Format("Can not recieve cargo {0} at {1}. The planned route start from {2}.", Identifier, location.UnLocode, Itinerary.InitialDepartureLocation);
				throw new ArgumentException(message);
			}
			return new InPortCargo(this, location.UnLocode, date);
		}
		
		
		public override CargoState ClearCustoms (Challenge00.DDDSample.Location.ILocation location, DateTime date)
		{
			string message = string.Format("Can not clear customs for the {0} cargo, since it's not yet recieved.", Identifier);
			throw new System.InvalidOperationException(message);
		}
		
		
		public override CargoState Claim (Challenge00.DDDSample.Location.ILocation location, DateTime date)
		{
			string message = string.Format("Can not claim the {0} cargo, since it's not yet recieved.", Identifier);
			throw new System.InvalidOperationException(message);
		}
		
		
		public override CargoState LoadOn (Challenge00.DDDSample.Voyage.IVoyage voyage, DateTime date)
		{
			string message = string.Format("Can not load the {0} cargo on the {1} voyage, since it's not yet recieved.", Identifier, voyage.Number);
			throw new System.InvalidOperationException(message);
		}
		
		
		public override CargoState Unload (Challenge00.DDDSample.Voyage.IVoyage voyage, DateTime date)
		{
			string message = string.Format("Can not unload the {0} cargo from the {1} voyage, since it's not yet recieved.", Identifier, voyage.Number);
			throw new System.InvalidOperationException(message);
		}
		
		
		public override Challenge00.DDDSample.Voyage.VoyageNumber CurrentVoyage 
		{
			get 
			{
				return null;
			}
		}
		
		
		public override Challenge00.DDDSample.Location.UnLocode LastKnownLocation 
		{
			get 
			{
				return null;
			}
		}
		
		
		public override TransportStatus TransportStatus 
		{
			get 
			{
				return TransportStatus.NotReceived;
			}
		}
		
		#endregion

		
		
	}
}

