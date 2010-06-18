using System;
using Challenge00.DDDSample.Location;
namespace Challenge00.DDDSample.Cargo
{
	[Serializable]
	public class HandlingEventArgs : EventArgs
	{
		public readonly IDelivery Delivery;
		public readonly DateTime CompletionDate;
		
		public HandlingEventArgs (IDelivery delivery, DateTime completionDate)
		{
			if (null == delivery)
				throw new ArgumentNullException ("delivery");
			Delivery = delivery;
			CompletionDate = completionDate;
		}
	}
}

