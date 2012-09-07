//  
//  MovingVoyageTester.cs
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
using Rhino.Mocks;
using Challenge00.DDDSample.Voyage;
using Challenge00.DDDSample.Location;
namespace DefaultImplementation.Voyage
{
	[TestFixture]
	public class MovingVoyageQA
	{
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void Ctor_01(int index)
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
		
			// act:
			MovingVoyage state = new MovingVoyage(number, schedule, index);
		
			// assert:
			Assert.AreSame(number, state.Number);
			Assert.AreSame(schedule, state.Schedule);
			Assert.IsTrue(state.IsMoving);
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Ctor_02 ()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
		
			// act:
			new MovingVoyage(number, null, 0);
		}
		
		[TestCase(-1)]
		[TestCase(3)]
		[TestCase(4)]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void Ctor_03(int index)
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
		
			// act:
			new MovingVoyage(number, schedule, index);
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Ctor_04 ()
		{
			// arrange:
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
		
			// act:
			new MovingVoyage(null, schedule, 0);
		}
		
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void LastKnownLocation_01(int index)
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			UnLocode initialLocation = new UnLocode("DPLOC");
			ICarrierMovement movement = MockRepository.GenerateStrictMock<ICarrierMovement>();
			movement.Expect(m => m.DepartureLocation).Return(initialLocation).Repeat.Once();
			schedule.Expect(s => s[index]).Return(movement).Repeat.Once();
		
			// act:
			MovingVoyage state = new MovingVoyage(number, schedule, index);
		
