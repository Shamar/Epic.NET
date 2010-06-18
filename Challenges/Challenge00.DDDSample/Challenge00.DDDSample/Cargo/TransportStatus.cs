using System;
namespace Challenge00.DDDSample.Cargo
{
	/// <summary>
	/// Describes status of cargo transportation.
	/// </summary>
	public enum TransportStatus
	{
		/// <summary>
		/// Cargo hasn't been received yet.
		/// </summary>
		NotReceived,
		/// <summary>
		/// Cargo is onboard carrier.
		/// </summary>
		OnboardCarrier,
		/// <summary>
		/// Cargo is in port.
		/// </summary>
		InPort,
		/// <summary>
		/// Cargo has been claimed.
		/// </summary>
		Claimed,
		/// <summary>
		/// Cargo transport state is unknown.
		/// </summary>
      	Unknown
   }
}

