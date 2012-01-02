//  
//  EnterpriseQA.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2012 Giacomo Tesio
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
using Rhino.Mocks;
using Epic.Environment;
namespace Epic
{
	[TestFixture]
	public class EnterpriseQA
	{
		[SetUp]
		public void ResetEnterprise()
		{
			TestUtilities.ResetEnterprise();
		}
		
		[Test]
		public void whenUninitialized_throwsInvalidOperationException ()
		{
			// arrange:
			IEnvironment environment = null;
			IOrganization organization = null;
			
			// assert:
			Assert.AreEqual("Uninitialized", Enterprise.Name);
			Assert.Throws<InvalidOperationException>(delegate { environment = Enterprise.Environment; });
			Assert.Throws<InvalidOperationException>(delegate { organization = Enterprise.Organization; });
			Assert.IsNull(environment);
			Assert.IsNull(organization);
		}
		
		[Test]
		public void whenInitialized_forwardToEnterpriseSingleton ()
		{
			// arrange:
			string appName = "SampleApp";
			EnvironmentBase environment = MockRepository.GenerateStrictMock<EnvironmentBase>();
			IOrganization organization = MockRepository.GenerateStrictMock<IOrganization>();
			EnterpriseBase appSingleton = MockRepository.GeneratePartialMock<EnterpriseBase>(appName);
			appSingleton.Expect(a => a.Environment).Return(environment).Repeat.Once();
			appSingleton.Expect(a => a.Organization).Return(organization).Repeat.Once();
			
			// act:
			Enterprise.Initialize(appSingleton);
			
			// assert:
			Assert.AreEqual(appName,    Enterprise.Name);
			Assert.AreSame(environment, Enterprise.Environment);
			Assert.AreSame(organization,  Enterprise.Organization);
			appSingleton.VerifyAllExpectations();
		}
		
		[Test]
		public void Initialize_withNull_throwsArgumentNullException ()
		{
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { Enterprise.Initialize(null); });
		}
		
		[Test]
		public void Initialize_twice_throwsInvalidOperationException ()
		{
			// arrange:
			string appName = "SampleApp";
			EnterpriseBase appSingleton = MockRepository.GeneratePartialMock<EnterpriseBase>(appName);
			EnterpriseBase secondSingleton = MockRepository.GeneratePartialMock<EnterpriseBase>("test");
			
			// act:
			Enterprise.Initialize(appSingleton);
			
			// assert:
			Assert.Throws<InvalidOperationException>(delegate { Enterprise.Initialize(secondSingleton); });
			appSingleton.VerifyAllExpectations();
		}		
	}
}

