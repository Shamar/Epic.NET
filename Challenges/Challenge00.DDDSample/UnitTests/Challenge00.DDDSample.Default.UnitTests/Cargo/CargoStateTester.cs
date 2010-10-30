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
namespace DefaultImplementation.Cargo
{
	class FakeState : CargoState
	{
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
				throw new System.NotImplementedException();
			}
		}
		
		
		public override Challenge00.DDDSample.Location.UnLocode LastKnownLocation {
			get {
				throw new System.NotImplementedException();
			}
		}
		
		
		public override TransportStatus TransportStatus {
			get {
				throw new System.NotImplementedException();
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
	}
	
	[TestFixture]
	public class CargoStateTester
	{
		[Test]
		public void Test_Ctor_01()
		{
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new FakeState(null); });
		}
	
		[Test]
		public void Test_Ctor_02()
		{
			// arrange:
			TrackingId identifier = new TrackingId("CARGO01");
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			CargoState mock = MockRepository.GeneratePartialMock<CargoState>(identifier, route);
			
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new FakeState(mock, null as IItinerary); });
		}
		
	
		[Test]
		public void Test_Ctor_03()
		{
			// arrange:
			TrackingId identifier = new TrackingId("CARGO01");
			IRouteSpecification route = MockRepository.GenerateStrictMock<IRouteSpecification>();
			CargoState mock = MockRepository.GeneratePartialMock<CargoState>(identifier, route);
			
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new FakeState(mock, null as IRouteSpecification); });
		}
	}
}

