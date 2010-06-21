using NUnit.Framework;
using System;
using Challenge00.DDDSample.Location;
using Rhino.Mocks;
using Challenge00.DDDSample.Voyage;
namespace Challenge00.DDDSample.UnitTests.Voyage
{
	[TestFixture()]
	public class VoyageEventArgsTester
	{
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Ctor_01 ()
		{
			// arrange:
			ILocation destination = MockRepository.GenerateStrictMock<ILocation>();
		
			// act:
			new VoyageEventArgs(null, destination);
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Ctor_02 ()
		{
			// arrange:
			ILocation previous = MockRepository.GenerateStrictMock<ILocation>();
		
			// act:
			new VoyageEventArgs(previous, null);
		}
		
		[Test]
		public void Test_Ctor_03()
		{
			// arrange:
			ILocation previous = MockRepository.GenerateStrictMock<ILocation>();
			ILocation destination = MockRepository.GenerateStrictMock<ILocation>();
			// act:
			VoyageEventArgs args = new VoyageEventArgs(previous, destination);
		
			// assert:
			Assert.AreSame(previous, args.PreviousLocation);
			Assert.AreSame(destination, args.DestinationLocation);
		}
	}
}

