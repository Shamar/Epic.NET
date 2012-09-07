//  
//  IntegrationTests.cs
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
using Challenge00.DDDSample.Location;
using Rhino.Mocks;
using Challenge00.DDDSample.Voyage;
namespace DefaultImplementation.Voyage
{
	[TestFixture()]
	public class IntegrationTests
	{
		[Test()]
		public void Voyage_LiveCycle_01 ()
		{
			// arrange:
			UnLocode code1 = new UnLocode("CODAA");
			ILocation loc1 = MockRepository.GenerateStrictMock<ILocation>();
			loc1.Expect(l => l.UnLocode).Return(code1).Repeat.Any();
			loc1.Expect(l => l.Name).Return("First location").Repeat.Any();
			UnLocode code2 = new UnLocode("CODAB");
			ILocation loc2 = MockRepository.GenerateStrictMock<ILocation>();
			loc2.Expect(l => l.UnLocode).Return(code2).Repeat.Any();
			loc2.Expect(l => l.Name).Return("Second location").Repeat.Any();
			UnLocode code3 = new UnLocode("CODAC");
			ILocation loc3 = MockRepository.GenerateStrictMock<ILocation>();
			loc3.Expect(l => l.UnLocode).Return(code3).Repeat.Any();
			loc3.Expect(l => l.Name).Return("Third location").Repeat.Any();
			
			ISchedule schedule = new Schedule();
			schedule = schedule.Append(new CarrierMovement(loc1, DateTime.UtcNow, loc2, DateTime.UtcNow + TimeSpan.FromDays(2)));
			schedule = schedule.Append(new CarrierMovement(loc2, DateTime.UtcNow + TimeSpan.FromDays(3), loc3, DateTime.UtcNow + TimeSpan.FromDays(4)));

			VoyageNumber number = new VoyageNumber("TESTVYG");
			VoyageEventArgs departedEvent = null;
			VoyageEventArgs stoppedEvent = null;
			
			// act:
			Challenge00.DDDSample.Voyage.Voyage voyage = new Challenge00.DDDSample.Voyage.Voyage(number, schedule);
			voyage.Departed += delegate(object sender, VoyageEventArgs e) {
				departedEvent = e;
			};
			voyage.Stopped += delegate(object sender, VoyageEventArgs e) {
				stoppedEvent = e;
			};
			
			// assert:
			Assert.AreSame(number, voyage.Number);
			Assert.IsFalse(voyage.IsMoving);
			Assert.AreEqual(code1, voyage.LastKnownLocation);
			Assert.AreEqual(code2, voyage.NextExpectedLocation);
			Assert.IsNull(departedEvent);
			Assert.IsNull(stoppedEvent);
			loc1.VerifyAllExpectations();
			loc2.VerifyAllExpectations();
			loc3.VerifyAllExpectations();
		}
		
