using System;
namespace Challenge00.DDDSample.Location
{
	/// <summary>
	/// A location is our model is stops on a journey, such as cargo origin or destination, or carrier movement endpoints. It is uniquely identified by a UN Locode.  
	/// </summary>
	public interface ILocation : IEquatable<ILocation>
	{
		/// <summary>
		/// Actual name of this location, e.g. "Stockholm". 
		/// </summary>
		string Name { get; }
		
		/// <summary>
		/// UN Locode for this location.
		/// </summary>
		UnLocode UnLocode { get; }
	}
}

