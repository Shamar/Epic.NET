using System;
namespace Challenge00.DDDSample.Cargo
{
	/// <summary>
	/// Routing status.  
	/// </summary>
	[Serializable]
	public enum RoutingStatus
	{
		NOT_ROUTED,
		ROUTED,
		MISROUTED
	}
}