		[Test()]
		public void Voyage_LiveCycle_02 ()
		{
			// arrange:
			UnLocode code1 = new UnLocode("CODAA");
			ILocation loc1 = MockRepository.GenerateStrictMock<ILocation>();
			loc1.Expect(l => l.UnLocode).Return(code1).Repeat.Any();
			loc1.Expect(l => l.Name).Return("First location").Repeat.Any();
			UnLocode code2 = new UnLocode("CODAB");
			ILocation loc2 = MockRepository.GenerateStrictMock<ILocation>();
			loc2.Expect(l => l.UnLocode).Return(code2).Repeat.Any();
			loc2.Expect(l => l.Name).Return("Second location").Repeat.Any();
			UnLocode code3 = new UnLocode("CODAC");
			ILocation loc3 = MockRepository.GenerateStrictMock<ILocation>();
			loc3.Expect(l => l.UnLocode).Return(code3).Repeat.Any();
			loc3.Expect(l => l.Name).Return("Third location").Repeat.Any();
			
			ISchedule schedule = new Schedule();
			schedule = schedule.Append(new CarrierMovement(loc1, DateTime.UtcNow, loc2, DateTime.UtcNow + TimeSpan.FromDays(2)));
			schedule = schedule.Append(new CarrierMovement(loc2, DateTime.UtcNow + TimeSpan.FromDays(3), loc3, DateTime.UtcNow + TimeSpan.FromDays(4)));

			VoyageNumber number = new VoyageNumber("TESTVYG");
			VoyageEventArgs departedEvent = null;
			VoyageEventArgs stoppedEvent = null;
			Challenge00.DDDSample.Voyage.Voyage voyage = new Challenge00.DDDSample.Voyage.Voyage(number, schedule);
			voyage.Departed += delegate(object sender, VoyageEventArgs e) {
				departedEvent = e;
			};
			voyage.Stopped += delegate(object sender, VoyageEventArgs e) {
				stoppedEvent = e;
			};
			
			// act:
			voyage.DepartFrom(loc1);
			
			// assert:
			Assert.AreSame(number, voyage.Number);
			Assert.IsTrue(voyage.IsMoving);
			Assert.AreEqual(code1, voyage.LastKnownLocation);
			Assert.AreEqual(code2, voyage.NextExpectedLocation);
			Assert.IsNotNull(departedEvent);
			Assert.AreEqual(code1, departedEvent.PreviousLocation);
			Assert.AreEqual(code2, departedEvent.DestinationLocation);
			Assert.IsNull(stoppedEvent);
			loc1.VerifyAllExpectations();
			loc2.VerifyAllExpectations();
			loc3.VerifyAllExpectations();
		}
		
		[Test()]
		public void Voyage_LiveCycle_03 ()
		{
			// arrange:
			UnLocode code1 = new UnLocode("CODAA");
			ILocation loc1 = MockRepository.GenerateStrictMock<ILocation>();
			loc1.Expect(l => l.UnLocode).Return(code1).Repeat.Any();
			loc1.Expect(l => l.Name).Return("First location").Repeat.Any();
			UnLocode code2 = new UnLocode("CODAB");
			ILocation loc2 = MockRepository.GenerateStrictMock<ILocation>();
			loc2.Expect(l => l.UnLocode).Return(code2).Repeat.Any();
			loc2.Expect(l => l.Name).Return("Second location").Repeat.Any();
			UnLocode code3 = new UnLocode("CODAC");
			ILocation loc3 = MockRepository.GenerateStrictMock<ILocation>();
			loc3.Expect(l => l.UnLocode).Return(code3).Repeat.Any();
			loc3.Expect(l => l.Name).Return("Third location").Repeat.Any();
			
			ISchedule schedule = new Schedule();
			schedule = schedule.Append(new CarrierMovement(loc1, DateTime.UtcNow, loc2, DateTime.UtcNow + TimeSpan.FromDays(2)));
			schedule = schedule.Append(new CarrierMovement(loc2, DateTime.UtcNow + TimeSpan.FromDays(3), loc3, DateTime.UtcNow + TimeSpan.FromDays(4)));

			VoyageNumber number = new VoyageNumber("TESTVYG");
			VoyageEventArgs departedEvent = null;
			VoyageEventArgs stoppedEvent = null;
			Challenge00.DDDSample.Voyage.Voyage voyage = new Challenge00.DDDSample.Voyage.Voyage(number, schedule);
			voyage.DepartFrom(loc1);
			voyage.Departed += delegate(object sender, VoyageEventArgs e) {
				departedEvent = e;
			};
			voyage.Stopped += delegate(object sender, VoyageEventArgs e) {
				stoppedEvent = e;
			};
			
			// act:
			voyage.StopOverAt(loc2);
			
			// assert:
			Assert.AreSame(number, voyage.Number);
			Assert.IsFalse(voyage.IsMoving);
			Assert.AreEqual(code2, voyage.LastKnownLocation);
			Assert.AreEqual(code3, voyage.NextExpectedLocation);
			Assert.IsNull(departedEvent);
			Assert.IsNotNull(stoppedEvent);
			Assert.AreEqual(code1, stoppedEvent.PreviousLocation);
			Assert.AreEqual(code2, stoppedEvent.DestinationLocation);
			loc1.VerifyAllExpectations();
			loc2.VerifyAllExpectations();
			loc3.VerifyAllExpectations();
		}
		
