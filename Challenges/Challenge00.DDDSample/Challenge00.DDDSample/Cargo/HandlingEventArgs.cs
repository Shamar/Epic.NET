using System;
using Challenge00.DDDSample.Location;
namespace Challenge00.DDDSample.Cargo
{
	/// <summary>
	/// Event related to cargo handling.
	/// </summary>
	[Serializable]
	public sealed class HandlingEventArgs : EventArgs
	{
		/// <summary>
		/// New delivery status of the cargo
		/// </summary>
		public readonly IDelivery Delivery;
		
		/// <summary>
		/// Completion date 
		/// </summary>
		public readonly DateTime CompletionDate;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="delivery">
		/// The new cargo <see cref="IDelivery"/> status. May not be null.
		/// </param>
		/// <param name="completionDate">
		/// The completion <see cref="DateTime"/>
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="delivery"/> is <value>null</value>.</exception>
		public HandlingEventArgs (IDelivery delivery, DateTime completionDate)
		{
			if (null == delivery)
				throw new ArgumentNullException ("delivery");
			Delivery = delivery;
			CompletionDate = completionDate;
		}
	}
}

