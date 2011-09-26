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
using Epic.Fakes;

namespace Epic.Enterprise
{
	[TestFixture]
	public class EnterpriseBaseQA : RhinoMocksFixtureBase
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
		protected virtual OrganizationBase CreateNewEnterpriseToTestSerialization()
		{
			return new Fakes.FakeOrganization();
		}

		[Test]
		public void Serialization_works()
		{
			// arrange:
			OrganizationBase enterprise = CreateNewEnterpriseToTestSerialization();
			ApplicationBase app = new Fakes.FakeApplication(null, enterprise);
			Application.Initialize(app);
			
			// act:
			Stream stream = TestUtilities.Serialize<IOrganization>(enterprise);
			
			// assert:
			Assert.IsNotNull(stream);
		}
		
		[Test]
		public void Deserialization_returnApplicationEnterpriseInstance()
		{
			// arrange:
			OrganizationBase enterprise = CreateNewEnterpriseToTestSerialization();
			ApplicationBase app = new Fakes.FakeApplication(null, enterprise);
			Application.Initialize(app);
			Stream stream = TestUtilities.Serialize<IOrganization>(enterprise);
			
			// act:
			IOrganization deserialized = TestUtilities.Deserialize<IOrganization>(stream);
			
			// assert:
			Assert.AreSame(enterprise, deserialized);
		}
		