			// assert:
			Assert.AreSame(initialLocation, state.LastKnownLocation);
			movement.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
		}
		
	
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void NextExpectedLocation_01(int index)
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			UnLocode initialLocation = new UnLocode("ARLOC");
			ICarrierMovement movement = MockRepository.GenerateStrictMock<ICarrierMovement>();
			movement.Expect(m => m.ArrivalLocation).Return(initialLocation).Repeat.Once();
			schedule.Expect(s => s[index]).Return(movement).Repeat.Once();
		
			// act:
			MovingVoyage state = new MovingVoyage(number, schedule, index);
		
			// assert:
			Assert.AreSame(initialLocation, state.NextExpectedLocation);
			movement.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
		}

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void StopOverAt_01(int index)
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(4).Repeat.Any();
			UnLocode arrivalLocation = new UnLocode("ARLOC");
			ICarrierMovement movement1 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			movement1.Expect(m => m.ArrivalLocation).Return(arrivalLocation).Repeat.Twice();
			schedule.Expect(s => s[index]).Return(movement1).Repeat.Twice();
			ICarrierMovement movement2 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			movement2.Expect(m => m.DepartureLocation).Return(arrivalLocation).Repeat.Once();
			schedule.Expect(s => s[index + 1]).Return(movement2).Repeat.Once();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(arrivalLocation).Repeat.Any();

			
			// act:
			MovingVoyage state = new MovingVoyage(number, schedule, index);
			VoyageState stopped = state.StopOverAt(location);
		
			// assert:
			Assert.IsInstanceOf<StoppedVoyage>(stopped);
			Assert.AreSame(state.NextExpectedLocation, stopped.LastKnownLocation);
			Assert.IsFalse(stopped.IsMoving);
			schedule.VerifyAllExpectations();
			movement1.VerifyAllExpectations();
			movement2.VerifyAllExpectations();
			location.VerifyAllExpectations();
		}
		
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void StopOverAt_02(int index)
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			UnLocode arrivalLocation = new UnLocode("ARLOC");
			ICarrierMovement movement = MockRepository.GenerateStrictMock<ICarrierMovement>();
			movement.Expect(m => m.ArrivalLocation).Return(arrivalLocation).Repeat.Any();
			schedule.Expect(s => s[index]).Return(movement).Repeat.Any();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(new UnLocode("ANTHR")).Repeat.Any();
			
			// act:
			MovingVoyage state = new MovingVoyage(number, schedule, index);

			// assert:
			Assert.Throws<WrongLocationException>(delegate {state.StopOverAt(location);});
			schedule.VerifyAllExpectations();
			movement.VerifyAllExpectations();
			location.VerifyAllExpectations();
		}
		
		[TestCase(0, 1)]
		[TestCase(1, 2)]
		[TestCase(2, 3)]
		[TestCase(9, 10)]
		public void StopOverAt_Destination_01(int index, int movementsCount)
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(movementsCount).Repeat.Any();
			UnLocode arrivalLocation = new UnLocode("ARLOC");
			ICarrierMovement movement1 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			movement1.Expect(m => m.ArrivalLocation).Return(arrivalLocation).Repeat.Times(3);
			schedule.Expect(s => s[index]).Return(movement1).Repeat.Times(3);
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(arrivalLocation).Repeat.Any();

			
			// act:
			MovingVoyage state = new MovingVoyage(number, schedule, index);
			VoyageState stopped = state.StopOverAt(location);
		
			// assert:
			Assert.IsInstanceOf<CompletedVoyage>(stopped);
			Assert.AreSame(state.NextExpectedLocation, stopped.LastKnownLocation);
			Assert.IsFalse(stopped.IsMoving);
			schedule.VerifyAllExpectations();
			movement1.VerifyAllExpectations();
			location.VerifyAllExpectations();
		}
		
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void DepartFrom_01(int index)
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			UnLocode initialLocation = new UnLocode("DPLOC");
			UnLocode destinationLocation = new UnLocode("ARLOC");
			ICarrierMovement movement = MockRepository.GenerateStrictMock<ICarrierMovement>();
			movement.Expect(m => m.DepartureLocation).Return(initialLocation).Repeat.Any();
			movement.Expect(m => m.ArrivalLocation).Return(destinationLocation).Repeat.Any();
			schedule.Expect(s => s[index]).Return(movement).Repeat.Any();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(initialLocation).Repeat.Any();
			
			// act:
			MovingVoyage state = new MovingVoyage(number, schedule, index);
			VoyageState moving = state.DepartFrom(location);
		
			// assert:
			Assert.AreSame(state, moving);
			schedule.VerifyAllExpectations();
			movement.VerifyAllExpectations();
			location.VerifyAllExpectations();
		} 
		
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void DepartFrom_02(int index)
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			UnLocode initialLocation = new UnLocode("DPLOC");
			ICarrierMovement movement = MockRepository.GenerateStrictMock<ICarrierMovement>();
			movement.Expect(m => m.DepartureLocation).Return(initialLocation).Repeat.Any();
			schedule.Expect(s => s[index]).Return(movement).Repeat.Any();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(new UnLocode("ANTHR")).Repeat.Any();
			
			// act:
			MovingVoyage state = new MovingVoyage(number, schedule, index);

			// assert:
			Assert.Throws<WrongLocationException>(delegate {state.DepartFrom(location);});
			schedule.VerifyAllExpectations();
			movement.VerifyAllExpectations();
			location.VerifyAllExpectations();
		}
		
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void Equals_01(int index)
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			schedule.Expect(s => s.Equals(schedule)).Return(true).Repeat.Any();

			// act:
			MovingVoyage state1 = new MovingVoyage(number, schedule, index);
			MovingVoyage state2 = new MovingVoyage(number, schedule, index);
			
			// assert:
			Assert.IsFalse(state1.Equals(null));
			Assert.IsTrue(state1.Equals(state1));
			Assert.IsTrue(state1.Equals(state2));
			Assert.IsTrue(state2.Equals(state1));
			Assert.IsTrue(state1.Equals((object)state1));
			Assert.IsTrue(state1.Equals((object)state2));
			Assert.IsTrue(state2.Equals((object)state1));
			schedule.VerifyAllExpectations();
		}
		
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void Equals_02(int index)
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(4).Repeat.Any();
			schedule.Expect(s => s.Equals(schedule)).Return(true).Repeat.Any();

			// act:
			MovingVoyage state1 = new MovingVoyage(number, schedule, index);
			MovingVoyage state2 = new MovingVoyage(number, schedule, index + 1);
			
			// assert:
			Assert.IsFalse(state1.Equals(state2));
			Assert.IsFalse(state2.Equals(state1));
			schedule.VerifyAllExpectations();
		}

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void Equals_03(int index)
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule1 = MockRepository.GenerateStrictMock<ISchedule>();
			ISchedule schedule2 = MockRepository.GenerateStrictMock<ISchedule>();
			schedule1.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			schedule1.Expect(s => s.Equals(schedule2)).Return(false).Repeat.Any();
			schedule2.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			schedule2.Expect(s => s.Equals(schedule1)).Return(false).Repeat.Any();

			// act:
			MovingVoyage state1 = new MovingVoyage(number, schedule1, index);
			MovingVoyage state2 = new MovingVoyage(number, schedule2, index);
			
			// assert:
			Assert.IsFalse(state1.Equals(state2));
			Assert.IsFalse(state2.Equals(state1));
			schedule1.VerifyAllExpectations();
			schedule2.VerifyAllExpectations();
		}
		
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void Equals_04(int index)
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			schedule.Expect(s => s.Equals(schedule)).Return(true).Repeat.Any();

			// act:
			MovingVoyage state1 = new MovingVoyage(number, schedule, index);
			VoyageState state2 = new StoppedVoyage(number, schedule, index);
			
			// assert:
			Assert.IsFalse(state1.Equals(state2));
			schedule.VerifyAllExpectations();
		}
		
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void Equals_05(int index)
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();

			// act:
			MovingVoyage state1 = new MovingVoyage(number, schedule, index);
			VoyageState state2 = MockRepository.GeneratePartialMock<VoyageState>(number, schedule);
			
			// assert:
			Assert.IsFalse(state1.Equals(state2));
			schedule.VerifyAllExpectations();
			state2.VerifyAllExpectations();
		}
		
		[Test]
		public void WillStopOverAt_01()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			UnLocode loc2 = new UnLocode("ARLCA");
			UnLocode loc3 = new UnLocode("ARLCB");
			UnLocode loc4 = new UnLocode("ARLCC");
			ICarrierMovement mov1 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			mov1.Expect(m => m.ArrivalLocation).Return(loc2).Repeat.AtLeastOnce();
			schedule.Expect(s => s[0]).Return(mov1).Repeat.Any();
			ICarrierMovement mov2 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			//mov2.Expect(m => m.DepartureLocation).Return(loc2).Repeat.Any();
			mov2.Expect(m => m.ArrivalLocation).Return(loc3).Repeat.AtLeastOnce();
			schedule.Expect(s => s[1]).Return(mov2).Repeat.Any();
			ICarrierMovement mov3 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			//mov3.Expect(m => m.DepartureLocation).Return(loc3).Repeat.Any();
			mov3.Expect(m => m.ArrivalLocation).Return(loc4).Repeat.AtLeastOnce();
			schedule.Expect(s => s[2]).Return(mov3).Repeat.Any();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(loc4).Repeat.AtLeastOnce();
	
			// act:
			MovingVoyage state = new MovingVoyage(number, schedule, 0);
			bool willStopOverAtLocation = state.WillStopOverAt(location);
		
			// assert:
			Assert.IsTrue(willStopOverAtLocation);
			location.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
			mov1.VerifyAllExpectations();
			mov2.VerifyAllExpectations();
			mov3.VerifyAllExpectations();
		}
		
		[Test]
		public void WillStopOverAt_02()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			UnLocode loc2 = new UnLocode("ARLCA");
			UnLocode loc3 = new UnLocode("ARLCB");
			UnLocode loc4 = new UnLocode("ARLCC");
			ICarrierMovement mov2 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			mov2.Expect(m => m.DepartureLocation).Return(loc2).Repeat.Any();
			mov2.Expect(m => m.ArrivalLocation).Return(loc3).Repeat.AtLeastOnce();
			schedule.Expect(s => s[1]).Return(mov2).Repeat.Any();
			ICarrierMovement mov3 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			//mov3.Expect(m => m.DepartureLocation).Return(loc3).Repeat.Any();
			mov3.Expect(m => m.ArrivalLocation).Return(loc4).Repeat.AtLeastOnce();
			schedule.Expect(s => s[2]).Return(mov3).Repeat.Any();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(loc4).Repeat.AtLeastOnce();
	
			// act:
			MovingVoyage state = new MovingVoyage(number, schedule, 1);
			bool willStopOverAtLocation = state.WillStopOverAt(location);
		
			// assert:
			Assert.IsTrue(willStopOverAtLocation);
			location.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
			mov2.VerifyAllExpectations();
			mov3.VerifyAllExpectations();
		}
				
		[Test]
		public void WillStopOverAt_03()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			UnLocode loc3 = new UnLocode("ARLCB");
			UnLocode loc4 = new UnLocode("ARLCC");
			ICarrierMovement mov3 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			mov3.Expect(m => m.DepartureLocation).Return(loc3).Repeat.Any();
			mov3.Expect(m => m.ArrivalLocation).Return(loc4).Repeat.AtLeastOnce();
			schedule.Expect(s => s[2]).Return(mov3).Repeat.Any();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(loc4).Repeat.AtLeastOnce();
	
			// act:
			MovingVoyage state = new MovingVoyage(number, schedule, 2);
			bool willStopOverAtLocation = state.WillStopOverAt(location);
		
			// assert:
			Assert.IsTrue(willStopOverAtLocation);
			location.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
			mov3.VerifyAllExpectations();
		}
		
		[Test]
		public void WillStopOverAt_04()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			UnLocode loc2 = new UnLocode("ARLCA");
			UnLocode loc3 = new UnLocode("ARLCB");
			UnLocode loc4 = new UnLocode("ARLCC");
			ICarrierMovement mov1 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			mov1.Expect(m => m.ArrivalLocation).Return(loc2).Repeat.AtLeastOnce();
			schedule.Expect(s => s[0]).Return(mov1).Repeat.Any();
			ICarrierMovement mov2 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			//mov2.Expect(m => m.DepartureLocation).Return(loc2).Repeat.Any();
			mov2.Expect(m => m.ArrivalLocation).Return(loc3).Repeat.AtLeastOnce();
			schedule.Expect(s => s[1]).Return(mov2).Repeat.Any();
			ICarrierMovement mov3 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			//mov3.Expect(m => m.DepartureLocation).Return(loc3).Repeat.Any();
			mov3.Expect(m => m.ArrivalLocation).Return(loc4).Repeat.AtLeastOnce();
			schedule.Expect(s => s[2]).Return(mov3).Repeat.Any();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			UnLocode outLocation = new UnLocode("LCOUT");
			location.Expect(l => l.UnLocode).Return(outLocation).Repeat.AtLeastOnce();
	
			// act:
			MovingVoyage state = new MovingVoyage(number, schedule, 0);
			bool willStopOverAtLocation = state.WillStopOverAt(location);
		
			// assert:
			Assert.IsFalse(willStopOverAtLocation);
			location.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
			mov1.VerifyAllExpectations();
			mov2.VerifyAllExpectations();
			mov3.VerifyAllExpectations();
		}
		
		[Test]
		public void WillStopOverAt_05()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			UnLocode loc2 = new UnLocode("ARLCA");
			UnLocode loc3 = new UnLocode("ARLCB");
			UnLocode loc4 = new UnLocode("ARLCC");
			ICarrierMovement mov2 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			mov2.Expect(m => m.DepartureLocation).Return(loc2).Repeat.Any();
			mov2.Expect(m => m.ArrivalLocation).Return(loc3).Repeat.AtLeastOnce();
			schedule.Expect(s => s[1]).Return(mov2).Repeat.Any();
			ICarrierMovement mov3 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			//mov3.Expect(m => m.DepartureLocation).Return(loc3).Repeat.Any();
			mov3.Expect(m => m.ArrivalLocation).Return(loc4).Repeat.AtLeastOnce();
			schedule.Expect(s => s[2]).Return(mov3).Repeat.Any();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			UnLocode outLocation = new UnLocode("LCOUT");
			location.Expect(l => l.UnLocode).Return(outLocation).Repeat.AtLeastOnce();
	
			// act:
			MovingVoyage state = new MovingVoyage(number, schedule, 1);
			bool willStopOverAtLocation = state.WillStopOverAt(location);
		
			// assert:
			Assert.IsFalse(willStopOverAtLocation);
			location.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
			mov2.VerifyAllExpectations();
			mov3.VerifyAllExpectations();
		}
				
		[Test]
		public void WillStopOverAt_06()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			UnLocode loc3 = new UnLocode("ARLCB");
			UnLocode loc4 = new UnLocode("ARLCC");
			ICarrierMovement mov3 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			mov3.Expect(m => m.DepartureLocation).Return(loc3).Repeat.Any();
			mov3.Expect(m => m.ArrivalLocation).Return(loc4).Repeat.AtLeastOnce();
			schedule.Expect(s => s[2]).Return(mov3).Repeat.Any();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			UnLocode outLocation = new UnLocode("LCOUT");
			location.Expect(l => l.UnLocode).Return(outLocation).Repeat.AtLeastOnce();
	
			// act:
			MovingVoyage state = new MovingVoyage(number, schedule, 2);
			bool willStopOverAtLocation = state.WillStopOverAt(location);
		
			// assert:
			Assert.IsFalse(willStopOverAtLocation);
			location.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
			mov3.VerifyAllExpectations();
		}
		
		[Test]
		public void WillStopOverAt_07()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			UnLocode loc1 = new UnLocode("DPLOC");
			UnLocode loc2 = new UnLocode("ARLCA");
			UnLocode loc3 = new UnLocode("ARLCB");
			UnLocode loc4 = new UnLocode("ARLCC");
			ICarrierMovement mov2 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			mov2.Expect(m => m.DepartureLocation).Return(loc2).Repeat.Any();
			mov2.Expect(m => m.ArrivalLocation).Return(loc3).Repeat.AtLeastOnce();
			schedule.Expect(s => s[1]).Return(mov2).Repeat.Any();
			ICarrierMovement mov3 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			//mov3.Expect(m => m.DepartureLocation).Return(loc3).Repeat.Any();
			mov3.Expect(m => m.ArrivalLocation).Return(loc4).Repeat.AtLeastOnce();
			schedule.Expect(s => s[2]).Return(mov3).Repeat.Any();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(loc1).Repeat.AtLeastOnce();
	
			// act:
			MovingVoyage state = new MovingVoyage(number, schedule, 1);
			bool willStopOverAtLocation = state.WillStopOverAt(location);
		
			// assert:
			Assert.IsFalse(willStopOverAtLocation);
			location.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
			mov2.VerifyAllExpectations();
			mov3.VerifyAllExpectations();
		}
		
		[Test]
		public void WillStopOverAt_08()
		{
			// arrange:
			VoyageNumber number = new VoyageNumber("VYGTEST01");
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			schedule.Expect(s => s.MovementsCount).Return(3).Repeat.Any();
			UnLocode loc1 = new UnLocode("DPLOC");
			UnLocode loc2 = new UnLocode("ARLCA");
			UnLocode loc3 = new UnLocode("ARLCB");
			UnLocode loc4 = new UnLocode("ARLCC");
			ICarrierMovement mov1 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			mov1.Expect(m => m.ArrivalLocation).Return(loc2).Repeat.AtLeastOnce();
			schedule.Expect(s => s[0]).Return(mov1).Repeat.Any();
			ICarrierMovement mov2 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			//mov2.Expect(m => m.DepartureLocation).Return(loc2).Repeat.Any();
			mov2.Expect(m => m.ArrivalLocation).Return(loc3).Repeat.AtLeastOnce();
			schedule.Expect(s => s[1]).Return(mov2).Repeat.Any();
			ICarrierMovement mov3 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			//mov3.Expect(m => m.DepartureLocation).Return(loc3).Repeat.Any();
			mov3.Expect(m => m.ArrivalLocation).Return(loc4).Repeat.AtLeastOnce();
			schedule.Expect(s => s[2]).Return(mov3).Repeat.Any();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(loc1).Repeat.AtLeastOnce();
	
			// act:
			MovingVoyage state = new MovingVoyage(number, schedule, 0);
			bool willStopOverAtLocation = state.WillStopOverAt(location);
		
			// assert:
			Assert.IsFalse(willStopOverAtLocation);
			location.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
			mov1.VerifyAllExpectations();
			mov2.VerifyAllExpectations();
			mov3.VerifyAllExpectations();
		}
	}
}

