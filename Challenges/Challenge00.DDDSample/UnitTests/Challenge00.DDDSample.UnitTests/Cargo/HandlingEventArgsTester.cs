using System;
using NUnit.Framework;
using Challenge00.DDDSample.Cargo;
using Rhino.Mocks;
namespace Challenge00.DDDSample.UnitTests
{
	[TestFixture]
	public class HandlingEventArgsTester
	{
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Ctor_01 ()
		{
			// act:
			new HandlingEventArgs(null, DateTime.Now);
		}
		
		[Test]
		public void Test_Ctor_02()
		{
			// arrange:
			IDelivery mDelivery = MockRepository.GenerateStrictMock<IDelivery>();
			DateTime cTime = DateTime.Now;
		
			// act:
			HandlingEventArgs args = new HandlingEventArgs(mDelivery, cTime);
		
			// assert:
			Assert.AreEqual(cTime, args.CompletionDate);
			Assert.AreSame(mDelivery, args.Delivery);
		}
	}
}

