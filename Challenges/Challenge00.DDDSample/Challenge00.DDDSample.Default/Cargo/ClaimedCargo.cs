//  
//  ClaimedCargo.cs
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
namespace Challenge00.DDDSample.Cargo
{
	[Serializable]
	public class ClaimedCargo : CargoState
	{
		public ClaimedCargo (CargoState previousState)
			: base(previousState)
		{
		}
		
		#region implemented abstract members of Challenge00.DDDSample.Cargo.CargoState
		public override CargoState SpecifyNewRoute (IRouteSpecification routeSpecification)
		{
			throw new System.NotImplementedException();
		}
		
		
		public override CargoState AssignToRoute (IItinerary itinerary)
		{
			throw new System.NotImplementedException();
		}
		
		
		public override CargoState Recieve (Challenge00.DDDSample.Location.ILocation location, DateTime date)
		{
			throw new System.NotImplementedException();
		}
		
		
		public override CargoState ClearCustoms (Challenge00.DDDSample.Location.ILocation location, DateTime date)
		{
			throw new System.NotImplementedException();
		}
		
		
		public override CargoState Claim (Challenge00.DDDSample.Location.ILocation location, DateTime date)
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
		
		
		public override Challenge00.DDDSample.Voyage.VoyageNumber CurrentVoyage {
			get {
				throw new System.NotImplementedException();
			}
		}
		
		
		public override Challenge00.DDDSample.Location.UnLocode LastKnownLocation 
		{
			get 
			{
				throw new System.NotImplementedException();
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

