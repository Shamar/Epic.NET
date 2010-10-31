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
		#region Ctor
		
		[Test]
		public void Ctor_01()
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
		public void Ctor_02 ()
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
		public void Ctor_03()
		{
			// assert:
			Assert.Throws<ArgumentNullException>(delegate{ new FakeCargo(null); });
		}
				
		#endregion Ctor
		
		#region SpecifyNewRoute

		[Test]
		public void AssignToRoute_01()
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
		public void AssignToRoute_02()
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
		public void AssignToRoute_03()
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
		public void AssignToRoute_04()
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
		public void AssignToRoute_05()
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
			EventHandler<ChangeEventArgs<IItinerary>> handler = delegate(object sender, ChangeEventArgs<IItinerary> e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			TCargo underTest = new FakeCargo(mockState1);
			underTest.ItineraryChanged += handler;
			underTest.ItineraryChanged -= handler;
			underTest.AssignToRoute(itinerary);
		
			// assert:
			Assert.AreSame(itinerary, underTest.Itinerary);
			Assert.IsNull(eventArguments);
			Assert.IsNull(eventSender);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		#endregion AssignToRoute
		
		#region SpecifyNewRoute

		[Test]
		public void SpecifyNewRoute_01()
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
		public void SpecifyNewRoute_02()
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
		public void SpecifyNewRoute_03()
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
		public void SpecifyNewRoute_04()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId identifier = new TrackingId("CARGO01");
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route);
			IRouteSpecification route2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route2);
			CargoState mockState1 = MockRepository.GeneratePartialMock<CargoState>(identifier, route);
			CargoState mockState2 = MockRepository.GeneratePartialMock<CargoState>(mockState1, route2);
			mockState1.Expect(s => s.SpecifyNewRoute(route2)).Return(mockState2).Repeat.Once();
			mockState1.Expect(s => s.Equals(mockState2)).Return(false).Repeat.Any();
			mocks.Add(mockState1);
			mocks.Add(mockState2);
			
			ChangeEventArgs<IRouteSpecification> eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			TCargo underTest = new FakeCargo(mockState1);
			underTest.NewRouteSpecified += delegate(object sender, ChangeEventArgs<IRouteSpecification> e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			underTest.SpecifyNewRoute(route2);
		
			// assert:
			Assert.AreSame(route2, underTest.RouteSpecification);
			Assert.IsNotNull(eventArguments);
			Assert.AreSame(route2, eventArguments.NewValue);
			Assert.AreSame(route, eventArguments.OldValue);
			Assert.AreSame(underTest, eventSender);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void SpecifyNewRoute_05()
		{
			// arrange:
			GList mocks = new GList();
			Exception eThrown = new Exception("Catch me.");
			TrackingId identifier = new TrackingId("CARGO01");
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route);
			IRouteSpecification route2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route2);
			CargoState mockState1 = MockRepository.GeneratePartialMock<CargoState>(identifier, route);
			mockState1.Expect(s => s.SpecifyNewRoute(route2)).Throw(eThrown);
			mocks.Add(mockState1);
			
			ChangeEventArgs<IRouteSpecification> eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			TCargo underTest = new FakeCargo(mockState1);
			underTest.NewRouteSpecified += delegate(object sender, ChangeEventArgs<IRouteSpecification> e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			Assert.Throws<Exception>(delegate { underTest.SpecifyNewRoute(route2); }, "Catch me.");
		
			// assert:
			Assert.AreSame(route, underTest.RouteSpecification);
			Assert.IsNull(eventArguments);
			Assert.IsNull(eventSender);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void SpecifyNewRoute_06()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId identifier = new TrackingId("CARGO01");
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route);
			IRouteSpecification route2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route2);
			CargoState mockState1 = MockRepository.GeneratePartialMock<CargoState>(identifier, route);
			CargoState mockState2 = MockRepository.GeneratePartialMock<CargoState>(mockState1, route2);
			mockState1.Expect(s => s.SpecifyNewRoute(route2)).Return(mockState2).Repeat.Once();
			mockState1.Expect(s => s.Equals(mockState2)).Return(false).Repeat.Any();
			mocks.Add(mockState1);
			mocks.Add(mockState2);
			
			ChangeEventArgs<IRouteSpecification> eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			EventHandler<ChangeEventArgs<IRouteSpecification>> handler = delegate(object sender, ChangeEventArgs<IRouteSpecification> e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			TCargo underTest = new FakeCargo(mockState1);
			underTest.NewRouteSpecified += handler;
			underTest.NewRouteSpecified -= handler;
			underTest.SpecifyNewRoute(route2);
		
			// assert:
			Assert.AreSame(route2, underTest.RouteSpecification);
			Assert.IsNull(eventArguments);
			Assert.IsNull(eventSender);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		#endregion SpecifyNewRoute
		
		#region Recieve

		[Test]
		public void Recieve_01()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId identifier = new TrackingId("CARGO01");
			DateTime arrivalDate = DateTime.Now + TimeSpan.FromDays(30);
			DateTime recieveDate = DateTime.Now + TimeSpan.FromDays(1);
			UnLocode recUnLocode = new UnLocode("RECLC");
			ILocation recLocation = MockRepository.GenerateStrictMock<ILocation>();
			recLocation.Expect(l => l.UnLocode).Return(recUnLocode).Repeat.AtLeastOnce();
			mocks.Add(recLocation);
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(arrivalDate).Repeat.Any();
			itinerary.Expect(i => i.InitialDepartureLocation).Return(recUnLocode).Repeat.Any();
			mocks.Add(itinerary);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			route.Expect(r => r.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			mocks.Add(route);
			HandlingEventArgs eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			TCargo underTest = new TCargo(identifier, route);
			underTest.AssignToRoute(itinerary);
			underTest.Recieved += delegate(object sender, HandlingEventArgs e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			underTest.Recieve(recLocation, recieveDate);
		
			// assert:
			Assert.AreEqual(RoutingStatus.Routed, underTest.Delivery.RoutingStatus);
			Assert.AreEqual(TransportStatus.InPort, underTest.Delivery.TransportStatus);
			Assert.AreSame(recUnLocode, underTest.Delivery.LastKnownLocation);
			Assert.IsNotNull(eventArguments);
			Assert.AreSame(underTest.Delivery.LastKnownLocation, eventArguments.Delivery.LastKnownLocation);
			Assert.AreSame(eventSender, underTest);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Recieve_02()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId identifier = new TrackingId("CARGO01");
			DateTime arrivalDate = DateTime.Now + TimeSpan.FromDays(30);
			DateTime recieveDate = DateTime.Now + TimeSpan.FromDays(1);
			UnLocode recUnLocode = new UnLocode("RECLC");
			ILocation recLocation = MockRepository.GenerateStrictMock<ILocation>();
			recLocation.Expect(l => l.UnLocode).Return(recUnLocode).Repeat.AtLeastOnce();
			mocks.Add(recLocation);
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(arrivalDate).Repeat.Any();
			itinerary.Expect(i => i.InitialDepartureLocation).Return(recUnLocode).Repeat.Any();
			mocks.Add(itinerary);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			route.Expect(r => r.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			mocks.Add(route);
			HandlingEventArgs eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			EventHandler<HandlingEventArgs> handler = delegate(object sender, HandlingEventArgs e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			TCargo underTest = new TCargo(identifier, route);
			underTest.AssignToRoute(itinerary);
			underTest.Recieved += handler;
			underTest.Recieved -= handler;
			underTest.Recieve(recLocation, recieveDate);
		
			// assert:
			Assert.AreEqual(RoutingStatus.Routed, underTest.Delivery.RoutingStatus);
			Assert.AreEqual(TransportStatus.InPort, underTest.Delivery.TransportStatus);
			Assert.AreSame(recUnLocode, underTest.Delivery.LastKnownLocation);
			Assert.IsNull(eventArguments);
			Assert.IsNull(eventSender);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Recieve_03()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId identifier = new TrackingId("CARGO01");
			DateTime date = DateTime.Now + TimeSpan.FromDays(1);
			UnLocode unlocode = new UnLocode("RECLC");
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			mocks.Add(location);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route);
			IRouteSpecification route2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route2);
			CargoState mockState1 = MockRepository.GeneratePartialMock<CargoState>(identifier, route);
			CargoState mockState2 = MockRepository.GeneratePartialMock<CargoState>(mockState1, route2);
			mockState1.Expect(s => s.Recieve(location, date)).Return(mockState2).Repeat.Once();
			mockState1.Expect(s => s.Equals(mockState2)).Return(false).Repeat.Any();
			mockState2.Expect(s => s.LastKnownLocation).Return(unlocode).Repeat.AtLeastOnce();
			mocks.Add(mockState1);
			mocks.Add(mockState2);
			
			HandlingEventArgs eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			TCargo underTest = new FakeCargo(mockState1);
			underTest.Recieved += delegate(object sender, HandlingEventArgs e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			underTest.Recieve(location, date);
		
			// assert:
			Assert.AreSame(unlocode, underTest.Delivery.LastKnownLocation);
			Assert.IsNotNull(eventArguments);
			Assert.AreSame(unlocode, eventArguments.Delivery.LastKnownLocation);
			Assert.AreSame(underTest, eventSender);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Recieve_04()
		{
			// arrange:
			GList mocks = new GList();
			DateTime date = DateTime.Now + TimeSpan.FromDays(1);
			Exception eThrown = new Exception("Catch me.");
			TrackingId identifier = new TrackingId("CARGO01");
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			mocks.Add(location);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route);
			CargoState mockState1 = MockRepository.GeneratePartialMock<CargoState>(identifier, route);
			mockState1.Expect(s => s.Recieve(location, date)).Throw(eThrown);
			mocks.Add(mockState1);
			
			HandlingEventArgs eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			TCargo underTest = new FakeCargo(mockState1);
			underTest.Recieved += delegate(object sender, HandlingEventArgs e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			Assert.Throws<Exception>(delegate { underTest.Recieve(location, date); }, "Catch me.");
		
			// assert:
			Assert.IsNull(eventArguments);
			Assert.IsNull(eventSender);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		#endregion Recieve
		
		#region ClearCustoms
		
		[Test]
		public void ClearCustoms_01()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId identifier = new TrackingId("CARGO01");
			DateTime arrivalDate = DateTime.Now + TimeSpan.FromDays(30);
			DateTime recieveDate = DateTime.Now + TimeSpan.FromDays(1);
			UnLocode recUnLocode = new UnLocode("RECLC");
			ILocation recLocation = MockRepository.GenerateStrictMock<ILocation>();
			recLocation.Expect(l => l.UnLocode).Return(recUnLocode).Repeat.AtLeastOnce();
			mocks.Add(recLocation);
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(arrivalDate).Repeat.Any();
			itinerary.Expect(i => i.InitialDepartureLocation).Return(recUnLocode).Repeat.Any();
			mocks.Add(itinerary);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			route.Expect(r => r.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			mocks.Add(route);
			HandlingEventArgs eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			EventHandler<HandlingEventArgs> handler = delegate(object sender, HandlingEventArgs e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			TCargo underTest = new TCargo(identifier, route);
			underTest.AssignToRoute(itinerary);
			underTest.Recieve(recLocation, recieveDate);
			underTest.CustomsCleared += handler;
			underTest.ClearCustoms(recLocation, recieveDate + TimeSpan.FromHours(6));
		
			// assert:
			Assert.AreEqual(RoutingStatus.Routed, underTest.Delivery.RoutingStatus);
			Assert.AreEqual(TransportStatus.InPort, underTest.Delivery.TransportStatus);
			Assert.AreSame(recUnLocode, underTest.Delivery.LastKnownLocation);
			Assert.IsNotNull(eventArguments);
			Assert.AreSame(underTest.Delivery.LastKnownLocation, eventArguments.Delivery.LastKnownLocation);
			Assert.AreSame(eventSender, underTest);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void ClearCustoms_02()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId identifier = new TrackingId("CARGO01");
			DateTime arrivalDate = DateTime.Now + TimeSpan.FromDays(30);
			DateTime recieveDate = DateTime.Now + TimeSpan.FromDays(1);
			UnLocode recUnLocode = new UnLocode("RECLC");
			ILocation recLocation = MockRepository.GenerateStrictMock<ILocation>();
			recLocation.Expect(l => l.UnLocode).Return(recUnLocode).Repeat.AtLeastOnce();
			mocks.Add(recLocation);
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(arrivalDate).Repeat.Any();
			itinerary.Expect(i => i.InitialDepartureLocation).Return(recUnLocode).Repeat.Any();
			mocks.Add(itinerary);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			route.Expect(r => r.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			mocks.Add(route);
			HandlingEventArgs eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			EventHandler<HandlingEventArgs> handler = delegate(object sender, HandlingEventArgs e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			TCargo underTest = new TCargo(identifier, route);
			underTest.AssignToRoute(itinerary);
			underTest.Recieve(recLocation, recieveDate);
			underTest.CustomsCleared += handler;
			underTest.CustomsCleared -= handler;
			underTest.ClearCustoms(recLocation, recieveDate + TimeSpan.FromHours(6));
		
			// assert:
			Assert.AreEqual(RoutingStatus.Routed, underTest.Delivery.RoutingStatus);
			Assert.AreEqual(TransportStatus.InPort, underTest.Delivery.TransportStatus);
			Assert.AreSame(recUnLocode, underTest.Delivery.LastKnownLocation);
			Assert.IsNull(eventArguments);
			Assert.IsNull(eventSender);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void ClearCustoms_03()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId identifier = new TrackingId("CARGO01");
			DateTime date = DateTime.Now + TimeSpan.FromDays(1);
			UnLocode unlocode = new UnLocode("RECLC");
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			mocks.Add(location);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route);
			IRouteSpecification route2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route2);
			CargoState mockState1 = MockRepository.GeneratePartialMock<CargoState>(identifier, route);
			CargoState mockState2 = MockRepository.GeneratePartialMock<CargoState>(mockState1, route2);
			mockState1.Expect(s => s.ClearCustoms(location, date)).Return(mockState2).Repeat.Once();
			mockState1.Expect(s => s.Equals(mockState2)).Return(false).Repeat.Any();
			mockState2.Expect(s => s.LastKnownLocation).Return(unlocode).Repeat.AtLeastOnce();
			mocks.Add(mockState1);
			mocks.Add(mockState2);
			
			HandlingEventArgs eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			TCargo underTest = new FakeCargo(mockState1);
			underTest.CustomsCleared += delegate(object sender, HandlingEventArgs e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			underTest.ClearCustoms(location, date);
		
			// assert:
			Assert.AreSame(unlocode, underTest.Delivery.LastKnownLocation);
			Assert.IsNotNull(eventArguments);
			Assert.AreSame(unlocode, eventArguments.Delivery.LastKnownLocation);
			Assert.AreSame(underTest, eventSender);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void ClearCustoms_04()
		{
			// arrange:
			GList mocks = new GList();
			DateTime date = DateTime.Now + TimeSpan.FromDays(1);
			Exception eThrown = new Exception("Catch me.");
			TrackingId identifier = new TrackingId("CARGO01");
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			mocks.Add(location);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route);
			
			CargoState mockState1 = MockRepository.GeneratePartialMock<CargoState>(identifier, route);
			mockState1.Expect(s => s.ClearCustoms(location, date)).Throw(eThrown);
			mocks.Add(mockState1);
			
			HandlingEventArgs eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			TCargo underTest = new FakeCargo(mockState1);
			underTest.Recieved += delegate(object sender, HandlingEventArgs e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			Assert.Throws<Exception>(delegate { underTest.ClearCustoms(location, date); }, "Catch me.");
		
			// assert:
			Assert.IsNull(eventArguments);
			Assert.IsNull(eventSender);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		#endregion ClearCustoms
		
		#region LoadOn
		
		[Test]
		public void LoadOn_01()
		{
			// arrange:
			GList mocks = new GList();
			VoyageNumber voyageNumber = new VoyageNumber("VYG001");
			TrackingId identifier = new TrackingId("CARGO01");
			DateTime arrivalDate = DateTime.Now + TimeSpan.FromDays(30);
			DateTime recDate = DateTime.Now + TimeSpan.FromHours(1);
			DateTime loadDate = DateTime.Now + TimeSpan.FromDays(1);
			UnLocode initialLocation = new UnLocode("INITL");
			ILocation recLocation = MockRepository.GenerateStrictMock<ILocation>();
			recLocation.Expect(l => l.UnLocode).Return(initialLocation).Repeat.AtLeastOnce();
			mocks.Add(recLocation);
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.IsMoving).Return(false).Repeat.AtLeastOnce();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.AtLeastOnce();
			voyage.Expect(v => v.LastKnownLocation).Return(initialLocation).Repeat.AtLeastOnce();
			mocks.Add(voyage);
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(arrivalDate).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalLocation).Return(new UnLocode("FINAL")).Repeat.Any();
			itinerary.Expect(i => i.InitialDepartureLocation).Return(initialLocation).Repeat.Any();
			mocks.Add(itinerary);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			route.Expect(r => r.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			mocks.Add(route);
			HandlingEventArgs eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			EventHandler<HandlingEventArgs> handler = delegate(object sender, HandlingEventArgs e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			TCargo underTest = new TCargo(identifier, route);
			underTest.AssignToRoute(itinerary);
			underTest.Recieve(recLocation, recDate);
			underTest.Loaded += handler;
			underTest.LoadOn(voyage, loadDate);
		
			// assert:
			Assert.AreEqual(RoutingStatus.Routed, underTest.Delivery.RoutingStatus);
			Assert.AreEqual(TransportStatus.OnboardCarrier, underTest.Delivery.TransportStatus);
			Assert.AreSame(initialLocation, underTest.Delivery.LastKnownLocation);
			Assert.AreSame(voyageNumber, underTest.Delivery.CurrentVoyage);
			Assert.IsNotNull(eventArguments);
			Assert.AreSame(underTest.Delivery, eventArguments.Delivery);
			Assert.AreSame(eventSender, underTest);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}

		[Test]
		public void LoadOn_02()
		{
			// arrange:
			GList mocks = new GList();
			VoyageNumber voyageNumber = new VoyageNumber("VYG001");
			TrackingId identifier = new TrackingId("CARGO01");
			DateTime arrivalDate = DateTime.Now + TimeSpan.FromDays(30);
			DateTime recDate = DateTime.Now + TimeSpan.FromHours(1);
			DateTime loadDate = DateTime.Now + TimeSpan.FromDays(1);
			UnLocode initialLocation = new UnLocode("INITL");
			ILocation recLocation = MockRepository.GenerateStrictMock<ILocation>();
			recLocation.Expect(l => l.UnLocode).Return(initialLocation).Repeat.AtLeastOnce();
			mocks.Add(recLocation);
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.IsMoving).Return(false).Repeat.AtLeastOnce();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.AtLeastOnce();
			voyage.Expect(v => v.LastKnownLocation).Return(initialLocation).Repeat.AtLeastOnce();
			mocks.Add(voyage);
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(arrivalDate).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalLocation).Return(new UnLocode("FINAL")).Repeat.Any();
			itinerary.Expect(i => i.InitialDepartureLocation).Return(initialLocation).Repeat.Any();
			mocks.Add(itinerary);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			route.Expect(r => r.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			mocks.Add(route);
			HandlingEventArgs eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			EventHandler<HandlingEventArgs> handler = delegate(object sender, HandlingEventArgs e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			TCargo underTest = new TCargo(identifier, route);
			underTest.AssignToRoute(itinerary);
			underTest.Recieve(recLocation, recDate);
			underTest.Loaded += handler;
			underTest.Loaded -= handler;
			underTest.LoadOn(voyage, loadDate);
				
			// assert:
			Assert.AreEqual(RoutingStatus.Routed, underTest.Delivery.RoutingStatus);
			Assert.AreEqual(TransportStatus.OnboardCarrier, underTest.Delivery.TransportStatus);
			Assert.AreSame(initialLocation, underTest.Delivery.LastKnownLocation);
			Assert.AreSame(voyageNumber, underTest.Delivery.CurrentVoyage);
			Assert.IsNull(eventArguments);
			Assert.IsNull(eventSender);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}

		[Test]
		public void LoadOn_03()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId identifier = new TrackingId("CARGO01");
			DateTime date = DateTime.Now + TimeSpan.FromDays(1);
			VoyageNumber voyageNumber = new VoyageNumber("VYG001");
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			mocks.Add(voyage);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route);
			IRouteSpecification route2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route2);
			CargoState mockState1 = MockRepository.GeneratePartialMock<CargoState>(identifier, route);
			CargoState mockState2 = MockRepository.GeneratePartialMock<CargoState>(mockState1, route2);
			mockState1.Expect(s => s.LoadOn(voyage, date)).Return(mockState2).Repeat.Once();
			mockState1.Expect(s => s.Equals(mockState2)).Return(false).Repeat.Any();
			mockState2.Expect(s => s.CurrentVoyage).Return(voyageNumber).Repeat.AtLeastOnce();
			mocks.Add(mockState1);
			mocks.Add(mockState2);
			
			HandlingEventArgs eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			TCargo underTest = new FakeCargo(mockState1);
			underTest.Loaded += delegate(object sender, HandlingEventArgs e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			underTest.LoadOn(voyage, date);
		
			// assert:
			Assert.AreSame(voyageNumber, underTest.Delivery.CurrentVoyage);
			Assert.IsNotNull(eventArguments);
			Assert.AreSame(underTest.Delivery, eventArguments.Delivery);
			Assert.AreSame(underTest, eventSender);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}

		[Test]
		public void LoadOn_04()
		{
			// arrange:
			GList mocks = new GList();
			DateTime date = DateTime.Now + TimeSpan.FromDays(1);
			VoyageNumber voyageNumber = new VoyageNumber("VYG001");
			Exception eThrown = new Exception("Catch me.");
			TrackingId identifier = new TrackingId("CARGO01");
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			mocks.Add(voyage);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route);
			
			IRouteSpecification route2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route2);
			CargoState mockState1 = MockRepository.GeneratePartialMock<CargoState>(identifier, route);
			mockState1.Expect(s => s.LoadOn(voyage, date)).Throw(eThrown);
			mockState1.Expect(s => s.CurrentVoyage).Return(voyageNumber).Repeat.AtLeastOnce();
			mocks.Add(mockState1);
			
			HandlingEventArgs eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			TCargo underTest = new FakeCargo(mockState1);
			underTest.Loaded += delegate(object sender, HandlingEventArgs e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			Assert.Throws<Exception>(delegate { underTest.LoadOn(voyage, date); }, "Catch me.");
		
			// assert:
			Assert.AreSame(voyageNumber, underTest.Delivery.CurrentVoyage);
			Assert.IsNull(eventArguments);
			Assert.IsNull(eventSender);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}

		#endregion LoadOn
		
		#region Unload
		
		[Test]
		public void Unload_01()
		{
			// arrange:
			GList mocks = new GList();
			VoyageNumber voyageNumber = new VoyageNumber("VYG001");
			TrackingId identifier = new TrackingId("CARGO01");
			DateTime arrivalDate = DateTime.Now + TimeSpan.FromDays(30);
			DateTime recDate = DateTime.Now + TimeSpan.FromHours(1);
			DateTime loadDate = DateTime.Now + TimeSpan.FromDays(1);
			UnLocode initialLocation = new UnLocode("INITL");
			UnLocode finalLocation = new UnLocode("FINAL");
			ILocation recLocation = MockRepository.GenerateStrictMock<ILocation>();
			recLocation.Expect(l => l.UnLocode).Return(initialLocation).Repeat.AtLeastOnce();
			mocks.Add(recLocation);
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.IsMoving).Return(false).Repeat.AtLeastOnce();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.AtLeastOnce();
			voyage.Expect(v => v.LastKnownLocation).Return(initialLocation).Repeat.Twice();
			voyage.Expect(v => v.LastKnownLocation).Return(finalLocation).Repeat.Once();
			mocks.Add(voyage);
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(arrivalDate).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalLocation).Return(finalLocation).Repeat.Any();
			itinerary.Expect(i => i.InitialDepartureLocation).Return(initialLocation).Repeat.Any();
			mocks.Add(itinerary);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			route.Expect(r => r.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			mocks.Add(route);
			HandlingEventArgs eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			EventHandler<HandlingEventArgs> handler = delegate(object sender, HandlingEventArgs e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			TCargo underTest = new TCargo(identifier, route);
			underTest.AssignToRoute(itinerary);
			underTest.Recieve(recLocation, recDate);
			underTest.LoadOn(voyage, loadDate);
			underTest.Unloaded += handler;
			underTest.Unload(voyage, loadDate + TimeSpan.FromDays(10));
		
			// assert:
			Assert.AreEqual(RoutingStatus.Routed, underTest.Delivery.RoutingStatus);
			Assert.AreEqual(TransportStatus.InPort, underTest.Delivery.TransportStatus);
			Assert.AreSame(finalLocation, underTest.Delivery.LastKnownLocation);
			Assert.IsTrue(underTest.Delivery.IsUnloadedAtDestination);
			Assert.IsNull(underTest.Delivery.CurrentVoyage);
			Assert.IsNotNull(eventArguments);
			Assert.AreSame(underTest.Delivery, eventArguments.Delivery);
			Assert.AreSame(eventSender, underTest);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Unload_02()
		{
			// arrange:
			GList mocks = new GList();
			VoyageNumber voyageNumber = new VoyageNumber("VYG001");
			TrackingId identifier = new TrackingId("CARGO01");
			DateTime arrivalDate = DateTime.Now + TimeSpan.FromDays(30);
			DateTime recDate = DateTime.Now + TimeSpan.FromHours(1);
			DateTime loadDate = DateTime.Now + TimeSpan.FromDays(1);
			UnLocode initialLocation = new UnLocode("INITL");
			UnLocode finalLocation = new UnLocode("FINAL");
			ILocation recLocation = MockRepository.GenerateStrictMock<ILocation>();
			recLocation.Expect(l => l.UnLocode).Return(initialLocation).Repeat.AtLeastOnce();
			mocks.Add(recLocation);
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.IsMoving).Return(false).Repeat.AtLeastOnce();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.AtLeastOnce();
			voyage.Expect(v => v.LastKnownLocation).Return(initialLocation).Repeat.Twice();
			voyage.Expect(v => v.LastKnownLocation).Return(finalLocation).Repeat.Once();
			mocks.Add(voyage);
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(arrivalDate).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalLocation).Return(finalLocation).Repeat.Any();
			itinerary.Expect(i => i.InitialDepartureLocation).Return(initialLocation).Repeat.Any();
			mocks.Add(itinerary);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			route.Expect(r => r.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			mocks.Add(route);
			HandlingEventArgs eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			EventHandler<HandlingEventArgs> handler = delegate(object sender, HandlingEventArgs e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			TCargo underTest = new TCargo(identifier, route);
			underTest.AssignToRoute(itinerary);
			underTest.Recieve(recLocation, recDate);
			underTest.LoadOn(voyage, loadDate);
			underTest.Unloaded += handler;
			underTest.Unloaded -= handler;
			underTest.Unload(voyage, loadDate + TimeSpan.FromDays(10));
				
			// assert:
			Assert.AreEqual(RoutingStatus.Routed, underTest.Delivery.RoutingStatus);
			Assert.AreEqual(TransportStatus.InPort, underTest.Delivery.TransportStatus);
			Assert.AreSame(finalLocation, underTest.Delivery.LastKnownLocation);
			Assert.IsTrue(underTest.Delivery.IsUnloadedAtDestination);
			Assert.IsNull(underTest.Delivery.CurrentVoyage);
			Assert.IsNull(eventArguments);
			Assert.IsNull(eventSender);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}

		[Test]
		public void Unload_03()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId identifier = new TrackingId("CARGO01");
			DateTime date = DateTime.Now + TimeSpan.FromDays(1);
			VoyageNumber voyageNumber = new VoyageNumber("VYG001");
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			mocks.Add(voyage);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route);
			IRouteSpecification route2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route2);
			CargoState mockState1 = MockRepository.GeneratePartialMock<CargoState>(identifier, route);
			CargoState mockState2 = MockRepository.GeneratePartialMock<CargoState>(mockState1, route2);
			mockState1.Expect(s => s.Unload(voyage, date)).Return(mockState2).Repeat.Once();
			mockState1.Expect(s => s.Equals(mockState2)).Return(false).Repeat.Any();
			mockState2.Expect(s => s.CurrentVoyage).Return(voyageNumber).Repeat.AtLeastOnce();
			mocks.Add(mockState1);
			mocks.Add(mockState2);
			
			HandlingEventArgs eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			TCargo underTest = new FakeCargo(mockState1);
			underTest.Unloaded += delegate(object sender, HandlingEventArgs e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			underTest.Unload(voyage, date);
		
			// assert:
			Assert.AreSame(voyageNumber, underTest.Delivery.CurrentVoyage);
			Assert.IsNotNull(eventArguments);
			Assert.AreSame(underTest.Delivery, eventArguments.Delivery);
			Assert.AreSame(underTest, eventSender);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}

		[Test]
		public void Unload_04()
		{
			// arrange:
			GList mocks = new GList();
			DateTime date = DateTime.Now + TimeSpan.FromDays(1);
			VoyageNumber voyageNumber = new VoyageNumber("VYG001");
			Exception eThrown = new Exception("Catch me.");
			TrackingId identifier = new TrackingId("CARGO01");
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			mocks.Add(voyage);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route);
			
			IRouteSpecification route2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route2);
			CargoState mockState1 = MockRepository.GeneratePartialMock<CargoState>(identifier, route);
			mockState1.Expect(s => s.Unload(voyage, date)).Throw(eThrown);
			mockState1.Expect(s => s.CurrentVoyage).Return(voyageNumber).Repeat.AtLeastOnce();
			mocks.Add(mockState1);
			
			HandlingEventArgs eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			TCargo underTest = new FakeCargo(mockState1);
			underTest.Loaded += delegate(object sender, HandlingEventArgs e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			Assert.Throws<Exception>(delegate { underTest.Unload(voyage, date); }, "Catch me.");
		
			// assert:
			Assert.AreSame(voyageNumber, underTest.Delivery.CurrentVoyage);
			Assert.IsNull(eventArguments);
			Assert.IsNull(eventSender);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}

		#endregion Unload
		
		#region Claim

		[Test]
		public void Claim_01()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId identifier = new TrackingId("CARGO01");
			VoyageNumber voyageNumber = new VoyageNumber("VYG001");
			DateTime arrivalDate = DateTime.Now + TimeSpan.FromDays(30);
			DateTime recieveDate = DateTime.Now + TimeSpan.FromDays(1);
			UnLocode recUnLocode = new UnLocode("RECLC");
			UnLocode endUnLocode = new UnLocode("FINAL");
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.IsMoving).Return(false).Repeat.AtLeastOnce();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.AtLeastOnce();
			voyage.Expect(v => v.LastKnownLocation).Return(recUnLocode).Repeat.Twice();
			voyage.Expect(v => v.LastKnownLocation).Return(endUnLocode).Repeat.Once();
			mocks.Add(voyage);
			ILocation recLocation = MockRepository.GenerateStrictMock<ILocation>();
			recLocation.Expect(l => l.UnLocode).Return(recUnLocode).Repeat.AtLeastOnce();
			mocks.Add(recLocation);
			ILocation clmLocation = MockRepository.GenerateStrictMock<ILocation>();
			clmLocation.Expect(l => l.UnLocode).Return(endUnLocode).Repeat.AtLeastOnce();
			mocks.Add(clmLocation);
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(arrivalDate).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalLocation).Return(endUnLocode).Repeat.Any();
			itinerary.Expect(i => i.InitialDepartureLocation).Return(recUnLocode).Repeat.Any();
			mocks.Add(itinerary);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			route.Expect(r => r.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			mocks.Add(route);
			HandlingEventArgs eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			EventHandler<HandlingEventArgs> handler = delegate(object sender, HandlingEventArgs e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			TCargo underTest = new TCargo(identifier, route);
			underTest.AssignToRoute(itinerary);
			underTest.Recieve(recLocation, recieveDate);
			underTest.LoadOn(voyage, recieveDate + TimeSpan.FromDays(1));
			underTest.Unload(voyage, recieveDate + TimeSpan.FromDays(10));
			underTest.Claimed += handler;
			underTest.Claim(clmLocation, recieveDate + TimeSpan.FromDays(11));

		
			// assert:
			Assert.AreEqual(RoutingStatus.Routed, underTest.Delivery.RoutingStatus);
			Assert.AreEqual(TransportStatus.Claimed, underTest.Delivery.TransportStatus);
			Assert.AreSame(endUnLocode, underTest.Delivery.LastKnownLocation);
			Assert.IsNotNull(eventArguments);
			Assert.AreSame(underTest.Delivery, eventArguments.Delivery);
			Assert.AreSame(eventSender, underTest);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}

		[Test]
		public void Claim_02()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId identifier = new TrackingId("CARGO01");
			VoyageNumber voyageNumber = new VoyageNumber("VYG001");
			DateTime arrivalDate = DateTime.Now + TimeSpan.FromDays(30);
			DateTime recieveDate = DateTime.Now + TimeSpan.FromDays(1);
			UnLocode recUnLocode = new UnLocode("RECLC");
			UnLocode endUnLocode = new UnLocode("FINAL");
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.IsMoving).Return(false).Repeat.AtLeastOnce();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.AtLeastOnce();
			voyage.Expect(v => v.LastKnownLocation).Return(recUnLocode).Repeat.Twice();
			voyage.Expect(v => v.LastKnownLocation).Return(endUnLocode).Repeat.Once();
			mocks.Add(voyage);
			ILocation recLocation = MockRepository.GenerateStrictMock<ILocation>();
			recLocation.Expect(l => l.UnLocode).Return(recUnLocode).Repeat.AtLeastOnce();
			mocks.Add(recLocation);
			ILocation clmLocation = MockRepository.GenerateStrictMock<ILocation>();
			clmLocation.Expect(l => l.UnLocode).Return(endUnLocode).Repeat.AtLeastOnce();
			mocks.Add(clmLocation);
			IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
			itinerary.Expect(i => i.Equals(null)).Return(false).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalDate).Return(arrivalDate).Repeat.Any();
			itinerary.Expect(i => i.FinalArrivalLocation).Return(endUnLocode).Repeat.Any();
			itinerary.Expect(i => i.InitialDepartureLocation).Return(recUnLocode).Repeat.Any();
			mocks.Add(itinerary);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			route.Expect(r => r.IsSatisfiedBy(itinerary)).Return(true).Repeat.AtLeastOnce();
			mocks.Add(route);
			HandlingEventArgs eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			EventHandler<HandlingEventArgs> handler = delegate(object sender, HandlingEventArgs e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			TCargo underTest = new TCargo(identifier, route);
			underTest.AssignToRoute(itinerary);
			underTest.Recieve(recLocation, recieveDate);
			underTest.LoadOn(voyage, recieveDate + TimeSpan.FromDays(1));
			underTest.Unload(voyage, recieveDate + TimeSpan.FromDays(10));
			underTest.Claimed += handler;
			underTest.Claimed -= handler;
			underTest.Claim(clmLocation, recieveDate + TimeSpan.FromDays(11));
		
			// assert:
			Assert.AreEqual(RoutingStatus.Routed, underTest.Delivery.RoutingStatus);
			Assert.AreEqual(TransportStatus.Claimed, underTest.Delivery.TransportStatus);
			Assert.AreSame(endUnLocode, underTest.Delivery.LastKnownLocation);
			Assert.IsNull(eventArguments);
			Assert.IsNull(eventSender);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Claim_03()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId identifier = new TrackingId("CARGO01");
			DateTime date = DateTime.Now + TimeSpan.FromDays(1);
			UnLocode unlocode = new UnLocode("FINAL");
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			mocks.Add(location);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route);
			IRouteSpecification route2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route2);
			CargoState mockState1 = MockRepository.GeneratePartialMock<CargoState>(identifier, route);
			CargoState mockState2 = MockRepository.GeneratePartialMock<CargoState>(mockState1, route2);
			mockState1.Expect(s => s.Claim(location, date)).Return(mockState2).Repeat.Once();
			mockState1.Expect(s => s.Equals(mockState2)).Return(false).Repeat.Any();
			mockState2.Expect(s => s.LastKnownLocation).Return(unlocode).Repeat.AtLeastOnce();
			mockState2.Expect(s => s.TransportStatus).Return(TransportStatus.Claimed).Repeat.AtLeastOnce();
			mocks.Add(mockState1);
			mocks.Add(mockState2);
			
			HandlingEventArgs eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			TCargo underTest = new FakeCargo(mockState1);
			underTest.Claimed += delegate(object sender, HandlingEventArgs e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			underTest.Claim(location, date);
		
			// assert:
			Assert.AreEqual(TransportStatus.Claimed, underTest.Delivery.TransportStatus);
			Assert.AreSame(unlocode, underTest.Delivery.LastKnownLocation);
			Assert.IsNotNull(eventArguments);
			Assert.AreSame(unlocode, eventArguments.Delivery.LastKnownLocation);
			Assert.AreSame(underTest, eventSender);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Claim_04()
		{
			// arrange:
			GList mocks = new GList();
			DateTime date = DateTime.Now + TimeSpan.FromDays(1);
			Exception eThrown = new Exception("Catch me.");
			TrackingId identifier = new TrackingId("CARGO01");
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			mocks.Add(location);
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(route);
			
			CargoState mockState1 = MockRepository.GeneratePartialMock<CargoState>(identifier, route);
			mockState1.Expect(s => s.Claim(location, date)).Throw(eThrown);
			mocks.Add(mockState1);
			
			HandlingEventArgs eventArguments = null;
			ICargo eventSender = null;
		
			// act:
			TCargo underTest = new FakeCargo(mockState1);
			underTest.Claimed += delegate(object sender, HandlingEventArgs e) {
				eventArguments = e;
				eventSender = sender as ICargo;
			};
			Assert.Throws<Exception>(delegate { underTest.Claim(location, date); }, "Catch me.");
		
			// assert:
			Assert.IsNull(eventArguments);
			Assert.IsNull(eventSender);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}

		#endregion Claim

	}
}

