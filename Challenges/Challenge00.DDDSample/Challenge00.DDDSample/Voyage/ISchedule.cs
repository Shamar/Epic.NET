using System;
using System.Collections.Generic;
namespace Challenge00.DDDSample.Voyage
{
	/// <summary>
	/// A voyage schedule.  
	/// </summary>
	/// <remarks>
	/// <para>
	/// Two schedule are equals if they contains the same <see cref="ICarrierMovement"/>.
	/// </para>
	/// <para>
	/// To override <see cref="object.GetHashCode()"/> use the exclusive OR between the 
	/// the fist and the last <see cref="ICarrierMovement"/>.
	/// </para>
	/// </remarks>
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
		
		/// <summary>
		/// Number of movements.
		/// </summary>
		int MovementsCount { get; }
		
		/// <summary>
		/// Return the <see cref="ICarrierMovement"/> at <paramref name="index"/>.
		/// </summary>
		/// <param name="index">
		/// The movement index in the schedule sequence. 
		/// </param>
		/// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is not valid.</exception>
		ICarrierMovement this[int index] { get; }
	}
}

