//  
//  StringIdentifierTester.cs
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
namespace Challenge00.DDDSample.UnitTests
{
	public abstract class StringIdentifierTester<TIdentifier>
		where TIdentifier : StringIdentifier<TIdentifier>
	{
		protected abstract TIdentifier CreateNewInstance(out string stringUsed);

		protected abstract TIdentifier CreateDifferentInstance(out string stringUsed);
		
		protected abstract TIdentifier CreateMatchingInstance(out Regex regEx);
		
		protected abstract TIdentifier CreateUnMatchingInstance(out Regex regEx);
		
		[Test()]
		public void Test_ToString_01 ()
		{
			string idString = null;
			TIdentifier id = CreateNewInstance(out idString);
			
			Assert.AreEqual(idString, id.ToString());
		}
		
		[Test()]
		public void Test_Equals_01 ()
		{
			string idString = null;
			TIdentifier id = CreateNewInstance(out idString);
			
			Assert.IsFalse(id.Equals(null));
		}
		
		[Test()]
		public void Test_Equals_02 ()
		{
			string idString = null;
			TIdentifier id = CreateNewInstance(out idString);
			
			Assert.IsTrue(id.Equals(id));
		}
		
		[Test()]
		public void Test_Equals_03 ()
		{
			string idString = null;
			TIdentifier id1 = CreateNewInstance(out idString);
			TIdentifier id2 = CreateNewInstance(out idString);
			
			Assert.IsTrue(id1.Equals(id2));
		}
		
		[Test()]
		public void Test_Equals_04 ()
		{
			string idString = null;
			string idString2 = null;
			TIdentifier id1 = CreateNewInstance(out idString);
			TIdentifier id2 = CreateNewInstance(out idString2);
			
			Assert.IsTrue(id1.Equals(id2));
		}
		
		[Test()]
		public void Test_Equals_05 ()
		{
			string idString = null;
			string idString2 = null;
			TIdentifier id1 = CreateNewInstance(out idString);
			TIdentifier id2 = CreateDifferentInstance(out idString2);
			
			Assert.IsFalse(id1.Equals(id2));
		}
		
		[Test()]
		public void Test_Operator_01()
		{
			string idString = null;
			string idString2 = null;
			TIdentifier id1 = CreateNewInstance(out idString);
			TIdentifier id2 = CreateNewInstance(out idString2);
			
			Assert.IsTrue(id1 == id2);
		}
		
		[Test()]
		public void Test_Operator_02()
		{
			string idString = null;
			string idString2 = null;
			TIdentifier id1 = CreateNewInstance(out idString);
			TIdentifier id2 = CreateDifferentInstance(out idString2);
			
			Assert.IsTrue(id1 != id2);
		}
		
		[Test()]
		public void Test_Operator_03()
		{
			string idString = null;
			TIdentifier id1 = CreateNewInstance(out idString);
#pragma warning disable 1718
			Assert.IsTrue(id1 == id1);
#pragma warning restore 1718
		}
		
		[Test()]
		public void Test_Operator_04()
		{
			string idString = null;
			TIdentifier id1 = CreateNewInstance(out idString);
			
			Assert.IsTrue(id1 != null);
		}
		
		[Test()]
		public void Test_Contains_01 ()
		{
			string idString = null;
			TIdentifier id = CreateNewInstance(out idString);
			
			Assert.IsFalse(id.Contains(new Guid().ToString()));
		}
		
		[Test()]
		public void Test_Contains_02 ()
		{
			string idString = null;
			TIdentifier id = CreateNewInstance(out idString);
			
			Assert.IsTrue(id.Contains(idString.Substring(1, 2)));
		}
		
		[Test()]
		public void Test_StartsWith_01 ()
		{
			string idString = null;
			TIdentifier id = CreateNewInstance(out idString);
			
			Assert.IsFalse(id.StartsWith(idString.Substring(1, 2)));
		}
		
		[Test()]
		public void Test_StartsWith_02 ()
		{
			string idString = null;
			TIdentifier id = CreateNewInstance(out idString);
			
			Assert.IsTrue(id.StartsWith(idString.Substring(0, 2)));
		}
		
		[Test()]
		public void Test_EndsWith_01 ()
		{
			string idString = null;
			TIdentifier id = CreateNewInstance(out idString);
			
			Assert.IsFalse(id.EndsWith(new Guid().ToString()));
		}
		
		[Test()]
		public void Test_EndsWith_02 ()
		{
			string idString = null;
			TIdentifier id = CreateNewInstance(out idString);
			
			Assert.IsTrue(id.EndsWith(idString.Substring(idString.Length - 2, 2)));
		}
		
		[Test()]
		public void Test_Matches_01 ()
		{
			Regex expression = null;
			
			TIdentifier id = CreateMatchingInstance(out expression);
			
			Assert.IsTrue(id.Matches(expression));
		}
		
		[Test()]
		public void Test_Matches_02()
		{
			Regex expression = null;
			TIdentifier id = CreateUnMatchingInstance(out expression);
			
			Assert.IsFalse(id.Matches(expression));
		}	
	}
}

