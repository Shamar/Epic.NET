//  
//  VoyageTester.cs
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
using System;
using NUnit.Framework;
using Challenge00.DDDSample.Voyage;
using Rhino.Mocks;
using Challenge00.DDDSample.Location;
namespace DefaultImplementation.Voyage
{
	[TestFixture]
	public class VoyageTester
	{
		[Test]
		public void Test_Ctor_01 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			UnLocode departure = new UnLocode("DPLOC");
			UnLocode arrival = new UnLocode("ARLOC");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			ICarrierMovement movement = MockRepository.GenerateStrictMock<ICarrierMovement>();
			movement.Expect(m => m.DepartureLocation).Return(departure).Repeat.Once();
			movement.Expect(m => m.ArrivalLocation).Return(arrival).Repeat.Once();
			schedule.Expect(s => s[0]).Return(movement).Repeat.AtLeastOnce();
			
			// act:
			IVoyage voyage = new Challenge00.DDDSample.Voyage.Voyage(number, schedule);
			
			// assert:
			Assert.AreEqual(number, voyage.Number);
			Assert.AreSame(departure, voyage.LastKnownLocation);
			Assert.AreSame(arrival, voyage.NextExpectedLocation);
			Assert.AreSame(schedule, voyage.Schedule);
			Assert.IsFalse(voyage.IsMoving);
		}
		
		[Test]
		public void Test_Ctor_02 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			UnLocode departure = new UnLocode("DPLOC");
			UnLocode arrival = new UnLocode("ARLOC");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(schedule);
			state.Expect(s => s.LastKnownLocation).Return(departure).Repeat.Once();
			state.Expect(s => s.NextExpectedLocation).Return(arrival).Repeat.Once();
			state.Expect(s => s.IsMoving).Return(false).Repeat.Once();
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(number, state);
			
			// assert:
			Assert.AreEqual(number, voyage.Number);
			Assert.AreSame(departure, voyage.LastKnownLocation);
			Assert.AreSame(arrival, voyage.NextExpectedLocation);
			Assert.AreSame(schedule, voyage.Schedule);
			Assert.IsFalse(voyage.IsMoving);
			state.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_DepartFrom_01 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			UnLocode departure = new UnLocode("DPLOC");
			UnLocode arrival = new UnLocode("ARLOC");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(schedule);
			VoyageState state2 = MockRepository.GeneratePartialMock<VoyageState>(schedule);
			state.Expect(s => s.DepartFrom(location)).Return(state2).Repeat.Once();
			state.Expect(s => s.Equals(state2)).Return(false).Repeat.Once();
			state2.Expect(s => s.LastKnownLocation).Return(departure).Repeat.Once();
			state2.Expect(s => s.NextExpectedLocation).Return(arrival).Repeat.Once();
			VoyageEventArgs eventArguments = null;
			IVoyage eventSender = null;
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(number, state);
			voyage.Departed += delegate(object sender, VoyageEventArgs e) {
				eventArguments = e;
				eventSender = sender as IVoyage;
			};
			voyage.DepartFrom(location);
			
			// assert:
			Assert.AreEqual(number, voyage.Number);
			Assert.AreSame(voyage, eventSender);
			Assert.AreSame(departure, eventArguments.PreviousLocation);
			Assert.AreSame(arrival, eventArguments.DestinationLocation);
			state.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
			location.VerifyAllExpectations();
			state2.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_DepartFrom_02 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			UnLocode departure = new UnLocode("DPLOC");
			UnLocode arrival = new UnLocode("ARLOC");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(schedule);
			state.Expect(s => s.DepartFrom(location)).Return(state).Repeat.Once();
			state.Expect(s => s.Equals(state)).Return(true).Repeat.Once();
			VoyageEventArgs eventArguments = null;
			IVoyage eventSender = null;
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(number, state);
			voyage.Departed += delegate(object sender, VoyageEventArgs e) {
				eventArguments = e;
				eventSender = sender as IVoyage;
			};
			voyage.DepartFrom(location);
			
			// assert:
			Assert.AreEqual(number, voyage.Number);
			Assert.IsNull(eventSender);
			Assert.IsNull(eventArguments);
			state.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
			location.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_DepartFrom_03 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(schedule);
			VoyageEventArgs eventArguments = null;
			IVoyage eventSender = null;
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(number, state);
			voyage.Departed += delegate(object sender, VoyageEventArgs e) {
				eventArguments = e;
				eventSender = sender as IVoyage;
			};
			Assert.Throws<ArgumentNullException>(delegate { voyage.DepartFrom(null); });
			
			// assert:
			Assert.IsNull(eventSender);
			Assert.IsNull(eventArguments);
			state.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_DepartFrom_04 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(schedule);
			state.Expect(s => s.DepartFrom(location)).Throw(new ArgumentException()).Repeat.Once();
			VoyageEventArgs eventArguments = null;
			IVoyage eventSender = null;
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(number, state);
			voyage.Departed += delegate(object sender, VoyageEventArgs e) {
				eventArguments = e;
				eventSender = sender as IVoyage;
			};
			Assert.Throws<ArgumentException>(delegate { voyage.DepartFrom(location); });
			
			// assert:
			Assert.IsNull(eventSender);
			Assert.IsNull(eventArguments);
			state.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
			location.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_DepartFrom_05 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(schedule);
			state.Expect(s => s.DepartFrom(location)).Throw(new InvalidOperationException()).Repeat.Once();
			VoyageEventArgs eventArguments = null;
			IVoyage eventSender = null;
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(number, state);
			voyage.Departed += delegate(object sender, VoyageEventArgs e) {
				eventArguments = e;
				eventSender = sender as IVoyage;
			};
			Assert.Throws<InvalidOperationException>(delegate { voyage.DepartFrom(location); });
			
