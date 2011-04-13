//  
//  StringIdentifierTester.cs
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
using System.Text.RegularExpressions;
using Challenge00.DDDSample.Shared;

namespace Contracts.Shared
{
	public abstract class StringIdentifierQA<TIdentifier>
		where TIdentifier : StringIdentifier<TIdentifier>
	{
		protected abstract TIdentifier CreateNewInstance(out string stringUsed);

		protected abstract TIdentifier CreateDifferentInstance(out string stringUsed);
		
		protected abstract TIdentifier CreateMatchingInstance(out Regex regEx);
		
		protected abstract TIdentifier CreateUnMatchingInstance(out Regex regEx);
		
		[Test()]
		public void ToString_matchTheIdentifier ()
		{
			string idString = null;
			TIdentifier id = CreateNewInstance(out idString);
			
			Assert.AreEqual(idString, id.ToString());
		}
		
		[Test()]
		public void Equals_withNull_isFalse ()
		{
			string idString = null;
			TIdentifier id = CreateNewInstance(out idString);
			
			Assert.IsFalse(id.Equals(null));
			Assert.IsFalse(id.Equals((object)null));
		}
		
		[Test()]
		public void Equals_withItself_isTrue ()
		{
			string idString = null;
			TIdentifier id = CreateNewInstance(out idString);
			
			Assert.IsTrue(id.Equals(id));
			Assert.IsTrue(id.Equals((object)id));
		}
		
		[Test()]
		public void Equals_withEqualIdentifier_isTrue_01 ()
		{
			string idString = null;
			TIdentifier id1 = CreateNewInstance(out idString);
			TIdentifier id2 = CreateNewInstance(out idString);
			
			Assert.IsTrue(id1.Equals(id2));
			Assert.IsTrue(id1.Equals((object)id2));
		}
		
		[Test()]
		public void Equals_withEqualIdentifier_isTrue_02 ()
		{
			string idString = null;
			string idString2 = null;
			TIdentifier id1 = CreateNewInstance(out idString);
			TIdentifier id2 = CreateNewInstance(out idString2);
			
			Assert.IsTrue(id1.Equals(id2));
			Assert.IsTrue(id1.Equals((object)id2));
		}
		
		[Test()]
		public void Equals_withDifferentIdentifier_isFalse ()
		{
			string idString = null;
			string idString2 = null;
			TIdentifier id1 = CreateNewInstance(out idString);
			TIdentifier id2 = CreateDifferentInstance(out idString2);
			
			Assert.IsFalse(id1.Equals(id2));
			Assert.IsFalse(id1.Equals((object)id2));
		}
		
		[Test()]
		public void operatorEqual_withEqualsIdentifiers_returnTrue()
		{
			string idString = null;
			string idString2 = null;
			TIdentifier id1 = CreateNewInstance(out idString);
			TIdentifier id2 = CreateNewInstance(out idString2);
			
			Assert.IsFalse(id1 != id2);
		}
		
		[Test()]
		public void operatorNotEqual_withEqualsIdentifiers_returnFalse()
		{
			string idString = null;
			string idString2 = null;
			TIdentifier id1 = CreateNewInstance(out idString);
			TIdentifier id2 = CreateNewInstance(out idString2);
			
			Assert.IsFalse(id1 != id2);
		}
		
		[Test()]
		public void operatorEqual_withDifferentIdentifiers_returnFalse()
		{
			string idString = null;
			string idString2 = null;
			TIdentifier id1 = CreateNewInstance(out idString);
			TIdentifier id2 = CreateDifferentInstance(out idString2);
			
			Assert.IsFalse(id1 == id2);
		}
		
		[Test()]
		public void operatorNotEqual_withDifferentIdentifiers_returnTrue()
		{
			string idString = null;
			string idString2 = null;
			TIdentifier id1 = CreateNewInstance(out idString);
			TIdentifier id2 = CreateDifferentInstance(out idString2);
			
			Assert.IsTrue(id1 != id2);
		}
		
		[Test()]
		public void operatorEqual_withSameInstance_returnTrue()
		{
			string idString = null;
			TIdentifier id1 = CreateNewInstance(out idString);
#pragma warning disable 1718
			Assert.IsTrue(id1 == id1);
#pragma warning restore 1718
		}

		[Test()]
		public void operatorNotEqual_withSameInstance_returnFalse()
		{
			string idString = null;
			TIdentifier id1 = CreateNewInstance(out idString);
#pragma warning disable 1718
			Assert.IsFalse(id1 != id1);
#pragma warning restore 1718
		}

		
		[Test()]
		public void operatorEqual_withNull_returnFalse()
		{
			string idString = null;
			TIdentifier id1 = CreateNewInstance(out idString);
			
			Assert.IsFalse(id1 == null);
			Assert.IsFalse(null == id1);
		}		
		
		[Test()]
		public void operatorNotEqual_withNull_returnTrue()
		{
			string idString = null;
			TIdentifier id1 = CreateNewInstance(out idString);
			
			Assert.IsTrue(id1 != null);
			Assert.IsTrue(null != id1);
		}

		[Test()]
		public void operatorNotEqual_betweenNulls_returnFalse()
		{
			Assert.IsFalse((new object() as TIdentifier) != (new object() as TIdentifier));
		}

		[Test()]
		public void Contains_withUncontainedString_returnFalse ()
		{
			string idString = null;
			TIdentifier id = CreateNewInstance(out idString);
			
			Assert.IsFalse(id.Contains(new Guid().ToString()));
		}
		
		[Test()]
		public void Contains_withContainedString_returnTrue ()
		{
			string idString = null;
			TIdentifier id = CreateNewInstance(out idString);
			
			Assert.IsTrue(id.Contains(idString.Substring(1, 2)));
		}
		
		[Test()]
		public void StartsWith_withNotInitialSubstring_returnFalse ()
		{
			string idString = null;
			TIdentifier id = CreateNewInstance(out idString);
			
			Assert.IsFalse(id.StartsWith(idString.Substring(1, 2)));
		}
		
		[Test()]
		public void StartsWith_withInitialSubstring_returnTrue ()
		{
			string idString = null;
			TIdentifier id = CreateNewInstance(out idString);
			
			Assert.IsTrue(id.StartsWith(idString.Substring(0, 2)));
		}
		
		[Test()]
		public void EndsWith_withNotEndingSubstring_returnFalse ()
		{
			string idString = null;
			TIdentifier id = CreateNewInstance(out idString);
			
			Assert.IsFalse(id.EndsWith(new Guid().ToString()));
		}
		
		[Test()]
		public void EndsWith_withEndingSubstring_returnTrue ()
		{
			string idString = null;
			TIdentifier id = CreateNewInstance(out idString);
			
			Assert.IsTrue(id.EndsWith(idString.Substring(idString.Length - 2, 2)));
		}
		
		[Test()]
		public void Matches_withMatchingRegEx_returnTrue ()
		{
			Regex expression = null;
			
			TIdentifier id = CreateMatchingInstance(out expression);
			
			Assert.IsTrue(id.Matches(expression));
		}
		
		[Test()]
		public void Matches_withUnMatchingRegEx_returnFalse()
		{
			Regex expression = null;
			TIdentifier id = CreateUnMatchingInstance(out expression);
			
			Assert.IsFalse(id.Matches(expression));
		}	
	}
}

