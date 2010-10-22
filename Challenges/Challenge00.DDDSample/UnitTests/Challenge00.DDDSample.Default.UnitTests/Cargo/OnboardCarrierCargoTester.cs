//  
//  OnboardCarrierCargoTester.cs
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
using GList = System.Collections.Generic.List<object>;
using Challenge00.DDDSample.Location;

namespace DefaultImplementation.Cargo
{
	[TestFixture]
	public class OnboardCarrierCargoTester
	{
		[Test]
		public void Test_Ctor_01()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId id = new TrackingId("START");
			DateTime loadTime = DateTime.Now;
			VoyageNumber voyageNumber = new VoyageNumber("VYG001");
			UnLocode location = new UnLocode("CURLC");
            IItinerary itinerary = MockRepository.GenerateStrictMock<IItinerary>();
            itinerary.Expect(i => i.Equals(null)).IgnoreArguments().Return(false).Repeat.Any();
            itinerary.Expect(i => i.FinalArrivalDate).Return(DateTime.UtcNow + TimeSpan.FromDays(1)).Repeat.Any();
            itinerary.Expect(i => i.FinalArrivalLocation).Return(new UnLocode("ENDLC")).Repeat.Any();
            mocks.Add(itinerary);
            IRouteSpecification specification = MockRepository.GenerateStrictMock<IRouteSpecification>();
            specification.Expect(s => s.IsSatisfiedBy(itinerary)).Return(true).Repeat.Any();
            CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
            mocks.Add(previousState);
            previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
            mocks.Add(previousState);
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.AtLeastOnce();
			voyage.Expect(v => v.LastKnownLocation).Return(location).Repeat.AtLeastOnce();
			mocks.Add(voyage);
			
			// act:
			CargoState state = new OnboardCarrierCargo(previousState, voyage, loadTime);
		
			// assert:
			Assert.IsNotNull(state);
			Assert.AreSame(voyageNumber, state.CurrentVoyage);
			Assert.AreEqual(TransportStatus.OnboardCarrier, state.TransportStatus);
			Assert.AreSame(location, state.LastKnownLocation);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_SpecifyNewRoute_01()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId id = new TrackingId("START");
			DateTime loadTime = DateTime.Now;
			VoyageNumber voyageNumber = new VoyageNumber("VYG001");
			UnLocode location = new UnLocode("CURLC");
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
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.AtLeastOnce();
			voyage.Expect(v => v.LastKnownLocation).Return(location).Repeat.AtLeastOnce();
			mocks.Add(voyage);
            IRouteSpecification newSpecification = MockRepository.GenerateStrictMock<IRouteSpecification>();
			mocks.Add(newSpecification);
			CargoState state = new OnboardCarrierCargo(previousState, voyage, loadTime);
		
			// assert:
			Assert.Throws<InvalidOperationException>(delegate { state.SpecifyNewRoute(newSpecification); });
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_AssignToRoute_01()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId id = new TrackingId("START");
			DateTime loadTime = DateTime.Now;
			VoyageNumber voyageNumber = new VoyageNumber("VYG001");
			UnLocode location = new UnLocode("CURLC");
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
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.AtLeastOnce();
			voyage.Expect(v => v.LastKnownLocation).Return(location).Repeat.AtLeastOnce();
			mocks.Add(voyage);
            IItinerary newItinerary = MockRepository.GenerateStrictMock<IItinerary>();
			mocks.Add(newItinerary);
			CargoState state = new OnboardCarrierCargo(previousState, voyage, loadTime);
		
			// assert:
			Assert.Throws<InvalidOperationException>(delegate { state.AssignToRoute(newItinerary); });
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Recieve_01()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId id = new TrackingId("START");
			DateTime loadTime = DateTime.Now;
			VoyageNumber voyageNumber = new VoyageNumber("VYG001");
			UnLocode locCode = new UnLocode("CURLC");
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(locCode).Repeat.Any();
			mocks.Add(location);
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
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.AtLeastOnce();
			voyage.Expect(v => v.LastKnownLocation).Return(locCode).Repeat.AtLeastOnce();
			mocks.Add(voyage);
			CargoState state = new OnboardCarrierCargo(previousState, voyage, loadTime);
		
			// assert:
			Assert.Throws<InvalidOperationException>(delegate { state.Recieve(location, DateTime.Now + TimeSpan.FromDays(1)); });
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_ClearCustoms_01()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId id = new TrackingId("START");
			DateTime loadTime = DateTime.Now;
			VoyageNumber voyageNumber = new VoyageNumber("VYG001");
			UnLocode locCode = new UnLocode("CURLC");
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(locCode).Repeat.Any();
			mocks.Add(location);
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
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.AtLeastOnce();
			voyage.Expect(v => v.LastKnownLocation).Return(locCode).Repeat.AtLeastOnce();
			mocks.Add(voyage);
			CargoState state = new OnboardCarrierCargo(previousState, voyage, loadTime);
		
