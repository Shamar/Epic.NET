//  
//  LegTester.cs
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
using Challenge00.DDDSample.Voyage;
using Rhino.Mocks;
using Challenge00.DDDSample.Location;
using Challenge00.DDDSample.Cargo;
namespace DefaultImplementation.Cargo
{
	[TestFixture()]
	public class LegTester
	{
		[Test]
		public void Test_Ctor_01()
		{
			// arrange:
			DateTime loadTime = DateTime.Now;
			DateTime unloadTime = DateTime.Now + TimeSpan.FromDays(3);
			
			VoyageNumber voyageNumber = new VoyageNumber("VYGTEST");
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.Any();
			
			UnLocode code1 = new UnLocode("CODAA");
			ILocation loc1 = MockRepository.GenerateStrictMock<ILocation>();
			loc1.Expect(l => l.UnLocode).Return(code1).Repeat.Any();

			UnLocode code2 = new UnLocode("CODAB");
			ILocation loc2 = MockRepository.GenerateStrictMock<ILocation>();
			loc2.Expect(l => l.UnLocode).Return(code2).Repeat.Any();
			loc1.Expect(l => l.Equals(loc2)).Return(false).Repeat.Any();

			// act:
			ILeg leg = new Leg(voyage, loc1, loadTime, loc2, unloadTime);
		
			// assert:
			Assert.AreEqual(voyageNumber, leg.Voyage);
			Assert.AreEqual(code1, leg.LoadLocation);
			Assert.AreEqual(code2, leg.UnloadLocation);
			Assert.AreEqual(loadTime, leg.LoadTime);
			Assert.AreEqual(unloadTime, leg.UnloadTime);
		}
		
		[Test]
		public void Test_Ctor_02()
		{
			// arrange:
			DateTime loadTime = DateTime.Now;
			DateTime unloadTime = DateTime.Now + TimeSpan.FromDays(3);
			
			ILocation loc1 = MockRepository.GenerateStrictMock<ILocation>();

			ILocation loc2 = MockRepository.GenerateStrictMock<ILocation>();
		
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new Leg(null, loc1, loadTime, loc2, unloadTime);});
		}
		
		[Test]
		public void Test_Ctor_03()
		{
			// arrange:
			DateTime loadTime = DateTime.Now;
			DateTime unloadTime = DateTime.Now + TimeSpan.FromDays(3);
			
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();

			ILocation loc = MockRepository.GenerateStrictMock<ILocation>();
		
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new Leg(voyage, null, loadTime, loc, unloadTime);});
		}
		
		[Test]
		public void Test_Ctor_04()
		{
			// arrange:
			DateTime loadTime = DateTime.Now;
			DateTime unloadTime = DateTime.Now + TimeSpan.FromDays(3);
			
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();

			ILocation loc = MockRepository.GenerateStrictMock<ILocation>();
		
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new Leg(voyage, loc, loadTime, null, unloadTime);});
		}
		
		
		[Test]
		public void Test_Ctor_05()
		{
			// arrange:
			DateTime loadTime = DateTime.Now;
			DateTime unloadTime = DateTime.Now + TimeSpan.FromDays(3);
			
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();

			ILocation loc1 = MockRepository.GenerateStrictMock<ILocation>();

			ILocation loc2 = MockRepository.GenerateStrictMock<ILocation>();
			loc1.Expect(l => l.Equals(loc2)).Return(true).Repeat.Any();
		
			// assert:
			Assert.Throws<ArgumentException>(delegate { new Leg(voyage, loc1, loadTime, loc2, unloadTime);});
			loc1.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Ctor_06()
		{
			// arrange:
			DateTime loadTime = DateTime.Now;
			DateTime unloadTime = loadTime;
			
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();

			ILocation loc1 = MockRepository.GenerateStrictMock<ILocation>();

			ILocation loc2 = MockRepository.GenerateStrictMock<ILocation>();
		
			// assert:
			Assert.Throws<ArgumentException>(delegate { new Leg(voyage, loc1, loadTime, loc2, unloadTime);});
		}
		
		[Test]
		public void Test_Ctor_07()
		{
			// arrange:
			DateTime loadTime = DateTime.Now;
			DateTime unloadTime = loadTime - TimeSpan.FromDays(1);
			
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();

			ILocation loc1 = MockRepository.GenerateStrictMock<ILocation>();

			ILocation loc2 = MockRepository.GenerateStrictMock<ILocation>();
			loc1.Expect(l => l.Equals(loc2)).Return(false).Repeat.Any();
		
			// assert:
			Assert.Throws<ArgumentException>(delegate { new Leg(voyage, loc1, loadTime, loc2, unloadTime);});
			loc1.VerifyAllExpectations();
		}
	}
}

