using NUnit.Framework;
using System;
using Rhino.Mocks;
using Challenge00.DDDSample.Location;
using Challenge00.DDDSample.Voyage;
using Challenge00.DDDSample.Default.Voyage;
namespace Challenge00.DDDSample.Default.UnitTests.Voyage
{
	[TestFixture()]
	public class CarrierMovementTester
	{
		[Test]
		public void Test_Ctor_01()
		{
			// arrange:
			ILocation dpLocation = MockRepository.GenerateStrictMock<ILocation>();
			DateTime dpTime = DateTime.Now - new TimeSpan(48,0,0);
			ILocation arLocation = MockRepository.GenerateStrictMock<ILocation>();
			DateTime arTime = DateTime.Now + new TimeSpan(48,0,0);
			dpLocation.Expect(l => l.Equals(arLocation)).Return(false).Repeat.Once();
		
			// act:
			ICarrierMovement target = new CarrierMovement(dpLocation, dpTime, arLocation, arTime);
		
			// assert:
			Assert.AreEqual(dpTime, target.DepartureTime);
			Assert.AreEqual(arTime, target.ArrivalTime);
			Assert.AreSame(dpLocation, target.DepartureLocation);
			Assert.AreSame(arLocation, target.ArrivalLocation);
			dpLocation.VerifyAllExpectations();
			arLocation.VerifyAllExpectations();
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Ctor_02 ()
		{
			// arrange:
			ILocation dpLocation = MockRepository.GenerateStrictMock<ILocation>();
			DateTime dpTime = DateTime.Now - new TimeSpan(48,0,0);
			DateTime arTime = DateTime.Now + new TimeSpan(48,0,0);
			
			// act:
			new CarrierMovement(dpLocation, dpTime, null, arTime);
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Ctor_03 ()
		{
			// arrange:
			ILocation arLocation = MockRepository.GenerateStrictMock<ILocation>();
			DateTime dpTime = DateTime.Now - new TimeSpan(48,0,0);
			DateTime arTime = DateTime.Now + new TimeSpan(48,0,0);
			
			// act:
			new CarrierMovement(null, dpTime, arLocation, arTime);
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void Test_Ctor_04 ()
		{
			// arrange:
			ILocation dpLocation = MockRepository.GenerateStrictMock<ILocation>();
			DateTime dpTime = DateTime.Now - new TimeSpan(48,0,0);
			ILocation arLocation = MockRepository.GenerateStrictMock<ILocation>();
			DateTime arTime = dpTime;
			dpLocation.Expect(l => l.Equals(arLocation)).Return(false).Repeat.Once();
			
			// act:
			new CarrierMovement(dpLocation, dpTime, arLocation, arTime);
			dpLocation.VerifyAllExpectations();
			arLocation.VerifyAllExpectations();
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void Test_Ctor_05 ()
		{
			// arrange:
			ILocation dpLocation = MockRepository.GenerateStrictMock<ILocation>();
			DateTime dpTime = DateTime.Now - new TimeSpan(48,0,0);
			ILocation arLocation = MockRepository.GenerateStrictMock<ILocation>();
			DateTime arTime = dpTime - new TimeSpan(48,0,0);
			dpLocation.Expect(l => l.Equals(arLocation)).Return(false).Repeat.Once();
			
			// act:
			new CarrierMovement(dpLocation, dpTime, arLocation, arTime);
			dpLocation.VerifyAllExpectations();
			arLocation.VerifyAllExpectations();
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_Ctor_06 ()
		{
			// arrange:
			ILocation arLocation = MockRepository.GenerateStrictMock<ILocation>();
			DateTime dpTime = DateTime.Now - new TimeSpan(48,0,0);
			DateTime arTime = DateTime.Now + new TimeSpan(48,0,0);
			arLocation.Expect(l => l.Equals(arLocation)).Return(true).Repeat.Once();
			
			// act:
			new CarrierMovement(arLocation, dpTime, arLocation, arTime);
		}
		
		[Test]
		public void Test_Equals_01()
		{
			// arrange:
			ILocation targetDpLocation = MockRepository.GenerateStrictMock<ILocation>();
			ILocation targetArLocation = MockRepository.GenerateStrictMock<ILocation>();
			targetDpLocation.Expect(l => l.Equals(targetArLocation)).Return(false).Repeat.Once();
			DateTime dpTime = DateTime.Now - new TimeSpan(48,0,0);
			DateTime arTime = DateTime.Now + new TimeSpan(48,0,0);
			
			ILocation dpLocation = MockRepository.GenerateStrictMock<ILocation>();
			ILocation arLocation = MockRepository.GenerateStrictMock<ILocation>();
			targetDpLocation.Expect(l => l.Equals(dpLocation)).Return(true).Repeat.Once();
			targetArLocation.Expect(l => l.Equals(arLocation)).Return(true).Repeat.Once();
			
			ICarrierMovement mock = MockRepository.GenerateStrictMock<ICarrierMovement>();
			mock.Expect(m => m.DepartureLocation).Return(dpLocation).Repeat.Once();
			mock.Expect(m => m.ArrivalLocation).Return(arLocation).Repeat.Once();
			mock.Expect(m => m.ArrivalTime).Return(arTime).Repeat.Once();
			mock.Expect(m => m.DepartureTime).Return(dpTime).Repeat.Once();
			
		
			// act:
			ICarrierMovement target = new CarrierMovement(targetDpLocation, dpTime, targetArLocation, arTime);
			bool areEquals = target.Equals(mock);
		
			// assert:
			Assert.IsTrue(areEquals);
			dpLocation.VerifyAllExpectations();
			arLocation.VerifyAllExpectations();
			targetDpLocation.VerifyAllExpectations();
			targetArLocation.VerifyAllExpectations();
			mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Equals_02()
		{
			// arrange:
			ILocation targetDpLocation = MockRepository.GenerateStrictMock<ILocation>();
			ILocation targetArLocation = MockRepository.GenerateStrictMock<ILocation>();
			targetDpLocation.Expect(l => l.Equals(targetArLocation)).Return(false).Repeat.Once();
			DateTime dpTime = DateTime.Now - new TimeSpan(48,0,0);
			DateTime arTime = DateTime.Now + new TimeSpan(48,0,0);
			
			ILocation dpLocation = MockRepository.GenerateStrictMock<ILocation>();
			ILocation arLocation = MockRepository.GenerateStrictMock<ILocation>();
			targetDpLocation.Expect(l => l.Equals(dpLocation)).Return(true).Repeat.Once();
			targetArLocation.Expect(l => l.Equals(arLocation)).Return(false).Repeat.Once();
			
			ICarrierMovement mock = MockRepository.GenerateStrictMock<ICarrierMovement>();
			mock.Expect(m => m.DepartureLocation).Return(dpLocation).Repeat.Once();
			mock.Expect(m => m.ArrivalLocation).Return(arLocation).Repeat.Once();
		
			// act:
			ICarrierMovement target = new CarrierMovement(targetDpLocation, dpTime, targetArLocation, arTime);
			bool areEquals = target.Equals(mock);
		
			// assert:
			Assert.IsFalse(areEquals);
			dpLocation.VerifyAllExpectations();
			arLocation.VerifyAllExpectations();
			targetDpLocation.VerifyAllExpectations();
			targetArLocation.VerifyAllExpectations();
			mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Equals_03()
		{
			// arrange:
			ILocation targetDpLocation = MockRepository.GenerateStrictMock<ILocation>();
			ILocation targetArLocation = MockRepository.GenerateStrictMock<ILocation>();
			targetDpLocation.Expect(l => l.Equals(targetArLocation)).Return(false).Repeat.Once();
			DateTime dpTime = DateTime.Now - new TimeSpan(48,0,0);
			DateTime arTime = DateTime.Now + new TimeSpan(48,0,0);
			
			ILocation dpLocation = MockRepository.GenerateStrictMock<ILocation>();
			ILocation arLocation = MockRepository.GenerateStrictMock<ILocation>();
			targetDpLocation.Expect(l => l.Equals(dpLocation)).Return(false).Repeat.Once();
			
			ICarrierMovement mock = MockRepository.GenerateStrictMock<ICarrierMovement>();
			mock.Expect(m => m.DepartureLocation).Return(dpLocation).Repeat.Once();
		
			// act:
			ICarrierMovement target = new CarrierMovement(targetDpLocation, dpTime, targetArLocation, arTime);
			bool areEquals = target.Equals(mock);
		
			// assert:
			Assert.IsFalse(areEquals);
			dpLocation.VerifyAllExpectations();
			arLocation.VerifyAllExpectations();
			targetDpLocation.VerifyAllExpectations();
			targetArLocation.VerifyAllExpectations();
			mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Equals_04()
		{
			// arrange:
			ILocation targetDpLocation = MockRepository.GenerateStrictMock<ILocation>();
			ILocation targetArLocation = MockRepository.GenerateStrictMock<ILocation>();
			targetDpLocation.Expect(l => l.Equals(targetArLocation)).Return(false).Repeat.Once();
			DateTime dpTime = DateTime.Now - new TimeSpan(48,0,0);
			DateTime arTime = DateTime.Now + new TimeSpan(48,0,0);
			
			ILocation dpLocation = MockRepository.GenerateStrictMock<ILocation>();
			ILocation arLocation = MockRepository.GenerateStrictMock<ILocation>();
			targetDpLocation.Expect(l => l.Equals(dpLocation)).Return(true).Repeat.Once();
			targetArLocation.Expect(l => l.Equals(arLocation)).Return(true).Repeat.Once();
			
			ICarrierMovement mock = MockRepository.GenerateStrictMock<ICarrierMovement>();
			mock.Expect(m => m.DepartureLocation).Return(dpLocation).Repeat.Once();
			mock.Expect(m => m.ArrivalLocation).Return(arLocation).Repeat.Once();
			mock.Expect(m => m.ArrivalTime).Return(arTime).Repeat.Once();
			mock.Expect(m => m.DepartureTime).Return(dpTime).Repeat.Once();
			
		
			// act:
			ICarrierMovement target = new CarrierMovement(targetDpLocation, dpTime - new TimeSpan(48,0,0), targetArLocation, arTime);
			bool areEquals = target.Equals(mock);
		
			// assert:
			Assert.IsFalse(areEquals);
			dpLocation.VerifyAllExpectations();
			arLocation.VerifyAllExpectations();
			targetDpLocation.VerifyAllExpectations();
			targetArLocation.VerifyAllExpectations();
			mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Equals_05()
		{
			// arrange:
			ILocation targetDpLocation = MockRepository.GenerateStrictMock<ILocation>();
			ILocation targetArLocation = MockRepository.GenerateStrictMock<ILocation>();
			targetDpLocation.Expect(l => l.Equals(targetArLocation)).Return(false).Repeat.Once();
			DateTime dpTime = DateTime.Now - new TimeSpan(48,0,0);
			DateTime arTime = DateTime.Now + new TimeSpan(48,0,0);
			
			ILocation dpLocation = MockRepository.GenerateStrictMock<ILocation>();
			ILocation arLocation = MockRepository.GenerateStrictMock<ILocation>();
			targetDpLocation.Expect(l => l.Equals(dpLocation)).Return(true).Repeat.Once();
			targetArLocation.Expect(l => l.Equals(arLocation)).Return(true).Repeat.Once();
			
			ICarrierMovement mock = MockRepository.GenerateStrictMock<ICarrierMovement>();
			mock.Expect(m => m.DepartureLocation).Return(dpLocation).Repeat.Once();
			mock.Expect(m => m.ArrivalLocation).Return(arLocation).Repeat.Once();
			mock.Expect(m => m.ArrivalTime).Return(arTime).Repeat.Once();
			
		
			// act:
			ICarrierMovement target = new CarrierMovement(targetDpLocation, dpTime, targetArLocation, arTime + new TimeSpan(48,0,0));
			bool areEquals = target.Equals(mock);
		
			// assert:
			Assert.IsFalse(areEquals);
			dpLocation.VerifyAllExpectations();
			arLocation.VerifyAllExpectations();
			targetDpLocation.VerifyAllExpectations();
			targetArLocation.VerifyAllExpectations();
			mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Equals_06()
		{
			// arrange:
			ILocation targetDpLocation = MockRepository.GenerateStrictMock<ILocation>();
			ILocation targetArLocation = MockRepository.GenerateStrictMock<ILocation>();
			targetDpLocation.Expect(l => l.Equals(targetArLocation)).Return(false).Repeat.Once();
			DateTime dpTime = DateTime.Now - new TimeSpan(48,0,0);
			DateTime arTime = DateTime.Now + new TimeSpan(48,0,0);
			
			ILocation dpLocation = MockRepository.GenerateStrictMock<ILocation>();
			ILocation arLocation = MockRepository.GenerateStrictMock<ILocation>();
			targetDpLocation.Expect(l => l.Equals(dpLocation)).Return(true).Repeat.Once();
			targetArLocation.Expect(l => l.Equals(arLocation)).Return(true).Repeat.Once();
			
			ICarrierMovement mock = MockRepository.GenerateStrictMock<ICarrierMovement>();
			mock.Expect(m => m.DepartureLocation).Return(dpLocation).Repeat.Once();
			mock.Expect(m => m.ArrivalLocation).Return(arLocation).Repeat.Once();
			mock.Expect(m => m.ArrivalTime).Return(arTime).Repeat.Once();
			mock.Expect(m => m.DepartureTime).Return(dpTime).Repeat.Once();
			
		
			// act:
			ICarrierMovement target = new CarrierMovement(targetDpLocation, dpTime, targetArLocation, arTime);
			bool areEquals = target.Equals((object)mock);
		
			// assert:
			Assert.IsTrue(areEquals);
			dpLocation.VerifyAllExpectations();
			arLocation.VerifyAllExpectations();
			targetDpLocation.VerifyAllExpectations();
			targetArLocation.VerifyAllExpectations();
			mock.VerifyAllExpectations();
		}
	}
}

