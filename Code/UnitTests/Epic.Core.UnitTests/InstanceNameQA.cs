//  
//  InstanceNameQA.cs
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
namespace Epic
{
	[TestFixture]
	public class InstanceNameQA
	{
		[TestCase((int)1)]
		[TestCase((double)1.0)]
		[TestCase((string)"dummyValue")]
		public void Ctor_withoutName_throwsArgumentNullException<T>(T dummyValue)
		{
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new InstanceName<T>(null); });
			Assert.Throws<ArgumentNullException>(delegate { new InstanceName<T>(string.Empty); });
		}
		
		[TestCase("testName", (int)1)]
		[TestCase("testName", (double)1.0)]
		[TestCase("testName", (string)"dummyValue")]
		public void Ctor_withName_works<T>(string name, T dummyValue)
		{
			// act:
			new InstanceName<T>(name);
		}
		
		[TestCase("testName", (int)1)]
		[TestCase("testName", (double)1.0)]
		[TestCase("testName", (string)"dummyValue")]
		public void Equals_toNull_isFalse<T>(string name, T dummyValue)
		{
			// arrange:
			InstanceName<T> instanceName =  new InstanceName<T>(name);
			
			// act:
			bool result = instanceName.Equals(null);
			
			// assert:
			Assert.IsFalse(result);
		}
		
		[TestCase("testName", (int)1)]
		[TestCase("testName", (double)1.0)]
		[TestCase("testName", (string)"dummyValue")]
		public void Equals_toItself_isTrue<T>(string name, T dummyValue)
		{
			// arrange:
			InstanceName<T> instanceName =  new InstanceName<T>(name);
			
			// act:
			bool result = instanceName.Equals(instanceName);
			
			// assert:
			Assert.IsTrue(result);
		}
		
		[TestCase("testName", (int)1)]
		[TestCase("testName", (double)1.0)]
		[TestCase("testName", (string)"dummyValue")]
		public void Equals_toDifferentInstanceType_isFalse<T>(string name, T dummyValue)
		{
			// arrange:
			InstanceName<T> instanceName =  new InstanceName<T>(name);
			InstanceName<object> differentInstance = new InstanceName<object>(name);
			
			// act:
			bool result = instanceName.Equals(differentInstance);
			
			// assert:
			Assert.IsFalse(result);
		}
		
		[TestCase("testName", (int)1)]
		[TestCase("testName", (double)1.0)]
		[TestCase("testName", (string)"dummyValue")]
		public void Equals_toDifferentObjectType_isFalse<T>(string name, T dummyValue)
		{
			// arrange:
			InstanceName<T> instanceName =  new InstanceName<T>(name);
			object differentObject = new object();
			
			// act:
			bool result = instanceName.Equals(differentObject);
			
			// assert:
			Assert.IsFalse(result);
		}
		
		[TestCase("testName", (int)1)]
		[TestCase("testName", (double)1.0)]
		[TestCase("testName", (string)"dummyValue")]
		public void Equals_toDifferentNamedInstance_isFalse<T>(string name, T dummyValue)
		{
			// arrange:
			InstanceName<T> instanceName =  new InstanceName<T>(name);
			InstanceName<T> differentInstance =  new InstanceName<T>("differentName");
			
			// act:
			bool result = instanceName.Equals(differentInstance);
			
			// assert:
			Assert.IsFalse(result);
		}
		
		[TestCase("testName", (int)1)]
		[TestCase("testName", (double)1.0)]
		[TestCase("testName", (string)"dummyValue")]
		public void Equals_toDifferentNamedInstance_isTrue<T>(string name, T dummyValue)
		{
			// arrange:
			InstanceName<T> instanceName =  new InstanceName<T>(name);
			InstanceName<T> differentInstance =  new InstanceName<T>(name);
			
			// act:
			bool result = instanceName.Equals(differentInstance);
			
			// assert:
			Assert.IsTrue(result);
			Assert.AreEqual(instanceName.GetHashCode(), differentInstance.GetHashCode());
		}
		
		[TestCase("testName", (int)1)]
		[TestCase("testName", (double)1.0)]
		[TestCase("testName", (string)"dummyValue")]
		public void ToString_containsNameAndType<T>(string name, T dummyValue)
		{
			// arrange:
			InstanceName<T> instanceName =  new InstanceName<T>(name);
			
			// act:
			string result = instanceName.ToString();
			
			// assert:
			Assert.IsTrue(result.Contains(name));
			Assert.IsTrue(result.Contains(typeof(T).FullName));
		}
	}
}

