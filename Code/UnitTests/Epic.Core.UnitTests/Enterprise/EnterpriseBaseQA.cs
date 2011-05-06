//  
//  EnterpriseBaseQA.cs
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

namespace Epic.Enterprise
{
	[TestFixture]
	public class EnterpriseBaseQA
	{
		[SetUp]
		public void ResetApplication()
		{
			TestUtilities.ResetApplication();
		}
		
		/// <summary>
		/// Initialize a new enterprise to test its serialization. To be overridden by derived fixtures.
		/// </summary>
		/// <returns>
		/// A <see cref="IEnterprise"/>
		/// </returns>
		protected virtual EnterpriseBase CreateNewEnterpriseToTestSerialization()
		{
			return new Fakes.FakeEnterprise();
		}

		[Test]
		public void Serialization_works()
		{
			// arrange:
			EnterpriseBase enterprise = CreateNewEnterpriseToTestSerialization();
			ApplicationBase app = new Fakes.FakeApplication(null, enterprise);
			Application.Initialize(app);
			
			// act:
			Stream stream = TestUtilities.Serialize<IEnterprise>(enterprise);
			
			// assert:
			Assert.IsNotNull(stream);
		}
		
		[Test]
		public void Deserialization_returnApplicationEnterpriseInstance()
		{
			// arrange:
			EnterpriseBase enterprise = CreateNewEnterpriseToTestSerialization();
			ApplicationBase app = new Fakes.FakeApplication(null, enterprise);
			Application.Initialize(app);
			Stream stream = TestUtilities.Serialize<IEnterprise>(enterprise);
			
			// act:
			IEnterprise deserialized = TestUtilities.Deserialize<IEnterprise>(stream);
			
			// assert:
			Assert.AreSame(enterprise, deserialized);
		}
		
		[Test]
		public void Ctor_withoutName_throwsArgumentNullException()
		{
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new Fakes.FakeEnterprise(null); });
			Assert.Throws<ArgumentNullException>(delegate { new Fakes.FakeEnterprise(string.Empty); });
		}
		
		[Test]
		public void Ctor_withName_works()
		{
			// arrange:
			string name = "TestApp";
			
			// act:
			IEnterprise enterprise = new Fakes.FakeEnterprise(name);
			
			// assert:
			Assert.IsNotNull(enterprise);
			Assert.AreEqual(name, enterprise.Name);
		}
	}
}