			// assert:
			Assert.Throws<InvalidOperationException>(delegate { state.ClearCustoms(location, DateTime.Now + TimeSpan.FromDays(1)); });
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Claim_01()
		{
			// arrange:
			GList mocks = new GList();
			TrackingId id = new TrackingId("START");
			DateTime loadTime = DateTime.Now;
			VoyageNumber voyageNumber = new VoyageNumber("VYG001");
			UnLocode locCode = new UnLocode("CURLC");
			ILocation location = MockRepository.GenerateStrictMock<ILocation>();
			location.Expect(l => l.UnLocode).Return(locCode).Repeat.Any();
			mocks.Add(location);
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
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.AtLeastOnce();
			voyage.Expect(v => v.LastKnownLocation).Return(locCode).Repeat.AtLeastOnce();
			mocks.Add(voyage);
			CargoState state = new OnboardCarrierCargo(previousState, voyage, loadTime);
		
			// assert:
			Assert.Throws<InvalidOperationException>(delegate { state.Claim(location, DateTime.Now + TimeSpan.FromDays(1)); });
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_LoadOn_01()
		{
			// arrange:
            GList mocks = new GList();

			UnLocode code = new UnLocode("START");
			VoyageNumber voyageNumber = new VoyageNumber("ATEST");
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
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.Any();
			voyage.Expect(v => v.LastKnownLocation).Return(code).Repeat.AtLeastOnce();
			mocks.Add(voyage);
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
            mocks.Add(previousState);
            previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
            mocks.Add(previousState);
            previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			OnboardCarrierCargo state = new OnboardCarrierCargo(previousState, voyage, arrival);
		
			// act:
			CargoState newState = state.LoadOn(voyage, arrival + TimeSpan.FromDays(2));
		
			// assert:
			Assert.AreSame(newState, state);
            foreach (object mock in mocks)
                mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_LoadOn_02()
		{
			// arrange:
            GList mocks = new GList();

			UnLocode code = new UnLocode("START");
			VoyageNumber voyageNumber = new VoyageNumber("ATEST");
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
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.Any();
			voyage.Expect(v => v.LastKnownLocation).Return(code).Repeat.AtLeastOnce();
			mocks.Add(voyage);
			IVoyage voyage2 = MockRepository.GenerateStrictMock<IVoyage>();
			voyage2.Expect(v => v.Number).Return(new VoyageNumber("VYGOTHER")).Repeat.Any();
			mocks.Add(voyage2);
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
            mocks.Add(previousState);
            previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
            mocks.Add(previousState);
            previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			OnboardCarrierCargo state = new OnboardCarrierCargo(previousState, voyage, arrival);
		
			// assert:
			Assert.Throws<InvalidOperationException>(delegate {state.LoadOn(voyage2, arrival + TimeSpan.FromDays(2));});
            foreach (object mock in mocks)
                mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_LoadOn_03()
		{
			// arrange:
            GList mocks = new GList();

			UnLocode code = new UnLocode("START");
			VoyageNumber voyageNumber = new VoyageNumber("ATEST");
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
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.Any();
			voyage.Expect(v => v.LastKnownLocation).Return(code).Repeat.AtLeastOnce();
			mocks.Add(voyage);
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
            mocks.Add(previousState);
            previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
            mocks.Add(previousState);
            previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			OnboardCarrierCargo state = new OnboardCarrierCargo(previousState, voyage, arrival);
		
			// assert:
			Assert.Throws<ArgumentNullException>(delegate {state.LoadOn(null, arrival + TimeSpan.FromDays(2));});
            foreach (object mock in mocks)
                mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Unload_01()
		{
			// arrange:
            GList mocks = new GList();

			UnLocode code = new UnLocode("START");
			VoyageNumber voyageNumber = new VoyageNumber("ATEST");
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
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.Any();
			voyage.Expect(v => v.LastKnownLocation).Return(code).Repeat.AtLeastOnce();
			mocks.Add(voyage);
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
            mocks.Add(previousState);
            previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
            mocks.Add(previousState);
            previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			OnboardCarrierCargo state = new OnboardCarrierCargo(previousState, voyage, arrival);
		
			// assert:
			Assert.Throws<ArgumentNullException>(delegate {state.Unload(null, arrival + TimeSpan.FromDays(2));});
            foreach (object mock in mocks)
                mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Unload_02()
		{
			// arrange:
            GList mocks = new GList();

			UnLocode code = new UnLocode("START");
			VoyageNumber voyageNumber = new VoyageNumber("ATEST");
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
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.Any();
			voyage.Expect(v => v.LastKnownLocation).Return(code).Repeat.AtLeastOnce();
			mocks.Add(voyage);
			IVoyage voyage2 = MockRepository.GenerateStrictMock<IVoyage>();
			voyage2.Expect(v => v.Number).Return(new VoyageNumber("VYGOTHER")).Repeat.Any();
			mocks.Add(voyage2);
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
            mocks.Add(previousState);
            previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
            mocks.Add(previousState);
            previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			OnboardCarrierCargo state = new OnboardCarrierCargo(previousState, voyage, arrival);
		
			// assert:
			Assert.Throws<ArgumentException>(delegate {state.Unload(voyage2, arrival + TimeSpan.FromDays(2));});
            foreach (object mock in mocks)
                mock.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Unload_03()
		{
			// arrange:
            GList mocks = new GList();

			UnLocode code = new UnLocode("START");
			VoyageNumber voyageNumber = new VoyageNumber("ATEST");
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
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.Any();
			voyage.Expect(v => v.LastKnownLocation).Return(code).Repeat.AtLeastOnce();
			voyage.Expect(v => v.NextExpectedLocation).Return(new UnLocode("NEXTL")).Repeat.Any();
			voyage.Expect(v => v.IsMoving).Return(true).Repeat.AtLeastOnce();
			mocks.Add(voyage);
			CargoState previousState = MockRepository.GenerateStrictMock<CargoState>(id, specification);
            mocks.Add(previousState);
            previousState = MockRepository.GenerateStrictMock<CargoState>(previousState, itinerary);
            mocks.Add(previousState);
            previousState.Expect(s => s.LastKnownLocation).Return(code).Repeat.Any();
			OnboardCarrierCargo state = new OnboardCarrierCargo(previousState, voyage, arrival);
		
			// assert:
			Assert.Throws<ArgumentException>(delegate {state.Unload(voyage, arrival + TimeSpan.FromDays(2));});
            foreach (object mock in mocks)
                mock.VerifyAllExpectations();
		}
	}
}

