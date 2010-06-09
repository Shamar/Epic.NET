using System;
using System.Text.RegularExpressions;

namespace Challenge00.DDDSample.Cargo
{
	/// <summary>
	/// Uniquely identifies a particular cargo.
	/// </summary>
    [Serializable]
    public class TrackingId : StringIdentifier<TrackingId>
    {
        public TrackingId(string identifier)
			: base(identifier)
        {
        }
    }
}

