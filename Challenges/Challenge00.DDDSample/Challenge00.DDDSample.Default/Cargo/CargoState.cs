//  
//  CargoState.cs
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
using Challenge00.DDDSample.Shared;
namespace Challenge00.DDDSample.Cargo
{
	[Serializable]
	public class CargoState
	{
		private readonly DateTime _creationDate;
		public CargoState (TrackingId identifier, IRouteSpecification routeSpecification)
		{
			if(null == identifier)
				throw new ArgumentNullException("identifier");
			if(null == routeSpecification)
				throw new ArgumentNullException("routeSpecification");
			this.Identifier = identifier;
			this.RouteSpecification = routeSpecification;
		}
		
		protected CargoState (CargoState previousState, IItinerary newItinerary)
		{
			if(!previousState.RouteSpecification.IsSatisfiedBy(newItinerary))
			{
				string message = string.Format("The itinerary provided do not satisfy the route of {0}.", Identifier);
				throw new ArgumentException(message, "itinerary");
			}
			
		}
		
		protected CargoState (CargoState previousState, IRouteSpecification newRoute)
		{
		}
		
		protected CargoState (CargoState previousState, IDelivery newDelivery)
		{
		}
		
		public readonly TrackingId Identifier;

		public readonly IItinerary Itinerary;

		public readonly IRouteSpecification RouteSpecification;
		
		public readonly IDelivery Delivery;
		
		public virtual CargoState SpecifyNewRoute (IRouteSpecification routeSpecification)
		{
			return new CargoState(this, routeSpecification);
		}

		public virtual CargoState AssignToRoute (IItinerary itinerary)
		{
			return new CargoState(this, routeSpecification);
		}

		public virtual CargoState Recieve (Location.ILocation location, DateTime date)
		{
			throw new NotImplementedException ();
		}

		public virtual CargoState ClearCustoms (Location.ILocation location, DateTime date)
		{
			throw new NotImplementedException ();
		}

		public virtual CargoState Claim (Location.ILocation location, DateTime date)
		{
			throw new NotImplementedException ();
		}

		public virtual CargoState LoadOn (Voyage.IVoyage voyage, DateTime date)
		{
			throw new NotImplementedException ();
		}

		public virtual CargoState Unload (Voyage.IVoyage voyage, DateTime date)
		{
			throw new NotImplementedException ();
		}
	}
}

