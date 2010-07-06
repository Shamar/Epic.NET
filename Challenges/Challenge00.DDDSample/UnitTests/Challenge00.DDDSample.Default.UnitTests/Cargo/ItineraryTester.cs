//  
//  ItineraryTester.cs
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
using Challenge00.DDDSample.Cargo;
using Rhino.Mocks;
using Challenge00.DDDSample.Location;
namespace DefaultImplementation.Cargo
{
	[TestFixture()]
	public class ItineraryTester
	{
		[Test]
		public void Test_Ctor_01()
		{
			// arrange:
		
			// act:
			IItinerary itinerary = new Itinerary();
		
			// assert:
			Assert.IsNull(itinerary.InitialDepartureLocation);
			Assert.IsNull(itinerary.FinalArrivalLocation);
		}
		
		[Test]
		public void Test_Append_01()
		{
			// arrange:
			UnLocode loc1 = new UnLocode("CODLD");
			UnLocode loc2 = new UnLocode("CODUN");
			ILeg leg = MockRepository.GenerateStrictMock<ILeg>();
			leg.Expect(l => l.LoadLocation).Return(loc1).Repeat.Once();
			leg.Expect(l => l.UnloadLocation).Return(loc2).Repeat.Once();
			Itinerary empty = new Itinerary();
		
			// act:
			IItinerary tested = empty.Append(leg);
		
			// assert:
			Assert.IsNotNull(tested);
			Assert.AreEqual(loc1, tested.InitialDepartureLocation);
			Assert.AreEqual(loc2, tested.FinalArrivalLocation);
		}
		
	}
}

