using System;
using System.Collections.Generic;
namespace Challenge00.DDDSample.Voyage
{
	/// <summary>
	/// A voyage schedule.  
	/// </summary>
	public interface ISchedule : IEquatable<ISchedule>, IEnumerable<ICarrierMovement>
	{
		/// <summary>
		/// Create a new schedule by appending a carrier movement. 
		/// </summary>
		/// <param name="movement">
		/// A <see cref="ICarrierMovement"/> to append
		/// </param>
		/// <returns>
		/// A new <see cref="ISchedule"/>
		/// </returns>
		ISchedule Append(ICarrierMovement movement);
	}
}

