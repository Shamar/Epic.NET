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
		
		/// <summary>
		/// Create a new itinerary by appending a new <paramref name="leg"/>.
		/// </summary>
		/// <param name="leg">
		/// A <see cref="ILeg"/> to append.
		/// </param>
		/// <returns>
		/// A new <see cref="IItinerary"/>
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="leg"/> is <value>null</value>.</exception>
		/// <exception cref="ArgumentException">The <see cref="ILeg.LoadLocation"/> do not match the last leg 
		/// <see cref="UnloadLocation"/>.</exception>
		IItinerary Append(ILeg leg);
		
		/// <summary>
		/// Create a new itinerary by replacing a segment. 
		/// </summary>
		/// <param name="fromLeg">
		/// The first <see cref="ILeg"/> to replace.
		/// </param>
		/// <param name="toLeg">
		/// The last leg to replace <see cref="ILeg"/>.
		/// </param>
		/// <param name="legs">
		/// The new legs.
		/// </param>
		/// <returns>
		/// A <see cref="IItinerary"/>
		/// </returns>
		/// <exception cref="ArgumentNullException">Any argument is <value>null</value>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="fromLeg"/> or <paramref name="toLeg"/> do not belong to this itinerary.</exception>
		/// <exception cref="ArgumentException">The first and the last legs in <paramref name="legs"/> 
		/// do not match the previous legs requirements.</exception>
		IItinerary Replace(ILeg fromLeg, ILeg toLeg, IEnumerable<ILeg> legs);
	}
}

