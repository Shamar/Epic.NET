//  
//  VoyageNumberTester.cs
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
using System.Text.RegularExpressions;
using Challenge00.DDDSample.Voyage;
using Contracts.Shared;
namespace Contracts.Voyage
{
	[TestFixture]
	public class VoyageNumberTester : StringIdentifierTester<VoyageNumber>
	{
		#region implemented abstract members of Contracts.StringIdentifierTester[UnLocode]
		
		protected override VoyageNumber CreateNewInstance (out string stringUsed)
		{
			stringUsed = "VG01";
			return new VoyageNumber(stringUsed);
		}
		
		
		protected override VoyageNumber CreateDifferentInstance (out string stringUsed)
		{
			stringUsed = "VG02";
			return new VoyageNumber(stringUsed);
		}
		
				
		protected override VoyageNumber CreateMatchingInstance (out Regex regEx)
		{
			regEx = new Regex("VG*");
			return new VoyageNumber("VG123123");
		}
		
		
		protected override VoyageNumber CreateUnMatchingInstance (out Regex regEx)
		{
			regEx = new Regex("XX*");
			return new VoyageNumber("VG123123");
		}
		
		#endregion
		
		[Test]
		public void Test_Ctor_01()
		{
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new VoyageNumber(null); });
			Assert.Throws<ArgumentNullException>(delegate { new VoyageNumber(string.Empty); });
		}
		
	}
}

