using System;
using Challenge00.DDDSample.Location;
namespace Challenge00.DDDSample.Voyage
{
	/// <summary>
	/// A carrier movement is a vessel voyage from one location to another.  
	/// </summary>
	/// <remarks>
	/// <para>
	/// Two carrier movement are equals if they have the same properties' values.
	/// </para>
	/// <para>
	/// To override <see cref="object.GetHashCode()"/> use the exclusive OR between the 
	/// <see cref="ICarrierMovement.ArrivalLocation"/> and the <see cref="ICarrierMovement.DepartureLocation"/>
	/// </para>
	/// </remarks>
	public interface ICarrierMovement : IEquatable<ICarrierMovement>
	{
		/// <summary>
		/// Departure location. 
		/// </summary>
		ILocation DepartureLocation { get; }
		
		/// <summary>
		/// Arrival location. 
		/// </summary>
		ILocation ArrivalLocation { get; }
		
		/// <summary>
		/// Time of departure. 
		/// </summary>
		DateTime DepartureTime { get; }
		
		/// <summary>
		/// Time of arrival. 
		/// </summary>
		DateTime ArrivalTime { get; }
		
	}
}
