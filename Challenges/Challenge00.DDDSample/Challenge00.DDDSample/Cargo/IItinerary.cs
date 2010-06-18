using System;
using Challenge00.DDDSample.Location;
using System.Collections.Generic;
namespace Challenge00.DDDSample.Cargo
{
	/// <summary>
	/// An itinerary, the plan for a cargo.
	/// </summary>
	public interface IItinerary : IEquatable<IItinerary>, IEnumerable<ILeg>
	{
		/// <summary>
		/// The location of first departure according to this itinerary. 
		/// </summary>
		ILocation InitialDepartureLocation { get; }
		
		/// <summary>
		/// The location of last arrival according to this itinerary. 
		/// </summary>
		ILocation FinalArrivalLocation { get; }
	}
}

