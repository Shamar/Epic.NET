//  
//  InPortCargoTester.cs
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
using Challenge00.DDDSample.Location;
namespace DefaultImplementation.Cargo
{
	[TestFixture()]
	public class InPortCargoTester
	{
		[Test]
		public void Test_Ctor_01()
		{
			// arrange:
			UnLocode code = new UnLocode("START");
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
			DateTime arrival = DateTime.UtcNow;
			TrackingId id = new TrackingId("CARGO01");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
		
			// act:
			InPortCargo state = new InPortCargo(previousState, location, arrival);
		
			// assert:
			Assert.IsTrue(TransportStatus.InPort == state.TransportStatus);
			Assert.AreSame(code, state.LastKnownLocation);
			Assert.IsFalse(state.IsUnloadedAtDestination);
			Assert.AreSame(id, state.Identifier);
		}
		
		[Test]
		public void Test_ClearCustoms_01()
		{
			// arrange:
			UnLocode code = new UnLocode("START");
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
			DateTime arrival = DateTime.UtcNow;
			TrackingId id = new TrackingId("CARGO01");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			InPortCargo state = new InPortCargo(previousState, location, arrival);
		
			// act:
			CargoState newState = state.ClearCustoms(location, DateTime.UtcNow);
		
			// assert:
			Assert.IsNotNull(newState);
			Assert.IsTrue(TransportStatus.InPort == state.TransportStatus);
			Assert.AreSame(code, state.LastKnownLocation);
			Assert.Throws<InvalidOperationException>(delegate { newState.ClearCustoms(location, DateTime.UtcNow); }); // Can't clear customs twice.
			Assert.IsFalse(state.IsUnloadedAtDestination);
			Assert.AreSame(id, state.Identifier);
		}
		
		[Test]
		public void Test_ClearCustoms_02()
		{
			// arrange:
			UnLocode code1 = new UnLocode("FSTCD");
			UnLocode code2 = new UnLocode("SNDCD");
			ILocation location1 = MockRepository.GenerateStrictMock<ILocation>();
			location1.Expect(l => l.UnLocode).Return(code1).Repeat.AtLeastOnce();
			ILocation location2 = MockRepository.GenerateStrictMock<ILocation>();
			location2.Expect(l => l.UnLocode).Return(code2).Repeat.AtLeastOnce();
			DateTime arrival = DateTime.UtcNow;
			TrackingId id = new TrackingId("CARGO01");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState.Expect(s => s.LastKnownLocation).Return(code1).Repeat.Any();
			InPortCargo state = new InPortCargo(previousState, location1, arrival);
		
			// assert:
			Assert.Throws<ArgumentException>( delegate { state.ClearCustoms(location2, DateTime.UtcNow); });
		}	
		
		[Test]
		public void Test_ClearCustoms_03()
		{
			// arrange:
			UnLocode code = new UnLocode("START");
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
			DateTime arrival = DateTime.UtcNow;
			DateTime customs = arrival - TimeSpan.FromHours(1);
			TrackingId id = new TrackingId("CARGO01");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			InPortCargo state = new InPortCargo(previousState, location, arrival);
		
			// assert:
			Assert.Throws<ArgumentException>( delegate { state.ClearCustoms(location, customs); });
		}
		
		[Test]
		public void Test_SpecifyNewRoute_01()
		{
			// arrange:
			TrackingId id = new TrackingId("CRG01");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			UnLocode code = new UnLocode("START");
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
			DateTime arrival = DateTime.UtcNow;
			IRouteSpecification specification2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			specification.Expect(s => s.Equals(specification2)).Return(false).Repeat.Any();
			specification2.Expect(s => s.Equals(specification)).Return(false).Repeat.Any();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			InPortCargo state = new InPortCargo(previousState, location, arrival);
			
			// act:
			CargoState newState = state.SpecifyNewRoute(specification2);
		
			// assert:
			Assert.AreSame(specification2, newState.RouteSpecification);
			Assert.IsTrue(state.RoutingStatus == newState.RoutingStatus);
			Assert.IsTrue(state.TransportStatus == newState.TransportStatus);
			Assert.AreSame(code, newState.LastKnownLocation);
		}
		
		[Test]
		public void Test_SpecifyNewRoute_02()
		{
			// arrange:
			TrackingId id = new TrackingId("CRG01");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			UnLocode code = new UnLocode("START");
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
			DateTime arrival = DateTime.UtcNow;
			IRouteSpecification specification2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			specification.Expect(s => s.Equals(specification2)).Return(true).Repeat.Any();
			specification2.Expect(s => s.Equals(specification)).Return(true).Repeat.Any();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			InPortCargo state = new InPortCargo(previousState, location, arrival);
			
			// act:
			CargoState newState = state.SpecifyNewRoute(specification2);
		
			// assert:
			Assert.AreSame(state, newState);
		}
		
