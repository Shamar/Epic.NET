using System;
using Challenge00.DDDSample.Shared;
using Challenge00.DDDSample.Location;
namespace Challenge00.DDDSample.Cargo
{
	/// <summary>
	/// Route specification. Describes where a cargo orign and destination is, and the arrival deadline.  
	/// </summary>
	public interface IRouteSpecification : ISpecification<IItinerary>
	{
		/// <summary>
		/// Arrival deadline 
		/// </summary>
		DateTime ArrivalDeadline { get; }
		
		/// <summary>
		/// Specfied destination location.
		/// </summary>
		ILocation Destination { get; }
		
		/// <summary>
		/// Specified origin location. 
		/// </summary>
		ILocation Origin { get; }
	}
}

