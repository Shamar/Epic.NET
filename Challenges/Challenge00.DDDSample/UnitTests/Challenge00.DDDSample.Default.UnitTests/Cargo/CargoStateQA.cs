//  
//  CargoStateTester.cs
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
using System.Threading;
namespace DefaultImplementation.Cargo
{
	class FakeState : CargoState
	{
		public Challenge00.DDDSample.Location.UnLocode _lastKnownLocation;

		public bool _isUnloadedAtDestination = true;
		public override bool IsUnloadedAtDestination {
			get {
				return _isUnloadedAtDestination;
			}
		}
		
		#region implemented abstract members of Challenge00.DDDSample.Cargo.CargoState
		public override CargoState SpecifyNewRoute (IRouteSpecification routeSpecification)
		{
			throw new System.NotImplementedException();
		}
		
		
		public override CargoState AssignToRoute (IItinerary itinerary)
		{
			throw new System.NotImplementedException();
		}
		
		
		public override CargoState Recieve (Challenge00.DDDSample.Location.ILocation location, DateTime date)
		{
			throw new System.NotImplementedException();
		}
		
		
		public override CargoState ClearCustoms (Challenge00.DDDSample.Location.ILocation location, DateTime date)
		{
			throw new System.NotImplementedException();
		}
		
		
		public override CargoState Claim (Challenge00.DDDSample.Location.ILocation location, DateTime date)
		{
			throw new System.NotImplementedException();
		}
		
		
		public override CargoState LoadOn (Challenge00.DDDSample.Voyage.IVoyage voyage, DateTime date)
		{
			throw new System.NotImplementedException();
		}
		
		
		public override CargoState Unload (Challenge00.DDDSample.Voyage.IVoyage voyage, DateTime date)
		{
			throw new System.NotImplementedException();
		}
		
		
		public override Challenge00.DDDSample.Voyage.VoyageNumber CurrentVoyage {
			get {
				return new Challenge00.DDDSample.Voyage.VoyageNumber("CURRENTVYG");
			}
		}
		
		
		public override Challenge00.DDDSample.Location.UnLocode LastKnownLocation {
			get {
				return _lastKnownLocation;
			}
		}
		
		
		public override TransportStatus TransportStatus {
			get {
				return TransportStatus.Unknown;
			}
		}
		
		#endregion
		public FakeState(CargoState previousState)
			: base(previousState)
		{
		}
		
		public FakeState(CargoState previousState, IItinerary itinerary)
			: base(previousState, itinerary)
		{
		}
		
		public FakeState(CargoState previousState, IRouteSpecification route)
			: base(previousState, route)
		{
		}
		
		public FakeState (TrackingId identifier, IRouteSpecification route)
			: base(identifier, route)
		{
		}
	}
	
	[TestFixture]
	public class CargoStateQA
	{
		[Test]
		public void Ctor_withNullPreviousState_throwsArgumentNullException()
		{
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new FakeState(null); });
		}
	
		[Test]
		public void Ctor_withNullItinerary_throwsArgumentNullException()
		{
			// arrange:
			TrackingId identifier = new TrackingId("CARGO01");
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			CargoState mock = MockRepository.GeneratePartialMock<CargoState>(identifier, route);
			
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new FakeState(mock, null as IItinerary); });
		}
		
	
		[Test]
		public void Ctor_withNullRoute_throwsArgumentNullException()
		{
			// arrange:
			TrackingId identifier = new TrackingId("CARGO01");
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			CargoState mock = MockRepository.GeneratePartialMock<CargoState>(identifier, route);
			
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new FakeState(mock, null as IRouteSpecification); });
		}
		
		[Test]
		public void Ctor_withValidArgs_works()
		{
			// arrange:
			TrackingId identifier = new TrackingId("CARGO01");
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			
			// act:
			CargoState state = new FakeState(identifier, route);
		
			// assert:
			Assert.AreSame(identifier, state.Identifier);
			Assert.AreSame(route, state.RouteSpecification);
			Assert.AreEqual(RoutingStatus.NotRouted, state.RoutingStatus);
			Assert.IsFalse(state.EstimatedTimeOfArrival.HasValue);
		}
		
		
		[Test]
		public void Ctor_withSameArgs_produceDifferentCalculationDate()
		{
			// arrange:
			TrackingId identifier = new TrackingId("CARGO01");
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			
			// act:
			CargoState state1 = new FakeState(identifier, route);
			Thread.Sleep(100);
			CargoState state2 = new FakeState(identifier, route);
		
			// assert:
			Assert.AreNotEqual(state1.CalculationDate, state2.CalculationDate);
		}
		
		[Test]
		public void ToString_containsStateName()
		{
			// arrange:
			TrackingId identifier = new TrackingId("CARGO01");
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			CargoState state = new FakeState(identifier, route);
		
			// act:
			string stateString = state.ToString();
		
			// assert:
			Assert.IsTrue(stateString.Contains(state.GetType().Name));
		}
		
		[Test]
		public void Equals_withDifferentLastKnownLocation_ifFalse()
		{
			// arrange:
			TrackingId identifier = new TrackingId("CARGO01");
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			FakeState state1 = new FakeState(identifier, route);
			state1._lastKnownLocation = new Challenge00.DDDSample.Location.UnLocode("TESTA");
			FakeState state2 = new FakeState(identifier, route);
			state2._lastKnownLocation = new Challenge00.DDDSample.Location.UnLocode("TESTB");
		
			// act:
		
			// assert:
			Assert.IsFalse(state1.Equals(state2));
		}
		
		[Test]
		public void Equals_withDifferentDestinations_ifFalse()
		{
			// arrange:
			TrackingId identifier = new TrackingId("CARGO01");
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			FakeState state1 = new FakeState(identifier, route);
			state1._lastKnownLocation = new Challenge00.DDDSample.Location.UnLocode("TESTA");
			state1._isUnloadedAtDestination = false;
			FakeState state2 = new FakeState(identifier, route);
			state2._lastKnownLocation = new Challenge00.DDDSample.Location.UnLocode("TESTA");
		
			// act:
		
			// assert:
			Assert.IsFalse(state1.Equals(state2));
		}
		
		[Test]
		public void Equals_withDifferentIdentifiers_ifFalse()
		{
			// arrange:
			TrackingId identifier = new TrackingId("CARGO01");
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			FakeState state1 = new FakeState(identifier, route);
			state1._lastKnownLocation = new Challenge00.DDDSample.Location.UnLocode("TESTA");
			FakeState state2 = new FakeState(new TrackingId("CARGO02"), route);
			state2._lastKnownLocation = new Challenge00.DDDSample.Location.UnLocode("TESTA");
		
			// act:
		
			// assert:
			Assert.IsFalse(state1.Equals(state2));
		}
		
		
		[Test]
		public void Equals_withDifferentRoute_ifFalse()
		{
			// arrange:
			TrackingId identifier = new TrackingId("CARGO01");
			IRouteSpecification route1 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			FakeState state1 = new FakeState(identifier, route1);
			state1._lastKnownLocation = new Challenge00.DDDSample.Location.UnLocode("TESTA");
			IRouteSpecification route2 = MockRepository.GenerateStrictMock<IRouteSpecification>();
			FakeState state2 = new FakeState(identifier, route2);
			state2._lastKnownLocation = new Challenge00.DDDSample.Location.UnLocode("TESTA");
			route2.Expect(s => s.Equals(route1)).Return(false).Repeat.Any();
			route1.Expect(s => s.Equals(route2)).Return(false).Repeat.Any();
			
			// act:
		
			// assert:
			Assert.IsFalse(state1.Equals(state2));
		}
	}
}

