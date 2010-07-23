//  
//  RouteSpecificationTester.cs
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
using Challenge00.DDDSample.Location;
using Rhino.Mocks;
using Challenge00.DDDSample.Cargo;
namespace DefaultImplementation.Cargo
{
	[TestFixture()]
	public class RouteSpecificationTester
	{
		[Test]
		public void Test_Ctor_01()
		{
			// arrange:
			UnLocode first = new UnLocode("CDFST");
			UnLocode second = new UnLocode("CDSND");
			ILocation origin = MockRepository.GenerateStrictMock<ILocation>();
			origin.Expect(l => l.UnLocode).Return(first).Repeat.AtLeastOnce();
			ILocation destination = MockRepository.GenerateStrictMock<ILocation>();
			destination.Expect(l => l.UnLocode).Return(second).Repeat.AtLeastOnce();
			DateTime arrivalDeadline = DateTime.UtcNow + TimeSpan.FromDays(5);
		
			// act:
			IRouteSpecification specification = new RouteSpecification(origin, destination, arrivalDeadline);
		
			// assert:
			Assert.IsNotNull(specification);
			Assert.AreSame(first, specification.Origin);
			Assert.AreSame(second, specification.Destination);
			Assert.AreEqual(arrivalDeadline, specification.ArrivalDeadline);
			origin.VerifyAllExpectations();
			destination.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Ctor_02()
		{
			// arrange:
			ILocation destination = MockRepository.GenerateStub<ILocation>();
			DateTime arrivalDeadline = DateTime.UtcNow + TimeSpan.FromDays(5);
		
			// assert:
			Assert.Throws<ArgumentNullException>( delegate { new RouteSpecification(null, destination, arrivalDeadline); });
			destination.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Ctor_03()
		{
			// arrange:
			ILocation origin = MockRepository.GenerateStub<ILocation>();
			DateTime arrivalDeadline = DateTime.UtcNow + TimeSpan.FromDays(5);
		
			// assert:
			Assert.Throws<ArgumentNullException>( delegate { new RouteSpecification(origin, null, arrivalDeadline); });
			origin.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Ctor_04()
		{
			// arrange:
			ILocation origin = MockRepository.GenerateStub<ILocation>();
			DateTime arrivalDeadline = DateTime.UtcNow + TimeSpan.FromDays(5);
		
			// assert:
			Assert.Throws<ArgumentNullException>( delegate { new RouteSpecification(origin, Unknown.Location, arrivalDeadline); });
			origin.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Ctor_05()
		{
			// arrange:
			ILocation destination = MockRepository.GenerateStub<ILocation>();
			DateTime arrivalDeadline = DateTime.UtcNow + TimeSpan.FromDays(5);
		
			// assert:
			Assert.Throws<ArgumentNullException>( delegate { new RouteSpecification(Unknown.Location, destination, arrivalDeadline); });
			destination.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Ctor_06()
		{
			// arrange:
			ILocation origin = MockRepository.GenerateStub<ILocation>();
			ILocation destination = MockRepository.GenerateStub<ILocation>();
			DateTime arrivalDeadline = DateTime.UtcNow - TimeSpan.FromDays(1);
		
			// assert:
			Assert.Throws<ArgumentException>( delegate { new RouteSpecification(origin, destination, arrivalDeadline); });
			origin.VerifyAllExpectations();
			destination.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Equals_01()
		{
			// arrange:
			UnLocode first = new UnLocode("CDFST");
			UnLocode second = new UnLocode("CDSND");
			ILocation origin = MockRepository.GenerateStrictMock<ILocation>();
			origin.Expect(l => l.UnLocode).Return(first).Repeat.Once();
			ILocation destination = MockRepository.GenerateStrictMock<ILocation>();
			destination.Expect(l => l.UnLocode).Return(second).Repeat.Once();
			DateTime arrivalDeadline = DateTime.UtcNow + TimeSpan.FromDays(5);
			IRouteSpecification specification = new RouteSpecification(origin, destination, arrivalDeadline);
		
			// assert:
			Assert.IsFalse(specification.Equals(null));
			Assert.IsTrue(specification.Equals(specification));
			Assert.IsTrue(specification.Equals((object)specification));
			origin.VerifyAllExpectations();
			destination.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Equals_02()
		{
			// arrange:
			UnLocode first = new UnLocode("CDFST");
			UnLocode second = new UnLocode("CDSND");
			ILocation origin = MockRepository.GenerateStrictMock<ILocation>();
			origin.Expect(l => l.UnLocode).Return(first).Repeat.Any();
			ILocation destination = MockRepository.GenerateStrictMock<ILocation>();
			destination.Expect(l => l.UnLocode).Return(second).Repeat.Any();
			DateTime arrivalDeadline = DateTime.UtcNow + TimeSpan.FromDays(5);
			IRouteSpecification specification1 = new RouteSpecification(origin, destination, arrivalDeadline);
			IRouteSpecification specification2 = new RouteSpecification(origin, destination, arrivalDeadline);
		
			// assert:
			Assert.IsTrue(specification1.Equals(specification2));
			Assert.IsTrue(specification2.Equals(specification1));
			Assert.IsTrue(specification1.Equals((object)specification2));
			Assert.IsTrue(specification2.Equals((object)specification1));
			origin.VerifyAllExpectations();
			destination.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Equals_03()
		{
			// arrange:
			UnLocode first = new UnLocode("CDFST");
			UnLocode second = new UnLocode("CDSND");
			ILocation origin = MockRepository.GenerateStrictMock<ILocation>();
			origin.Expect(l => l.UnLocode).Return(first).Repeat.Any();
			ILocation destination = MockRepository.GenerateStrictMock<ILocation>();
			destination.Expect(l => l.UnLocode).Return(second).Repeat.Any();
			DateTime arrivalDeadline = DateTime.UtcNow + TimeSpan.FromDays(5);
			IRouteSpecification specification1 = new RouteSpecification(origin, destination, arrivalDeadline);
			IRouteSpecification specification2 = new RouteSpecification(origin, destination, arrivalDeadline + TimeSpan.FromDays(5));
		
			// assert:
			Assert.IsFalse(specification1.Equals(specification2));
			Assert.IsFalse(specification2.Equals(specification1));
			Assert.IsFalse(specification1.Equals((object)specification2));
			Assert.IsFalse(specification2.Equals((object)specification1));
			origin.VerifyAllExpectations();
			destination.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Equals_04()
		{
			// arrange:
			UnLocode first = new UnLocode("CDFST");
			UnLocode second = new UnLocode("CDSND");
			UnLocode third = new UnLocode("CDTRD");
			ILocation origin = MockRepository.GenerateStrictMock<ILocation>();
			origin.Expect(l => l.UnLocode).Return(first).Repeat.Any();
			ILocation destination = MockRepository.GenerateStrictMock<ILocation>();
			destination.Expect(l => l.UnLocode).Return(second).Repeat.Any();
			ILocation origin2 = MockRepository.GenerateStrictMock<ILocation>();
			origin2.Expect(l => l.UnLocode).Return(third).Repeat.Any();
			DateTime arrivalDeadline = DateTime.UtcNow + TimeSpan.FromDays(5);
			IRouteSpecification specification1 = new RouteSpecification(origin, destination, arrivalDeadline);
			IRouteSpecification specification2 = new RouteSpecification(origin2, destination, arrivalDeadline);
		
			// assert:
			Assert.IsFalse(specification1.Equals(specification2));
			Assert.IsFalse(specification2.Equals(specification1));
			Assert.IsFalse(specification1.Equals((object)specification2));
			Assert.IsFalse(specification2.Equals((object)specification1));
			origin.VerifyAllExpectations();
			destination.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Equals_05()
		{
			// arrange:
			UnLocode first = new UnLocode("CDFST");
			UnLocode second = new UnLocode("CDSND");
			UnLocode third = new UnLocode("CDTRD");
			ILocation origin = MockRepository.GenerateStrictMock<ILocation>();
			origin.Expect(l => l.UnLocode).Return(first).Repeat.Any();
			ILocation destination = MockRepository.GenerateStrictMock<ILocation>();
			destination.Expect(l => l.UnLocode).Return(second).Repeat.Any();
			ILocation destination2 = MockRepository.GenerateStrictMock<ILocation>();
			destination2.Expect(l => l.UnLocode).Return(third).Repeat.Any();
			DateTime arrivalDeadline = DateTime.UtcNow + TimeSpan.FromDays(5);
			IRouteSpecification specification1 = new RouteSpecification(origin, destination, arrivalDeadline);
			IRouteSpecification specification2 = new RouteSpecification(origin, destination2, arrivalDeadline);
		
			// assert:
			Assert.IsFalse(specification1.Equals(specification2));
			Assert.IsFalse(specification2.Equals(specification1));
			Assert.IsFalse(specification1.Equals((object)specification2));
			Assert.IsFalse(specification2.Equals((object)specification1));
			origin.VerifyAllExpectations();
			destination.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_IsSatisfiedBy_01()
		{
			// arrange:
			UnLocode first = new UnLocode("CDFST");
			UnLocode second = new UnLocode("CDSND");
			ILocation origin = MockRepository.GenerateStrictMock<ILocation>();
			origin.Expect(l => l.UnLocode).Return(first).Repeat.Once();
			ILocation destination = MockRepository.GenerateStrictMock<ILocation>();
			destination.Expect(l => l.UnLocode).Return(second).Repeat.Once();
			DateTime arrivalDeadline = DateTime.UtcNow + TimeSpan.FromDays(5);
			
			IItinerary candidate = MockRepository.GenerateStrictMock<IItinerary>();
			candidate.Expect(i => i.InitialDepartureLocation).Return(first).Repeat.Once();
			candidate.Expect(i => i.FinalArrivalLocation).Return(second).Repeat.Once();
			candidate.Expect(i => i.FinalArrivalDate).Return(arrivalDeadline - TimeSpan.FromHours(1));
			
			IRouteSpecification specification = new RouteSpecification(origin, destination, arrivalDeadline);
		
			// act:
			bool satisfied = specification.IsSatisfiedBy(candidate);
		
			// assert:
			Assert.IsTrue(satisfied);
			origin.VerifyAllExpectations();
			destination.VerifyAllExpectations();
			candidate.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_IsSatisfiedBy_02()
		{
			// arrange:
			UnLocode first = new UnLocode("CDFST");
			UnLocode second = new UnLocode("CDSND");
			ILocation origin = MockRepository.GenerateStrictMock<ILocation>();
			origin.Expect(l => l.UnLocode).Return(first).Repeat.Once();
			ILocation destination = MockRepository.GenerateStrictMock<ILocation>();
			destination.Expect(l => l.UnLocode).Return(second).Repeat.Once();
			DateTime arrivalDeadline = DateTime.UtcNow + TimeSpan.FromDays(5);
			
			IItinerary candidate = MockRepository.GenerateStrictMock<IItinerary>();
			candidate.Expect(i => i.InitialDepartureLocation).Return(first).Repeat.Once();
			candidate.Expect(i => i.FinalArrivalLocation).Return(second).Repeat.Once();
			candidate.Expect(i => i.FinalArrivalDate).Return(arrivalDeadline + TimeSpan.FromHours(1));
			
			IRouteSpecification specification = new RouteSpecification(origin, destination, arrivalDeadline);
		
			// act:
			bool satisfied = specification.IsSatisfiedBy(candidate);
		
			// assert:
			Assert.IsFalse(satisfied);
			origin.VerifyAllExpectations();
			destination.VerifyAllExpectations();
			candidate.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_IsSatisfiedBy_03()
		{
			// arrange:
			UnLocode first = new UnLocode("CDFST");
			UnLocode second = new UnLocode("CDSND");
			ILocation origin = MockRepository.GenerateStrictMock<ILocation>();
			origin.Expect(l => l.UnLocode).Return(first).Repeat.Once();
			ILocation destination = MockRepository.GenerateStrictMock<ILocation>();
			destination.Expect(l => l.UnLocode).Return(second).Repeat.Once();
			DateTime arrivalDeadline = DateTime.UtcNow + TimeSpan.FromDays(5);
			
			IItinerary candidate = MockRepository.GenerateStrictMock<IItinerary>();
			candidate.Expect(i => i.InitialDepartureLocation).Return(first).Repeat.Once();
			candidate.Expect(i => i.FinalArrivalLocation).Return(second).Repeat.Once();
			candidate.Expect(i => i.FinalArrivalDate).Return(arrivalDeadline);
			
			IRouteSpecification specification = new RouteSpecification(origin, destination, arrivalDeadline);
		
			// act:
			bool satisfied = specification.IsSatisfiedBy(candidate);
		
			// assert:
			Assert.IsTrue(satisfied);
			origin.VerifyAllExpectations();
			destination.VerifyAllExpectations();
			candidate.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_IsSatisfiedBy_04()
		{
			// arrange:
			UnLocode first = new UnLocode("CDFST");
			UnLocode second = new UnLocode("CDSND");
			ILocation origin = MockRepository.GenerateStrictMock<ILocation>();
			origin.Expect(l => l.UnLocode).Return(first).Repeat.Once();
			ILocation destination = MockRepository.GenerateStrictMock<ILocation>();
			destination.Expect(l => l.UnLocode).Return(second).Repeat.Once();
			DateTime arrivalDeadline = DateTime.UtcNow + TimeSpan.FromDays(5);
			
			IRouteSpecification specification = new RouteSpecification(origin, destination, arrivalDeadline);
		
			// act:
			bool satisfied = specification.IsSatisfiedBy(null);
		
			// assert:
			Assert.IsFalse(satisfied);
			origin.VerifyAllExpectations();
			destination.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_IsSatisfiedBy_05()
		{
			// arrange:
			UnLocode first = new UnLocode("CDFST");
			UnLocode second = new UnLocode("CDSND");
			UnLocode third = new UnLocode("CDTRD");
			ILocation origin = MockRepository.GenerateStrictMock<ILocation>();
			origin.Expect(l => l.UnLocode).Return(first).Repeat.Once();
			ILocation destination = MockRepository.GenerateStrictMock<ILocation>();
			destination.Expect(l => l.UnLocode).Return(second).Repeat.Once();
			DateTime arrivalDeadline = DateTime.UtcNow + TimeSpan.FromDays(5);
			
			IItinerary candidate = MockRepository.GenerateStrictMock<IItinerary>();
			candidate.Expect(i => i.InitialDepartureLocation).Return(third).Repeat.Once();
			candidate.Expect(i => i.FinalArrivalLocation).Return(second).Repeat.Any();
			candidate.Expect(i => i.FinalArrivalDate).Return(arrivalDeadline - TimeSpan.FromHours(1)).Repeat.Any();
			
			IRouteSpecification specification = new RouteSpecification(origin, destination, arrivalDeadline);
		
			// act:
			bool satisfied = specification.IsSatisfiedBy(candidate);
		
			// assert:
			Assert.IsFalse(satisfied);
			origin.VerifyAllExpectations();
			destination.VerifyAllExpectations();
			candidate.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_IsSatisfiedBy_06()
		{
			// arrange:
			UnLocode first = new UnLocode("CDFST");
			UnLocode second = new UnLocode("CDSND");
			UnLocode third = new UnLocode("CDTRD");
			ILocation origin = MockRepository.GenerateStrictMock<ILocation>();
			origin.Expect(l => l.UnLocode).Return(first).Repeat.Once();
			ILocation destination = MockRepository.GenerateStrictMock<ILocation>();
			destination.Expect(l => l.UnLocode).Return(second).Repeat.Once();
			DateTime arrivalDeadline = DateTime.UtcNow + TimeSpan.FromDays(5);
			
			IItinerary candidate = MockRepository.GenerateStrictMock<IItinerary>();
			candidate.Expect(i => i.InitialDepartureLocation).Return(first).Repeat.Once();
			candidate.Expect(i => i.FinalArrivalLocation).Return(third).Repeat.Once();
			candidate.Expect(i => i.FinalArrivalDate).Return(arrivalDeadline - TimeSpan.FromHours(1)).Repeat.Any();
			
			IRouteSpecification specification = new RouteSpecification(origin, destination, arrivalDeadline);
		
			// act:
			bool satisfied = specification.IsSatisfiedBy(candidate);
		
			// assert:
			Assert.IsFalse(satisfied);
			origin.VerifyAllExpectations();
			destination.VerifyAllExpectations();
			candidate.VerifyAllExpectations();
		}
	}
}

