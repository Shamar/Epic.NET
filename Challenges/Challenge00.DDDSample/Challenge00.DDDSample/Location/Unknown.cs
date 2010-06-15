using System;
namespace Challenge00.DDDSample.Location
{
	/// <summary>
	/// Unknown location. 
	/// </summary>
	[Serializable]
	public sealed class Unknown : ILocation
	{
		private static Unknown _location = new Unknown();
		
		public static ILocation Location
		{
			get
			{
				return _location;
			}
		}
		
		private Unknown ()
		{
		}
		
		#region ILocation implementation
		public string Name {
			get {
				return "Unknown";
			}
		}
		
		
		public UnLocode UnLocode {
			get {
				return null;
			}
		}
		
		#endregion
		#region IEquatable<ILocation> implementation
		
		public bool Equals (ILocation other)
		{
			return false;
		}
		
		#endregion
	}
}

