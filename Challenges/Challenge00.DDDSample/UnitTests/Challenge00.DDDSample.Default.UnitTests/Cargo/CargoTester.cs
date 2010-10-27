//  
//  CargoTester.cs
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
using Challenge00.DDDSample.Voyage;
using Challenge00.DDDSample.Location;

using TCargo = Challenge00.DDDSample.Cargo.Cargo;
using GList = System.Collections.Generic.List<object>;
using Challenge00.DDDSample.Shared;

namespace DefaultImplementation.Cargo
{
	class FakeCargo : TCargo
	{
		public FakeCargo(CargoState initialState)
			: base(initialState)
		{
		}
	}
	
	[TestFixture]
	public class CargoTester
	{
		[Test]
		public void Test_Ctor_01()
		{
			// arrange:
			TrackingId identifier = new TrackingId("CARGO01");
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
		
			// act:
			TCargo tested = new TCargo(identifier, route);
		
			// assert:
			Assert.AreSame(identifier, tested.TrackingId);
			Assert.AreSame(route, tested.RouteSpecification);
			Assert.IsNotNull(tested.Delivery);
			Assert.AreEqual(TransportStatus.NotReceived, tested.Delivery.TransportStatus);
			Assert.AreEqual(RoutingStatus.NotRouted, tested.Delivery.RoutingStatus);
			Assert.IsFalse(tested.Delivery.IsUnloadedAtDestination);
			Assert.IsNull(tested.Delivery.CurrentVoyage);
			Assert.IsNull(tested.Delivery.EstimatedTimeOfArrival);
			Assert.IsNull(tested.Delivery.LastKnownLocation);
			Assert.IsNull(tested.Itinerary);
		}
		
