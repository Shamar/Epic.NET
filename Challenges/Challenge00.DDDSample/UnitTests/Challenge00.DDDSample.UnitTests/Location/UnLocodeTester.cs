using NUnit.Framework;
using System;
using System.Text.RegularExpressions;
namespace Challenge00.DDDSample.UnitTests.Location
{
	[TestFixture()]
	public class UnLocodeTester : StringIdentifierTester<UnLocode>
	{
		#region implemented abstract members of Challenge00.DDDSample.UnitTests.StringIdentifierTester[UnLocode]
		
		protected override UnLocode CreateNewInstance (out string stringUsed)
		{
			stringUsed = "ITLBA";
			return new UnLocode(stringUsed);
		}
		
		
		protected override UnLocode CreateDifferentInstance (out string stringUsed)
		{
			stringUsed = "ITTRN";
			return new UnLocode(stringUsed);
		}
		
				
		protected override UnLocode CreateMatchingInstance (out Regex regEx)
		{
			regEx = new Regex("IT.*");
			return new UnLocode("ITLBA");
		}
		
		
		protected override UnLocode CreateUnMatchingInstance (out Regex regEx)
		{
			regEx = new Regex("US.*");
			return new UnLocode("ITTRN");
		}
		
		#endregion
	}
}