			// assert:
			Assert.IsNull(eventSender);
			Assert.IsNull(eventArguments);
			state.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
			location.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_StopOverAt_01 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			UnLocode departure = new UnLocode("DPLOC");
			UnLocode arrival = new UnLocode("ARLOC");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(schedule);
			VoyageState state2 = MockRepository.GeneratePartialMock<VoyageState>(schedule);
			state.Expect(s => s.StopOverAt(location)).Return(state2).Repeat.Once();
			state.Expect(s => s.Equals(state2)).Return(false).Repeat.Once();
			state2.Expect(s => s.LastKnownLocation).Return(departure).Repeat.Once();
			state2.Expect(s => s.NextExpectedLocation).Return(arrival).Repeat.Once();
			VoyageEventArgs eventArguments = null;
			IVoyage eventSender = null;
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(number, state);
			voyage.Stopped += delegate(object sender, VoyageEventArgs e) {
				eventArguments = e;
				eventSender = sender as IVoyage;
			};
			voyage.StopOverAt(location);
			
			// assert:
			Assert.AreEqual(number, voyage.Number);
			Assert.AreSame(voyage, eventSender);
			Assert.AreSame(departure, eventArguments.PreviousLocation);
			Assert.AreSame(arrival, eventArguments.DestinationLocation);
			state.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
			location.VerifyAllExpectations();
			state2.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_StopOverAt_02 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			UnLocode departure = new UnLocode("DPLOC");
			UnLocode arrival = new UnLocode("ARLOC");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(schedule);
			state.Expect(s => s.StopOverAt(location)).Return(state).Repeat.Once();
			state.Expect(s => s.Equals(state)).Return(true).Repeat.Once();
			VoyageEventArgs eventArguments = null;
			IVoyage eventSender = null;
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(number, state);
			voyage.Stopped += delegate(object sender, VoyageEventArgs e) {
				eventArguments = e;
				eventSender = sender as IVoyage;
			};
			voyage.StopOverAt(location);
			
			// assert:
			Assert.AreEqual(number, voyage.Number);
			Assert.IsNull(eventSender);
			Assert.IsNull(eventArguments);
			state.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
			location.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_StopOverAt_03 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(schedule);
			VoyageEventArgs eventArguments = null;
			IVoyage eventSender = null;
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(number, state);
			voyage.Stopped += delegate(object sender, VoyageEventArgs e) {
				eventArguments = e;
				eventSender = sender as IVoyage;
			};
			Assert.Throws<ArgumentNullException>(delegate { voyage.StopOverAt(null); });
			
			// assert:
			Assert.IsNull(eventSender);
			Assert.IsNull(eventArguments);
			state.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_StopOverAt_04 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(schedule);
			state.Expect(s => s.StopOverAt(location)).Throw(new ArgumentException()).Repeat.Once();
			VoyageEventArgs eventArguments = null;
			IVoyage eventSender = null;
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(number, state);
			voyage.Stopped += delegate(object sender, VoyageEventArgs e) {
				eventArguments = e;
				eventSender = sender as IVoyage;
			};
			Assert.Throws<ArgumentException>(delegate { voyage.StopOverAt(location); });
			
			// assert:
			Assert.IsNull(eventSender);
			Assert.IsNull(eventArguments);
			state.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
			location.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_StopOverAt_05 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(schedule);
			state.Expect(s => s.StopOverAt(location)).Throw(new InvalidOperationException()).Repeat.Once();
			VoyageEventArgs eventArguments = null;
			IVoyage eventSender = null;
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(number, state);
			voyage.Stopped += delegate(object sender, VoyageEventArgs e) {
				eventArguments = e;
				eventSender = sender as IVoyage;
			};
			Assert.Throws<InvalidOperationException>(delegate { voyage.StopOverAt(location); });
			
			// assert:
			Assert.IsNull(eventSender);
			Assert.IsNull(eventArguments);
			state.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
			location.VerifyAllExpectations();
		}
		
		
		[Test]
		public void Test_Equals_01()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
		
			// act:
			Challenge00.DDDSample.Voyage.Voyage voyage = new Challenge00.DDDSample.Voyage.Voyage(number, schedule);
			bool equals = voyage.Equals(voyage);
		
			// assert:
			Assert.IsTrue(equals);
		}
		
		[Test]
		public void Test_Equals_02()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();

			// act:
			Challenge00.DDDSample.Voyage.Voyage voyage = new Challenge00.DDDSample.Voyage.Voyage(number, schedule);
			bool equals = voyage.Equals(null);
		
			// assert:
			Assert.IsFalse(equals);
		}
		
		[Test]
		public void Test_Equals_03()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();

			// act:
			Challenge00.DDDSample.Voyage.Voyage voyage1 = new Challenge00.DDDSample.Voyage.Voyage(number, schedule);
			Challenge00.DDDSample.Voyage.Voyage voyage2 = new Challenge00.DDDSample.Voyage.Voyage(number, schedule);
			bool equals = voyage1.Equals(voyage2);
		
			// assert:
			Assert.IsTrue(equals);
		}
		
		[Test]
		public void Test_Equals_04()
		{
			// arrange:
			VoyageNumber number1 = new VoyageNumber("VYG01");
			VoyageNumber number2 = new VoyageNumber("VYG02");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();

			// act:
			Challenge00.DDDSample.Voyage.Voyage voyage1 = new Challenge00.DDDSample.Voyage.Voyage(number1, schedule);
			Challenge00.DDDSample.Voyage.Voyage voyage2 = new Challenge00.DDDSample.Voyage.Voyage(number2, schedule);
			bool equals = voyage1.Equals(voyage2);
		
			// assert:
			Assert.IsFalse(equals);
		}
	}
}

