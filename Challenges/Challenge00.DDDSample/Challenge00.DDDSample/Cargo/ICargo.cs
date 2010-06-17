using System;
using Challenge00.DDDSample.Voyage;
using Challenge00.DDDSample.Location;
namespace Challenge00.DDDSample.Cargo
{
	/// <summary>
	/// A Cargo 
	/// </summary>
	public interface ICargo : IEquatable<ICargo>
	{
		TrackingId TrackingId { get; }

		IDelivery Delivery { get; }

		IItinerary Itinerary { get; }

		IRouteSpecification RouteSpecification { get; }

		void AssignNewRoute (IRouteSpecification routeSpecification);

		event EventHandler<ChangeEventArgs<IRouteSpecification>> RouteChanging;

		event EventHandler<ChangeEventArgs<IRouteSpecification>> RouteChanged;

		void AssignNewItinerary (IItinerary itinerary);

		event EventHandler<ChangeEventArgs<IItinerary>> ItineraryChanging;

		event EventHandler<ChangeEventArgs<IItinerary>> ItineraryChanged;

		void LoadOn (IVoyage voyage);

		void Unload ();
	}
}