		[Test()]
		public void Voyage_LiveCycle_04 ()
		{
			// arrange:
			UnLocode code1 = new UnLocode("CODAA");
			ILocation loc1 = MockRepository.GenerateStrictMock<ILocation>();
			loc1.Expect(l => l.UnLocode).Return(code1).Repeat.Any();
			loc1.Expect(l => l.Name).Return("First location").Repeat.Any();
			UnLocode code2 = new UnLocode("CODAB");
			ILocation loc2 = MockRepository.GenerateStrictMock<ILocation>();
			loc2.Expect(l => l.UnLocode).Return(code2).Repeat.Any();
			loc2.Expect(l => l.Name).Return("Second location").Repeat.Any();
			UnLocode code3 = new UnLocode("CODAC");
			ILocation loc3 = MockRepository.GenerateStrictMock<ILocation>();
			loc3.Expect(l => l.UnLocode).Return(code3).Repeat.Any();
			loc3.Expect(l => l.Name).Return("Third location").Repeat.Any();
			
			ISchedule schedule = new Schedule();
			schedule = schedule.Append(new CarrierMovement(loc1, DateTime.UtcNow, loc2, DateTime.UtcNow + TimeSpan.FromDays(2)));
			schedule = schedule.Append(new CarrierMovement(loc2, DateTime.UtcNow + TimeSpan.FromDays(3), loc3, DateTime.UtcNow + TimeSpan.FromDays(4)));

			VoyageNumber number = new VoyageNumber("TESTVYG");
			VoyageEventArgs departedEvent = null;
			VoyageEventArgs stoppedEvent = null;
			Challenge00.DDDSample.Voyage.Voyage voyage = new Challenge00.DDDSample.Voyage.Voyage(number, schedule);
			voyage.DepartFrom(loc1);
			voyage.StopOverAt(loc2);
			voyage.Departed += delegate(object sender, VoyageEventArgs e) {
				departedEvent = e;
			};
			voyage.Stopped += delegate(object sender, VoyageEventArgs e) {
				stoppedEvent = e;
			};
			
			// act:
			voyage.DepartFrom(loc2);
			
			// assert:
			Assert.AreSame(number, voyage.Number);
			Assert.IsTrue(voyage.IsMoving);
			Assert.AreEqual(code2, voyage.LastKnownLocation);
			Assert.AreEqual(code3, voyage.NextExpectedLocation);
			Assert.IsNotNull(departedEvent);
			Assert.AreEqual(code2, departedEvent.PreviousLocation);
			Assert.AreEqual(code3, departedEvent.DestinationLocation);
			Assert.IsNull(stoppedEvent);
			loc1.VerifyAllExpectations();
			loc2.VerifyAllExpectations();
			loc3.VerifyAllExpectations();
		}
		
		[Test()]
		public void Voyage_LiveCycle_05 ()
		{
			// arrange:
			UnLocode code1 = new UnLocode("CODAA");
			ILocation loc1 = MockRepository.GenerateStrictMock<ILocation>();
			loc1.Expect(l => l.UnLocode).Return(code1).Repeat.Any();
			loc1.Expect(l => l.Name).Return("First location").Repeat.Any();
			UnLocode code2 = new UnLocode("CODAB");
			ILocation loc2 = MockRepository.GenerateStrictMock<ILocation>();
			loc2.Expect(l => l.UnLocode).Return(code2).Repeat.Any();
			loc2.Expect(l => l.Name).Return("Second location").Repeat.Any();
			UnLocode code3 = new UnLocode("CODAC");
			ILocation loc3 = MockRepository.GenerateStrictMock<ILocation>();
			loc3.Expect(l => l.UnLocode).Return(code3).Repeat.Any();
			loc3.Expect(l => l.Name).Return("Third location").Repeat.Any();
			
			ISchedule schedule = new Schedule();
			schedule = schedule.Append(new CarrierMovement(loc1, DateTime.UtcNow, loc2, DateTime.UtcNow + TimeSpan.FromDays(2)));
			schedule = schedule.Append(new CarrierMovement(loc2, DateTime.UtcNow + TimeSpan.FromDays(3), loc3, DateTime.UtcNow + TimeSpan.FromDays(4)));

			VoyageNumber number = new VoyageNumber("TESTVYG");
			VoyageEventArgs departedEvent = null;
			VoyageEventArgs stoppedEvent = null;
			Challenge00.DDDSample.Voyage.Voyage voyage = new Challenge00.DDDSample.Voyage.Voyage(number, schedule);
			voyage.DepartFrom(loc1);
			voyage.StopOverAt(loc2);
			voyage.DepartFrom(loc2);
			voyage.Departed += delegate(object sender, VoyageEventArgs e) {
				departedEvent = e;
			};
			voyage.Stopped += delegate(object sender, VoyageEventArgs e) {
				stoppedEvent = e;
			};
			
			// act:
			voyage.StopOverAt(loc3);
			
			// assert:
			Assert.AreSame(number, voyage.Number);
			Assert.IsFalse(voyage.IsMoving);
			Assert.AreEqual(code3, voyage.LastKnownLocation);
			Assert.AreEqual(code3, voyage.NextExpectedLocation);
			Assert.IsNull(departedEvent);
			Assert.IsNotNull(stoppedEvent);
			Assert.AreEqual(code2, stoppedEvent.PreviousLocation);
			Assert.AreEqual(code3, stoppedEvent.DestinationLocation);
			loc1.VerifyAllExpectations();
			loc2.VerifyAllExpectations();
			loc3.VerifyAllExpectations();
		}
		
