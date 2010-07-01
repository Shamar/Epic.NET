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
namespace DefaultImplementation.Cargo
{
	[TestFixture()]
	public class LegTester
	{
		[Test]
		public void Test_Ctor_01()
		{
			// arrange:
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
			
		
			// assert:
		
		}
		
		
	}
}

