//  
//  ItineraryTester.cs
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
using Challenge00.DDDSample.Cargo;
using Rhino.Mocks;
using Challenge00.DDDSample.Location;
using System.Linq;
using System.Collections.Generic;
namespace DefaultImplementation.Cargo
{
	[TestFixture()]
	public class ItineraryQA
	{
		[Test]
		public void Ctor_01()
		{
			// arrange:
		
			// act:
			IItinerary itinerary = new Itinerary();
			IItinerary itinerary2 = new Itinerary();
		
			// assert:
			Assert.IsNull(itinerary.InitialDepartureLocation);
			Assert.IsNull(itinerary.FinalArrivalLocation);
			Assert.AreEqual(DateTime.MaxValue, itinerary.FinalArrivalDate);
			Assert.IsTrue(itinerary.Equals(itinerary));
			Assert.IsTrue(itinerary.Equals(itinerary2));
			Assert.IsFalse(itinerary.Equals(null));
			Assert.IsTrue(itinerary2.Equals(itinerary));
			Assert.AreEqual(itinerary.GetHashCode(), itinerary2.GetHashCode());
			CollectionAssert.AreEqual(itinerary, itinerary.AsWeakEnumerable());
		}
		
		[Test]
		public void Append_01()
		{
			// arrange:
			UnLocode loc1 = new UnLocode("CODLD");
			UnLocode loc2 = new UnLocode("CODUN");
			DateTime arrivalDate = DateTime.UtcNow + TimeSpan.FromDays(10);
			ILeg leg = MockRepository.GenerateStrictMock<ILeg>();
			leg.Expect(l => l.LoadLocation).Return(loc1).Repeat.Once();
			leg.Expect(l => l.UnloadLocation).Return(loc2).Repeat.Once();
			leg.Expect(l => l.UnloadTime).Return(arrivalDate).Repeat.Once();
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
			Assert.AreEqual(arrivalDate, tested.FinalArrivalDate);
			leg.VerifyAllExpectations();
		}
		
		[Test]
		public void Append_02()
		{
			// arrange:
			Itinerary empty = new Itinerary();
		
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { empty.Append(null); } );
		}
		
		[Test]
		public void Append_03()
		{
			// arrange:
			UnLocode loc1 = new UnLocode("CODAA");
			UnLocode loc2 = new UnLocode("CODAB");
			UnLocode loc3 = new UnLocode("CODAC");
			DateTime arrivalDate = DateTime.UtcNow + TimeSpan.FromDays(2);
			ILeg leg = MockRepository.GenerateStrictMock<ILeg>();
			leg.Expect(l => l.LoadLocation).Return(loc1).Repeat.Once();
			leg.Expect(l => l.UnloadLocation).Return(loc2).Repeat.Once();
			leg.Expect(l => l.UnloadTime).Return(DateTime.UtcNow).Repeat.Once();
			ILeg leg2 = MockRepository.GenerateStrictMock<ILeg>();
			leg2.Expect(l => l.LoadLocation).Return(loc2).Repeat.Once();
			leg2.Expect(l => l.UnloadLocation).Return(loc3).Repeat.Once();
			leg2.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Once();
			leg2.Expect(l => l.UnloadTime).Return(arrivalDate).Repeat.Once();
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
			Assert.AreEqual(arrivalDate, tested.FinalArrivalDate);
			leg.VerifyAllExpectations();
			leg2.VerifyAllExpectations();
		}
		
		[Test]
		public void Append_04()
		{
			// arrange:
			UnLocode loc2 = new UnLocode("CODAB");
			UnLocode loc3 = new UnLocode("CDOUT");
			ILeg leg = MockRepository.GenerateStrictMock<ILeg>();
			leg.Expect(l => l.UnloadLocation).Return(loc2).Repeat.AtLeastOnce();
			ILeg leg2 = MockRepository.GenerateStrictMock<ILeg>();
			leg2.Expect(l => l.LoadLocation).Return(loc3).Repeat.AtLeastOnce();
			IItinerary initial = new Itinerary();
			initial = initial.Append(leg);
			
			// assert:
			Assert.Throws<WrongLocationException>(delegate { initial.Append(leg2); });
			leg.VerifyAllExpectations();
			leg2.VerifyAllExpectations();
		}
		
