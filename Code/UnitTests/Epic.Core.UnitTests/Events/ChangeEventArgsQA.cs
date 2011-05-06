//  
//  ChangeEventArgsQA.cs
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
using System.Collections.Generic;
using System.IO;
namespace Epic.Events
{
	[TestFixture]
	public class ChangeEventArgsQA
	{
		[TestCase("OldValue", "NewValue")]
		[TestCase((int) 10, (int)3)]
		[TestCase((double) -10.1, (double) 10.2)]
		[TestCase(default(int), (int)10)]
		public void Ctor_withAPairOfValue_exposeTheValues<T>(T oldValue, T newValue)
		{
			// act:
			ChangeEventArgs<T> args = new ChangeEventArgs<T>(oldValue, newValue);
			
			// assert:
			Assert.IsTrue(EqualityComparer<T>.Default.Equals(oldValue, args.OldValue));
			Assert.IsTrue(EqualityComparer<T>.Default.Equals(newValue, args.NewValue));
		}
		
		[Test]
		public void Ctor_withNullOldValue_exposeTheValues()
		{
			// arrange:
			string newValue = "NewValue";
			
			// act:
			ChangeEventArgs<string> args = new ChangeEventArgs<string>(null, newValue);
			
			// assert:
			Assert.IsNull(args.OldValue);
			Assert.AreEqual(newValue, args.NewValue);
		}
		
		[Test]
		public void Ctor_withNullNewValue_exposeTheValues()
		{
			// arrange:
			string oldValue = "OldValue";
			
			// act:
			ChangeEventArgs<string> args = new ChangeEventArgs<string>(oldValue, null);
			
			// assert:
			Assert.IsNull(args.NewValue);
			Assert.AreEqual(oldValue, args.OldValue);
		}
		
		[TestCase("OldValue", "NewValue")]
		[TestCase((int) 10, (int)3)]
		[TestCase((double) -10.1, (double) 10.2)]
		[TestCase(default(int), (int)10)]
		public void Serialize_works<T>(T oldValue, T newValue)
		{
			// arrange:
			ChangeEventArgs<T> args = new ChangeEventArgs<T>(oldValue, newValue);
			
			// act:
			Stream stream = TestUtilities.Serialize(args);
			
			// assert:
			Assert.IsNotNull(stream);
		}

		[TestCase("OldValue", "NewValue")]
		[TestCase((int) 10, (int)3)]
		[TestCase((double) -10.1, (double) 10.2)]
		[TestCase(default(int), (int)10)]
		public void Deserialize_works<T>(T oldValue, T newValue)
		{
			// arrange:
			ChangeEventArgs<T> args = new ChangeEventArgs<T>(oldValue, newValue);
			Stream stream = TestUtilities.Serialize(args);
			
			// act:
			ChangeEventArgs<T> deserialized = TestUtilities.Deserialize<ChangeEventArgs<T>>(stream);
			
			// assert:
			Assert.IsNotNull(deserialized);
			Assert.AreEqual(oldValue, deserialized.OldValue);
			Assert.AreEqual(newValue, deserialized.NewValue);
		}

	}
}

