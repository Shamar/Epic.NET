using System;
using Challenge00.DDDSample.Voyage;
using Challenge00.DDDSample.Location;
namespace Challenge00.DDDSample.Cargo
{
	/// <summary>
	/// An itinerary consists of one or more legs.  
	/// </summary>
	public interface ILeg : IEquatable<ILeg>
	{
		/// <summary>
		/// Voyage 
		/// </summary>
		IVoyage Voyage { get; }
		
		/// <summary>
		/// Load time
		/// </summary>
		DateTime LoadTime { get; }
		
		/// <summary>
		/// Load location 
		/// </summary>
		ILocation LoadLocation { get; }
		
		/// <summary>
		/// Unload time 
		/// </summary>
		DateTime UnloadTime { get; }
		
		/// <summary>
		/// Unload location 
		/// </summary>
		ILocation UnloadLocation { get; }
	}
}