		[Test]
		public void Append_05()
		{
			// arrange:
			UnLocode loc1 = new UnLocode("CODAA");
			UnLocode loc2 = new UnLocode("CODAB");
			ILeg leg = MockRepository.GenerateStrictMock<ILeg>();
			leg.Expect(l => l.LoadLocation).Return(loc1).Repeat.Any();
			leg.Expect(l => l.UnloadLocation).Return(loc2).Repeat.Once();
			leg.Expect(l => l.UnloadTime).Return(DateTime.UtcNow).Repeat.Once();
			ILeg leg2 = MockRepository.GenerateStrictMock<ILeg>();
			leg2.Expect(l => l.LoadLocation).Return(loc2).Repeat.Any();
			leg2.Expect(l => l.LoadTime).Return(DateTime.UtcNow - TimeSpan.FromDays(1)).Repeat.Once();
			IItinerary initial = new Itinerary();
			initial = initial.Append(leg);
		
			// assert:
			Assert.Throws<ArgumentException>(delegate { initial.Append(leg2); });
			leg.VerifyAllExpectations();
			leg2.VerifyAllExpectations();
		}
		
		[Test]
		public void Equals_01()
		{
			// arrange:
			UnLocode loc1 = new UnLocode("CODAA");
			UnLocode loc2 = new UnLocode("CODAB");
			UnLocode loc3 = new UnLocode("CODAC");
			ILeg leg = MockRepository.GenerateStrictMock<ILeg>();
			leg.Expect(l => l.LoadLocation).Return(loc1).Repeat.Once();
			leg.Expect(l => l.UnloadLocation).Return(loc2).Repeat.Once();
			leg.Expect(l => l.UnloadTime).Return(DateTime.UtcNow).Repeat.Once();
			ILeg leg2 = MockRepository.GenerateStrictMock<ILeg>();
			leg2.Expect(l => l.LoadLocation).Return(loc2).Repeat.Once();
			leg2.Expect(l => l.UnloadLocation).Return(loc3).Repeat.Once();
			leg2.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Once();
			
		
			// act:
			IItinerary tested = new Itinerary();
			tested = tested.Append(leg);
			tested = tested.Append(leg2);
		
		
			// assert:
			Assert.IsFalse(tested.Equals(null));
			Assert.IsTrue(tested.Equals(tested));
			Assert.IsTrue(tested.Equals((object)tested));
		}
		
		[Test]
		public void Equals_02()
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
			ILeg leg = MockRepository.GenerateStrictMock<ILeg, IObject>();
			leg.Expect(l => l.Equals(leg)).Return(true).Repeat.AtLeastOnce();
			leg.Expect(l => l.Equals(legCopy)).Return(true).Repeat.AtLeastOnce();
			leg.Expect(l => l.LoadLocation).Return(loc1).Repeat.AtLeastOnce();
			leg.Expect(l => l.UnloadLocation).Return(loc2).Repeat.AtLeastOnce();
			leg.Expect(l => l.UnloadTime).Return(DateTime.UtcNow).Repeat.AtLeastOnce();
			leg.Expect(l => l.GetHashCode()).Return(543210).Repeat.AtLeastOnce();
			ILeg leg2 = MockRepository.GenerateStrictMock<ILeg, IObject>();
			leg2.Expect(l => l.Equals(leg2)).Return(true).Repeat.AtLeastOnce();
			leg2.Expect(l => l.Equals(leg2Copy)).Return(true).Repeat.AtLeastOnce();
			leg2.Expect(l => l.LoadLocation).Return(loc2).Repeat.AtLeastOnce();
			leg2.Expect(l => l.UnloadLocation).Return(loc3).Repeat.AtLeastOnce();
			leg2.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.AtLeastOnce();
			IItinerary candidate = MockRepository.GenerateStrictMock<IItinerary>();
			candidate.Expect(c => c.InitialDepartureLocation).Return(loc1).Repeat.AtLeastOnce();
			candidate.Expect(c => c.FinalArrivalLocation).Return(loc3).Repeat.AtLeastOnce();
			candidate.Expect(c => c.GetEnumerator()).Return(legs.GetEnumerator()).Repeat.Once();
			candidate.Expect(c => c.GetEnumerator()).Return(legs.GetEnumerator()).Repeat.Once();
			candidate.Expect(c => c.GetEnumerator()).Return(legs.GetEnumerator()).Repeat.Once();
			candidate.Expect(c => c.GetEnumerator()).Return(legs.GetEnumerator()).Repeat.Once();
		
			// act:
			IItinerary tested = new Itinerary();
			tested = tested.Append(leg);
			tested = tested.Append(leg2);
			IItinerary tested2 = new Itinerary();
			tested2 = tested2.Append(leg);
			tested2 = tested2.Append(leg2);
		
