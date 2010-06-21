using System;
using Challenge00.DDDSample.Location;
namespace Challenge00.DDDSample.Voyage
{
	/// <summary>
	/// Event arguments for voyage executions.
	/// </summary>
	[Serializable]
	public class VoyageEventArgs : EventArgs
	{
		/// <summary>
		/// Previous location.
		/// </summary>
		public readonly ILocation PreviousLocation;

		/// <summary>
		/// Destination location.
		/// </summary>
		public readonly ILocation DestinationLocation;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="previousLocation">
		/// A <see cref="ILocation"/>
		/// </param>
		/// <param name="destinationLocation">
		/// A <see cref="ILocation"/>
		/// </param>
		/// <exception cref="ArgumentNullException">Any argument is <value>null</value>.</exception>
		public VoyageEventArgs (ILocation previousLocation, ILocation destinationLocation)
		{
			if (null == previousLocation)
				throw new ArgumentNullException ("previousLocation");
			if (null == destinationLocation)
				throw new ArgumentNullException ("destinationLocation");
			PreviousLocation = previousLocation;
			DestinationLocation = destinationLocation;
		}
	}
}

