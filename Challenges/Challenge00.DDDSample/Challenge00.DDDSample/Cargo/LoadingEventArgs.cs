using System;
using Challenge00.DDDSample.Location;
using Challenge00.DDDSample.Voyage;
namespace Challenge00.DDDSample.Cargo
{
	[Serializable]
	public class LoadingEventArgs : HandlingEventArgs
	{
		public readonly IVoyage Voyage;
		public LoadingEventArgs (IVoyage voyage, IDelivery delivery, DateTime completionDate)
			: base(delivery, completionDate)
		{
			if(null == voyage)
				throw new ArgumentNullException("voyage");
			Voyage = voyage;
		}
	}
}