		[Test]
		public void Test_Ctor_02 ()
		{
			// arrange:
			TrackingId identifier = new TrackingId("CARGO01");
			VoyageNumber voyage = new VoyageNumber("VYG01");
			DateTime estimatedTimeOfArrival = DateTime.Now;
			UnLocode lastKnownLocation = new UnLocode("LSTLC");
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.FinalArrivalDate).Return(estimatedTimeOfArrival).Repeat.AtLeastOnce();
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			route.Expect(r => r.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			CargoState state = MockRepository.GeneratePartialMock<CargoState>(identifier, route);
			state = MockRepository.GeneratePartialMock<CargoState>(state, itinerary);
			state.Expect(s => s.CurrentVoyage).Return(voyage).Repeat.AtLeastOnce();
			state.Expect(s => s.IsUnloadedAtDestination).Return(false).Repeat.AtLeastOnce();
			state.Expect(s => s.LastKnownLocation).Return(lastKnownLocation).Repeat.AtLeastOnce();
			state.Expect(s => s.TransportStatus).Return(TransportStatus.NotReceived).Repeat.AtLeastOnce();
			
			// act:
			ICargo cargo = MockRepository.GeneratePartialMock<TCargo>(state);
			
			// assert:
			Assert.AreSame(identifier, cargo.TrackingId);
			Assert.IsFalse(cargo.Delivery.IsUnloadedAtDestination);
			Assert.AreSame(lastKnownLocation, cargo.Delivery.LastKnownLocation);
			Assert.AreSame(itinerary, cargo.Itinerary);
			Assert.AreSame(voyage, cargo.Delivery.CurrentVoyage);
			Assert.AreSame(route, cargo.RouteSpecification);
			Assert.AreEqual(estimatedTimeOfArrival, cargo.Delivery.EstimatedTimeOfArrival);
			Assert.AreEqual(RoutingStatus.Routed, cargo.Delivery.RoutingStatus);
			Assert.AreEqual(TransportStatus.NotReceived, cargo.Delivery.TransportStatus);
			itinerary.VerifyAllExpectations();
			state.VerifyAllExpectations();
			route.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Ctor_03()
		{
			// assert:
			Assert.Throws<ArgumentNullException>(delegate{ new FakeCargo(null); });
		}
		
		[Test]
		public void Test_AssignToRoute_01()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId identifier = new TrackingId("CARGO01");
			DateTime arrivalDate = DateTime.Now + TimeSpan.FromDays(30);
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(arrivalDate).Repeat.Any();
			mocks.Add(itinerary);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			route.Expect(r => r.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			mocks.Add(route);
			ChangeEventArgs<IItinerary> eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			TCargo underTest = new TCargo(identifier, route);
			underTest.ItineraryChanged += delegate(object sender, ChangeEventArgs<IItinerary> e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			underTest.AssignToRoute(itinerary);
		
			// assert:
			Assert.AreEqual(RoutingStatus.Routed, underTest.Delivery.RoutingStatus);
			Assert.AreSame(itinerary, underTest.Itinerary);
			Assert.AreSame(itinerary, eventArguments.NewValue);
			Assert.IsNull(eventArguments.OldValue);
			Assert.AreSame(eventSender, underTest);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_AssignToRoute_02()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId identifier = new TrackingId("CARGO01");
			DateTime arrivalDate = DateTime.Now + TimeSpan.FromDays(30);
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(arrivalDate).Repeat.Any();
			mocks.Add(itinerary);
			IItinerary itinerary2 = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary2.Expect(i => i.Equals((IItinerary)itinerary)).Return(false).Repeat.Any();
			itinerary2.Expect(i => i.FinalArrivalDate).Return(arrivalDate).Repeat.Any();
			mocks.Add(itinerary2);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			route.Expect(r => r.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			route.Expect(r => r.IsSatisfiedBy(itinerary2)).Return(true).Repeat.AtLeastOnce();
			mocks.Add(route);
			ChangeEventArgs<IItinerary> eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			TCargo underTest = new TCargo(identifier, route);
			underTest.AssignToRoute(itinerary);
			Assert.AreSame(itinerary, underTest.Itinerary);
			underTest.ItineraryChanged += delegate(object sender, ChangeEventArgs<IItinerary> e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			underTest.AssignToRoute(itinerary2);
		
			// assert:
			Assert.AreEqual(RoutingStatus.Routed, underTest.Delivery.RoutingStatus);
			Assert.AreSame(itinerary2, underTest.Itinerary);
			Assert.AreSame(itinerary2, eventArguments.NewValue);
			Assert.AreSame(itinerary, eventArguments.OldValue);
			Assert.AreSame(eventSender, underTest);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_AssignToRoute_03()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId identifier = new TrackingId("CARGO01");
			DateTime arrivalDate = DateTime.Now + TimeSpan.FromDays(30);
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(arrivalDate).Repeat.Any();
			mocks.Add(itinerary);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			route.Expect(r => r.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			mocks.Add(route);
			CargoState mockState1 = MockRepository.GeneratePartialMock<CargoState>(identifier, route);
			CargoState mockState2 = MockRepository.GeneratePartialMock<CargoState>(mockState1, itinerary);
			mockState1.Expect(s => s.AssignToRoute(itinerary)).Return(mockState2).Repeat.Once();
			mockState1.Expect(s => s.Equals(mockState2)).Return(false).Repeat.Any();
			mocks.Add(mockState1);
			mocks.Add(mockState2);
			
			ChangeEventArgs<IItinerary> eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			TCargo underTest = new FakeCargo(mockState1);
			underTest.ItineraryChanged += delegate(object sender, ChangeEventArgs<IItinerary> e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			underTest.AssignToRoute(itinerary);
		
			// assert:
			Assert.AreSame(itinerary, underTest.Itinerary);
			Assert.IsNotNull(eventArguments);
			Assert.AreSame(itinerary, eventArguments.NewValue);
			Assert.AreSame(underTest, eventSender);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_AssignToRoute_04()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId identifier = new TrackingId("CARGO01");
			Exception eThrown = new Exception("Catch me.");
			DateTime arrivalDate = DateTime.Now + TimeSpan.FromDays(30);
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(arrivalDate).Repeat.Any();
			mocks.Add(itinerary);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route);
			CargoState mockState1 = MockRepository.GeneratePartialMock<CargoState>(identifier, route);
			mockState1.Expect(s => s.AssignToRoute(itinerary)).Throw(eThrown).Repeat.Any();
			mocks.Add(mockState1);
			
			ChangeEventArgs<IItinerary> eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			TCargo underTest = new FakeCargo(mockState1);
			underTest.ItineraryChanged += delegate(object sender, ChangeEventArgs<IItinerary> e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			Assert.Throws<Exception>(delegate{ underTest.AssignToRoute(itinerary);},"Catch me.");
		
			// assert:
			Assert.IsNull(underTest.Itinerary);
			Assert.IsNull(eventArguments);
			Assert.IsNull(eventSender);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_SpecifyNewRoute_01()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId identifier = new TrackingId("CARGO01");
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			IRouteSpecification route2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			route.Expect(r => r.Equals(route2)).Return(false).Repeat.AtLeastOnce();
			mocks.Add(route);
			mocks.Add(route2);
			ChangeEventArgs<IRouteSpecification> eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			TCargo underTest = new TCargo(identifier, route);
			underTest.NewRouteSpecified += delegate(object sender, ChangeEventArgs<IRouteSpecification> e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			underTest.SpecifyNewRoute(route2);
		
			// assert:
			Assert.AreEqual(RoutingStatus.NotRouted, underTest.Delivery.RoutingStatus);
			Assert.AreSame(route2, underTest.RouteSpecification);
			Assert.AreSame(route2, eventArguments.NewValue);
			Assert.AreSame(route, eventArguments.OldValue);
			Assert.AreSame(eventSender, underTest);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_SpecifyNewRoute_02()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId identifier = new TrackingId("CARGO01");
			DateTime arrivalDate = DateTime.Now + TimeSpan.FromDays(30);
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(arrivalDate).Repeat.Any();
			mocks.Add(itinerary);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			IRouteSpecification route2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			route.Expect(r => r.Equals(route2)).Return(false).Repeat.AtLeastOnce();
			route.Expect(r => r.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			route2.Expect(r => r.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			mocks.Add(route);
			mocks.Add(route2);
			ChangeEventArgs<IRouteSpecification> eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			TCargo underTest = new TCargo(identifier, route);
			underTest.AssignToRoute(itinerary);
			underTest.NewRouteSpecified += delegate(object sender, ChangeEventArgs<IRouteSpecification> e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			underTest.SpecifyNewRoute(route2);
		
			// assert:
			Assert.AreEqual(RoutingStatus.Routed, underTest.Delivery.RoutingStatus);
			Assert.AreSame(route2, underTest.RouteSpecification);
			Assert.AreSame(route2, eventArguments.NewValue);
			Assert.AreSame(route, eventArguments.OldValue);
			Assert.AreSame(eventSender, underTest);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}	
		
		[Test]
		public void Test_SpecifyNewRoute_03()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId identifier = new TrackingId("CARGO01");
			DateTime arrivalDate = DateTime.Now + TimeSpan.FromDays(30);
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(arrivalDate).Repeat.Any();
			mocks.Add(itinerary);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			IRouteSpecification route2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			route.Expect(r => r.Equals(route2)).Return(false).Repeat.AtLeastOnce();
			route.Expect(r => r.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			route2.Expect(r => r.IsSatisfiedBy(itinerary)).Return(false).Repeat.AtLeastOnce();
			mocks.Add(route);
			mocks.Add(route2);
			ChangeEventArgs<IRouteSpecification> eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			TCargo underTest = new TCargo(identifier, route);
			underTest.AssignToRoute(itinerary);
			underTest.NewRouteSpecified += delegate(object sender, ChangeEventArgs<IRouteSpecification> e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			underTest.SpecifyNewRoute(route2);
		
			// assert:
			Assert.AreEqual(RoutingStatus.Misrouted, underTest.Delivery.RoutingStatus);
			Assert.AreSame(route2, underTest.RouteSpecification);
			Assert.AreSame(route2, eventArguments.NewValue);
			Assert.AreSame(route, eventArguments.OldValue);
			Assert.AreSame(eventSender, underTest);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_SpecifyNewRoute_04()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId identifier = new TrackingId("CARGO01");
			DateTime arrivalDate = DateTime.Now + TimeSpan.FromDays(30);
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(arrivalDate).Repeat.Any();
			mocks.Add(itinerary);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			route.Expect(r => r.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			mocks.Add(route);
			CargoState mockState1 = MockRepository.GeneratePartialMock<CargoState>(identifier, route);
			CargoState mockState2 = MockRepository.GeneratePartialMock<CargoState>(mockState1, itinerary);
			mockState1.Expect(s => s.AssignToRoute(itinerary)).Return(mockState2).Repeat.Once();
			mockState1.Expect(s => s.Equals(mockState2)).Return(false).Repeat.Any();
			mocks.Add(mockState1);
			mocks.Add(mockState2);
			
			ChangeEventArgs<IItinerary> eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			TCargo underTest = new FakeCargo(mockState1);
			underTest.ItineraryChanged += delegate(object sender, ChangeEventArgs<IItinerary> e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			underTest.AssignToRoute(itinerary);
		
			// assert:
			Assert.AreSame(itinerary, underTest.Itinerary);
			Assert.IsNotNull(eventArguments);
			Assert.AreSame(itinerary, eventArguments.NewValue);
			Assert.AreSame(underTest, eventSender);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
	}
}

