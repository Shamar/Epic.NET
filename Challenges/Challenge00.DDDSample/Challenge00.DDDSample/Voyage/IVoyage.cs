using System;
using Challenge00.DDDSample.Location;
using Challenge00.DDDSample.Cargo;
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

		/// <summary>
		/// Last known location. 
		/// </summary>
		ILocation LastKnownLocation { get; }
		
		/// <summary>
		/// True if the voyage is moving toward the next arrival. 
		/// </summary>
		bool IsMoving { get; }
		
		/// <summary>
		/// The voyage is lost.
		/// </summary>
		bool IsLost { get; }
		
		/// <summary>
		/// Move the last knwon location to the next scheduled arrival.
		/// </summary>
		void Arrive();
		
		event EventHandler<VoyageEventArgs> Arrived;
		
		/// <summary>
		/// Start a voyage movement.
		/// </summary>
		void Depart();
		
		event EventHandler<VoyageEventArgs> Departed;
		
		/// <summary>
		/// Mark the voyage as lost. 
		/// </summary>
		/// <param name="lastKnownLocation">
		/// The last known <see cref="ILocation"/>
		/// </param>
		void MarkAsLost(ILocation lastKnownLocation);
		
		event EventHandler<VoyageEventArgs> Losing;
	}
}

