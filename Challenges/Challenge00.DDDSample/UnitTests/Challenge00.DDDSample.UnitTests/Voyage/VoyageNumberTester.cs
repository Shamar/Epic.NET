using System;
using NUnit.Framework;
using System.Text.RegularExpressions;
using Challenge00.DDDSample.Voyage;
namespace Challenge00.DDDSample.UnitTests
{
	[TestFixture]
	public class VoyageNumberTester : StringIdentifierTester<VoyageNumber>
	{
		#region implemented abstract members of Challenge00.DDDSample.UnitTests.StringIdentifierTester[UnLocode]
		
		protected override VoyageNumber CreateNewInstance (out string stringUsed)
		{
			stringUsed = "VG01";
			return new VoyageNumber(stringUsed);
		}
		
		
		protected override VoyageNumber CreateDifferentInstance (out string stringUsed)
		{
			stringUsed = "VG02";
			return new VoyageNumber(stringUsed);
		}
		
				
		protected override VoyageNumber CreateMatchingInstance (out Regex regEx)
		{
			regEx = new Regex("VG*");
			return new VoyageNumber("VG123123");
		}
		
		
		protected override VoyageNumber CreateUnMatchingInstance (out Regex regEx)
		{
			regEx = new Regex("XX*");
			return new VoyageNumber("VG123123");
		}
		
		#endregion
	}
}