		[Test]
		public void Ctor_withoutName_throwsArgumentNullException()
		{
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new Fakes.FakeOrganization(null); });
			Assert.Throws<ArgumentNullException>(delegate { new Fakes.FakeOrganization(string.Empty); });
		}
		
		[Test]
		public void Ctor_withName_works()
		{
			// arrange:
			string name = "TestApp";
			
			// act:
			IOrganization enterprise = new Fakes.FakeOrganization(name);
			
			// assert:
			Assert.IsNotNull(enterprise);
			Assert.AreEqual(name, enterprise.Name);
		}
		
		[Test]
		public void StartWorkingSession_withoutOwner_throwsArgumentNullException()
		{
			// arrange:
			IOrganization enterprise = new Fakes.FakeOrganization();
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
			Fakes.FakeOrganization enterprise = MockRepository.GeneratePartialMock<Fakes.FakeOrganization>();
			mocks.Add(enterprise);
			IPrincipal owner = MockRepository.GenerateStrictMock<IPrincipal>();
			mocks.Add(owner);
			WorkingSessionBase outWorkingSessionBase = null;
			WorkingSessionBase session = MockRepository.GeneratePartialMock<WorkingSessionBase>("MockWorkingSession", owner);
			mocks.Add(session);
			enterprise.Expect(e => e.CallStartWorkingSession(owner, out outWorkingSessionBase)).OutRef(session).Repeat.Once();
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
			IOrganization enterprise = new Fakes.FakeOrganization();
			
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { enterprise.AcquireWorkingSession(null, sessionName); });
		}
		
		[Test]
		public void AcquireWorkingSession_withoutIdentifier_throwsArgumentNullException()
		{
			// arrange:
			IPrincipal owner = new GenericPrincipal(new GenericIdentity("test", "test"), new string[] {});
			IOrganization enterprise = new Fakes.FakeOrganization();
			
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
			Fakes.FakeOrganization enterprise = MockRepository.GeneratePartialMock<Fakes.FakeOrganization>();
			mocks.Add(enterprise);
			IPrincipal owner = MockRepository.GenerateStrictMock<IPrincipal>();
			mocks.Add(owner);
			WorkingSessionBase session = MockRepository.GeneratePartialMock<WorkingSessionBase>("MockWorkingSession", owner);
			mocks.Add(session);
			enterprise.Expect(e => e.CallAcquireWorkingSessionReal(owner, identifier)).Return(session).Repeat.Once();
			
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
			IOrganization enterprise = new Fakes.FakeOrganization();
			IWorkingSession session = MockRepository.GenerateStrictMock<IWorkingSession>();
			
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { enterprise.EndWorkingSession(null, session); });
		}
		
		[Test]
		public void EndWorkingSession_withoutSession_throwsArgumentNullException()
		{
			// arrange:
			IOrganization enterprise = new Fakes.FakeOrganization();
			IPrincipal owner = new GenericPrincipal(new GenericIdentity("test", "test"), new string[] {});
			
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { enterprise.EndWorkingSession(owner, null); });
		}
		
		[Test]
		public void EndWorkingSession_withWrongSessionType_throwsInvalidOperationException()
		{
			// arrange:
			IOrganization enterprise = new Fakes.FakeOrganization();
			IPrincipal owner = new GenericPrincipal(new GenericIdentity("test", "test"), new string[] {});
			IWorkingSession session = MockRepository.GenerateStrictMock<IWorkingSession>();
			
			// assert:
			Assert.Throws<InvalidOperationException>(delegate { enterprise.EndWorkingSession(owner, session); });
		}
		
		[Test]
		public void EndWorkingSession_withValidArguments_callTemplateMethod()
		{
			// arrange:
			System.Collections.Generic.List<object> mocks = new System.Collections.Generic.List<object>();
			Fakes.FakeOrganization enterprise = MockRepository.GeneratePartialMock<Fakes.FakeOrganization>();
			mocks.Add(enterprise);
			IPrincipal owner = MockRepository.GenerateStrictMock<IPrincipal>();
			mocks.Add(owner);
			FakeWorkingSession session = MockRepository.GeneratePartialMock<FakeWorkingSession>("MockWorkingSession", owner);
			session.Expect(s => s.CallBeforeDispose()).Repeat.Once();
			mocks.Add(session);
			enterprise.Expect(e => e.CallBeforeWorkingSessionEnd(owner, session)).Repeat.Once();
			
			// act:
			enterprise.EndWorkingSession(owner, session);
			
			// assert:
			foreach(object m in mocks)
				m.VerifyAllExpectations();
		}

		[Test]
		public void EndWorkingSession_onSubclassException_throwsInvalidOperationException()
		{
			// arrange:
			System.Collections.Generic.List<object> mocks = new System.Collections.Generic.List<object>();
			Exception dummyException = new Exception("dummyException");
			Fakes.FakeOrganization enterprise = MockRepository.GeneratePartialMock<Fakes.FakeOrganization>();
			mocks.Add(enterprise);
			IPrincipal owner = MockRepository.GenerateStrictMock<IPrincipal>();
			owner.Expect(p => p.Identity).Return(new GenericIdentity("testPrincipal"));
			mocks.Add(owner);
			WorkingSessionBase session = MockRepository.GeneratePartialMock<WorkingSessionBase>("MockWorkingSession", owner);
			mocks.Add(session);
			enterprise.Expect(e => e.CallBeforeWorkingSessionEnd(owner, session)).Throw(dummyException).Repeat.Once();
			InvalidOperationException cought = null;
			
			// act:
			try
			{
				enterprise.EndWorkingSession(owner, session);
			}
			catch (InvalidOperationException e)
			{
				cought = e;
			}
			
			// assert:
			Assert.IsNotNull(cought);
			Assert.AreSame(dummyException, cought.InnerException);
			foreach(object m in mocks)
				m.VerifyAllExpectations();
		}
		

		[Test]
		public void EndWorkingSession_onSubclassException_dontWrapInvalidOperationException()
		{
			// arrange:
			System.Collections.Generic.List<object> mocks = new System.Collections.Generic.List<object>();
			InvalidOperationException thrownException = new InvalidOperationException("thrownException");
			Fakes.FakeOrganization enterprise = MockRepository.GeneratePartialMock<Fakes.FakeOrganization>();
			mocks.Add(enterprise);
			IPrincipal owner = MockRepository.GenerateStrictMock<IPrincipal>();
			mocks.Add(owner);
			WorkingSessionBase session = MockRepository.GeneratePartialMock<WorkingSessionBase>("MockWorkingSession", owner);
			mocks.Add(session);
			enterprise.Expect(e => e.CallBeforeWorkingSessionEnd(owner, session)).Throw(thrownException).Repeat.Once();
			InvalidOperationException cought = null;
			
			// act:
			try
			{
				enterprise.EndWorkingSession(owner, session);
			}
			catch (InvalidOperationException e)
			{
				cought = e;
			}
			
			// assert:
			Assert.IsNotNull(cought);
			Assert.AreSame(thrownException, cought);
			foreach(object m in mocks)
				m.VerifyAllExpectations();
		}
		
		[Test]
		public void EndWorkingSession_onWorkingSessionException_dontWrapInvalidOperationException()
		{
			// arrange:
			System.Collections.Generic.List<object> mocks = new System.Collections.Generic.List<object>();
			InvalidOperationException thrownException = new InvalidOperationException("thrownException");
			Fakes.FakeOrganization enterprise = MockRepository.GeneratePartialMock<Fakes.FakeOrganization>();
			mocks.Add(enterprise);
			IPrincipal owner = MockRepository.GenerateStrictMock<IPrincipal>();
			mocks.Add(owner);
			FakeWorkingSession session = MockRepository.GeneratePartialMock<FakeWorkingSession>("MockWorkingSession", owner);
			session.Expect(s => s.CallBeforeDispose()).Throw(thrownException).Repeat.Once();
			mocks.Add(session);
			enterprise.Expect(e => e.CallBeforeWorkingSessionEnd(owner, session)).Repeat.Once();
			InvalidOperationException cought = null;
			
			// act:
			try
			{
				enterprise.EndWorkingSession(owner, session);
			}
			catch (InvalidOperationException e)
			{
				cought = e;
			}
			
			// assert:
			Assert.IsNotNull(cought);
			Assert.AreSame(thrownException, cought);
			foreach(object m in mocks)
				m.VerifyAllExpectations();
		}
		
		[Test]
		public void EndWorkingSession_onWorkingSessionException_throwsInvalidOperationException()
		{
			// arrange:
			System.Collections.Generic.List<object> mocks = new System.Collections.Generic.List<object>();
			Exception dummyException = new Exception("dummyException");
			Fakes.FakeOrganization enterprise = MockRepository.GeneratePartialMock<Fakes.FakeOrganization>();
			mocks.Add(enterprise);
			IPrincipal owner = MockRepository.GenerateStrictMock<IPrincipal>();
			owner.Expect(p => p.Identity).Return(new GenericIdentity("testPrincipal"));
			mocks.Add(owner);
			FakeWorkingSession session = MockRepository.GeneratePartialMock<FakeWorkingSession>("MockWorkingSession", owner);
			session.Expect(s => s.CallBeforeDispose()).Throw(dummyException).Repeat.Once();
			mocks.Add(session);
			enterprise.Expect(e => e.CallBeforeWorkingSessionEnd(owner, session)).Repeat.Once();
			InvalidOperationException cought = null;
			
			// act:
			try
			{
				enterprise.EndWorkingSession(owner, session);
			}
			catch (InvalidOperationException e)
			{
				cought = e;
			}
			
			// assert:
			Assert.IsNotNull(cought);
			Assert.AreSame(dummyException, cought.InnerException);
			foreach(object m in mocks)
				m.VerifyAllExpectations();
		}

	}
}

