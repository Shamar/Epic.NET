using System;
using Challenge00.DDDSample.Location;
namespace Challenge00.DDDSample.Voyage
{
	/// <summary>
	/// A carrier movement is a vessel voyage from one location to another.  
	/// </summary>
	public interface ICarrierMovement : IEquatable<ICarrierMovement>
	{
		/// <summary>
		/// Arrival location. 
		/// </summary>
		ILocation ArrivalLocation { get; }
		
		/// <summary>
		/// Departure location. 
		/// </summary>
		ILocation DepartureLocation { get; }
		
		/// <summary>
		/// Time of arrival. 
		/// </summary>
		DateTime ArrivalTime { get; }
		
		/// <summary>
		/// Time of departure. 
		/// </summary>
		DateTime DepartureTime { get; }
	}
}
