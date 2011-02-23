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
	public class LegQA
	{
		[Test]
		public void Ctor_01()
		{
			// arrange:
			DateTime loadTime = DateTime.UtcNow;
			DateTime unloadTime = DateTime.UtcNow + TimeSpan.FromDays(3);
			
			VoyageNumber voyageNumber = new VoyageNumber("VYGTEST");
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.Any();
			
			UnLocode code1 = new UnLocode("CODAA");
			ILocation loc1 = MockRepository.GenerateStrictMock<ILocation>();
			loc1.Expect(l => l.UnLocode).Return(code1).Repeat.Any();

			UnLocode code2 = new UnLocode("CODAB");
			ILocation loc2 = MockRepository.GenerateStrictMock<ILocation>();
			loc2.Expect(l => l.UnLocode).Return(code2).Repeat.Any();
			
			voyage.Expect(v => v.WillStopOverAt(loc1)).Return(true).Repeat.AtLeastOnce();
			voyage.Expect(v => v.WillStopOverAt(loc2)).Return(true).Repeat.AtLeastOnce();
			
			// act:
			ILeg leg = new Leg(voyage, loc1, loadTime, loc2, unloadTime);
		
			// assert:
			Assert.IsTrue(leg.Equals(leg));
			Assert.IsTrue(leg.Equals((object)leg));
			Assert.IsFalse(leg.Equals(null));
			Assert.AreEqual(voyageNumber, leg.Voyage);
			Assert.AreEqual(code1, leg.LoadLocation);
			Assert.AreEqual(code2, leg.UnloadLocation);
			Assert.AreEqual(loadTime, leg.LoadTime);
			Assert.AreEqual(unloadTime, leg.UnloadTime);
			voyage.VerifyAllExpectations();
			loc1.VerifyAllExpectations();
			loc2.VerifyAllExpectations();
		}
		
		[Test]
		public void Ctor_02()
		{
			// arrange:
			DateTime loadTime = DateTime.UtcNow;
			DateTime unloadTime = DateTime.UtcNow + TimeSpan.FromDays(3);
			
			ILocation loc1 = MockRepository.GenerateStrictMock<ILocation>();

			ILocation loc2 = MockRepository.GenerateStrictMock<ILocation>();
		
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new Leg(null, loc1, loadTime, loc2, unloadTime);});
		}
		
		[Test]
		public void Ctor_03()
		{
			// arrange:
			DateTime loadTime = DateTime.UtcNow;
			DateTime unloadTime = DateTime.UtcNow + TimeSpan.FromDays(3);
			
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();

			ILocation loc = MockRepository.GenerateStrictMock<ILocation>();
		
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new Leg(voyage, null, loadTime, loc, unloadTime);});
		}
		
		[Test]
		public void Ctor_04()
		{
			// arrange:
			DateTime loadTime = DateTime.UtcNow;
			DateTime unloadTime = DateTime.UtcNow + TimeSpan.FromDays(3);
			
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();

			ILocation loc = MockRepository.GenerateStrictMock<ILocation>();
		
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new Leg(voyage, loc, loadTime, null, unloadTime);});
		}
		
		
		[Test]
		public void Ctor_05()
		{
			// arrange:
			UnLocode code = new UnLocode("UNCOD");
			DateTime loadTime = DateTime.UtcNow;
			DateTime unloadTime = DateTime.UtcNow + TimeSpan.FromDays(3);
			
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();

			ILocation loc1 = MockRepository.GenerateStrictMock<ILocation>();
			loc1.Expect(l => l.UnLocode).Return(code).Repeat.Any();

			ILocation loc2 = MockRepository.GenerateStrictMock<ILocation>();
			loc2.Expect(l => l.UnLocode).Return(code).Repeat.Any();
		
			// assert:
			Assert.Throws<ArgumentException>(delegate { new Leg(voyage, loc1, loadTime, loc2, unloadTime);});
			loc1.VerifyAllExpectations();
			loc2.VerifyAllExpectations();
		}
		
		[Test]
		public void Ctor_06()
		{
			// arrange:
			UnLocode code1 = new UnLocode("UNFST");
			UnLocode code2 = new UnLocode("UNSND");
			DateTime loadTime = DateTime.UtcNow;
			DateTime unloadTime = loadTime;
			
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();

			ILocation loc1 = MockRepository.GenerateStrictMock<ILocation>();
			loc1.Expect(l => l.UnLocode).Return(code1).Repeat.Any();

			ILocation loc2 = MockRepository.GenerateStrictMock<ILocation>();
			loc2.Expect(l => l.UnLocode).Return(code2).Repeat.Any();
		
			// assert:
			Assert.Throws<ArgumentException>(delegate { new Leg(voyage, loc1, loadTime, loc2, unloadTime);});
			loc1.VerifyAllExpectations();
			loc2.VerifyAllExpectations();
		}
		
		[Test]
		public void Ctor_07()
		{
			// arrange:
			UnLocode code1 = new UnLocode("UNFST");
			UnLocode code2 = new UnLocode("UNSND");
			DateTime loadTime = DateTime.UtcNow;
			DateTime unloadTime = loadTime - TimeSpan.FromDays(1);
			
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();

			ILocation loc1 = MockRepository.GenerateStrictMock<ILocation>();
			loc1.Expect(l => l.UnLocode).Return(code1).Repeat.Any();

			ILocation loc2 = MockRepository.GenerateStrictMock<ILocation>();
			loc2.Expect(l => l.UnLocode).Return(code2).Repeat.Any();
		
			// assert:
			Assert.Throws<ArgumentException>(delegate { new Leg(voyage, loc1, loadTime, loc2, unloadTime);});
			loc1.VerifyAllExpectations();
			loc2.VerifyAllExpectations();
		}
		
		[Test]
		public void Ctor_08()
		{
			// arrange:
			UnLocode code1 = new UnLocode("UNFST");
			UnLocode code2 = new UnLocode("UNSND");
			DateTime loadTime = DateTime.UtcNow;
			DateTime unloadTime = loadTime + TimeSpan.FromDays(1);
			
			ILocation loc1 = MockRepository.GenerateStrictMock<ILocation>();
			loc1.Expect(l => l.UnLocode).Return(code1).Repeat.AtLeastOnce();
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.Number).Return(new VoyageNumber("VYGTEST")).Repeat.Once();

			ILocation loc2 = MockRepository.GenerateStrictMock<ILocation>();
			loc2.Expect(l => l.UnLocode).Return(code2).Repeat.Any();
			voyage.Expect(v => v.WillStopOverAt(loc1)).Return(false).Repeat.Once();
		
			// assert:
			Assert.Throws<ArgumentException>(delegate { 
				new Leg(voyage, loc1, loadTime, loc2, unloadTime);
			});
			voyage.VerifyAllExpectations();
			loc1.VerifyAllExpectations();
			loc2.VerifyAllExpectations();
		}
		
		[Test]
		public void Ctor_09()
		{
			// arrange:
			UnLocode code1 = new UnLocode("UNFST");
			DateTime loadTime = DateTime.UtcNow;
			DateTime unloadTime = loadTime + TimeSpan.FromDays(1);
			
			ILocation loc1 = MockRepository.GenerateStrictMock<ILocation>();
			loc1.Expect(l => l.UnLocode).Return(code1).Repeat.AtLeastOnce();

			ILocation loc2 = MockRepository.GenerateStrictMock<ILocation>();
			loc2.Expect(l => l.UnLocode).Return(new UnLocode("SNDLC")).Repeat.AtLeastOnce();
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.Number).Return(new VoyageNumber("VYGTEST")).Repeat.Once();
			voyage.Expect(v => v.WillStopOverAt(loc1)).Return(true).Repeat.Once();
			voyage.Expect(v => v.WillStopOverAt(loc2)).Return(false).Repeat.Once();
		
			// assert:
			Assert.Throws<ArgumentException>(delegate { new Leg(voyage, loc1, loadTime, loc2, unloadTime);});
			voyage.VerifyAllExpectations();
			loc1.VerifyAllExpectations();
			loc2.VerifyAllExpectations();
		}
		
		[Test]
		public void Equals_01()
		{
			// arrange:
			DateTime loadTime = DateTime.UtcNow;
			DateTime unloadTime = DateTime.UtcNow + TimeSpan.FromDays(3);
			
			VoyageNumber voyageNumber = new VoyageNumber("VYGTEST");
			IVoyage voyage = MockRepository.GenerateStrictMock<IVoyage>();
			voyage.Expect(v => v.Number).Return(voyageNumber).Repeat.Any();
			
			UnLocode code1 = new UnLocode("CODAA");
			ILocation loc1 = MockRepository.GenerateStrictMock<ILocation>();
			loc1.Expect(l => l.UnLocode).Return(code1).Repeat.Any();

			UnLocode code2 = new UnLocode("CODAB");
			ILocation loc2 = MockRepository.GenerateStrictMock<ILocation>();
			loc2.Expect(l => l.UnLocode).Return(code2).Repeat.Any();
			
			voyage.Expect(v => v.WillStopOverAt(loc1)).Return(true).Repeat.AtLeastOnce();
			voyage.Expect(v => v.WillStopOverAt(loc2)).Return(true).Repeat.AtLeastOnce();
			
			// act:
			ILeg leg1 = new Leg(voyage, loc1, loadTime, loc2, unloadTime);
			ILeg leg2 = new Leg(voyage, loc1, loadTime, loc2, unloadTime);
			ILeg leg3 = MockRepository.GenerateStrictMock<ILeg>();
			leg3.Expect(l => l.Voyage).Return(voyageNumber).Repeat.AtLeastOnce();
			leg3.Expect(l => l.LoadLocation).Return(code1).Repeat.AtLeastOnce();
			leg3.Expect(l => l.UnloadLocation).Return(code2).Repeat.AtLeastOnce();
			leg3.Expect(l => l.LoadTime).Return(loadTime).Repeat.AtLeastOnce();
			leg3.Expect(l => l.UnloadTime).Return(unloadTime).Repeat.AtLeastOnce();
		
			// assert:
			Assert.AreEqual(leg1.GetHashCode(), leg2.GetHashCode());
			Assert.IsTrue(leg1.Equals(leg2));
			Assert.IsTrue(leg1.Equals(leg3));
			Assert.IsTrue(leg2.Equals(leg1));
			Assert.IsTrue(leg2.Equals(leg3));
			Assert.IsTrue(leg1.Equals((object)leg2));
			Assert.IsTrue(leg1.Equals((object)leg3));
			Assert.IsTrue(leg2.Equals((object)leg1));
			Assert.IsTrue(leg2.Equals((object)leg3));
			voyage.VerifyAllExpectations();
			loc1.VerifyAllExpectations();
			loc2.VerifyAllExpectations();
			leg3.VerifyAllExpectations();
		}
		
	}
}

