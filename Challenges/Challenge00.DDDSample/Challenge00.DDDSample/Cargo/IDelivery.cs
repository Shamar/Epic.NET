using System;
using Challenge00.DDDSample.Voyage;
using Challenge00.DDDSample.Location;
namespace Challenge00.DDDSample
{
	/// <summary>
	/// The actual transportation of the cargo, as opposed to the 
	/// customer requirement (RouteSpecification) and the plan (Itinerary). 
	/// </summary>
	public interface IDelivery : IEquatable<IDelivery>
	{
		/// <summary>
		/// When this delivery was calculated. 
		/// </summary>
		DateTime CalculationDate { get; }
		
		/// <summary>
		/// Current voyage. 
		/// </summary>
		IVoyage CurrentVoyage { get; }
		
		/// <summary>
		/// Estimated time of arrival.
		/// </summary>
		DateTime EstimatedTimeOfArrival { get; }
		
		/// <summary>
		/// True if the cargo has been unloaded at the final destination. 
		/// </summary>
		bool IsUnloadedAtDestination { get; }
		
		/// <summary>
		/// True if cargo is misdirected.  
		/// </summary>
		/// <remarks>
		/// <list type="bullet">
		/// <item>A cargo is misdirected if it is in a location that's not in the itinerary.</item>
		/// <item>A cargo with no itinerary can not be misdirected.</item>
		/// <item>A cargo that has received no handling events can not be misdirected.</item>>
		/// </list>
		/// </remarks>
		bool IsMisdirected { get; }
		
		/// <summary>
		/// Last known location of the cargo, or <see cref="Location.Unknown"/> if 
		/// the delivery history is empty 
		/// </summary>
		ILocation LastKnownLocation { get; }
		
	}
}

