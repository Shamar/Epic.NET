using System;
using Challenge00.DDDSample.Voyage;
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
		DateTime estimatedTimeOfArrival { get; }
		
	}
}

