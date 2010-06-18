using System;
namespace Challenge00.DDDSample.Cargo
{
	/// <summary>
	/// Routing status.  
	/// </summary>
	[Serializable]
	public enum RoutingStatus
	{
		/// <summary>
		/// Cargo hasn't been routed yet.
		/// </summary>
		NotRouted,
		/// <summary>
		/// Cargo is misrouted.
		/// </summary>
		Misrouted,
		/// <summary>
		/// Cargo is on its route.
		/// </summary>
	    Routed
	}
}

