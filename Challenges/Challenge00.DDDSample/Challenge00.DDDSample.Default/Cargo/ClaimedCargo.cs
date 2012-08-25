//  
//  ClaimedCargo.cs
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
using Challenge00.DDDSample.Voyage;
using Challenge00.DDDSample.Location;
namespace Challenge00.DDDSample.Cargo
{
	[Serializable]
	public class ClaimedCargo : CargoState
	{
		private readonly DateTime _claimDate;
		public ClaimedCargo (CargoState previousState, DateTime claimDate)
			: base(previousState)
		{
			if(previousState.RoutingStatus == RoutingStatus.Misrouted)
				throw new ArgumentException(string.Format("Can not claim the misrouted cargo {0}.", Identifier));
			if(previousState.RoutingStatus == RoutingStatus.NotRouted)
				throw new ArgumentException(string.Format("Can not claim the cargo {0} since it is not routed.", Identifier));
			if(previousState.TransportStatus != TransportStatus.InPort)
				throw new ArgumentException(string.Format("Can not claim the cargo {0} since it is not yet in port.", Identifier));
			if(!previousState.IsUnloadedAtDestination)
				throw new ArgumentException(string.Format("Can not claim the cargo {0} since it is not yet been unloaded at destination.", Identifier));
			_claimDate = claimDate;
		}
		
		public override bool IsUnloadedAtDestination 
		{
			get 
			{
				return true;
			}
		}
		
		#region implemented abstract members of Challenge00.DDDSample.Cargo.CargoState
		public override CargoState SpecifyNewRoute (IRouteSpecification routeSpecification)
		{
			string message = string.Format("The cargo {0} has been claimed.", Identifier);
			throw new AlreadyClaimedException(Identifier, message);
		}
		
		
		public override CargoState AssignToRoute (IItinerary itinerary)
		{
			string message = string.Format("The cargo {0} has been claimed.", Identifier);
			throw new AlreadyClaimedException(Identifier, message);
		}
		
		
		public override CargoState Recieve (ILocation location, DateTime date)
		{
			string message = string.Format("The cargo {0} has been claimed.", Identifier);
			throw new AlreadyClaimedException(Identifier, message);
		}
		
		
		public override CargoState ClearCustoms (ILocation location, DateTime date)
		{
			string message = string.Format("The cargo {0} has been claimed.", Identifier);
			throw new AlreadyClaimedException(Identifier, message);
		}
		
		
		public override CargoState Claim (ILocation location, DateTime date)
		{
			if(null == location)
				throw new ArgumentNullException("location");
			if(location.UnLocode.Equals(LastKnownLocation) && date == _claimDate)
				return this;
			string message = string.Format("The cargo {0} has been claimed.", Identifier);
			throw new AlreadyClaimedException(Identifier, message);
		}
		
		
		public override CargoState LoadOn (IVoyage voyage, DateTime date)
		{
			string message = string.Format("The cargo {0} has been claimed.", Identifier);
			throw new AlreadyClaimedException(Identifier, message);
		}
		
		
		public override CargoState Unload (IVoyage voyage, DateTime date)
		{
			string message = string.Format("The cargo {0} has been claimed.", Identifier);
			throw new AlreadyClaimedException(Identifier, message);
		}
		
		
		public override VoyageNumber CurrentVoyage 
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
				return Itinerary.FinalArrivalLocation;
			}
		}
		
		
		public override TransportStatus TransportStatus 
		{
			get 
			{
				return TransportStatus.Claimed;
			}
		}
		
		#endregion
	}
}

