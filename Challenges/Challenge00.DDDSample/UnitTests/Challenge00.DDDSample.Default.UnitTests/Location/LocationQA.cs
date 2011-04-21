//  
//  LocationTester.cs
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
using Challenge00.DDDSample.Location;
using Rhino.Mocks;
namespace DefaultImplementation.Location
{
	[TestFixture()]
	public class LocationQA
	{
		[Test]
		public void Ctor_withNullIdentifier_throwsArgumentNullException ()
		{
			// assert:
			Assert.Throws<ArgumentNullException>(delegate{ new Challenge00.DDDSample.Location.Location(null, "Name"); });
		}
		
		[Test]
		public void Ctor_withNullName_throwsArgumentNullException ()
		{
			// arrange:
			UnLocode code = new UnLocode("UNLOC");
		
			// assert:
			Assert.Throws<ArgumentNullException>(delegate{ new Challenge00.DDDSample.Location.Location(code, null); });
		}

		[Test]
		public void Ctor_withEmptyName_throwsArgumentNullException ()
		{
			// arrange:
			UnLocode code = new UnLocode("UNLOC");
		
			// act:
			Assert.Throws<ArgumentNullException>(delegate{ new Challenge00.DDDSample.Location.Location(code, string.Empty); });
		}
		
		[Test]
		public void Ctor_withValidArguments_works()
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
	}
}

