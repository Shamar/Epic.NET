//  
//  EventQA.cs
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
using NUnit.Framework;
using System;
using Challenge00.DDDSample.Cargo;
using Rhino.Mocks;
using Challenge00.DDDSample.Cargo.Handling;
using Challenge00.DDDSample;
using System.Collections.Generic;
using Challenge00.DDDSample.Location;
using Challenge00.DDDSample.Voyage;
namespace DefaultImplementation.Cargo.Handling
{
	[TestFixture]
	public class EventQA
	{
		[TestCase(EventType.Claim)]
		[TestCase(EventType.Customs)]
		[TestCase(EventType.Load)]
		[TestCase(EventType.Receive)]
		[TestCase(EventType.Unload)]
		public void Ctor_withNullUser_throwsArgumentNullException(EventType type)
		{
			// arrange:
			ICargo cargo = MockRepository.GenerateStrictMock<ICargo>();
			DateTime completionDate = DateTime.UtcNow;
		
			// assert:
			Assert.Throws<ArgumentNullException>(delegate{ new Event(null, cargo, type, completionDate); });
		}
		
		[TestCase(EventType.Claim)]
		[TestCase(EventType.Customs)]
		[TestCase(EventType.Load)]
		[TestCase(EventType.Receive)]
		[TestCase(EventType.Unload)]
		public void Ctor_withNullCargo_throwsArgumentNullException(EventType type)
		{
			// arrange:
			IUser user = MockRepository.GenerateStrictMock<IUser>();
			DateTime completionDate = DateTime.UtcNow;
		
			// assert:
			Assert.Throws<ArgumentNullException>(delegate{ new Event(user, null, type, completionDate); });
		}
		
		[TestCase(EventType.Claim)]
		[TestCase(EventType.Customs)]
		[TestCase(EventType.Load)]
		[TestCase(EventType.Receive)]
		[TestCase(EventType.Unload)]
		public void Ctor_withValidArgs_works(EventType type)
		{
			// arrange:
			List<object> mocks = new List<object>();
			Username userId = new Username("Giacomo");
			IUser user = MockRepository.GenerateStrictMock<IUser>();
			user.Expect(u => u.Username).Return(userId).Repeat.Once();
			mocks.Add(user);
			TrackingId cargoId = new TrackingId("CARGO001");
			UnLocode location = new UnLocode("UNLOC");
			VoyageNumber voyage = new VoyageNumber("VYG001");
			ICargo cargo = MockRepository.GenerateStrictMock<ICargo>();
			cargo.Expect(c => c.TrackingId).Return(cargoId).Repeat.Once();
			IDelivery delivery = MockRepository.GenerateStrictMock<IDelivery>();
			delivery.Expect(d => d.LastKnownLocation).Return(location).Repeat.Once();
			delivery.Expect(d => d.CurrentVoyage).Return(voyage).Repeat.Once();
			mocks.Add(delivery);
			cargo.Expect(c => c.Delivery).Return(delivery).Repeat.Twice();
			mocks.Add(cargo);
			DateTime completionDate = DateTime.UtcNow;
			
			// act:
			IEvent underTest = new Event(user, cargo, type, completionDate);
		
			// assert:
			Assert.AreSame(userId, underTest.User);
			Assert.AreSame(cargoId, underTest.Cargo);
			Assert.AreSame(location, underTest.Location);
			Assert.AreSame(voyage, underTest.Voyage);
			Assert.AreEqual(completionDate, underTest.Date);
			Assert.AreEqual(type, underTest.Type);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
	}
}

