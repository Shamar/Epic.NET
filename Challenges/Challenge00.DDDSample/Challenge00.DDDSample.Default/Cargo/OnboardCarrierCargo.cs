//  
//  OnboardCarrierCargo.cs
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
	public class OnboardCarrierCargo : CargoState
	{
		private readonly VoyageNumber _voyage;
		private readonly DateTime _date;
		private readonly UnLocode _lastKnownLocation;
		public OnboardCarrierCargo (CargoState previousState, IVoyage voyage, DateTime loadDate)
			: base(previousState)
		{
			if(null == voyage)
				throw new ArgumentNullException("voyage");
			_voyage = voyage.Number;
			_date = loadDate;
			_lastKnownLocation = voyage.LastKnownLocation;
		}
		
		#region implemented abstract members of Challenge00.DDDSample.Cargo.CargoState
		public override CargoState SpecifyNewRoute (IRouteSpecification routeSpecification)
		{
			string message = string.Format("The cargo {0} has been loaded on the {1} voyage.", Identifier, _voyage);
			throw new InvalidOperationException(message);
		}
		
		
		public override CargoState AssignToRoute (IItinerary itinerary)
		{
			string message = string.Format("The cargo {0} has been loaded on the {1} voyage.", Identifier, _voyage);
			throw new InvalidOperationException(message);
		}
		
		
		public override CargoState Recieve (ILocation location, DateTime date)
		{
			throw new InvalidOperationException("Already recieved.");
		}
		
		
		public override CargoState ClearCustoms (ILocation location, DateTime date)
		{
			string message = string.Format("The cargo {0} has been loaded on the {1} voyage.", Identifier, _voyage);
			throw new InvalidOperationException(message);
		}
		
		
		public override CargoState Claim (ILocation location, DateTime date)
		{
			string message = string.Format("The cargo {0} has been loaded on the {1} voyage.", Identifier, _voyage);
			throw new InvalidOperationException(message);
		}
		
		
		public override CargoState LoadOn (IVoyage voyage, DateTime date)
		{
			if(null == voyage)
				throw new ArgumentNullException("voyage");
			if(voyage.Number.Equals(_voyage))
				return this;
			string message = string.Format("The cargo {0} is loaded on the voyage {1}, so it can not be loaded on the voyage {2}.", Identifier, _voyage, voyage.Number);
			throw new InvalidOperationException(message);
		}
		
		
		public override CargoState Unload (IVoyage voyage, DateTime date)
		{
			if(null == voyage)
				throw new ArgumentNullException("voyage");
			if(!voyage.Number.Equals(_voyage))
			{
				string message = string.Format("The cargo {0} is loaded on the voyage {1}, so it can not be unloaded from the voyage {2}.", Identifier, _voyage, voyage.Number);
				throw new ArgumentException(message, "voyage");
			}
			if(voyage.IsMoving)
			{
				string message = string.Format("The cargo {0} is loaded on the voyage {1}, since it's moving to {2}.", Identifier, _voyage, voyage.NextExpectedLocation);
				throw new ArgumentException(message, "voyage");
			}
			if(date < this._date)
			{
				string message = string.Format("The cargo {2} was loaded on the voyage {3} at {0}. Can not be unloaded at {1}.", this._date, date, Identifier, voyage.Number);
				throw new ArgumentException(message, "date");
			}
			return new InPortCargo(this, voyage.LastKnownLocation, date);
		}
		
		
		public override VoyageNumber CurrentVoyage 
		{
			get 
			{
				return _voyage;
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
				return TransportStatus.OnboardCarrier;
			}
		}
		
		#endregion

	}
}

