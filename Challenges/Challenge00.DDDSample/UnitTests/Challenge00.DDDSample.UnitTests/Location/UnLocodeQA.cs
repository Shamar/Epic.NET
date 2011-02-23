//  
//  UnLocodeTester.cs
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
using System.Text.RegularExpressions;
using Challenge00.DDDSample.Location;
using Contracts.Shared;
namespace Contracts.Location
{
	[TestFixture()]
	public class UnLocodeQA : StringIdentifierQA<UnLocode>
	{
		#region implemented abstract members of Contracts.StringIdentifierTester[UnLocode]
		
		protected override UnLocode CreateNewInstance (out string stringUsed)
		{
			stringUsed = "ITLBA";
			return new UnLocode(stringUsed);
		}
		
		
		protected override UnLocode CreateDifferentInstance (out string stringUsed)
		{
			stringUsed = "ITTRN";
			return new UnLocode(stringUsed);
		}
		
				
		protected override UnLocode CreateMatchingInstance (out Regex regEx)
		{
			regEx = new Regex("IT.*");
			return new UnLocode("ITLBA");
		}
		
		
		protected override UnLocode CreateUnMatchingInstance (out Regex regEx)
		{
			regEx = new Regex("US.*");
			return new UnLocode("ITTRN");
		}
		
		#endregion
		
		[Test]
		public void Ctor_01()
		{
			// act:
			Assert.Throws<ArgumentNullException>(delegate { new UnLocode(null); });
			Assert.Throws<ArgumentNullException>(delegate { new UnLocode(string.Empty); });
		}
		
		[Test]
		public void Ctor_02()
		{
			// act:
			Assert.Throws<ArgumentException>(delegate { new UnLocode("TEST OF INVALID LOCODE"); });
		}
	}
}

