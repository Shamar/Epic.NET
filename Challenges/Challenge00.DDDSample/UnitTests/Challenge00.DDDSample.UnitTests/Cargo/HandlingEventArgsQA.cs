//  
//  HandlingEventArgsTester.cs
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
namespace Contracts.Cargo
{
	[TestFixture]
	public class HandlingEventArgsQA
	{
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Ctor_withNullDelivery_throwsArgumentNullException ()
		{
			// act:
			new HandlingEventArgs(null, DateTime.UtcNow);
		}
		
		[Test]
		public void Ctor_withValidDelivery_works()
		{
			// arrange:
			IDelivery mDelivery = MockRepository.GenerateStrictMock<IDelivery>();
			DateTime cTime = DateTime.UtcNow;
		
			// act:
			HandlingEventArgs args = new HandlingEventArgs(mDelivery, cTime);
		
			// assert:
			Assert.AreEqual(cTime, args.CompletionDate);
			Assert.AreSame(mDelivery, args.Delivery);
		}
	}
}

