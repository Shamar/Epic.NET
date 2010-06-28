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
			ISchedule schedule = MockRepository.GenerateStrictMock<ISchedule>();
			
			// act:
			IVoyage voyage = new Challenge00.DDDSample.Voyage.Voyage(number, schedule);
			
			// assert:
			Assert.AreEqual(number, voyage.Number);
			Assert.AreSame(schedule, voyage.Schedule);
			Assert.AreEqual(false, voyage.IsLost);
			Assert.AreSame(false, voyage.IsMoving);
		}
	}
}

