using NUnit.Framework;
using System;
using System.Text.RegularExpressions;
using Challenge00.DDDSample.Cargo;
using System.Text;

namespace Challenge00.DDDSample.UnitTests.Cargo
{
	[TestFixture()]
	public class TrackingIdTester : StringIdentifierTester<TrackingId>
	{
		[Test()]
		public void Test_Constructor_01 ()
		{
			string idString = "test";
			TrackingId id = new TrackingId(idString);
		}
		
		#region implemented abstract members of Challenge00.DDDSample.UnitTests.StringIdentifierTester[TrackingId]
		
		protected override TrackingId CreateNewInstance (out string stringUsed)
		{
			stringUsed = new StringBuilder("tes").Append("t").ToString();
			return new TrackingId(stringUsed);
		}
		
		protected override TrackingId CreateDifferentInstance (out string stringUsed)
		{
			stringUsed = new StringBuilder("tes").Append("t2").ToString();
			return new TrackingId(stringUsed);
		}
		
		protected override TrackingId CreateMatchingInstance (out Regex regEx)
		{
			regEx = new Regex("t.*");
			return new TrackingId("test");
		}
		
		
		protected override TrackingId CreateUnMatchingInstance (out Regex regEx)
		{
			regEx = new Regex("z.*");
			return new TrackingId("test");
		}
		
		#endregion

	}
}