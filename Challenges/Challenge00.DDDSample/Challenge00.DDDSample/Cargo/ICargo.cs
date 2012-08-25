//  
//  ICargo.cs
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
using Challenge00.DDDSample.Shared;
using Challenge00.DDDSample.Voyage;
using Challenge00.DDDSample.Location;
using System.Collections.Generic;
namespace Challenge00.DDDSample.Cargo
{
	/// <summary>
	/// A Cargo.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This is the central class in the domain model, and it is the root of the 
	/// Cargo-Itinerary-Leg-Delivery-RouteSpecification aggregate.
	/// </para>
	/// <para>
	/// A cargo is identified by a unique <see cref="TrackingId"/>, and it always has an origin and a route specification.
	/// </para>
	/// <para>
	/// The life cycle of a cargo begins with the booking procedure, when the tracking id is assigned. 
	/// During a (short) period of time, between booking and initial routing, the cargo has no itinerary. 
	/// </para>
	/// <para>
	/// The booking clerk requests a list of possible routes, matching the route specification, 
	/// and assigns the cargo to one route. 
	/// The route to which a cargo is assigned is described by an itinerary. 
	/// </para>
	/// <para>
	/// A cargo can be re-routed during transport, on demand of the customer, in which case a 
	/// new route is specified for the cargo and a new route is requested. 
	/// The old itinerary, being a value object, is discarded and a new one is attached. 
	/// </para>
	/// <para>
	/// It may also happen that a cargo is accidentally misrouted, which should notify the proper 
	/// personnel and also trigger a re-routing procedure. 
	/// </para>
	/// <para>
	/// When a cargo is handled, the status of the delivery changes. 
	/// Everything about the delivery of the cargo is contained in the Delivery value object, 
	/// which is replaced whenever a cargo is handled by an asynchronous event triggered 
	/// by the registration of the handling event. 
	/// </para>
	/// <para>
	/// The delivery can also be affected by routing changes, i.e. when a the route specification changes, 
	/// or the cargo is assigned to a new route. 
	/// In that case, the delivery update is performed synchronously within the cargo aggregate. 
	/// </para>
	/// <para>
	/// The life cycle of a cargo ends when the cargo is claimed by the customer. 
	/// </para>
	/// <para>
	/// The cargo aggregate, and the entre domain model, is built to solve the problem of booking and tracking cargo. 
	/// All important business rules for determining whether or not a cargo is misdirected, 
	/// what the current status of the cargo is (on board carrier, in port etc), are captured in this aggregate. 
	/// <para>
	/// </remarks>
	public interface ICargo
	{
		/// <summary>
		/// Cargo identifier. 
		/// </summary>
		TrackingId TrackingId { get; }

		/// <summary>
		/// Delivery information 
		/// </summary>
		IDelivery Delivery { get; }

		/// <summary>
		/// Assigned itinerary. 
		/// </summary>
		IItinerary Itinerary { get; }

		/// <summary>
		/// Route specification
		/// </summary>
		IRouteSpecification RouteSpecification { get; }

		/// <summary>
		/// Assign a new route specification to the cargo.
		/// </summary>
		/// <param name="routeSpecification">
		/// A <see cref="IRouteSpecification"/>. May not be null.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="routeSpecification"/> is <see langword="null"/></exception>
		/// <exception cref="AlreadyClaimedException">The cargo has already be claimed by the customer.</exception>
		void SpecifyNewRoute (IRouteSpecification routeSpecification);

		event EventHandler<ChangeEventArgs<IRouteSpecification>> NewRouteSpecified;

		/// <summary>
		/// Attach a new itinerary to this cargo.  
		/// </summary>
		/// <param name="itinerary">
		/// A <see cref="IItinerary"/>. May not be null.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="itinerary"/> is <see langword="null"/></exception>
		/// <exception cref="AlreadyClaimedException">The cargo has already be claimed by the customer.</exception>
		void AssignToRoute (IItinerary itinerary);

		event EventHandler<ChangeEventArgs<IItinerary>> ItineraryChanged;

		/// <summary>
		/// Recieve the cargo at <paramref name="location"/>
		/// </summary>
		/// <param name="location">
		/// A <see cref="ILocation"/>. May not be null.
		/// </param>
		/// <param name="date">Date of occurence.</param>
		/// <exception cref="ArgumentNullException"><paramref name="location"/> is <see langword="null"/></exception>
		/// <exception cref="RountingException">The cargo is either not routed or misrouted.</exception>
		/// <exception cref="AlreadyClaimedException">The cargo has already be claimed by the customer.</exception>
		void Recieve(ILocation location, DateTime date);
		
		event EventHandler<HandlingEventArgs> Recieved;
		
		/// <summary>
		/// Clear through customs.
		/// </summary>
		/// <param name="location">
		/// A <see cref="ILocation"/>. May not be null.
		/// </param>
		/// <param name="date">
		/// A <see cref="DateTime"/> 
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="location"/> is <see langword="null"/></exception>
		/// <exception cref="AlreadyClaimedException">The cargo has already be claimed by the customer.</exception>
		void ClearCustoms(ILocation location, DateTime date);
		
		event EventHandler<HandlingEventArgs> CustomsCleared;
		
		/// <summary>
		/// Claim the cargo. 
		/// </summary>
		/// <param name="location">
		/// A <see cref="ILocation"/>. May not be null.
		/// </param>
		/// <param name="date">Date of occurence.</param>
		/// <exception cref="ArgumentNullException"><paramref name="location"/> is <see langword="null"/></exception>
		/// <exception cref="AlreadyClaimedException">The cargo has already be claimed by the customer.</exception>
		void Claim(ILocation location, DateTime date);
		
		event EventHandler<HandlingEventArgs> Claimed;
		
		/// <summary>
		/// Load the cargo on a vassel.
		/// </summary>
		/// <param name="voyage">
		/// A <see cref="IVoyage"/>. May not be null. 
		/// </param>
		/// <param name="date">Date of occurence.</param>
		/// <exception cref="ArgumentNullException"><paramref name="voyage"/> is <see langword="null"/></exception>
		/// <exception cref="AlreadyClaimedException">The cargo has already be claimed by the customer.</exception>
		void LoadOn(IVoyage voyage, DateTime date);
		
		event EventHandler<HandlingEventArgs> Loaded;

		/// <summary>
		/// Unload the cargo from the current vassel. 
		/// </summary>
		/// <param name="voyage">
		/// A <see cref="IVoyage"/>. May not be null. 
		/// </param>
		/// <param name="date">Date of occurence.</param>
		/// <exception cref="ArgumentNullException"><paramref name="voyage"/> is <see langword="null"/></exception>
		/// <exception cref="AlreadyClaimedException">The cargo has already be claimed by the customer.</exception>
		void Unload(IVoyage voyage, DateTime date);
		
		event EventHandler<HandlingEventArgs> Unloaded;
	}
}

