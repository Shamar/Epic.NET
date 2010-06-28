//  
//  JustCreatedVoyageTester.cs
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
using Challenge00.DDDSample.Voyage;
using Rhino.Mocks;
using Challenge00.DDDSample.Location;
namespace DefaultImplementation.Voyage
{
	[TestFixture()]
	public class AtPortVoyageTester
	{
		[Test]
		public void Test_Ctor_01()
		{
			// arrange:
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
		
			// act:
			AtPortVoyage state = new AtPortVoyage(schedule, 0);
		
			// assert:
			Assert.AreSame(schedule, state.Schedule);
			Assert.IsFalse(state.IsLost);
			Assert.IsFalse(state.IsMoving);
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Ctor_02 ()
		{
			// arrange:
			
		
			// act:
			new AtPortVoyage(null, 0);
		}
		
		[Test]
		public void Test_LastKnownLocation_01()
		{
			// arrange:
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			UnLocode initialLocation = new UnLocode("DPLOC");
			ICarrierMovement movement = MockRepository.GenerateStrictMock<ICarrierMovement>();
			movement.Expect(m => m.DepartureLocation).Return(initialLocation).Repeat.Once();
			schedule.Expect(s => s[0]).Return(movement).Repeat.Once();
		
			// act:
			AtPortVoyage state = new AtPortVoyage(schedule, 0);
		
			// assert:
			Assert.AreSame(initialLocation, state.LastKnownLocation);
			movement.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Arrive_01()
		{
			// arrange:
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			UnLocode initialLocation = new UnLocode("DPLOC");
			ICarrierMovement movement = MockRepository.GenerateStrictMock<ICarrierMovement>();
			movement.Expect(m => m.DepartureLocation).Return(initialLocation).Repeat.Once();
			schedule.Expect(s => s[0]).Return(movement).Repeat.Once();
			
			// act:
			AtPortVoyage state = new AtPortVoyage(schedule, 0);
			VoyageState arrived = state.Arrive();
		
			// assert:
			Assert.AreSame(state, arrived);
			movement.VerifyAllExpectations();
			schedule.VerifyAllExpectations();
		}
	}
}