		[Test()]
		public void Voyage_LiveCycle_06 ()
		{
			// arrange:
			UnLocode code1 = new UnLocode("CODAA");
			ILocation loc1 = MockRepository.GenerateStrictMock<ILocation>();
			loc1.Expect(l => l.UnLocode).Return(code1).Repeat.Any();
			loc1.Expect(l => l.Name).Return("First location").Repeat.Any();
			UnLocode code2 = new UnLocode("CODAB");
			ILocation loc2 = MockRepository.GenerateStrictMock<ILocation>();
			loc2.Expect(l => l.UnLocode).Return(code2).Repeat.Any();
			loc2.Expect(l => l.Name).Return("Second location").Repeat.Any();
			UnLocode code3 = new UnLocode("CODAC");
			ILocation loc3 = MockRepository.GenerateStrictMock<ILocation>();
			loc3.Expect(l => l.UnLocode).Return(code3).Repeat.Any();
			loc3.Expect(l => l.Name).Return("Third location").Repeat.Any();
			
			ISchedule schedule = new Schedule();
			schedule = schedule.Append(new CarrierMovement(loc1, DateTime.UtcNow, loc2, DateTime.UtcNow + TimeSpan.FromDays(2)));
			schedule = schedule.Append(new CarrierMovement(loc2, DateTime.UtcNow + TimeSpan.FromDays(3), loc3, DateTime.UtcNow + TimeSpan.FromDays(4)));

			VoyageNumber number = new VoyageNumber("TESTVYG");
			VoyageEventArgs departedEvent = null;
			VoyageEventArgs stoppedEvent = null;
			Challenge00.DDDSample.Voyage.Voyage voyage = new Challenge00.DDDSample.Voyage.Voyage(number, schedule);
			voyage.DepartFrom(loc1);
			voyage.StopOverAt(loc2);
			voyage.DepartFrom(loc2);
			voyage.StopOverAt(loc3);
			voyage.Departed += delegate(object sender, VoyageEventArgs e) {
				departedEvent = e;
			};
			voyage.Stopped += delegate(object sender, VoyageEventArgs e) {
				stoppedEvent = e;
			};
			
			// act:
			Assert.Throws<VoyageCompletedException>(() => voyage.DepartFrom(loc3));
			
			// assert:
			Assert.AreSame(number, voyage.Number);
			Assert.IsFalse(voyage.IsMoving);
			Assert.AreEqual(code3, voyage.LastKnownLocation);
			Assert.AreEqual(code3, voyage.NextExpectedLocation);
			Assert.IsNull(departedEvent);
			Assert.IsNull(stoppedEvent);
			loc1.VerifyAllExpectations();
			loc2.VerifyAllExpectations();
			loc3.VerifyAllExpectations();
		}
	}
}

