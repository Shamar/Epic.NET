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
using System.Collections.Generic;
using Challenge00.DDDSample.Voyage;
namespace DefaultImplementation.Cargo
{
	[TestFixture()]
	public class InPortCargoTester
	{
		[Test]
		public void Test_Ctor_01()
		{
			// arrange:
            List<object> mocks = new List<object>();

			UnLocode code = new UnLocode("START");
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
            mocks.Add(location);
			DateTime arrival = DateTime.UtcNow;
			TrackingId id = new TrackingId("CARGO01");
            IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
            itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.Any();
            itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
            itinerary.Expect(i => i.FinalArrivalLocation).Return(new UnLocode("ENDLC")).Repeat.Any();
            mocks.Add(itinerary);
            IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
            specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.Any();
            mocks.Add(specification);
            CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
            mocks.Add(previousState);
            previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
            mocks.Add(previousState);
            previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
		
			// act:
			InPortCargo state = new InPortCargo(previousState, location, arrival);
		
			// assert:
			Assert.IsTrue(TransportStatus.InPort == state.TransportStatus);
			Assert.IsNull(state.CurrentVoyage);
			Assert.AreSame(code, state.LastKnownLocation);
			Assert.IsFalse(state.IsUnloadedAtDestination);
			Assert.AreSame(id, state.Identifier);
            foreach (object mock in mocks)
                mock.VerifyAllExpectations();
		}

        [Test]
        public void Test_Ctor_02()
        {
            // arrange:
            List<object> mocks = new List<object>();

            UnLocode code = new UnLocode("START");
            ILocation location = MockRepository.GenerateStrictMock<ILocation>();
            location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
            mocks.Add(location);
            DateTime arrival = DateTime.UtcNow;
            TrackingId id = new TrackingId("CARGO01");
            IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
            itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.Any();
            itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
            itinerary.Expect(i => i.FinalArrivalLocation).Return(code).Repeat.Any();
            mocks.Add(itinerary);
            IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
            specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.Any();
            mocks.Add(specification);
            CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
            mocks.Add(previousState);
            previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
            mocks.Add(previousState);
            previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();

            // act:
            InPortCargo state = new InPortCargo(previousState, location, arrival);

            // assert:
            Assert.IsTrue(TransportStatus.InPort == state.TransportStatus);
			Assert.IsNull(state.CurrentVoyage);
            Assert.AreSame(code, state.LastKnownLocation);
            Assert.IsTrue(state.IsUnloadedAtDestination);
            Assert.AreSame(id, state.Identifier);
            foreach (object mock in mocks)
                mock.VerifyAllExpectations();
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
            IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
            itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.Any();
            itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
            itinerary.Expect(i => i.FinalArrivalLocation).Return(new UnLocode("ENDLC")).Repeat.Any();
            specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.Any();
            CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
            previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, specification);
            previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
            previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
            InPortCargo state = new InPortCargo(previousState, location, arrival);
		
			// act:
			CargoState newState = state.ClearCustoms(location, DateTime.UtcNow);
		
