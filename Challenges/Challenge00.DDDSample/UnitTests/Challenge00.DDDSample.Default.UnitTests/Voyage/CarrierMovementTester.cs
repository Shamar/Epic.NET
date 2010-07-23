//  
//  CarrierMovementTester.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010 Giacomo Tesio
// 
//  This file is part of Epic.NET.
// 
//  Epic.NET is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Affero General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Epic.NET is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Affero General Public License for more details.
// 
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  
using NUnit.Framework;
using System;
using Rhino.Mocks;
using Challenge00.DDDSample.Location;
using Challenge00.DDDSample.Voyage;
namespace DefaultImplementation.Voyage
{
	[TestFixture()]
	public class CarrierMovementTester
	{
		[Test]
		public void Test_Ctor_01()
		{
			// arrange:
			UnLocode dpLocode = new UnLocode("DPLOC");
			ILocation dpLocation = MockRepository.GenerateStrictMock<ILocation>();
			dpLocation.Expect(l => l.UnLocode).Return(dpLocode).Repeat.AtLeastOnce();
			DateTime dpTime = DateTime.UtcNow - new TimeSpan(48,0,0);
			UnLocode arLocode = new UnLocode("ARLOC");
			ILocation arLocation = MockRepository.GenerateStrictMock<ILocation>();
			arLocation.Expect(l => l.UnLocode).Return(arLocode).Repeat.AtLeastOnce();
			DateTime arTime = DateTime.UtcNow + new TimeSpan(48,0,0);
		
			// act:
			ICarrierMovement target = new CarrierMovement(dpLocation, dpTime, arLocation, arTime);
		
			// assert:
			Assert.AreEqual(dpTime, target.DepartureTime);
			Assert.AreEqual(arTime, target.ArrivalTime);
			Assert.AreSame(dpLocode, target.DepartureLocation);
			Assert.AreSame(arLocode, target.ArrivalLocation);
			dpLocation.VerifyAllExpectations();
			arLocation.VerifyAllExpectations();
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Ctor_02 ()
		{
			// arrange:
			ILocation dpLocation = MockRepository.GenerateStrictMock<ILocation>();
			DateTime dpTime = DateTime.UtcNow - new TimeSpan(48,0,0);
			DateTime arTime = DateTime.UtcNow + new TimeSpan(48,0,0);
			
			// act:
			new CarrierMovement(dpLocation, dpTime, null, arTime);
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Ctor_03 ()
		{
			// arrange:
			ILocation arLocation = MockRepository.GenerateStrictMock<ILocation>();
			DateTime dpTime = DateTime.UtcNow - new TimeSpan(48,0,0);
			DateTime arTime = DateTime.UtcNow + new TimeSpan(48,0,0);
			
			// act:
			new CarrierMovement(null, dpTime, arLocation, arTime);
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void Test_Ctor_04 ()
		{
			// arrange:
			UnLocode dpLocode = new UnLocode("DPLOC");
			UnLocode arLocode = new UnLocode("ARLOC");
			ILocation dpLocation = MockRepository.GenerateStrictMock<ILocation>();
			dpLocation.Expect(l => l.UnLocode).Return(dpLocode).Repeat.AtLeastOnce();
			DateTime dpTime = DateTime.UtcNow - new TimeSpan(48,0,0);
			ILocation arLocation = MockRepository.GenerateStrictMock<ILocation>();
			arLocation.Expect(l => l.UnLocode).Return(arLocode).Repeat.AtLeastOnce();
			DateTime arTime = dpTime;
			
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
			UnLocode dpLocode = new UnLocode("DPLOC");
			UnLocode arLocode = new UnLocode("ARLOC");
			ILocation dpLocation = MockRepository.GenerateStrictMock<ILocation>();
			dpLocation.Expect(l => l.UnLocode).Return(dpLocode).Repeat.AtLeastOnce();
			DateTime dpTime = DateTime.UtcNow - new TimeSpan(48,0,0);
			ILocation arLocation = MockRepository.GenerateStrictMock<ILocation>();
			arLocation.Expect(l => l.UnLocode).Return(arLocode).Repeat.AtLeastOnce();
			DateTime arTime = dpTime - new TimeSpan(48,0,0);
			
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
			UnLocode arLocode = new UnLocode("ARLOC");
			ILocation arLocation = MockRepository.GenerateStrictMock<ILocation>();
			DateTime dpTime = DateTime.UtcNow - new TimeSpan(48,0,0);
			DateTime arTime = DateTime.UtcNow + new TimeSpan(48,0,0);
			arLocation.Expect(l => l.UnLocode).Return(arLocode).Repeat.AtLeastOnce();
			
			// act:
			new CarrierMovement(arLocation, dpTime, arLocation, arTime);
		}
		
		[Test]
		public void Test_Equals_01()
		{
			// arrange:
			UnLocode dpLocode = new UnLocode("DPLOC");
			UnLocode arLocode = new UnLocode("ARLOC");
			ILocation targetDpLocation = MockRepository.GenerateStrictMock<ILocation>();
			ILocation targetArLocation = MockRepository.GenerateStrictMock<ILocation>();
			targetDpLocation.Expect(l => l.UnLocode).Return(dpLocode).Repeat.AtLeastOnce();
			targetArLocation.Expect(l => l.UnLocode).Return(arLocode).Repeat.AtLeastOnce();
			DateTime dpTime = DateTime.UtcNow - new TimeSpan(48,0,0);
			DateTime arTime = DateTime.UtcNow + new TimeSpan(48,0,0);
			
			ICarrierMovement mock = MockRepository.GenerateStrictMock<ICarrierMovement>();
			mock.Expect(m => m.DepartureLocation).Return(dpLocode).Repeat.AtLeastOnce();
			mock.Expect(m => m.ArrivalLocation).Return(arLocode).Repeat.AtLeastOnce();
			mock.Expect(m => m.ArrivalTime).Return(arTime).Repeat.Once();
			mock.Expect(m => m.DepartureTime).Return(dpTime).Repeat.Once();
			
		
			// act:
			ICarrierMovement target = new CarrierMovement(targetDpLocation, dpTime, targetArLocation, arTime);
			bool areEquals = target.Equals(mock);
		
			// assert:
			Assert.IsTrue(areEquals);
			targetDpLocation.VerifyAllExpectations();
			targetArLocation.VerifyAllExpectations();
			mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Equals_02()
		{
			// arrange:
			UnLocode dpLocode = new UnLocode("DPLOC");
			UnLocode arLocode = new UnLocode("ARLOC");
			ILocation targetDpLocation = MockRepository.GenerateStrictMock<ILocation>();
			ILocation targetArLocation = MockRepository.GenerateStrictMock<ILocation>();
			targetDpLocation.Expect(l => l.UnLocode).Return(dpLocode).Repeat.AtLeastOnce();
			targetArLocation.Expect(l => l.UnLocode).Return(arLocode).Repeat.AtLeastOnce();
			DateTime dpTime = DateTime.UtcNow - new TimeSpan(48,0,0);
			DateTime arTime = DateTime.UtcNow + new TimeSpan(48,0,0);
			
			ICarrierMovement mock = MockRepository.GenerateStrictMock<ICarrierMovement>();
			mock.Expect(m => m.DepartureLocation).Return(dpLocode).Repeat.Once();
			mock.Expect(m => m.ArrivalLocation).Return(new UnLocode("ARNEW")).Repeat.Once();
		
			// act:
			ICarrierMovement target = new CarrierMovement(targetDpLocation, dpTime, targetArLocation, arTime);
			bool areEquals = target.Equals(mock);
		
			// assert:
			Assert.IsFalse(areEquals);
			targetDpLocation.VerifyAllExpectations();
			targetArLocation.VerifyAllExpectations();
			mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Equals_03()
		{
			// arrange:
			UnLocode dpLocode = new UnLocode("DPLOC");
			UnLocode arLocode = new UnLocode("ARLOC");
			ILocation targetDpLocation = MockRepository.GenerateStrictMock<ILocation>();
			ILocation targetArLocation = MockRepository.GenerateStrictMock<ILocation>();
			targetDpLocation.Expect(l => l.UnLocode).Return(dpLocode).Repeat.AtLeastOnce();
			targetArLocation.Expect(l => l.UnLocode).Return(arLocode).Repeat.AtLeastOnce();
			DateTime dpTime = DateTime.UtcNow - new TimeSpan(48,0,0);
			DateTime arTime = DateTime.UtcNow + new TimeSpan(48,0,0);
			
			ICarrierMovement mock = MockRepository.GenerateStrictMock<ICarrierMovement>();
			mock.Expect(m => m.DepartureLocation).Return(new UnLocode("DPNEW")).Repeat.Once();
		
			// act:
			ICarrierMovement target = new CarrierMovement(targetDpLocation, dpTime, targetArLocation, arTime);
			bool areEquals = target.Equals(mock);
		
			// assert:
			Assert.IsFalse(areEquals);
			targetDpLocation.VerifyAllExpectations();
			targetArLocation.VerifyAllExpectations();
			mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Equals_04()
		{
			// arrange:
			UnLocode dpLocode = new UnLocode("DPLOC");
			UnLocode arLocode = new UnLocode("ARLOC");
			ILocation targetDpLocation = MockRepository.GenerateStrictMock<ILocation>();
			ILocation targetArLocation = MockRepository.GenerateStrictMock<ILocation>();
			targetDpLocation.Expect(l => l.UnLocode).Return(dpLocode).Repeat.AtLeastOnce();
			targetArLocation.Expect(l => l.UnLocode).Return(arLocode).Repeat.AtLeastOnce();
			DateTime dpTime = DateTime.UtcNow - new TimeSpan(48,0,0);
			DateTime arTime = DateTime.UtcNow + new TimeSpan(48,0,0);
			
			ICarrierMovement mock = MockRepository.GenerateStrictMock<ICarrierMovement>();
			mock.Expect(m => m.DepartureLocation).Return(dpLocode).Repeat.Once();
			mock.Expect(m => m.ArrivalLocation).Return(arLocode).Repeat.Once();
			mock.Expect(m => m.ArrivalTime).Return(arTime).Repeat.Once();
			mock.Expect(m => m.DepartureTime).Return(dpTime).Repeat.Once();
			
		
			// act:
			ICarrierMovement target = new CarrierMovement(targetDpLocation, dpTime - new TimeSpan(48,0,0), targetArLocation, arTime);
			bool areEquals = target.Equals(mock);
		
			// assert:
			Assert.IsFalse(areEquals);
			targetDpLocation.VerifyAllExpectations();
			targetArLocation.VerifyAllExpectations();
			mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Equals_05()
		{
			// arrange:
			UnLocode dpLocode = new UnLocode("DPLOC");
			UnLocode arLocode = new UnLocode("ARLOC");
			ILocation targetDpLocation = MockRepository.GenerateStrictMock<ILocation>();
			ILocation targetArLocation = MockRepository.GenerateStrictMock<ILocation>();
			targetDpLocation.Expect(l => l.UnLocode).Return(dpLocode).Repeat.AtLeastOnce();
			targetArLocation.Expect(l => l.UnLocode).Return(arLocode).Repeat.AtLeastOnce();
			DateTime dpTime = DateTime.UtcNow - new TimeSpan(48,0,0);
			DateTime arTime = DateTime.UtcNow + new TimeSpan(48,0,0);
			
			ICarrierMovement mock = MockRepository.GenerateStrictMock<ICarrierMovement>();
			mock.Expect(m => m.DepartureLocation).Return(dpLocode).Repeat.Once();
			mock.Expect(m => m.ArrivalLocation).Return(arLocode).Repeat.Once();
			mock.Expect(m => m.ArrivalTime).Return(arTime).Repeat.Once();
			
		
			// act:
			ICarrierMovement target = new CarrierMovement(targetDpLocation, dpTime, targetArLocation, arTime + new TimeSpan(48,0,0));
			bool areEquals = target.Equals(mock);
		
			// assert:
			Assert.IsFalse(areEquals);
			targetDpLocation.VerifyAllExpectations();
			targetArLocation.VerifyAllExpectations();
			mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Equals_06()
		{
			// arrange:
			UnLocode dpLocode = new UnLocode("DPLOC");
			UnLocode arLocode = new UnLocode("ARLOC");
			ILocation targetDpLocation = MockRepository.GenerateStrictMock<ILocation>();
			ILocation targetArLocation = MockRepository.GenerateStrictMock<ILocation>();
			targetDpLocation.Expect(l => l.UnLocode).Return(dpLocode).Repeat.AtLeastOnce();
			targetArLocation.Expect(l => l.UnLocode).Return(arLocode).Repeat.AtLeastOnce();
			DateTime dpTime = DateTime.UtcNow - new TimeSpan(48,0,0);
			DateTime arTime = DateTime.UtcNow + new TimeSpan(48,0,0);
			
			ICarrierMovement mock = MockRepository.GenerateStrictMock<ICarrierMovement>();
			mock.Expect(m => m.DepartureLocation).Return(dpLocode).Repeat.Once();
			mock.Expect(m => m.ArrivalLocation).Return(arLocode).Repeat.Once();
			mock.Expect(m => m.ArrivalTime).Return(arTime).Repeat.Once();
			mock.Expect(m => m.DepartureTime).Return(dpTime).Repeat.Once();
			
		
			// act:
			ICarrierMovement target = new CarrierMovement(targetDpLocation, dpTime, targetArLocation, arTime);
			bool areEquals = target.Equals((object)mock);
		
			// assert:
			Assert.IsTrue(areEquals);
			targetDpLocation.VerifyAllExpectations();
			targetArLocation.VerifyAllExpectations();
			mock.VerifyAllExpectations();
		}
	}
}

