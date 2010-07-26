//  
//  NewCargoTester.cs
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
using Challenge00.DDDSample.Cargo;
using Rhino.Mocks;
namespace DefaultImplementation.Cargo
{
	[TestFixture]
	public class NewCargoTester
	{
		[Test]
		public void Test_Ctor_01()
		{
			// arrange:
			TrackingId id = new TrackingId("CRG01");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
		
			// act:
			NewCargo state = new NewCargo(id, specification);
		
			// assert:
			Assert.AreSame(id, state.Identifier);
			Assert.AreSame(specification, state.RouteSpecification);
			Assert.IsNull(state.Itinerary);
			Assert.IsNull(state.CurrentVoyage);
			Assert.IsNull(state.EstimatedTimeOfArrival);
			Assert.AreEqual(RoutingStatus.NotRouted, state.RoutingStatus);
			Assert.AreEqual(TransportStatus.NotReceived, state.TransportStatus);
			Assert.IsFalse(state.IsUnloadedAtDestination);
			specification.VerifyAllExpectations();
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Ctor_02 ()
		{
			// arrange:
			TrackingId number = new TrackingId("VYGTEST01");
		
			// act:
			new NewCargo(number, null);
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Ctor_03 ()
		{
			// arrange:
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
		
			// act:
			new NewCargo(null, specification);
		}
		
		[Test]
		public void Test_AssignToRoute_01()
		{
			// arrange:
			TrackingId id = new TrackingId("CRG01");
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.Once();
			NewCargo state = new NewCargo(id, specification);
		
			// act:
			CargoState newState = state.AssignToRoute(itinerary);
		
			// assert:
			Assert.IsNotNull(newState);
			Assert.AreSame(itinerary, newState.Itinerary);
			Assert.AreNotSame(state, newState);
			Assert.IsFalse(state.Equals(newState));
			Assert.IsFalse(newState.Equals(state));
			itinerary.VerifyAllExpectations();
			specification.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_AssignToRoute_02()
		{
			// arrange:
			TrackingId id = new TrackingId("CRG01");
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(false).Repeat.Once();
			NewCargo state = new NewCargo(id, specification);
		
			// act:
			Assert.Throws<ArgumentException>(delegate { state.AssignToRoute(itinerary); } );
		
			// assert:
			itinerary.VerifyAllExpectations();
			specification.VerifyAllExpectations();
		}
	}
}

