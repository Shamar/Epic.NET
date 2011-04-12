//  
//  UsernameQA.cs
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
using System;
using NUnit.Framework;
using Contracts.Shared;
using Challenge00.DDDSample;
using System.Text;
using System.Text.RegularExpressions;
namespace Contracts
{
	[TestFixture]
	public class UsernameQA : StringIdentifierQA<Username>
	{
		#region implemented abstract members of Contracts.Shared.StringIdentifierQA[Username]
		protected override Username CreateNewInstance (out string stringUsed)
		{
			stringUsed = new StringBuilder("Giac").Append("omo").ToString();
			return new Username(stringUsed);
		}
		
		
		protected override Username CreateDifferentInstance (out string stringUsed)
		{
			stringUsed = new StringBuilder("Pasqu").Append("ale").ToString();
			return new Username(stringUsed);
		}
		
		
		protected override Username CreateMatchingInstance (out System.Text.RegularExpressions.Regex regEx)
		{
			regEx = new Regex("T.*");
			return new Username("Tesio");
		}
		
		
		protected override Username CreateUnMatchingInstance (out System.Text.RegularExpressions.Regex regEx)
		{
			regEx = new Regex("B.*");
			return new Username("Tesio");
		}
		
		#endregion
		
		[Test()]
		public void Ctor_withValidString_works ()
		{
			string idString = "test";
			new Username(idString);
		}
		
		[Test]
		public void Ctor_withNull_throwsArgumentNullException()
		{
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new Username(null); });
		}
		
		[Test]
		public void Ctor_withEmptyString_throwsArgumentNullException()
		{
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new Username(string.Empty); });
		}
	}
}

