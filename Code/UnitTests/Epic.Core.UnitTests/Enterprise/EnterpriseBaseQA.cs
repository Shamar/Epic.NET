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
using Epic;
using System.Security.Principal;
using Rhino.Mocks;

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
		
		[Test]
		public void StartWorkingSession_withoutOwner_throwsArgumentNullException()
		{
			// arrange:
			IEnterprise enterprise = new Fakes.FakeEnterprise();
			IWorkingSession session = null;
			
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { enterprise.StartWorkingSession(null, out session); });
			Assert.IsNull(session);
		}
		
		[Test]
		public void StartWorkingSession_withOwner_callTemplateMethod()
		{
			// arrange:
			System.Collections.Generic.List<object> mocks = new System.Collections.Generic.List<object>();
			Fakes.FakeEnterprise enterprise = MockRepository.GeneratePartialMock<Fakes.FakeEnterprise>();
			mocks.Add(enterprise);
			IPrincipal owner = MockRepository.GenerateStrictMock<IPrincipal>();
			mocks.Add(owner);
			WorkingSessionBase outWorkingSessionBase = null;
			WorkingSessionBase session = MockRepository.GeneratePartialMock<WorkingSessionBase>();
			mocks.Add(session);
			enterprise.Expect(e => e.ExecStartWorkingSession(owner, out outWorkingSessionBase)).OutRef(session).Repeat.Once();
			IWorkingSession outWorkingSession = null;
			
			// act:
			enterprise.StartWorkingSession(owner, out outWorkingSession);
			
			// assert:
			Assert.IsNotNull(outWorkingSession);
			Assert.AreSame(session, outWorkingSession);
			foreach(object m in mocks)
				m.VerifyAllExpectations();
		}
		
		[Test]
		public void AcquireWorkingSession_withoutOwner_throwsArgumentNullException()
		{
			// arrange:
			string sessionName = "test";
			IEnterprise enterprise = new Fakes.FakeEnterprise();
			
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { enterprise.AcquireWorkingSession(null, sessionName); });
		}
		
		[Test]
		public void AcquireWorkingSession_withoutIdentifier_throwsArgumentNullException()
		{
			// arrange:
			IPrincipal owner = new GenericPrincipal(new GenericIdentity("test", "test"), new string[] {});
			IEnterprise enterprise = new Fakes.FakeEnterprise();
			
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { enterprise.AcquireWorkingSession(owner, null); });
			Assert.Throws<ArgumentNullException>(delegate { enterprise.AcquireWorkingSession(owner, string.Empty); });
		}
		
		[Test]
		public void AcquireWorkingSession_withValidArguments_callTemplateMethod()
		{
			// arrange:
			System.Collections.Generic.List<object> mocks = new System.Collections.Generic.List<object>();
			string identifier = "testWorkingSession";
			Fakes.FakeEnterprise enterprise = MockRepository.GeneratePartialMock<Fakes.FakeEnterprise>();
			mocks.Add(enterprise);
			IPrincipal owner = MockRepository.GenerateStrictMock<IPrincipal>();
			mocks.Add(owner);
			WorkingSessionBase session = MockRepository.GeneratePartialMock<WorkingSessionBase>();
			mocks.Add(session);
			enterprise.Expect(e => e.ExecAcquireWorkingSessionReal(owner, identifier)).Return(session).Repeat.Once();
			
			// act:
			IWorkingSession returnedWorkingSession = enterprise.AcquireWorkingSession(owner, identifier);
			
			// assert:
			Assert.IsNotNull(returnedWorkingSession);
			Assert.AreSame(session, returnedWorkingSession);
			foreach(object m in mocks)
				m.VerifyAllExpectations();
		}
		
		[Test]
		public void EndWorkingSession_withoutOwner_throwsArgumentNullException()
		{
			// arrange:
			IEnterprise enterprise = new Fakes.FakeEnterprise();
			IWorkingSession session = MockRepository.GenerateStrictMock<IWorkingSession>();
			
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { enterprise.EndWorkingSession(null, session); });
		}
		
		[Test]
		public void EndWorkingSession_withoutSession_throwsArgumentNullException()
		{
			// arrange:
			IEnterprise enterprise = new Fakes.FakeEnterprise();
			IPrincipal owner = new GenericPrincipal(new GenericIdentity("test", "test"), new string[] {});
			
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { enterprise.EndWorkingSession(owner, null); });
		}
		
		[Test]
		public void EndWorkingSession_withWrongSessionType_throwsArgumentException()
		{
			// arrange:
			IEnterprise enterprise = new Fakes.FakeEnterprise();
			IPrincipal owner = new GenericPrincipal(new GenericIdentity("test", "test"), new string[] {});
			IWorkingSession session = MockRepository.GenerateStrictMock<IWorkingSession>();
			
			// assert:
			Assert.Throws<ArgumentException>(delegate { enterprise.EndWorkingSession(owner, session); });
		}

		[Test]
		public void EndWorkingSession_withValidArguments_callTemplateMethod()
		{
			// arrange:
			System.Collections.Generic.List<object> mocks = new System.Collections.Generic.List<object>();
			Fakes.FakeEnterprise enterprise = MockRepository.GeneratePartialMock<Fakes.FakeEnterprise>();
			mocks.Add(enterprise);
			IPrincipal owner = MockRepository.GenerateStrictMock<IPrincipal>();
			mocks.Add(owner);
			WorkingSessionBase session = MockRepository.GeneratePartialMock<WorkingSessionBase>();
			session.Expect(s => s.Dispose()).Repeat.Once();
			mocks.Add(session);
			enterprise.Expect(e => e.ExecBeforeWorkingSessionEnd(owner, session)).Repeat.Once();
			
			// act:
			enterprise.EndWorkingSession(owner, session);
			
			// assert:
			foreach(object m in mocks)
				m.VerifyAllExpectations();
		}
	}
}

