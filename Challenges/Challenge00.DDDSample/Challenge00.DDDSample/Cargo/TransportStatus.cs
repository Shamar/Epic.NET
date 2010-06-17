using System;
namespace Challenge00.DDDSample.Cargo
{
	/// <summary>
	/// Represents the different transport statuses for a cargo.
	/// </summary>
	[Serializable]
	public enum TransportStatus
	{
		CLAIMED,
		IN_PORT,
		NOT_RECEIVED,
		ONBOARD_CARRIER,
		UNKNOWN
	}
}