		[Test]
		public void Test_SpecifyNewRoute_03()
		{
			// arrange:
			TrackingId id = new TrackingId("CRG01");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			UnLocode code = new UnLocode("START");
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
			DateTime arrival = DateTime.UtcNow;
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			IRouteSpecification specification2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			specification.Expect(s => s.Equals(specification2)).Return(false).Repeat.Any();
			specification2.Expect(s => s.Equals(specification)).Return(false).Repeat.Any();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			specification2.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
			InPortCargo state = new InPortCargo(previousState, location, arrival);
			
			// act:
			CargoState newState = state.SpecifyNewRoute(specification2);
		
			// assert:
			Assert.AreSame(specification2, newState.RouteSpecification);
			Assert.IsTrue(state.RoutingStatus == newState.RoutingStatus);
			Assert.IsTrue(state.TransportStatus == newState.TransportStatus);
			Assert.AreSame(code, newState.LastKnownLocation);
		}
		
		[Test]
		public void Test_SpecifyNewRoute_04()
		{
			// arrange:
			TrackingId id = new TrackingId("CRG01");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			UnLocode code = new UnLocode("START");
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
			DateTime arrival = DateTime.UtcNow;
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			IRouteSpecification specification2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			specification.Expect(s => s.Equals(specification2)).Return(false).Repeat.Any();
			specification2.Expect(s => s.Equals(specification)).Return(false).Repeat.Any();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.Any();
			specification2.Expect(s => s.IsSatisfiedBy(itinerary)).Return(false).Repeat.Any();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
			InPortCargo state = new InPortCargo(previousState, location, arrival);
			
			// act:
			CargoState newState = state.SpecifyNewRoute(specification2);
		
			// assert:
			Assert.AreSame(specification2, newState.RouteSpecification);
			Assert.IsTrue(RoutingStatus.Misrouted == newState.RoutingStatus);
			Assert.IsTrue(state.TransportStatus == newState.TransportStatus);
			Assert.AreSame(code, newState.LastKnownLocation);
		}
		
		[Test]
		public void Test_AssignToRoute_01()
		{
			// arrange:
			UnLocode code = new UnLocode("START");
			TrackingId id = new TrackingId("CRG01");
			DateTime arrival = DateTime.UtcNow;
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.AtLeastOnce();
			itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			IItinerary itinerary2 = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary2.Expect(i => i.Equals(itinerary)).Return(false).Repeat.AtLeastOnce();
			itinerary2.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			specification.Expect(s => s.IsSatisfiedBy(itinerary2)).Return(true).Repeat.AtLeastOnce();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
			InPortCargo state = new InPortCargo(previousState, location, arrival);
		
			// act:
			CargoState newState = state.AssignToRoute(itinerary2);
		
			// assert:
			Assert.IsNotNull(newState);
			Assert.AreNotSame(state, newState);
			Assert.IsTrue(RoutingStatus.Routed == newState.RoutingStatus);
			Assert.IsTrue(state.TransportStatus == newState.TransportStatus);
		}
		
		[Test]
		public void Test_AssignToRoute_02()
		{
			// arrange:
			UnLocode code = new UnLocode("START");
			TrackingId id = new TrackingId("CRG01");
			DateTime arrival = DateTime.UtcNow;
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.AtLeastOnce();
			itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			IItinerary itinerary2 = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary2.Expect(i => i.Equals(itinerary)).Return(false).Repeat.AtLeastOnce();
			itinerary2.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			specification.Expect(s => s.IsSatisfiedBy(itinerary2)).Return(false).Repeat.AtLeastOnce();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
			InPortCargo state = new InPortCargo(previousState, location, arrival);
		
			// assert:
			Assert.Throws<ArgumentException>(delegate { state.AssignToRoute(itinerary2);} );
		}
		
		[Test]
		public void Test_AssignToRoute_03()
		{
			// arrange:
			UnLocode code = new UnLocode("START");
			TrackingId id = new TrackingId("CRG01");
			DateTime arrival = DateTime.UtcNow;
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.AtLeastOnce();
			itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			IItinerary itinerary2 = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary2.Expect(i => i.Equals(itinerary)).Return(true).Repeat.AtLeastOnce();
			itinerary2.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			specification.Expect(s => s.IsSatisfiedBy(itinerary2)).Return(true).Repeat.AtLeastOnce();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
			InPortCargo state = new InPortCargo(previousState, location, arrival);
		
			// act:
			CargoState newState = state.AssignToRoute(itinerary2);
		
			// assert:
			Assert.IsNotNull(newState);
			Assert.AreSame(state, newState);
		}

		[Test]
		public void Test_Recieve_01()
		{
			// arrange:
			UnLocode code = new UnLocode("START");
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
			DateTime arrival = DateTime.UtcNow;
			TrackingId id = new TrackingId("CARGO01");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			InPortCargo state = new InPortCargo(previousState, location, arrival);
		
			// assert:
			Assert.Throws<InvalidOperationException>(delegate { state.Recieve(location, arrival); });
		}
		
	}
}

