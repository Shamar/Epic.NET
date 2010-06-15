using System;
using Challenge00.DDDSample.Location;
namespace Challenge00.DDDSample.Cargo
{
	/// <summary>
	/// An itinerary, the plan for a cargo.
	/// </summary>
	public interface IItinerary : IEquatable<IItinerary>
	{
		/// <summary>
		/// Test if the given handling event is expected when executing this itinerary.
		/// </summary>
		/// <param name="handlingEvent">
		/// A <see cref="IHandlingEvent"/> to test
		/// </param>
		/// <returns>
		/// <value>true</value> if the event is expected
		/// </returns>	
		bool isExpected(IHandlingEvent handlingEvent);
	}
}

