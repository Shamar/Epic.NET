using System;
using Challenge00.DDDSample.Location;
namespace Challenge00.DDDSample.Voyage
{
	[Serializable]
	public class VoyageEventArgs : EventArgs
	{
		public readonly ILocation PreviousLocation;

		public readonly ILocation DestinationLocation;

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

