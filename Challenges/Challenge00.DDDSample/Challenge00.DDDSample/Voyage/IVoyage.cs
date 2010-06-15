using System;
namespace Challenge00.DDDSample.Voyage
{
	/// <summary>
	/// A Voyage.  
	/// </summary>
	public interface IVoyage : IEquatable<IVoyage>
	{
		/// <summary>
		/// Voyage identification number. 
		/// </summary>
		VoyageNumber Number { get; }
		
		/// <summary>
		/// Schedule. 
		/// </summary>
		ISchedule Schedule { get; }
		
	}
}

