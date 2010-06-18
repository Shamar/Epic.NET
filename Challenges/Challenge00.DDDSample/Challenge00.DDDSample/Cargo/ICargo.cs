using System;
using Challenge00.DDDSample.Voyage;
using Challenge00.DDDSample.Location;
using Challenge00.DDDSample.Handling;
using System.Collections.Generic;
namespace Challenge00.DDDSample.Cargo
{
	/// <summary>
	/// A Cargo 
	/// </summary>
	public interface ICargo : IEquatable<ICargo>
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
		/// <exception cref="ArgumentNullException"><paramref name="routeSpecification"/> is <value>null</value></exception>
		void SpecifyNewRoute (IRouteSpecification routeSpecification);

		event EventHandler<ChangeEventArgs<IRouteSpecification>> RouteChanged;

		/// <summary>
		/// Attach a new itinerary to this cargo.  
		/// </summary>
		/// <param name="itinerary">
		/// A <see cref="IItinerary"/>. May not be null.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="itinerary"/> is <value>null</value></exception>
		void AssignToRoute (IItinerary itinerary);

		event EventHandler<ChangeEventArgs<IItinerary>> ItineraryChanged;

		/// <summary>
		/// Recieve the cargo at <paramref name="location"/>
		/// </summary>
		/// <param name="location">
		/// A <see cref="ILocation"/>. May not be null.
		/// </param>
		/// <param name="date">Date of occurence.</param>
		/// <exception cref="ArgumentNullException"><paramref name="location"/> is <value>null</value></exception>
		void Recieve(ILocation location, DateTime date);
		
		event EventHandler<HandlingEventArgs> Recieved;
		
		/// <summary>
		/// Claim the cargo. 
		/// </summary>
		/// <param name="date">Date of occurence.</param>
		void Claim(DateTime date);
		
		event EventHandler<HandlingEventArgs> Claimed;
		
		/// <summary>
		/// Load the cargo on a vassel.
		/// </summary>
		/// <param name="voyage">
		/// A <see cref="IVoyage"/>. May not be null. 
		/// </param>
		/// <param name="date">Date of occurence.</param>
		/// <exception cref="ArgumentNullException"><paramref name="voyage"/> is <value>null</value></exception>
		void LoadOn(IVoyage voyage, DateTime date);
		
		event EventHandler<LoadingEventArgs> Loaded;

		/// <summary>
		/// Unload the cargo from the current vassel. 
		/// </summary>
		/// <param name="date">Date of occurence.</param>
		void Unload(DateTime date);
		
		event EventHandler<LoadingEventArgs> Unloaded;
	}
}