			// assert:
			Assert.IsTrue(tested.Equals(candidate));
			Assert.IsTrue(tested.Equals((object)candidate));
			Assert.IsTrue(tested.Equals(tested2));
			Assert.IsTrue(tested.Equals((object)tested2));
			Assert.IsTrue(tested2.Equals(tested));
			Assert.IsTrue(tested2.Equals((object)tested));
			Assert.AreEqual(tested.GetHashCode(), tested2.GetHashCode());
			CollectionAssert.AreEqual(tested, tested.AsWeakEnumerable());
			CollectionAssert.AreEqual(tested, tested2);
			candidate.VerifyAllExpectations();
			leg.VerifyAllExpectations();
			leg2.VerifyAllExpectations();
			foreach(ILeg l in legs)
			{
				l.VerifyAllExpectations();
			}
		}
		
		[Test]
		public void Equals_03()
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
			leg.Expect(l => l.UnloadTime).Return(DateTime.UtcNow).Repeat.Once();
			ILeg leg2 = MockRepository.GenerateStrictMock<ILeg>();
			leg2.Expect(l => l.Equals(leg2Copy)).Return(false).Repeat.Once();
			leg2.Expect(l => l.LoadLocation).Return(loc2).Repeat.Once();
			leg2.Expect(l => l.UnloadLocation).Return(loc3).Repeat.Once();
			leg2.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Once();
			IItinerary candidate = MockRepository.GenerateStrictMock<IItinerary>();
			candidate.Expect(c => c.InitialDepartureLocation).Return(loc1).Repeat.Once();
			candidate.Expect(c => c.FinalArrivalLocation).Return(loc3).Repeat.Once();
			candidate.Expect(c => c.GetEnumerator()).Return(legs.GetEnumerator()).Repeat.Once();
			candidate.Expect(c => c.GetEnumerator()).Return(legs.GetEnumerator()).Repeat.Once();
		
			// act:
			IItinerary tested = new Itinerary();
			tested = tested.Append(leg);
			tested = tested.Append(leg2);
		
			// assert:
			Assert.IsFalse(tested.Equals(candidate));
			candidate.VerifyAllExpectations();
			leg.VerifyAllExpectations();
			leg2.VerifyAllExpectations();
			foreach(ILeg l in legs)
			{
				l.VerifyAllExpectations();
			}
		}
		
		[Test]
		public void Equals_04()
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
			leg.Expect(l => l.Equals(legCopy)).Return(false).Repeat.Once();
			leg.Expect(l => l.LoadLocation).Return(loc1).Repeat.Once();
			leg.Expect(l => l.UnloadLocation).Return(loc2).Repeat.Once();
			leg.Expect(l => l.UnloadTime).Return(DateTime.UtcNow).Repeat.Once();
			ILeg leg2 = MockRepository.GenerateStrictMock<ILeg>();
			leg2.Expect(l => l.Equals(leg2Copy)).Return(true).Repeat.Any();
			leg2.Expect(l => l.LoadLocation).Return(loc2).Repeat.Any();
			leg2.Expect(l => l.UnloadLocation).Return(loc3).Repeat.Any();
			leg2.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			IItinerary candidate = MockRepository.GenerateStrictMock<IItinerary>();
			candidate.Expect(c => c.InitialDepartureLocation).Return(loc1).Repeat.Once();
			candidate.Expect(c => c.FinalArrivalLocation).Return(loc3).Repeat.Once();
			candidate.Expect(c => c.GetEnumerator()).Return(legs.GetEnumerator()).Repeat.Once();
			candidate.Expect(c => c.GetEnumerator()).Return(legs.GetEnumerator()).Repeat.Once();
	
			// act:
			IItinerary tested = new Itinerary();
			tested = tested.Append(leg);
			tested = tested.Append(leg2);
		
			// assert:
			Assert.IsFalse(tested.Equals(candidate));
			candidate.VerifyAllExpectations();
			leg.VerifyAllExpectations();
			leg2.VerifyAllExpectations();
			foreach(ILeg l in legs)
			{
				l.VerifyAllExpectations();
			}
		}
		
		[Test]
		public void Equals_05()
		{
			// arrange:
			UnLocode loc1 = new UnLocode("CODAA");
			UnLocode loc2 = new UnLocode("CODAB");
			UnLocode loc3 = new UnLocode("CODAC");
			UnLocode loc4 = new UnLocode("CODAD");
			List<ILeg> legs = new List<ILeg>();
			ILeg legCopy = MockRepository.GenerateStrictMock<ILeg>();
			legs.Add(legCopy);
			ILeg leg2Copy = MockRepository.GenerateStrictMock<ILeg>();
			legs.Add(leg2Copy);
			ILeg leg = MockRepository.GenerateStrictMock<ILeg>();
			leg.Expect(l => l.Equals(legCopy)).Return(true).Repeat.Any();
			leg.Expect(l => l.LoadLocation).Return(loc1).Repeat.Any();
			leg.Expect(l => l.UnloadLocation).Return(loc2).Repeat.Any();
			leg.Expect(l => l.UnloadTime).Return(DateTime.UtcNow).Repeat.Any();
			ILeg leg2 = MockRepository.GenerateStrictMock<ILeg>();
			leg2.Expect(l => l.Equals(leg2Copy)).Return(true).Repeat.Any();
			leg2.Expect(l => l.LoadLocation).Return(loc2).Repeat.Any();
			leg2.Expect(l => l.UnloadLocation).Return(loc3).Repeat.Any();
			leg2.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			IItinerary candidate = MockRepository.GenerateStrictMock<IItinerary>();
			candidate.Expect(c => c.InitialDepartureLocation).Return(loc1).Repeat.Once();
			candidate.Expect(c => c.FinalArrivalLocation).Return(loc4).Repeat.Once();
			candidate.Expect(c => c.GetEnumerator()).Return(legs.GetEnumerator()).Repeat.Any();
		
			// act:
			IItinerary tested = new Itinerary();
			tested = tested.Append(leg);
			tested = tested.Append(leg2);
		
			// assert:
			Assert.IsFalse(tested.Equals(candidate));
			candidate.VerifyAllExpectations();
			leg.VerifyAllExpectations();
			leg2.VerifyAllExpectations();
			foreach(ILeg l in legs)
			{
				l.VerifyAllExpectations();
			}
		}
		
		[Test]
		public void Equals_06()
		{
			// arrange:
			UnLocode loc1 = new UnLocode("CODAA");
			UnLocode loc2 = new UnLocode("CODAB");
			UnLocode loc3 = new UnLocode("CODAC");
			UnLocode loc4 = new UnLocode("CODAD");
			List<ILeg> legs = new List<ILeg>();
			ILeg legCopy = MockRepository.GenerateStrictMock<ILeg>();
			legs.Add(legCopy);
			ILeg leg2Copy = MockRepository.GenerateStrictMock<ILeg>();
			legs.Add(leg2Copy);
			ILeg leg = MockRepository.GenerateStrictMock<ILeg>();
			leg.Expect(l => l.Equals(legCopy)).Return(true).Repeat.Any();
			leg.Expect(l => l.LoadLocation).Return(loc1).Repeat.Any();
			leg.Expect(l => l.UnloadLocation).Return(loc2).Repeat.Any();
			leg.Expect(l => l.UnloadTime).Return(DateTime.UtcNow).Repeat.Any();
			ILeg leg2 = MockRepository.GenerateStrictMock<ILeg>();
			leg2.Expect(l => l.Equals(leg2Copy)).Return(true).Repeat.Any();
			leg2.Expect(l => l.LoadLocation).Return(loc2).Repeat.Any();
			leg2.Expect(l => l.UnloadLocation).Return(loc3).Repeat.Any();
			leg2.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			IItinerary candidate = MockRepository.GenerateStrictMock<IItinerary>();
			candidate.Expect(c => c.InitialDepartureLocation).Return(loc4).Repeat.Once();
			candidate.Expect(c => c.FinalArrivalLocation).Return(loc3).Repeat.Any();
			candidate.Expect(c => c.GetEnumerator()).Return(legs.GetEnumerator()).Repeat.Any();
		
			// act:
			IItinerary tested = new Itinerary();
			tested = tested.Append(leg);
			tested = tested.Append(leg2);
		
			// assert:
			Assert.IsFalse(tested.Equals(candidate));
			candidate.VerifyAllExpectations();
			leg.VerifyAllExpectations();
			leg2.VerifyAllExpectations();
			foreach(ILeg l in legs)
			{
				l.VerifyAllExpectations();
			}
		}
		
		[Test]
		public void Equals_07()
		{
			// arrange:
			UnLocode loc1 = new UnLocode("CODAA");
			UnLocode loc2 = new UnLocode("CODAB");
			UnLocode loc3 = new UnLocode("CODAC");
			List<ILeg> legs = new List<ILeg>();
			ILeg legCopy = MockRepository.GenerateStrictMock<ILeg>();
			legs.Add(legCopy);
			ILeg leg = MockRepository.GenerateStrictMock<ILeg>();
			leg.Expect(l => l.LoadLocation).Return(loc1).Repeat.Any();
			leg.Expect(l => l.UnloadLocation).Return(loc2).Repeat.Any();
			leg.Expect(l => l.UnloadTime).Return(DateTime.UtcNow).Repeat.Any();
			ILeg leg2 = MockRepository.GenerateStrictMock<ILeg>();
			leg2.Expect(l => l.LoadLocation).Return(loc2).Repeat.Any();
			leg2.Expect(l => l.UnloadLocation).Return(loc3).Repeat.Any();
			leg2.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			IItinerary candidate = MockRepository.GenerateStrictMock<IItinerary>();
			candidate.Expect(c => c.GetEnumerator()).Return(legs.GetEnumerator()).Repeat.Any();
		
			// act:
			IItinerary tested = new Itinerary();
			tested = tested.Append(leg);
			tested = tested.Append(leg2);
		
			// assert:
			Assert.IsFalse(tested.Equals(candidate));
			candidate.VerifyAllExpectations();
			leg.VerifyAllExpectations();
			leg2.VerifyAllExpectations();
			foreach(ILeg l in legs)
			{
				l.VerifyAllExpectations();
			}
		}
		
		[Test]
		public void ReplaceSegment_01()
		{
			// arrange:
			UnLocode loc1 = new UnLocode("CODAA");
			UnLocode loc2 = new UnLocode("CODAB");
			UnLocode loc3 = new UnLocode("CODAC");
			UnLocode loc4 = new UnLocode("CODAD");
			UnLocode loc5 = new UnLocode("CODAE");
			
			List<ILeg> newLegs = new List<ILeg>();
			ILeg newLeg1 = MockRepository.GenerateStrictMock<ILeg>();
			newLeg1.Expect(l => l.LoadLocation).Return(loc2).Repeat.AtLeastOnce();
			newLeg1.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.AtLeastOnce();
			newLeg1.Expect(l => l.UnloadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(3)).Repeat.Any();
			newLeg1.Expect(l => l.UnloadLocation).Return(loc3).Repeat.AtLeastOnce();
			newLegs.Add(newLeg1);
			ILeg newLeg2 = MockRepository.GenerateStrictMock<ILeg>();
			newLeg2.Expect(l => l.LoadLocation).Return(loc3).Repeat.AtLeastOnce();
			newLeg2.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(4)).Repeat.Any();
			newLeg2.Expect(l => l.UnloadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(6)).Repeat.Any();
			newLeg2.Expect(l => l.UnloadLocation).Return(loc4).Repeat.AtLeastOnce();
			newLegs.Add(newLeg2);
			ILeg newLeg3 = MockRepository.GenerateStrictMock<ILeg>();
			newLeg3.Expect(l => l.LoadLocation).Return(loc4).Repeat.AtLeastOnce();
			newLeg3.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(10)).Repeat.Any();
			newLeg3.Expect(l => l.UnloadLocation).Return(loc5).Repeat.AtLeastOnce();
			newLeg3.Expect(l => l.UnloadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(13)).Repeat.Any();
			newLegs.Add(newLeg3);
			IItinerary newSegment = MockRepository.GenerateStrictMock<IItinerary>();
			newSegment.Expect(s => s.GetEnumerator()).Return(newLegs.GetEnumerator()).Repeat.AtLeastOnce();
			newSegment.Expect(s => s.InitialDepartureLocation).Return(loc2).Repeat.AtLeastOnce();
			newSegment.Expect(s => s.FinalArrivalLocation).Return(loc5).Repeat.Any();
			
			ILeg leg = MockRepository.GenerateStrictMock<ILeg>();
			leg.Expect(l => l.LoadLocation).Return(loc1).Repeat.Any();
			leg.Expect(l => l.UnloadLocation).Return(loc2).Repeat.Any();
			leg.Expect(l => l.UnloadTime).Return(DateTime.UtcNow).Repeat.Any();
			ILeg leg2 = MockRepository.GenerateStrictMock<ILeg>();
			leg2.Expect(l => l.LoadLocation).Return(loc2).Repeat.Any();
			leg2.Expect(l => l.UnloadLocation).Return(loc3).Repeat.Any();
			leg2.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			IItinerary tested = new Itinerary();
			tested = tested.Append(leg);
			tested = tested.Append(leg2);
		
			// act:
			IItinerary replaced = tested.ReplaceSegment(newSegment);
		
			// assert:
			Assert.IsNotNull(replaced);
			Assert.AreEqual(4, replaced.Count());
			leg.VerifyAllExpectations();
			leg2.VerifyAllExpectations();
			foreach(ILeg l in newLegs)
			{
				l.VerifyAllExpectations();
			}
		}
		
		[Test]
		public void ReplaceSegment_02()
		{
			// arrange:
			UnLocode loc1 = new UnLocode("CODAA");
			UnLocode loc2 = new UnLocode("CODAB");
			UnLocode loc3 = new UnLocode("CODAC");
			UnLocode loc4 = new UnLocode("CODAD");
			UnLocode loc5 = new UnLocode("CODAE");
			UnLocode loc6 = new UnLocode("CODAF");
			
			List<ILeg> newLegs = new List<ILeg>();
			ILeg newLeg1 = MockRepository.GenerateStrictMock<ILeg>();
			newLeg1.Expect(l => l.LoadLocation).Return(loc3).Repeat.AtLeastOnce();
			newLeg1.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.AtLeastOnce();
			newLeg1.Expect(l => l.UnloadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(3)).Repeat.Any();
			newLeg1.Expect(l => l.UnloadLocation).Return(loc4).Repeat.AtLeastOnce();
			newLegs.Add(newLeg1);
			ILeg newLeg2 = MockRepository.GenerateStrictMock<ILeg>();
			newLeg2.Expect(l => l.LoadLocation).Return(loc4).Repeat.AtLeastOnce();
			newLeg2.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(4)).Repeat.Any();
			newLeg2.Expect(l => l.UnloadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(6)).Repeat.Any();
			newLeg2.Expect(l => l.UnloadLocation).Return(loc5).Repeat.AtLeastOnce();
			newLegs.Add(newLeg2);
			ILeg newLeg3 = MockRepository.GenerateStrictMock<ILeg>();
			newLeg3.Expect(l => l.LoadLocation).Return(loc5).Repeat.AtLeastOnce();
			newLeg3.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(10)).Repeat.Any();
			newLeg3.Expect(l => l.UnloadLocation).Return(loc6).Repeat.AtLeastOnce();
			newLeg3.Expect(l => l.UnloadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(13)).Repeat.Any();
			newLegs.Add(newLeg3);
			IItinerary newSegment = MockRepository.GenerateStrictMock<IItinerary>();
			newSegment.Expect(s => s.GetEnumerator()).Return(newLegs.GetEnumerator()).Repeat.AtLeastOnce();
			newSegment.Expect(s => s.InitialDepartureLocation).Return(loc3).Repeat.AtLeastOnce();
			newSegment.Expect(s => s.FinalArrivalLocation).Return(loc6).Repeat.Any();
			
			ILeg leg = MockRepository.GenerateStrictMock<ILeg>();
			leg.Expect(l => l.LoadLocation).Return(loc1).Repeat.Any();
			leg.Expect(l => l.UnloadLocation).Return(loc2).Repeat.Any();
			leg.Expect(l => l.UnloadTime).Return(DateTime.UtcNow).Repeat.Any();
			ILeg leg2 = MockRepository.GenerateStrictMock<ILeg>();
			leg2.Expect(l => l.LoadLocation).Return(loc2).Repeat.Any();
			leg2.Expect(l => l.UnloadLocation).Return(loc3).Repeat.Any();
			leg2.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromHours(5)).Repeat.Any();
			leg2.Expect(l => l.UnloadTime).Return(DateTime.UtcNow + TimeSpan.FromHours(20)).Repeat.Any();
			IItinerary tested = new Itinerary();
			tested = tested.Append(leg);
			tested = tested.Append(leg2);
		
			// act:
			IItinerary replaced = tested.ReplaceSegment(newSegment);
		
			// assert:
			Assert.IsNotNull(replaced);
			Assert.AreEqual(5, replaced.Count());
			Assert.AreEqual(loc1, replaced.InitialDepartureLocation);
			Assert.AreEqual(loc6, replaced.FinalArrivalLocation);
			leg.VerifyAllExpectations();
			leg2.VerifyAllExpectations();
			foreach(ILeg l in newLegs)
			{
				l.VerifyAllExpectations();
			}
		}
		
		[Test]
		public void ReplaceSegment_03()
		{
			// arrange:
			UnLocode loc1 = new UnLocode("CODAA");
			UnLocode loc2 = new UnLocode("CODAB");
			UnLocode loc3 = new UnLocode("CODAC");
			UnLocode loc4 = new UnLocode("CODAD");
			UnLocode loc5 = new UnLocode("CODAE");
			UnLocode loc6 = new UnLocode("CODAF");
			
			List<ILeg> newLegs = new List<ILeg>();
			ILeg newLeg1 = MockRepository.GenerateStrictMock<ILeg>();
			newLeg1.Expect(l => l.LoadLocation).Return(loc2).Repeat.AtLeastOnce();
			newLeg1.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.AtLeastOnce();
			newLeg1.Expect(l => l.UnloadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(3)).Repeat.Any();
			newLeg1.Expect(l => l.UnloadLocation).Return(loc5).Repeat.AtLeastOnce();
			newLegs.Add(newLeg1);
			ILeg newLeg2 = MockRepository.GenerateStrictMock<ILeg>();
			newLeg2.Expect(l => l.LoadLocation).Return(loc5).Repeat.AtLeastOnce();
			newLeg2.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(4)).Repeat.Any();
			newLeg2.Expect(l => l.UnloadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(6)).Repeat.Any();
			newLeg2.Expect(l => l.UnloadLocation).Return(loc6).Repeat.AtLeastOnce();
			newLegs.Add(newLeg2);
			ILeg newLeg3 = MockRepository.GenerateStrictMock<ILeg>();
			newLeg3.Expect(l => l.LoadLocation).Return(loc6).Repeat.AtLeastOnce();
			newLeg3.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(10)).Repeat.Any();
			newLeg3.Expect(l => l.UnloadLocation).Return(loc3).Repeat.AtLeastOnce();
			newLeg3.Expect(l => l.UnloadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(13)).Repeat.Any();
			newLegs.Add(newLeg3);
			IItinerary newSegment = MockRepository.GenerateStrictMock<IItinerary>();
			newSegment.Expect(s => s.GetEnumerator()).Return(newLegs.GetEnumerator()).Repeat.AtLeastOnce();
			newSegment.Expect(s => s.InitialDepartureLocation).Return(loc2).Repeat.AtLeastOnce();
			newSegment.Expect(s => s.FinalArrivalLocation).Return(loc3).Repeat.Any();
			
			ILeg leg = MockRepository.GenerateStrictMock<ILeg>();
			leg.Expect(l => l.LoadLocation).Return(loc1).Repeat.Any();
			leg.Expect(l => l.UnloadLocation).Return(loc2).Repeat.Any();
			leg.Expect(l => l.UnloadTime).Return(DateTime.UtcNow).Repeat.Any();
			ILeg leg2 = MockRepository.GenerateStrictMock<ILeg>();
			leg2.Expect(l => l.LoadLocation).Return(loc2).Repeat.Any();
			leg2.Expect(l => l.UnloadLocation).Return(loc3).Repeat.Any();
			leg2.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromHours(5)).Repeat.Any();
			leg2.Expect(l => l.UnloadTime).Return(DateTime.UtcNow + TimeSpan.FromHours(20)).Repeat.Any();
			ILeg leg3 = MockRepository.GenerateStrictMock<ILeg>();
			leg3.Expect(l => l.LoadLocation).Return(loc3).Repeat.Any();
			leg3.Expect(l => l.UnloadLocation).Return(loc4).Repeat.Any();
			leg3.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(30)).Repeat.Any();
			leg3.Expect(l => l.UnloadTime).Return(DateTime.UtcNow + TimeSpan.FromHours(32)).Repeat.Any();
			IItinerary tested = new Itinerary();
			tested = tested.Append(leg);
			tested = tested.Append(leg2);
			tested = tested.Append(leg3);
		
			// act:
			IItinerary replaced = tested.ReplaceSegment(newSegment);
		
			// assert:
			Assert.IsNotNull(replaced);
			Assert.AreEqual(5, replaced.Count());
			Assert.AreEqual(loc1, replaced.InitialDepartureLocation);
			Assert.AreEqual(loc4, replaced.FinalArrivalLocation);
			leg.VerifyAllExpectations();
			leg2.VerifyAllExpectations();
			leg3.VerifyAllExpectations();
			foreach(ILeg l in newLegs)
			{
				l.VerifyAllExpectations();
			}
		}
		
		[Test]
		public void ReplaceSegment_04()
		{
			// arrange:
			ILeg leg = MockRepository.GenerateStrictMock<ILeg>();
			Itinerary empty = new Itinerary();
			IItinerary tested = empty.Append(leg);
		
			// act:
			Assert.Throws<ArgumentNullException>(delegate {tested.ReplaceSegment(null);});
		
			// assert:
			leg.VerifyAllExpectations();
		}
		
		[Test]
		public void ReplaceSegment_05()
		{
			// arrange:
			UnLocode loc1 = new UnLocode("CODAA");
			UnLocode loc2 = new UnLocode("CODAB");
			UnLocode loc3 = new UnLocode("CODAC");
			UnLocode loc4 = new UnLocode("CODAD");
			UnLocode loc5 = new UnLocode("CODAE");
			UnLocode loc6 = new UnLocode("CODAF");
			
			List<ILeg> newLegs = new List<ILeg>();
			ILeg newLeg1 = MockRepository.GenerateStrictMock<ILeg>();
			newLeg1.Expect(l => l.LoadLocation).Return(new UnLocode("OTHER")).Repeat.Any();
			newLeg1.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			newLeg1.Expect(l => l.UnloadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(3)).Repeat.Any();
			newLeg1.Expect(l => l.UnloadLocation).Return(loc5).Repeat.Any();
			newLegs.Add(newLeg1);
			ILeg newLeg2 = MockRepository.GenerateStrictMock<ILeg>();
			newLeg2.Expect(l => l.LoadLocation).Return(loc5).Repeat.Any();
			newLeg2.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(4)).Repeat.Any();
			newLeg2.Expect(l => l.UnloadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(6)).Repeat.Any();
			newLeg2.Expect(l => l.UnloadLocation).Return(loc6).Repeat.Any();
			newLegs.Add(newLeg2);
			ILeg newLeg3 = MockRepository.GenerateStrictMock<ILeg>();
			newLeg3.Expect(l => l.LoadLocation).Return(loc6).Repeat.Any();
			newLeg3.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(10)).Repeat.Any();
			newLeg3.Expect(l => l.UnloadLocation).Return(loc3).Repeat.Any();
			newLeg3.Expect(l => l.UnloadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(13)).Repeat.Any();
			newLegs.Add(newLeg3);
			IItinerary newSegment = MockRepository.GenerateStrictMock<IItinerary>();
			newSegment.Expect(s => s.GetEnumerator()).Return(newLegs.GetEnumerator()).Repeat.Any();
			newSegment.Expect(s => s.InitialDepartureLocation).Return(new UnLocode("OTHER")).Repeat.Any();
			newSegment.Expect(s => s.FinalArrivalLocation).Return(loc3).Repeat.Any();
			
			ILeg leg = MockRepository.GenerateStrictMock<ILeg>();
			leg.Expect(l => l.LoadLocation).Return(loc1).Repeat.Any();
			leg.Expect(l => l.UnloadLocation).Return(loc2).Repeat.Any();
			leg.Expect(l => l.UnloadTime).Return(DateTime.UtcNow).Repeat.Any();
			ILeg leg2 = MockRepository.GenerateStrictMock<ILeg>();
			leg2.Expect(l => l.LoadLocation).Return(loc2).Repeat.Any();
			leg2.Expect(l => l.UnloadLocation).Return(loc3).Repeat.Any();
			leg2.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromHours(5)).Repeat.Any();
			leg2.Expect(l => l.UnloadTime).Return(DateTime.UtcNow + TimeSpan.FromHours(20)).Repeat.Any();
			ILeg leg3 = MockRepository.GenerateStrictMock<ILeg>();
			leg3.Expect(l => l.LoadLocation).Return(loc3).Repeat.Any();
			leg3.Expect(l => l.UnloadLocation).Return(loc4).Repeat.Any();
			leg3.Expect(l => l.LoadTime).Return(DateTime.UtcNow + TimeSpan.FromDays(30)).Repeat.Any();
			leg3.Expect(l => l.UnloadTime).Return(DateTime.UtcNow + TimeSpan.FromHours(32)).Repeat.Any();
			IItinerary tested = new Itinerary();
			tested = tested.Append(leg);
			tested = tested.Append(leg2);
			tested = tested.Append(leg3);
		
			// act:
			Assert.Throws<ArgumentOutOfRangeException>(delegate { tested.ReplaceSegment(newSegment); });
		
			// assert:
			leg.VerifyAllExpectations();
			leg2.VerifyAllExpectations();
			leg3.VerifyAllExpectations();
			foreach(ILeg l in newLegs)
			{
				l.VerifyAllExpectations();
			}
		}
	}
}
