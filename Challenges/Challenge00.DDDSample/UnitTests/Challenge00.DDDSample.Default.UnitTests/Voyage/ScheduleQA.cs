//  
//  ScheduleTester.cs
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
using NUnit.Framework;
using System;
using Challenge00.DDDSample.Voyage;
using System.Linq;
using Rhino.Mocks;
using Challenge00.DDDSample.Location;
using System.Collections;
namespace DefaultImplementation.Voyage
{
	[TestFixture()]
	public class ScheduleQA
	{
		[Test]
		public void Ctor_01()
		{
			// arrange:
			
		
			// act:
			ISchedule schedule = new Schedule();
		
			// assert:
			Assert.AreEqual(0, schedule.Count());
			CollectionAssert.AreEqual(schedule, schedule.AsWeakEnumerable());
			Assert.AreEqual(0, schedule.MovementsCount);
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Append_01 ()
		{
			// arrange:
			ISchedule schedule = new Schedule();
		
			// act:
			schedule.Append(null);
		}
		
		[Test]
		public void Append_02()
		{
			// arrange:
			ICarrierMovement m1 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			ISchedule empty = new Schedule();
		
			// act:
			ISchedule schedule = empty.Append(m1);
		
			// assert:
			Assert.IsFalse(schedule.Equals(empty));
			Assert.IsTrue(schedule.Equals(schedule));
			Assert.AreSame(m1, schedule[0]);
			Assert.AreEqual(1, schedule.Count());			
			Assert.AreEqual(1, schedule.MovementsCount);
		}
		
		[Test]
		public void Append_03()
		{
			// arrange:
			UnLocode c1 = new UnLocode("LOCDA");
			UnLocode c2 = new UnLocode("LOCDA");
			
			ICarrierMovement m1 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			m1.Expect(m => m.ArrivalLocation).Return(c1).Repeat.Any();
			m1.Expect(m => m.ArrivalTime).Return(DateTime.UtcNow + new TimeSpan(48, 0, 0)).Repeat.Any();
			ICarrierMovement m2 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			m2.Expect(m => m.DepartureLocation).Return(c2).Repeat.Any();
			m2.Expect(m => m.DepartureTime).Return(DateTime.UtcNow + new TimeSpan(72, 0, 0)).Repeat.Any();
			ISchedule empty = new Schedule();
			ISchedule schedule1 = empty.Append(m1);
		
			// act:
			ISchedule schedule2 = schedule1.Append(m2);
		
			// assert:
			Assert.IsFalse(schedule2.Equals(empty));
			Assert.IsFalse(schedule2.Equals(schedule1));
			Assert.AreSame(m1, schedule2[0]);
			Assert.AreSame(m2, schedule2[1]);
			Assert.AreEqual(2, schedule2.Count());			
			Assert.AreEqual(2, schedule2.MovementsCount);
			m1.VerifyAllExpectations();
			m2.VerifyAllExpectations();
		}
		
		[Test]
		[ExpectedException(typeof(WrongLocationException))]
		public void Append_04()
		{
			// arrange:
			UnLocode c1 = new UnLocode("LOCDA");
			UnLocode c2 = new UnLocode("LOCDB");
			
			ICarrierMovement m1 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			m1.Expect(m => m.ArrivalLocation).Return(c1).Repeat.Any();
			m1.Expect(m => m.ArrivalTime).Return(DateTime.UtcNow + new TimeSpan(48, 0, 0)).Repeat.Any();
			ICarrierMovement m2 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			m2.Expect(m => m.DepartureLocation).Return(c2).Repeat.Any();
			m2.Expect(m => m.DepartureTime).Return(DateTime.UtcNow + new TimeSpan(72, 0, 0)).Repeat.Any();
			ISchedule empty = new Schedule();
			ISchedule schedule1 = empty.Append(m1);
		
			// act:
			schedule1.Append(m2);
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Append_05()
		{
			// arrange:
			UnLocode c1 = new UnLocode("LOCDA");
			
			ICarrierMovement m1 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			m1.Expect(m => m.ArrivalLocation).Return(c1).Repeat.Any();
			m1.Expect(m => m.ArrivalTime).Return(DateTime.UtcNow + new TimeSpan(48, 0, 0)).Repeat.Any();
			ICarrierMovement m2 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			m2.Expect(m => m.DepartureLocation).Return(c1).Repeat.Any();
			m2.Expect(m => m.DepartureTime).Return(DateTime.UtcNow).Repeat.Any();
			ISchedule empty = new Schedule();
			ISchedule schedule1 = empty.Append(m1);
		
			// act:
			schedule1.Append(m2);
		}
		
		[Test]
		public void Equals_01()
		{
			// arrange:
			ISchedule schedule1 = new Schedule();
			ISchedule schedule2 = new Schedule();
		
			// act:
			bool equals = schedule1.Equals(schedule2);
		
			// assert:
			Assert.IsTrue(equals);
			Assert.AreEqual(schedule1.GetHashCode(), schedule2.GetHashCode());
		}
		
		[Test]
		public void Equals_02()
		{
			// arrange:
			ISchedule schedule1 = new Schedule();
		
			// act:
			bool equals = schedule1.Equals(null);
		
			// assert:
			Assert.IsFalse(equals);
		}
		
		[Test]
		public void Equals_03()
		{
			// arrange:
			ISchedule schedule1 = new Schedule();
			ISchedule schedule2 = new Schedule();
			
			// act:
			bool equals = schedule1.Equals((object)schedule2);
		
			// assert:
			Assert.IsTrue(equals);
			Assert.AreEqual(schedule1.GetHashCode(), schedule2.GetHashCode());
		}
		
		[Test]
		public void Equals_04()
		{
			// arrange:
			ICarrierMovement m1 = MockRepository.GenerateStrictMock<ICarrierMovement, IObject>();
			ICarrierMovement m2 = MockRepository.GenerateStrictMock<ICarrierMovement, IObject>();
			m1.Expect(m => m.Equals(m2)).Return(true).Repeat.AtLeastOnce();
			m2.Expect(m => m.Equals(m1)).Return(true).Repeat.AtLeastOnce();
			m1.Expect(m => m.GetHashCode()).Return(543210).Repeat.AtLeastOnce();
			m2.Expect(m => m.GetHashCode()).Return(543210).Repeat.AtLeastOnce();
			
			ISchedule empty = new Schedule();
			ISchedule schedule1 = empty.Append(m1);
			ISchedule schedule2 = empty.Append(m2);
		
			// act:
			bool equals1 = schedule1.Equals(schedule2);
			bool equals2 = schedule2.Equals(schedule1);
		
			// assert:
			Assert.IsTrue(equals1);
			Assert.IsTrue(equals2);
			Assert.AreEqual(schedule1.GetHashCode(), schedule2.GetHashCode());
			m1.VerifyAllExpectations();
			m2.VerifyAllExpectations();
		}
		
		[Test]
		public void Equals_05()
		{
			// arrange:
			ICarrierMovement m1 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			ICarrierMovement m2 = MockRepository.GenerateStrictMock<ICarrierMovement>();
			m1.Expect(m => m.Equals(m2)).Return(false).Repeat.AtLeastOnce();
			m2.Expect(m => m.Equals(m1)).Return(false).Repeat.AtLeastOnce();
			
			ISchedule empty = new Schedule();
			ISchedule schedule1 = empty.Append(m1);
			ISchedule schedule2 = empty.Append(m2);
		
			// act:
			bool equals1 = schedule1.Equals(schedule2);
			bool equals2 = schedule2.Equals(schedule1);
		
			// assert:
			Assert.IsFalse(equals1);
			Assert.IsFalse(equals2);
			m1.VerifyAllExpectations();
			m2.VerifyAllExpectations();
		}
	}
}

