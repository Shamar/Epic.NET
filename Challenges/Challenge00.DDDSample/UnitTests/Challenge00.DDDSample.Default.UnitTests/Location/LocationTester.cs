//  
//  LocationTester.cs
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
using Challenge00.DDDSample.Location;
using Rhino.Mocks;
namespace DefaultImplementation.Location
{
	[TestFixture()]
	public class LocationTester
	{
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Ctor_01 ()
		{
			// arrange:
			
		
			// act:
			new Challenge00.DDDSample.Location.Location(null, "Name");
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Ctor_02 ()
		{
			// arrange:
			UnLocode code = new UnLocode("UNLOC");
		
			// act:
			new Challenge00.DDDSample.Location.Location(code, null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Ctor_03 ()
		{
			// arrange:
			UnLocode code = new UnLocode("UNLOC");
		
			// act:
			new Challenge00.DDDSample.Location.Location(code, string.Empty);
		}
		
		[Test]
		public void Test_Ctor_04()
		{
			// arrange:
			UnLocode code = new UnLocode("UNLOC");
			string name = "Test Location";
		
			// act:
			ILocation location = new Challenge00.DDDSample.Location.Location(code, name);
		
			// assert:
			Assert.AreEqual(code, location.UnLocode);
			Assert.AreEqual(name, location.Name);
		}
		
		[Test]
		public void Test_Equals_01()
		{
			// arrange:
			UnLocode code = new UnLocode("UNLOC");
			string name = "Test Location";
			ILocation loc = MockRepository.GenerateStrictMock<ILocation>();
			loc.Expect(l => l.UnLocode).Return(new UnLocode("UNLOC")).Repeat.Once();
		
			// act:
			ILocation location = new Challenge00.DDDSample.Location.Location(code, name);
			bool equalsLocations = location.Equals(loc);
		
			// assert:
			Assert.IsTrue(equalsLocations);
			loc.AssertWasNotCalled(l => l.Name);
			loc.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Equals_02()
		{
			// arrange:
			UnLocode code = new UnLocode("UNLOC");
			string name = "Test Location";
			ILocation loc = MockRepository.GenerateStrictMock<ILocation>();
			loc.Expect(l => l.UnLocode).Return(new UnLocode("UNLOC")).Repeat.Once();
		
			// act:
			ILocation location = new Challenge00.DDDSample.Location.Location(code, name);
			bool equalsObjects = location.Equals((object)loc);
		
			// assert:
			Assert.IsTrue(equalsObjects);
			loc.AssertWasNotCalled(l => l.Name);
			loc.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Equals_03()
		{
			// arrange:
			UnLocode code = new UnLocode("UNLOC");
			string name = "Test Location";
			ILocation loc = MockRepository.GenerateStrictMock<ILocation>();
			loc.Expect(l => l.UnLocode).Return(new UnLocode("UNDIF")).Repeat.Once();
		
			// act:
			ILocation location = new Challenge00.DDDSample.Location.Location(code, name);
			bool equalsObjects = location.Equals((object)loc);
		
			// assert:
			Assert.IsFalse(equalsObjects);
			loc.AssertWasNotCalled(l => l.Name);
			loc.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Equals_04()
		{
			// arrange:
			UnLocode code = new UnLocode("UNLOC");
			string name = "Test Location";
			ILocation loc = MockRepository.GenerateStrictMock<ILocation>();
			loc.Expect(l => l.UnLocode).Return(new UnLocode("UNDIF")).Repeat.Once();
		
			// act:
			ILocation location = new Challenge00.DDDSample.Location.Location(code, name);
			bool equalsLocations = location.Equals(loc);
		
			// assert:
			Assert.IsFalse(equalsLocations);
			loc.AssertWasNotCalled(l => l.Name);
			loc.VerifyAllExpectations();
		}
		
		[Test]
		public void Test_Equals_05()
		{
			// arrange:
			UnLocode code = new UnLocode("UNLOC");
			string name = "Test Location";
			ILocation loc = MockRepository.GenerateStrictMock<ILocation>();
		
			// act:
			ILocation location = new Challenge00.DDDSample.Location.Location(code, name);
			bool equalsLocations = location.Equals(null);
		
			// assert:
			Assert.IsFalse(equalsLocations);
			loc.VerifyAllExpectations();
		}
	}
}

