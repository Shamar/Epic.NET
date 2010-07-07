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
using System.Linq;
using System.Collections.Generic;
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
			Assert.AreEqual(1, tested.Count());
			Assert.AreSame(leg, tested.First());
			Assert.AreSame(leg, tested.Last());
			Assert.AreEqual(loc1, tested.InitialDepartureLocation);
			Assert.AreEqual(loc2, tested.FinalArrivalLocation);
			leg.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Append_02()
		{
			// arrange:
			Itinerary empty = new Itinerary();
		
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { empty.Append(null); } );
		}
		
		[Test]
		public void Test_Append_03()
		{
			// arrange:
			UnLocode loc1 = new UnLocode("CODAA");
			UnLocode loc2 = new UnLocode("CODAB");
			UnLocode loc3 = new UnLocode("CODAC");
			ILeg leg = MockRepository.GenerateStrictMock<ILeg>();
			leg.Expect(l => l.LoadLocation).Return(loc1).Repeat.Once();
			leg.Expect(l => l.UnloadLocation).Return(loc2).Repeat.Once();
			leg.Expect(l => l.UnloadTime).Return(DateTime.Now).Repeat.Once();
			ILeg leg2 = MockRepository.GenerateStrictMock<ILeg>();
			leg2.Expect(l => l.LoadLocation).Return(loc2).Repeat.Once();
			leg2.Expect(l => l.UnloadLocation).Return(loc3).Repeat.Once();
			leg2.Expect(l => l.LoadTime).Return(DateTime.Now + TimeSpan.FromDays(1)).Repeat.Once();
			IItinerary initial = new Itinerary();
			initial = initial.Append(leg);
		
			// act:
			IItinerary tested = initial.Append(leg2);
		
			// assert:
			Assert.IsNotNull(tested);
			Assert.AreEqual(2, tested.Count());
			Assert.AreSame(leg, tested.First());
			Assert.AreSame(leg2, tested.Last());
			Assert.AreEqual(loc1, tested.InitialDepartureLocation);
			Assert.AreEqual(loc3, tested.FinalArrivalLocation);
			leg.VerifyAllExpectations();
			leg2.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Append_04()
		{
			// arrange:
			UnLocode loc1 = new UnLocode("CODAA");
			UnLocode loc2 = new UnLocode("CODAB");
			UnLocode loc3 = new UnLocode("CDOUT");
			UnLocode loc4 = new UnLocode("CODAC");
			ILeg leg = MockRepository.GenerateStrictMock<ILeg>();
			leg.Expect(l => l.UnloadLocation).Return(loc2).Repeat.Once();
			ILeg leg2 = MockRepository.GenerateStrictMock<ILeg>();
			leg2.Expect(l => l.LoadLocation).Return(loc3).Repeat.Once();
			IItinerary initial = new Itinerary();
			initial = initial.Append(leg);
			
			// assert:
			Assert.Throws<ArgumentException>(delegate { initial.Append(leg2); });
			leg.VerifyAllExpectations();
			leg2.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Append_05()
		{
			// arrange:
			UnLocode loc1 = new UnLocode("CODAA");
			UnLocode loc2 = new UnLocode("CODAB");
			UnLocode loc3 = new UnLocode("CODAC");
			ILeg leg = MockRepository.GenerateStrictMock<ILeg>();
			leg.Expect(l => l.LoadLocation).Return(loc1).Repeat.Any();
			leg.Expect(l => l.UnloadLocation).Return(loc2).Repeat.Once();
			leg.Expect(l => l.UnloadTime).Return(DateTime.Now).Repeat.Once();
			ILeg leg2 = MockRepository.GenerateStrictMock<ILeg>();
			leg2.Expect(l => l.LoadLocation).Return(loc2).Repeat.Any();
			leg2.Expect(l => l.LoadTime).Return(DateTime.Now - TimeSpan.FromDays(1)).Repeat.Once();
			IItinerary initial = new Itinerary();
			initial = initial.Append(leg);
		
			// assert:
			Assert.Throws<ArgumentException>(delegate { initial.Append(leg2); });
			leg.VerifyAllExpectations();
			leg2.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Equals_01()
		{
			// arrange:
			UnLocode loc1 = new UnLocode("CODAA");
			UnLocode loc2 = new UnLocode("CODAB");
			UnLocode loc3 = new UnLocode("CODAC");
			ILeg leg = MockRepository.GenerateStrictMock<ILeg>();
			leg.Expect(l => l.LoadLocation).Return(loc1).Repeat.Once();
			leg.Expect(l => l.UnloadLocation).Return(loc2).Repeat.Once();
			leg.Expect(l => l.UnloadTime).Return(DateTime.Now).Repeat.Once();
			ILeg leg2 = MockRepository.GenerateStrictMock<ILeg>();
			leg2.Expect(l => l.LoadLocation).Return(loc2).Repeat.Once();
			leg2.Expect(l => l.UnloadLocation).Return(loc3).Repeat.Once();
			leg2.Expect(l => l.LoadTime).Return(DateTime.Now + TimeSpan.FromDays(1)).Repeat.Once();
			
		
			// act:
			IItinerary tested = new Itinerary();
			tested = tested.Append(leg);
			tested = tested.Append(leg2);
		
		
			// assert:
			Assert.IsFalse(tested.Equals(null));
			Assert.IsTrue(tested.Equals(tested));
		}
		
		[Test]
		public void Test_Equals_02()
		{
			// arrange:
			UnLocode loc1 = new UnLocode("CODAA");
			UnLocode loc2 = new UnLocode("CODAB");
			UnLocode loc3 = new UnLocode("CODAC");
			List<ILeg> legs = new List<ILeg>();
			ILeg legCopy = MockRepository.GenerateStrictMock<ILeg>();
			legs.Add(legCopy);
			ILeg leg2Copy = MockRepository.GenerateStrictMock<ILeg>();
			legs.Add(leg2Copy);
			ILeg leg = MockRepository.GenerateStrictMock<ILeg>();
			leg.Expect(l => l.Equals(legCopy)).Return(true).Repeat.Once();
			leg.Expect(l => l.LoadLocation).Return(loc1).Repeat.Once();
			leg.Expect(l => l.UnloadLocation).Return(loc2).Repeat.Once();
			leg.Expect(l => l.UnloadTime).Return(DateTime.Now).Repeat.Once();
			ILeg leg2 = MockRepository.GenerateStrictMock<ILeg>();
			leg2.Expect(l => l.Equals(leg2Copy)).Return(true).Repeat.Once();
			leg2.Expect(l => l.LoadLocation).Return(loc2).Repeat.Once();
			leg2.Expect(l => l.UnloadLocation).Return(loc3).Repeat.Once();
			leg2.Expect(l => l.LoadTime).Return(DateTime.Now + TimeSpan.FromDays(1)).Repeat.Once();
			IItinerary candidate = MockRepository.GenerateStrictMock<IItinerary>();
			candidate.Expect(c => c.InitialDepartureLocation).Return(loc1).Repeat.Once();
			candidate.Expect(c => c.FinalArrivalLocation).Return(loc3).Repeat.Once();
			candidate.Expect(c => c.GetEnumerator()).Return(legs.GetEnumerator()).Repeat.Once();
		
			// act:
			IItinerary tested = new Itinerary();
			tested = tested.Append(leg);
			tested = tested.Append(leg2);
		
			// assert:
			Assert.IsTrue(tested.Equals(candidate));
			candidate.VerifyAllExpectations();
			leg.VerifyAllExpectations();
			leg2.VerifyAllExpectations();
			foreach(ILeg l in legs)
			{
				l.VerifyAllExpectations();
			}
		}
	}
}

