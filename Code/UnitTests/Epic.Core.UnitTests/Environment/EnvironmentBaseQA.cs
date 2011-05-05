//  
//  EnvironmentBaseQA.cs
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
using System.IO;
namespace Epic.Environment
{
	[TestFixture]
	public class EnvironmentBaseQA
	{
		[SetUp]
		public void ResetApplication()
		{
			TestUtilities.ResetApplication();
		}
		
		/// <summary>
		/// Initialize a new environment to test its serialization. To be overridden by derived fixtures.
		/// </summary>
		/// <returns>
		/// A <see cref="IEnvironment"/>
		/// </returns>
		protected virtual EnvironmentBase CreateNewEnvironmentToTest()
		{
			return new Fakes.FakeEnvironment();
		}

		[Test]
		public void Serialization_works()
		{
			// arrange:
			EnvironmentBase environment = CreateNewEnvironmentToTest();
			ApplicationBase app = new Fakes.FakeApplication(environment, null);
			Application.Initialize(app);
			
			// act:
			Stream stream = TestUtilities.Serialize<IEnvironment>(environment);
			
			// assert:
			Assert.IsNotNull(stream);
		}
		
		[Test]
		public void Deserialization_returnApplicationEnvironmentInstance()
		{
			// arrange:
			EnvironmentBase environment = CreateNewEnvironmentToTest();
			ApplicationBase app = new Fakes.FakeApplication(environment, null);
			Application.Initialize(app);
			Stream stream = TestUtilities.Serialize<IEnvironment>(environment);
			
			// act:
			IEnvironment deserialized = TestUtilities.Deserialize<IEnvironment>(stream);
			
			// assert:
			Assert.AreSame(environment, deserialized);
		}
	}
}

