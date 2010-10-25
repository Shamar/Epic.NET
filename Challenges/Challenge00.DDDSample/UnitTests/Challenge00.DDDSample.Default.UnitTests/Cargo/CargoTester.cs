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
using TCargo = Challenge00.DDDSample.Cargo.Cargo;

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
	}
}

