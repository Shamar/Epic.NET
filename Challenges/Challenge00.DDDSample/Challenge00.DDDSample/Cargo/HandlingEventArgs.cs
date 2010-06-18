using System;
using Challenge00.DDDSample.Location;
namespace Challenge00.DDDSample.Cargo
{
	[Serializable]
	public class HandlingEventArgs : EventArgs
	{
		public readonly ILocation Location;
		public readonly	DateTime RegistrationDate;
		public readonly DateTime CompletionDate;
		
		public HandlingEventArgs (ILocation location, DateTime registrationDate, DateTime completionDate)
		{
			if (null == location)
				throw new ArgumentNullException ("location");
			Location = location;
			RegistrationDate = registrationDate;
			CompletionDate = completionDate;
		}
	}
}

