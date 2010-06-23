//  
//  TrackingIdTester.cs
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
using Challenge00.DDDSample.Cargo;
using System.Text;
using Contracts.Shared;

namespace Contracts.Cargo
{
	[TestFixture()]
	public class TrackingIdTester : StringIdentifierTester<TrackingId>
	{
		[Test()]
		public void Test_Constructor_01 ()
		{
			string idString = "test";
			TrackingId id = new TrackingId(idString);
			Assert.AreEqual(idString, id.ToString());
		}
		
		#region implemented abstract members of Contracts.StringIdentifierTester[TrackingId]
		
		protected override TrackingId CreateNewInstance (out string stringUsed)
		{
			stringUsed = new StringBuilder("tes").Append("t").ToString();
			return new TrackingId(stringUsed);
		}
		
		protected override TrackingId CreateDifferentInstance (out string stringUsed)
		{
			stringUsed = new StringBuilder("tes").Append("t2").ToString();
			return new TrackingId(stringUsed);
		}
		
		protected override TrackingId CreateMatchingInstance (out Regex regEx)
		{
			regEx = new Regex("t.*");
			return new TrackingId("test");
		}
		
		
		protected override TrackingId CreateUnMatchingInstance (out Regex regEx)
		{
			regEx = new Regex("z.*");
			return new TrackingId("test");
		}
		
		#endregion

	}
}