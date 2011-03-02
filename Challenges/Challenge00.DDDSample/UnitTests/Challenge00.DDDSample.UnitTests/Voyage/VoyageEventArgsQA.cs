//  
//  VoyageEventArgsTester.cs
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
using Challenge00.DDDSample.Voyage;
namespace Contracts.Voyage
{
	[TestFixture()]
	public class VoyageEventArgsQA
	{
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Ctor_withNullPreviousLocation_throwsArgumentNullException ()
		{
			// arrange:
			UnLocode destination = new UnLocode("CDDES");
		
			// act:
			new VoyageEventArgs(null, destination);
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Ctor_withNullDestination_throwsArgumentNullException ()
		{
			// arrange:
			UnLocode previous = new UnLocode("CDPRV");
		
			// act:
			new VoyageEventArgs(previous, null);
		}
		
		[Test]
		public void Ctor_withValidLocations_works()
		{
			// arrange:
			UnLocode previous = new UnLocode("CDPRV");
			UnLocode destination = new UnLocode("CDDES");
			
			// act:
			VoyageEventArgs args = new VoyageEventArgs(previous, destination);
		
			// assert:
			Assert.AreSame(previous, args.PreviousLocation);
			Assert.AreSame(destination, args.DestinationLocation);
		}
	}
}