			// assert:
			Assert.IsNotNull(newState);
			Assert.IsTrue(TransportStatus.InPort == state.TransportStatus);
			Assert.AreSame(code, state.LastKnownLocation);
			Assert.Throws<InvalidOperationException>(delegate { newState.ClearCustoms(location, DateTime.UtcNow); }); // Can't clear customs twice.
			Assert.AreSame(id, state.Identifier);
            Assert.IsFalse(newState.IsUnloadedAtDestination);
            itinerary.VerifyAllExpectations();
            specification.VerifyAllExpectations();
            location.VerifyAllExpectations();
            previousState.VerifyAllExpectations();
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
		public void Test_SpecifyNewRoute_05()
		{
			// arrange:
			TrackingId id = new TrackingId("CRG01");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			UnLocode code = new UnLocode("START");
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
			DateTime arrival = DateTime.UtcNow;
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			InPortCargo state = new InPortCargo(previousState, location, arrival);
			
			// assert:
			Assert.Throws<ArgumentNullException>(delegate {state.SpecifyNewRoute(null);});
			location.VerifyAllExpectations();
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
			itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.Any();
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
			location.VerifyAllExpectations();
			itinerary.VerifyAllExpectations();
			itinerary2.VerifyAllExpectations();
			specification.VerifyAllExpectations();
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
			itinerary.Expect(i => i.Equals(null)).Return(false).Repeat.Any();
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
			location.VerifyAllExpectations();
			itinerary.VerifyAllExpectations();
			itinerary2.VerifyAllExpectations();
			specification.VerifyAllExpectations();
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
			itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			IItinerary itinerary2 = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary2.Expect(i => i.Equals(itinerary)).Return(true).Repeat.AtLeastOnce();
			itinerary2.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.Any();
			specification.Expect(s => s.IsSatisfiedBy(itinerary2)).Return(true).Repeat.Any();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
			InPortCargo state = new InPortCargo(previousState, location, arrival);
		
			// act:
			CargoState newState = state.AssignToRoute(itinerary2);
		
			// assert:
			Assert.IsNotNull(newState);
			Assert.AreSame(state, newState);
			location.VerifyAllExpectations();
			itinerary.VerifyAllExpectations();
			itinerary2.VerifyAllExpectations();
			specification.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_AssignToRoute_04()
		{
			// arrange:
			UnLocode code = new UnLocode("START");
			TrackingId id = new TrackingId("CRG01");
			DateTime arrival = DateTime.UtcNow;
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
			InPortCargo state = new InPortCargo(previousState, location, arrival);
		
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { state.AssignToRoute(null); });
			location.VerifyAllExpectations();
			itinerary.VerifyAllExpectations();
			specification.VerifyAllExpectations();
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
			specification.VerifyAllExpectations();
			location.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Claim_01()
		{
			// arrange:
			UnLocode code = new UnLocode("FINAL");
			TrackingId id = new TrackingId("CRG01");
			DateTime arrival = DateTime.UtcNow;
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalLocation).Return(code).Repeat.AtLeastOnce();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
			InPortCargo state = new InPortCargo(previousState, location, arrival);
		
			// act:
			CargoState newState = state.Claim(location, DateTime.UtcNow + TimeSpan.FromDays(2));
		
			// assert:
			Assert.IsNotNull(newState);
			Assert.IsInstanceOf<ClaimedCargo>(newState);
			location.VerifyAllExpectations();
			itinerary.VerifyAllExpectations();
			specification.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Claim_02()
		{
			// arrange:
			UnLocode code = new UnLocode("FINAL");
			TrackingId id = new TrackingId("CRG01");
			DateTime arrival = DateTime.UtcNow;
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
			InPortCargo state = new InPortCargo(previousState, location, arrival);
		
			// act:
			Assert.Throws<ArgumentNullException>(delegate { state.Claim(null, DateTime.UtcNow + TimeSpan.FromDays(2)); });
		
			// assert:
			location.VerifyAllExpectations();
			itinerary.VerifyAllExpectations();
			specification.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Claim_03()
		{
			// arrange:
            List<object> mocks = new List<object>();

			UnLocode code = new UnLocode("FINAL");
			TrackingId id = new TrackingId("CRG01");
			DateTime arrival = DateTime.UtcNow;
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
            mocks.Add(location);
			ILocation location2 = MockRepository.GenerateStrictMock<ILocation>();
			location2.Expect(l => l.UnLocode).Return(new UnLocode("OTHER")).Repeat.AtLeastOnce();
            mocks.Add(location2);
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
            mocks.Add(itinerary);
            IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
            specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
            mocks.Add(specification);
            CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
            mocks.Add(previousState);
            previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
            mocks.Add(previousState);
            InPortCargo state = new InPortCargo(previousState, location, arrival);
		
			// act:
			Assert.Throws<ArgumentException>(delegate { state.Claim(location2, DateTime.UtcNow + TimeSpan.FromDays(2)); });
		
			// assert:
            foreach (object mock in mocks)
                mock.VerifyAllExpectations();
        }

        [Test]
        public void Test_Claim_04()
        {
            // arrange:
            List<object> mocks = new List<object>();

            UnLocode code = new UnLocode("FINAL");
            TrackingId id = new TrackingId("CRG01");
            DateTime arrival = DateTime.UtcNow;
            ILocation location = MockRepository.GenerateStrictMock<ILocation>();
            location.Expect(l => l.UnLocode).Return(new UnLocode("OTHER")).Repeat.AtLeastOnce();
            mocks.Add(location);
            ILocation location2 = MockRepository.GenerateStrictMock<ILocation>();
            location2.Expect(l => l.UnLocode).Return(new UnLocode("OTHER")).Repeat.AtLeastOnce();
            mocks.Add(location2);
            IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
            itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.Any();
            itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
            itinerary.Expect(i => i.FinalArrivalLocation).Return(code).Repeat.AtLeastOnce();
            mocks.Add(itinerary);
            IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
            specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
            mocks.Add(specification);
            CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
            previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
            mocks.Add(previousState);
            previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
            mocks.Add(previousState);
            InPortCargo state = new InPortCargo(previousState, location, arrival);

            // act:
            Assert.Throws<ArgumentException>(delegate { state.Claim(location2, DateTime.UtcNow + TimeSpan.FromDays(2)); });

            // assert:
            foreach (object mock in mocks)
                mock.VerifyAllExpectations();
        }

        [Test]
        public void Test_Claim_05()
        {
            // arrange:
            List<object> mocks = new List<object>();

            UnLocode code = new UnLocode("FINAL");
            TrackingId id = new TrackingId("CRG01");
            DateTime arrival = DateTime.UtcNow;
            ILocation location = MockRepository.GenerateStrictMock<ILocation>();
            location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
            mocks.Add(location);
            ILocation location2 = MockRepository.GenerateStrictMock<ILocation>();
            location2.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
            mocks.Add(location2);
            IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
            itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.Any();
            itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
            itinerary.Expect(i => i.FinalArrivalLocation).Return(code).Repeat.AtLeastOnce();
            mocks.Add(itinerary);
            IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
            specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
            mocks.Add(specification);
            CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
            previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
            mocks.Add(previousState);
            previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
            mocks.Add(previousState);
            InPortCargo state = new InPortCargo(previousState, location, arrival);

            // act:
            Assert.Throws<ArgumentException>(delegate { state.Claim(location2, DateTime.UtcNow - TimeSpan.FromDays(1)); });

            // assert:
            foreach (object mock in mocks)
                mock.VerifyAllExpectations();
        }
		
		[Test]
		public void Test_LoadOn_01()
		{
			// arrange:
            List<object> mocks = new List<object>();

            UnLocode code = new UnLocode("FINAL");
            TrackingId id = new TrackingId("CRG01");
            DateTime arrival = DateTime.UtcNow;
			UnLocode voyageLocation = new UnLocode("OTHER");
            ILocation location2 = MockRepository.GenerateStrictMock<ILocation>();
            location2.Expect(l => l.UnLocode).Return(voyageLocation).Repeat.AtLeastOnce();
            mocks.Add(location2);
            IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
            itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.Any();
            itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
            itinerary.Expect(i => i.FinalArrivalLocation).Return(code).Repeat.AtLeastOnce();
            mocks.Add(itinerary);
            IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
            specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
            mocks.Add(specification);
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.IsMoving).Return(false).Repeat.AtLeastOnce();
			voyage.Expect(v => v.LastKnownLocation).Return(voyageLocation).Repeat.AtLeastOnce();
			mocks.Add(voyage);
            CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
            previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
            mocks.Add(previousState);
            previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
            mocks.Add(previousState);
            InPortCargo state = new InPortCargo(previousState, location2, arrival);
			
			// act:
			CargoState newState = state.LoadOn(voyage, arrival + TimeSpan.FromDays(2));
		
			// assert:
			Assert.IsNotNull(newState);
			Assert.IsInstanceOf<OnboardCarrierCargo>(newState);
			foreach (object mock in mocks)
                mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_LoadOn_02()
		{
			// arrange:
			UnLocode code = new UnLocode("FINAL");
			UnLocode voyageLocation = new UnLocode("OTHER");
			TrackingId id = new TrackingId("CRG01");
			DateTime arrival = DateTime.UtcNow;
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
            ILocation location2 = MockRepository.GenerateStrictMock<ILocation>();
            location2.Expect(l => l.UnLocode).Return(voyageLocation).Repeat.AtLeastOnce();
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
			InPortCargo state = new InPortCargo(previousState, location2, arrival);
		
			// act:
			Assert.Throws<ArgumentNullException>(delegate { state.LoadOn(null, DateTime.UtcNow + TimeSpan.FromDays(2)); });
		
			// assert:
			location2.VerifyAllExpectations();
			itinerary.VerifyAllExpectations();
			specification.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_LoadOn_03()
		{
			// arrange:
			UnLocode code = new UnLocode("FINAL");
			UnLocode voyageLocation = new UnLocode("OTHER");
			TrackingId id = new TrackingId("CRG01");
			VoyageNumber voyageNumber = new VoyageNumber("ATEST");
			DateTime arrival = DateTime.UtcNow;
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(voyageLocation).Repeat.AtLeastOnce();
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.IsMoving).Return(true).Repeat.AtLeastOnce();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.AtLeastOnce();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
			InPortCargo state = new InPortCargo(previousState, location, arrival);
		
			// act:
			Assert.Throws<ArgumentException>(delegate { state.LoadOn(voyage, DateTime.UtcNow + TimeSpan.FromDays(2)); });
		
			// assert:
			location.VerifyAllExpectations();
			itinerary.VerifyAllExpectations();
			specification.VerifyAllExpectations();
			voyage.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_LoadOn_04()
		{
			// arrange:
			UnLocode code = new UnLocode("FINAL");
			TrackingId id = new TrackingId("CRG01");
			VoyageNumber voyageNumber = new VoyageNumber("ATEST");
			DateTime arrival = DateTime.UtcNow;
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
            itinerary.Expect(i => i.FinalArrivalLocation).Return(code).Repeat.AtLeastOnce();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.IsMoving).Return(false).Repeat.AtLeastOnce();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.AtLeastOnce();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
			InPortCargo state = new InPortCargo(previousState, location, arrival);
		
			// act:
			Assert.Throws<InvalidOperationException>(delegate { state.LoadOn(voyage, DateTime.UtcNow + TimeSpan.FromDays(2)); });
		
			// assert:
			location.VerifyAllExpectations();
			itinerary.VerifyAllExpectations();
			specification.VerifyAllExpectations();
			voyage.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_LoadOn_05()
		{
			// arrange:
			UnLocode code = new UnLocode("FINAL");
			TrackingId id = new TrackingId("CRG01");
			UnLocode voyageLocation = new UnLocode("OTHER");
			VoyageNumber voyageNumber = new VoyageNumber("ATEST");
			DateTime arrival = DateTime.UtcNow;
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(new UnLocode("INMDL")).Repeat.AtLeastOnce();
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
            itinerary.Expect(i => i.FinalArrivalLocation).Return(code).Repeat.AtLeastOnce();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.LastKnownLocation).Return(voyageLocation).Repeat.AtLeastOnce();
			voyage.Expect(v => v.IsMoving).Return(false).Repeat.AtLeastOnce();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.AtLeastOnce();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
			InPortCargo state = new InPortCargo(previousState, location, arrival);
		
			// act:
			Assert.Throws<ArgumentException>(delegate { state.LoadOn(voyage, DateTime.UtcNow + TimeSpan.FromDays(2)); });
		
			// assert:
			location.VerifyAllExpectations();
			itinerary.VerifyAllExpectations();
			specification.VerifyAllExpectations();
			voyage.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_LoadOn_06()
		{
			// arrange:
            List<object> mocks = new List<object>();

            UnLocode code = new UnLocode("FINAL");
            TrackingId id = new TrackingId("CRG01");
			VoyageNumber voyageNumber = new VoyageNumber("ATEST");
            DateTime arrival = DateTime.UtcNow;
			UnLocode voyageLocation = new UnLocode("OTHER");
            ILocation location2 = MockRepository.GenerateStrictMock<ILocation>();
            location2.Expect(l => l.UnLocode).Return(voyageLocation).Repeat.AtLeastOnce();
            mocks.Add(location2);
            IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
            itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.Any();
            itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
            itinerary.Expect(i => i.FinalArrivalLocation).Return(code).Repeat.AtLeastOnce();
            mocks.Add(itinerary);
            IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
            specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
            mocks.Add(specification);
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.IsMoving).Return(false).Repeat.AtLeastOnce();
			voyage.Expect(v => v.LastKnownLocation).Return(voyageLocation).Repeat.AtLeastOnce();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.AtLeastOnce();
			mocks.Add(voyage);
            CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
            previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
            mocks.Add(previousState);
            previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
            mocks.Add(previousState);
            InPortCargo state = new InPortCargo(previousState, location2, arrival);
			
			// act:
			Assert.Throws<ArgumentException>(delegate { state.LoadOn(voyage, arrival - TimeSpan.FromDays(2)); });
		
			// assert:
			foreach (object mock in mocks)
                mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Unload_01()
		{
			// arrange:
            List<object> mocks = new List<object>();

			UnLocode code = new UnLocode("START");
			VoyageNumber voyageNumber = new VoyageNumber("ATEST");
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
            mocks.Add(location);
			DateTime arrival = DateTime.UtcNow;
			TrackingId id = new TrackingId("CARGO01");
            IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
            itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.Any();
            itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
            itinerary.Expect(i => i.FinalArrivalLocation).Return(new UnLocode("ENDLC")).Repeat.Any();
            mocks.Add(itinerary);
            IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
            specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.Any();
            mocks.Add(specification);
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.AtLeastOnce();
			mocks.Add(voyage);
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
            mocks.Add(previousState);
            previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
            mocks.Add(previousState);
            previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			InPortCargo state = new InPortCargo(previousState, location, arrival);
		
			// act:
			Assert.Throws<InvalidOperationException>(delegate { state.Unload(voyage, arrival + TimeSpan.FromDays(2)); });
		
			// assert:
            foreach (object mock in mocks)
                mock.VerifyAllExpectations();

		}
		
		[Test]
		public void Test_Equals_01()
		{
            List<object> mocks = new List<object>();

			UnLocode code = new UnLocode("START");
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(code).Repeat.AtLeastOnce();
            mocks.Add(location);
			DateTime arrival = DateTime.UtcNow;
			TrackingId id = new TrackingId("CARGO01");
            IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
            itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.Any();
            itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
            itinerary.Expect(i => i.FinalArrivalLocation).Return(new UnLocode("ENDLC")).Repeat.Any();
            mocks.Add(itinerary);
            IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
            specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.Any();
            mocks.Add(specification);
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
            mocks.Add(previousState);
            previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
            mocks.Add(previousState);
            previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			
			// act:
			CargoState state1 = new InPortCargo(previousState, location, arrival);
			CargoState state2 = new InPortCargo(previousState, location, arrival);
			CargoState state3 = state1.ClearCustoms(location, arrival + TimeSpan.FromDays(1));
		
			// assert:
			Assert.IsTrue(state1.Equals(state1));
			Assert.IsFalse(state1.Equals(state2)); // differente calculation date
			Assert.IsFalse(state1.Equals(state3));
			Assert.IsFalse(state3.Equals(state1));
			Assert.IsFalse(state3.Equals(state2));
            foreach (object mock in mocks)
                mock.VerifyAllExpectations();

		}
		
		
	}
}

