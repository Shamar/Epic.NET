using System;
namespace Challenge00.DDDSample
{
	[Serializable]
	public sealed class ChangeEventArgs<TChanged> : EventArgs
	{
		public readonly TChanged OldValue;
		public readonly TChanged NewValue;

		public ChangeEventArgs (TChanged oldValue, TChanged newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}

