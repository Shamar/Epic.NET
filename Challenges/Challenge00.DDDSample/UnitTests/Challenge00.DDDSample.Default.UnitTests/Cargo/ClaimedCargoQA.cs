//  
//  ClaimedCargoTester.cs
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
using System;
using NUnit.Framework;
using Challenge00.DDDSample.Cargo;
using Rhino.Mocks;
using Challenge00.DDDSample.Location;
using Challenge00.DDDSample.Voyage;
namespace DefaultImplementation.Cargo
{
	[TestFixture()]
	public class ClaimedCargoQA
	{
		[Test]
		public void Ctor_01()
		{
			// arrange:
			UnLocode final = new UnLocode("FINAL");
			TrackingId id = new TrackingId("CLAIM");
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow).Repeat.AtLeastOnce();
			itinerary.Expect(i => i.FinalArrivalLocation).Return(final).Repeat.AtLeastOnce();
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.Any();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
			previousState.Expect(s => s.IsUnloadedAtDestination).Return(true).Repeat.AtLeastOnce();
			previousState.Expect(s => s.TransportStatus).Return(TransportStatus.InPort).Repeat.AtLeastOnce();
			DateTime claimDate = DateTime.UtcNow;
			
		
			// act:
			ClaimedCargo state = new ClaimedCargo(previousState, claimDate);
		
			// assert:
			Assert.AreEqual(TransportStatus.Claimed, state.TransportStatus);
			Assert.AreEqual(RoutingStatus.Routed, state.RoutingStatus);
			Assert.AreSame(final, state.LastKnownLocation);
			Assert.AreSame(specification, state.RouteSpecification);
			Assert.IsNull(state.CurrentVoyage);
			Assert.IsTrue(state.IsUnloadedAtDestination);
			itinerary.VerifyAllExpectations();
			specification.VerifyAllExpectations();
			previousState.VerifyAllExpectations();
		}
		
		[Test]
		public void Ctor_02()
		{
			// arrange:
			System.Collections.Generic.List<object> mocks = new System.Collections.Generic.List<object>();
			TrackingId id = new TrackingId("CLAIM");
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow).Repeat.AtLeastOnce();
			mocks.Add(itinerary);
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.Any();
			mocks.Add(specification);
			IRouteSpecification specification2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			specification2.Expect(s => s.IsSatisfiedBy(itinerary)).Return(false).Repeat.Any();
			mocks.Add(specification2);
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			mocks.Add(previousState);
			previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
			mocks.Add(previousState);
			previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, specification2);
			mocks.Add(previousState);
			DateTime claimDate = DateTime.UtcNow;
			
		
			// act:
			Assert.Throws<ArgumentException>(delegate { new ClaimedCargo(previousState, claimDate); });
		
			// assert:
			foreach(object mock in mocks)
			{
				mock.VerifyAllExpectations();
			}
		}
		
		[Test]
		public void Ctor_03()
		{
			// arrange:
			TrackingId id = new TrackingId("CLAIM");
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			DateTime claimDate = DateTime.UtcNow;
			
		
			// act:
			Assert.Throws<ArgumentException>(delegate { new ClaimedCargo(previousState, claimDate); });
		
			// assert:
			specification.VerifyAllExpectations();
		}
		
		[Test]
		public void Ctor_04()
		{
			// arrange:
			TrackingId id = new TrackingId("CLAIM");
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow).Repeat.AtLeastOnce();
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.Any();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
			previousState.Expect(s => s.TransportStatus).Return(TransportStatus.OnboardCarrier).Repeat.AtLeastOnce();
			DateTime claimDate = DateTime.UtcNow;
			
		
			// act:
			Assert.Throws<ArgumentException>(delegate { new ClaimedCargo(previousState, claimDate); });
		
			// assert:
			itinerary.VerifyAllExpectations();
			specification.VerifyAllExpectations();
			previousState.VerifyAllExpectations();
		}
		
		[Test]
		public void Ctor_05()
		{
			// arrange:
			TrackingId id = new TrackingId("CLAIM");
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow).Repeat.AtLeastOnce();
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.Any();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
			previousState.Expect(s => s.IsUnloadedAtDestination).Return(false).Repeat.AtLeastOnce();
			previousState.Expect(s => s.TransportStatus).Return(TransportStatus.InPort).Repeat.AtLeastOnce();
			DateTime claimDate = DateTime.UtcNow;
			
		
			// act:
			Assert.Throws<ArgumentException>(delegate { new ClaimedCargo(previousState, claimDate); });
		
			// assert:
			itinerary.VerifyAllExpectations();
			specification.VerifyAllExpectations();
			previousState.VerifyAllExpectations();
		}

		[Test]
		public void StateTransitions_01()
		{
			// arrange:
			UnLocode final = new UnLocode("FINAL");
			TrackingId id = new TrackingId("CLAIM");
			ILocation finalLocation = MockRepository.GenerateStrictMock<ILocation>();
			finalLocation.Expect(l => l.UnLocode).Return(final).Repeat.AtLeastOnce();
			ILocation otherLocation = MockRepository.GenerateStrictMock<ILocation>();
			otherLocation.Expect(l => l.UnLocode).Return(new UnLocode("OTHER")).Repeat.AtLeastOnce();
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalLocation).Return(final).Repeat.Any();
			IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.Any();
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
			previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
			previousState.Expect(s => s.IsUnloadedAtDestination).Return(true).Repeat.Any();
			previousState.Expect(s => s.TransportStatus).Return(TransportStatus.InPort).Repeat.Any();
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			DateTime claimDate = DateTime.UtcNow;
			
			ClaimedCargo state = new ClaimedCargo(previousState, claimDate);
			
			// act:
			CargoState newState = state.Claim(finalLocation, claimDate);
		
			// assert:
			Assert.AreSame(state, newState);
			Assert.Throws<ArgumentNullException>(delegate { state.Claim(null, DateTime.UtcNow); });
			Assert.Throws<AlreadyClaimedException>(delegate { state.Claim(finalLocation, DateTime.UtcNow + TimeSpan.FromSeconds(10)); });
			Assert.Throws<AlreadyClaimedException>(delegate { state.Claim(otherLocation, claimDate); });
			Assert.Throws<AlreadyClaimedException>(delegate { state.LoadOn(voyage, DateTime.UtcNow + TimeSpan.FromSeconds(10)); });
			Assert.Throws<AlreadyClaimedException>(delegate { state.Unload(voyage, DateTime.UtcNow + TimeSpan.FromSeconds(10)); });
			Assert.Throws<AlreadyClaimedException>(delegate { state.SpecifyNewRoute(MockRepository.GenerateStrictMock<IRouteSpecification>()); });
			Assert.Throws<AlreadyClaimedException>(delegate { state.AssignToRoute(MockRepository.GenerateStrictMock<IItinerary>()); });
			Assert.Throws<AlreadyClaimedException>(delegate { state.Recieve(MockRepository.GenerateStrictMock<ILocation>(), DateTime.UtcNow + TimeSpan.FromSeconds(10) + TimeSpan.FromSeconds(10)); });
			Assert.Throws<AlreadyClaimedException>(delegate { state.ClearCustoms(MockRepository.GenerateStrictMock<ILocation>(), DateTime.UtcNow + TimeSpan.FromSeconds(10)); });
			itinerary.VerifyAllExpectations();
			specification.VerifyAllExpectations();
		}
		
	}
}

