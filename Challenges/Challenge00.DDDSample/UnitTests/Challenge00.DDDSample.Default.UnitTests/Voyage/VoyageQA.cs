//  
//  VoyageTester.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2011 Giacomo Tesio
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
	class FakeVoyage : Challenge00.DDDSample.Voyage.Voyage
	{
		public FakeVoyage(VoyageState initialState)
			: base(initialState)
		{
		}
	}
	
	[TestFixture]
	public class VoyageQA
	{
		[Test]
		public void Ctor_01 ()
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
		public void Ctor_02 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			UnLocode departure = new UnLocode("DPLOC");
			UnLocode arrival = new UnLocode("ARLOC");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(number, schedule);
			state.Expect(s => s.LastKnownLocation).Return(departure).Repeat.Once();
			state.Expect(s => s.NextExpectedLocation).Return(arrival).Repeat.Once();
			state.Expect(s => s.IsMoving).Return(false).Repeat.Once();
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(state);
			
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
		public void Ctor_03()
		{
			// assert:
			Assert.Throws<ArgumentNullException>(delegate{ new FakeVoyage(null); });
		}
		
		
		[Test]
		public void DepartFrom_01 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			UnLocode departure = new UnLocode("DPLOC");
			UnLocode arrival = new UnLocode("ARLOC");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(number, schedule);
			VoyageState state2 = MockRepository.GeneratePartialMock<VoyageState>(number, schedule);
			state.Expect(s => s.DepartFrom(location)).Return(state2).Repeat.Once();
			state.Expect(s => s.Equals(state2)).Return(false).Repeat.Once();
			state2.Expect(s => s.LastKnownLocation).Return(departure).Repeat.Once();
			state2.Expect(s => s.NextExpectedLocation).Return(arrival).Repeat.Once();
			VoyageEventArgs eventArguments = null;
			IVoyage eventSender = null;
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(state);
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
		public void DepartFrom_02 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(number, schedule);
			state.Expect(s => s.DepartFrom(location)).Return(state).Repeat.Once();
			state.Expect(s => s.Equals(state)).Return(true).Repeat.Once();
			VoyageEventArgs eventArguments = null;
			IVoyage eventSender = null;
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(state);
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
		public void DepartFrom_03 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(number, schedule);
			VoyageEventArgs eventArguments = null;
			IVoyage eventSender = null;
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(state);
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
		public void DepartFrom_04 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(number, schedule);
			state.Expect(s => s.DepartFrom(location)).Throw(new ArgumentException()).Repeat.Once();
			VoyageEventArgs eventArguments = null;
			IVoyage eventSender = null;
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(state);
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
		public void DepartFrom_05 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(number, schedule);
			state.Expect(s => s.DepartFrom(location)).Throw(new InvalidOperationException()).Repeat.Once();
			VoyageEventArgs eventArguments = null;
			IVoyage eventSender = null;
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(state);
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
		public void DepartFrom_06 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			UnLocode departure = new UnLocode("DPLOC");
			UnLocode arrival = new UnLocode("ARLOC");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(number, schedule);
			VoyageState state2 = MockRepository.GeneratePartialMock<VoyageState>(number, schedule);
			state.Expect(s => s.DepartFrom(location)).Return(state2).Repeat.Once();
			state.Expect(s => s.Equals(state2)).Return(false).Repeat.Once();
			state2.Expect(s => s.LastKnownLocation).Return(departure).Repeat.Once();
			state2.Expect(s => s.NextExpectedLocation).Return(arrival).Repeat.Once();
			VoyageEventArgs eventArguments = null;
			IVoyage eventSender = null;
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(state);
			
			EventHandler<VoyageEventArgs> handler = delegate(object sender, VoyageEventArgs e) {
				eventArguments = e;
				eventSender = sender as IVoyage;
			};
			voyage.Departed += handler;
			voyage.Departed -= handler;
			voyage.DepartFrom(location);
			
			// assert:
			Assert.AreEqual(number, voyage.Number);
			Assert.IsNull(eventSender);
			Assert.IsNull(eventArguments);
			state.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
			location.VerifyAllExpectations();
			state2.VerifyAllExpectations();
		}
		
		[Test]
		public void StopOverAt_01 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			UnLocode departure = new UnLocode("DPLOC");
			UnLocode arrival = new UnLocode("ARLOC");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(number, schedule);
			VoyageState state2 = MockRepository.GeneratePartialMock<VoyageState>(number, schedule);
			state.Expect(s => s.StopOverAt(location)).Return(state2).Repeat.Once();
			state.Expect(s => s.Equals(state2)).Return(false).Repeat.Once();
			state.Expect(s => s.LastKnownLocation).Return(departure).Repeat.Once();
			state.Expect(s => s.NextExpectedLocation).Return(arrival).Repeat.Once();
			VoyageEventArgs eventArguments = null;
			IVoyage eventSender = null;
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(state);
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
		public void StopOverAt_02 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(number, schedule);
			state.Expect(s => s.StopOverAt(location)).Return(state).Repeat.Once();
			state.Expect(s => s.Equals(state)).Return(true).Repeat.Once();
			VoyageEventArgs eventArguments = null;
			IVoyage eventSender = null;
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(state);
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
		public void StopOverAt_03 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(number, schedule);
			VoyageEventArgs eventArguments = null;
			IVoyage eventSender = null;
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(state);
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
		public void StopOverAt_04 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(number, schedule);
			state.Expect(s => s.StopOverAt(location)).Throw(new ArgumentException()).Repeat.Once();
			VoyageEventArgs eventArguments = null;
			IVoyage eventSender = null;
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(state);
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
		public void StopOverAt_05 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(number, schedule);
			state.Expect(s => s.StopOverAt(location)).Throw(new InvalidOperationException()).Repeat.Once();
			VoyageEventArgs eventArguments = null;
			IVoyage eventSender = null;
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(state);
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
		public void StopOverAt_06 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			UnLocode departure = new UnLocode("DPLOC");
			UnLocode arrival = new UnLocode("ARLOC");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(number, schedule);
			VoyageState state2 = MockRepository.GeneratePartialMock<VoyageState>(number, schedule);
			state.Expect(s => s.StopOverAt(location)).Return(state2).Repeat.Once();
			state.Expect(s => s.Equals(state2)).Return(false).Repeat.Once();
			state.Expect(s => s.LastKnownLocation).Return(departure).Repeat.Once();
			state.Expect(s => s.NextExpectedLocation).Return(arrival).Repeat.Once();
			VoyageEventArgs eventArguments = null;
			IVoyage eventSender = null;
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(state);
			EventHandler<VoyageEventArgs> handler = delegate(object sender, VoyageEventArgs e) {
				eventArguments = e;
				eventSender = sender as IVoyage;
			};
			voyage.Stopped += handler;
			voyage.Stopped -= handler;
			voyage.StopOverAt(location);
			
			// assert:
			Assert.AreEqual(number, voyage.Number);
			Assert.IsNull(eventSender);
			Assert.IsNull(eventArguments);
			state.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
			location.VerifyAllExpectations();
			state2.VerifyAllExpectations();
		}	
		[TestCase(true)]
		[TestCase(false)]
		public void WillStopOverAt_01 (bool returnedValue)
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(number, schedule);
			state.Expect(s => s.WillStopOverAt(location)).Return(returnedValue).Repeat.Once();
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(state);
			bool willStopOverAt = voyage.WillStopOverAt(location);
			
			// assert:
			Assert.AreEqual(returnedValue, willStopOverAt);
			location.VerifyAllExpectations();
			state.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
		}
		
		[Test]
		public void WillStopOverAt_02 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(number, schedule);
			state.Expect(s => s.WillStopOverAt(location)).Throw(new InvalidOperationException()).Repeat.Once();
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(state);
			
			// assert:
			Assert.Throws<InvalidOperationException>( delegate{ voyage.WillStopOverAt(location); });
			location.VerifyAllExpectations();
			state.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
		}
		
		[Test]
		public void WillStopOverAt_03 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYG01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			VoyageState state = MockRepository.GeneratePartialMock<VoyageState>(number, schedule);
			
			// act:
			IVoyage voyage = MockRepository.GeneratePartialMock<Challenge00.DDDSample.Voyage.Voyage>(state);
			
			// assert:
			Assert.Throws<ArgumentNullException>( delegate{ voyage.WillStopOverAt(null); });
			state.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
		}
	}
}

