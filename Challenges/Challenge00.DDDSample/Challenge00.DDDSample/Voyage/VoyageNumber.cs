using System;
namespace Challenge00.DDDSample.Voyage
{
	/// <summary>
	/// Identifies a voyage.  
	/// </summary>
	public class VoyageNumber : StringIdentifier<VoyageNumber>
	{
		public VoyageNumber (string identifier)
			: base(identifier)
		{
		}
	}
}

