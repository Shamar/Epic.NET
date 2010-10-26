//  
//  Cargo.cs
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
using System.Threading;
namespace Challenge00.DDDSample.Cargo
{
	[Serializable]
	public class Cargo : ICargo
	{
		protected CargoState CurrentState;
		
		public Cargo (TrackingId identifier, IRouteSpecification routeSpecification)
			: this(new NewCargo(identifier, routeSpecification))
		{
		}
	
		protected Cargo(CargoState state)
		{
			if(null == state)
				throw new ArgumentNullException("state");
			CurrentState = state;
		}

		#region ICargo implementation
		
		public event EventHandler<ChangeEventArgs<IRouteSpecification>> NewRouteSpecified;

		public event EventHandler<ChangeEventArgs<IItinerary>> ItineraryChanged;

		public event EventHandler<HandlingEventArgs> Recieved;

		public event EventHandler<HandlingEventArgs> CustomsCleared;

		public event EventHandler<HandlingEventArgs> Claimed;

		public event EventHandler<HandlingEventArgs> Loaded;

		public event EventHandler<HandlingEventArgs> Unloaded;

		public void SpecifyNewRoute (IRouteSpecification routeSpecification)
		{
			// Thread safe, lock free sincronization
	        CargoState stateBeforeTransition;
	        CargoState previousState = CurrentState;
	        do
	        {
	            stateBeforeTransition = previousState;
	            CargoState newValue = stateBeforeTransition.SpecifyNewRoute(routeSpecification);
	            previousState = Interlocked.CompareExchange<CargoState>(ref this.CurrentState, newValue, stateBeforeTransition);
	        }
	        while (previousState != stateBeforeTransition);

			if(!previousState.Equals(this.CurrentState))
			{
				ChangeEventArgs<IRouteSpecification> args = new ChangeEventArgs<IRouteSpecification>(previousState.RouteSpecification, CurrentState.RouteSpecification);
				
				EventHandler<ChangeEventArgs<IRouteSpecification>> handler = NewRouteSpecified;
				if(null != handler)
					handler(this, args);
			}
		}

		public void AssignToRoute (IItinerary itinerary)
		{
			// Thread safe, lock free sincronization
	        CargoState stateBeforeTransition;
	        CargoState previousState = CurrentState;
	        do
	        {
	            stateBeforeTransition = previousState;
	            CargoState newValue = stateBeforeTransition.AssignToRoute(itinerary);
	            previousState = Interlocked.CompareExchange<CargoState>(ref this.CurrentState, newValue, stateBeforeTransition);
	        }
	        while (previousState != stateBeforeTransition);

			if(!previousState.Equals(this.CurrentState))
			{
				ChangeEventArgs<IItinerary> args = new ChangeEventArgs<IItinerary>(previousState.Itinerary, CurrentState.Itinerary);
				
				EventHandler<ChangeEventArgs<IItinerary>> handler = ItineraryChanged;
				if(null != handler)
					handler(this, args);
			}
		}

		public void Recieve (Location.ILocation location, DateTime date)
		{
			// Thread safe, lock free sincronization
	        CargoState stateBeforeTransition;
	        CargoState previousState = CurrentState;
	        do
	        {
	            stateBeforeTransition = previousState;
	            CargoState newValue = stateBeforeTransition.Recieve(location, date);
	            previousState = Interlocked.CompareExchange<CargoState>(ref this.CurrentState, newValue, stateBeforeTransition);
	        }
	        while (previousState != stateBeforeTransition);

			if(!previousState.Equals(this.CurrentState))
			{
				HandlingEventArgs args = new HandlingEventArgs(CurrentState, CurrentState.CalculationDate);
				
				EventHandler<HandlingEventArgs> handler = Recieved;
				if(null != handler)
					handler(this, args);
			}
		}

		public void ClearCustoms (Location.ILocation location, DateTime date)
		{
			// Thread safe, lock free sincronization
	        CargoState stateBeforeTransition;
	        CargoState previousState = CurrentState;
	        do
	        {
	            stateBeforeTransition = previousState;
	            CargoState newValue = stateBeforeTransition.ClearCustoms(location, date);
	            previousState = Interlocked.CompareExchange<CargoState>(ref this.CurrentState, newValue, stateBeforeTransition);
	        }
	        while (previousState != stateBeforeTransition);

			if(!previousState.Equals(this.CurrentState))
			{
				HandlingEventArgs args = new HandlingEventArgs(CurrentState, CurrentState.CalculationDate);
				
				EventHandler<HandlingEventArgs> handler = CustomsCleared;
				if(null != handler)
					handler(this, args);
			}
		}

		public void Claim (Location.ILocation location, DateTime date)
		{
			// Thread safe, lock free sincronization
	        CargoState stateBeforeTransition;
	        CargoState previousState = CurrentState;
	        do
	        {
	            stateBeforeTransition = previousState;
	            CargoState newValue = stateBeforeTransition.Claim(location, date);
	            previousState = Interlocked.CompareExchange<CargoState>(ref this.CurrentState, newValue, stateBeforeTransition);
	        }
	        while (previousState != stateBeforeTransition);

			if(!previousState.Equals(this.CurrentState))
			{
				HandlingEventArgs args = new HandlingEventArgs(CurrentState, CurrentState.CalculationDate);
				
				EventHandler<HandlingEventArgs> handler = Claimed;
				if(null != handler)
					handler(this, args);
			}
		}

		public void LoadOn (Voyage.IVoyage voyage, DateTime date)
		{
			// Thread safe, lock free sincronization
	        CargoState stateBeforeTransition;
	        CargoState previousState = CurrentState;
	        do
	        {
	            stateBeforeTransition = previousState;
	            CargoState newValue = stateBeforeTransition.LoadOn(voyage, date);
	            previousState = Interlocked.CompareExchange<CargoState>(ref this.CurrentState, newValue, stateBeforeTransition);
	        }
	        while (previousState != stateBeforeTransition);

			if(!previousState.Equals(this.CurrentState))
			{
				HandlingEventArgs args = new HandlingEventArgs(CurrentState, CurrentState.CalculationDate);
				
				EventHandler<HandlingEventArgs> handler = Loaded;
				if(null != handler)
					handler(this, args);
			}
		}

		public void Unload (Voyage.IVoyage voyage, DateTime date)
		{
			// Thread safe, lock free sincronization
	        CargoState stateBeforeTransition;
	        CargoState previousState = CurrentState;
	        do
	        {
	            stateBeforeTransition = previousState;
	            CargoState newValue = stateBeforeTransition.Unload(voyage, date);
	            previousState = Interlocked.CompareExchange<CargoState>(ref this.CurrentState, newValue, stateBeforeTransition);
	        }
	        while (previousState != stateBeforeTransition);

			if(!previousState.Equals(this.CurrentState))
			{
				HandlingEventArgs args = new HandlingEventArgs(CurrentState, CurrentState.CalculationDate);
				
				EventHandler<HandlingEventArgs> handler = Unloaded;
				if(null != handler)
					handler(this, args);
			}
		}

		public TrackingId TrackingId 
		{
			get 
			{
				return CurrentState.Identifier;
			}
		}

		public IDelivery Delivery 
		{
			get 
			{
				return CurrentState;
			}
		}

		public IItinerary Itinerary 
		{
			get 
			{
				return CurrentState.Itinerary;
			}
		}

		public IRouteSpecification RouteSpecification 
		{
			get 
			{
				return CurrentState.RouteSpecification;
			}
		}
		#endregion
	}
}

