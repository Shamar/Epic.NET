using System;
namespace Challenge00.DDDSample
{
	/// <summary>
	/// A HandlingEvent is used to register the event when, for instance, a cargo is unloaded from a carrier at a some loacation at a given time.  
	/// </summary>
	public interface IHandlingEvent : IEquatable<IHandlingEvent>
	{
	}
}

