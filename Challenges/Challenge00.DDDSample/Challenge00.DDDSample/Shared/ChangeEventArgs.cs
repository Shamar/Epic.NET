using System;
namespace Challenge00.DDDSample
{
	/// <summary>
	/// Generic event argument that hold a status change.
	/// </summary>
	[Serializable]
	public sealed class ChangeEventArgs<TChanged> : EventArgs
	{
		/// <summary>
		/// Old <typeparamref cref="TChanged"/> value.
		/// </summary>
		public readonly TChanged OldValue;
		
		/// <summary>
		/// New <typeparamref cref="TChanged"/> value.
		/// </summary>
		public readonly TChanged NewValue;

		/// <summary>
		/// Constructor 
		/// </summary>
		/// <param name="oldValue">
		/// The old <see cref="TChanged"/>
		/// </param>
		/// <param name="newValue">
		/// The new <see cref="TChanged"/>
		/// </param>
		public ChangeEventArgs (TChanged oldValue, TChanged newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}

