//  
//  WorkingSessionBaseQA.cs
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
using System.Linq;
using NUnit.Framework;
using Epic.Fakes;
using System.IO;
using System.Security.Principal;
using Rhino.Mocks;
using Epic.Events;

namespace Epic.Enterprise
{
	[TestFixture]
	public class WorkingSessionBaseQA : RhinoMocksFixtureBase
	{
		#region Constructor
		
		[Test]
		public void Ctor_withoutIdentifier_throwsArgumentNullException()
		{
			// arrange:
			IPrincipal owner = new GenericPrincipal(new GenericIdentity("FakeUser"), new string[0]);
			
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new FakeWorkingSession(null, owner); });
			Assert.Throws<ArgumentNullException>(delegate { new FakeWorkingSession(string.Empty, owner); });
		}
		
		[Test]
		public void Ctor_withoutOwner_throwsArgumentNullException()
		{
			// arrange:
			string identifier = "test";
			
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new FakeWorkingSession(identifier, null); });
		}
		
		[Test]
		public void Ctor_withValidArguments_works()
		{
			// arrange:
			string identifier = "test";
			string userID = "FakeUser";
			IPrincipal owner = new GenericPrincipal(new GenericIdentity(userID), new string[0]);
			
			// act:
			IWorkingSession session = new FakeWorkingSession(identifier, owner);
			
			// assert:
			Assert.AreSame(identifier, session.Identifier);
			Assert.AreEqual(userID, session.Owner);
			Assert.AreSame(owner, ((FakeWorkingSession)session).Owner);
		}
		
		#endregion Constructor
		
		#region Serialization
		
		[TestCase("Test1")]
		[TestCase("Test2")]
		[TestCase("Test3")]
		public void Serialize_works(string identifier)
		{
			// arrange:
			string userID = "FakeUser";
			IPrincipal owner = new GenericPrincipal(new GenericIdentity(userID), new string[0]);
			IWorkingSession session = new FakeWorkingSession(identifier, owner);
			
			// act:
			Stream stream = TestUtilities.Serialize(session);
			
			// assert:
			Assert.IsNotNull(stream);
		}
		
		
		[TestCase("Test1")]
		[TestCase("Test2")]
		[TestCase("Test3")]
		public void Deserialize_works(string identifier)
		{
			// arrange:
			string userID = "FakeUser";
			IPrincipal owner = new GenericPrincipal(new GenericIdentity(userID), new string[0]);
			IWorkingSession session = new FakeWorkingSession(identifier, owner);
			Stream stream = TestUtilities.Serialize(session);
			
			// act:
			IWorkingSession deserialized = TestUtilities.Deserialize<IWorkingSession>(stream);
			
			// assert:
			Assert.IsNotNull(deserialized);
			Assert.AreEqual(identifier, deserialized.Identifier);
		}
		
		[Test]
		public void Serialize_afterAnAchieve_works()
		{
			// arrange:
			IWorkingSession session = new FakeSerializableWorkingSession();
			IFakeRole achievedRole = null;
			session.Achieve<IFakeRole>(out achievedRole);

			// act:
			Stream stream = TestUtilities.Serialize(session);

			// assert:
			Assert.IsNotNull(stream);
		}
		
		[Test]
		public void Deserialize_afterAnAchieve_works()
		{
			// arrange:
			IWorkingSession session = new FakeSerializableWorkingSession();
			IFakeRole achievedRole = null;
			session.Achieve<IFakeRole>(out achievedRole);
			Stream stream = TestUtilities.Serialize(session);

			// act:
			IFakeRole deserializedRole = null;
			IWorkingSession deserialized = TestUtilities.Deserialize<IWorkingSession>(stream);
			deserialized.Achieve<IFakeRole>(out deserializedRole);
			
			// assert:
			Assert.IsNotNull(deserialized);
			Assert.AreEqual(((FakeSerializableWorkingSession)session).CalledMethods.Count(), ((FakeSerializableWorkingSession)deserialized).CalledMethods.Count());
		}
		
		#endregion Serialization
	
		#region CanAchieve
		
		[TestCase(true)]
		[TestCase(false)]
		public void CanAchieve_newRole_callIsAllowedTemplateMethod(bool isAllowed)
		{
			// arrange:
			FakeWorkingSession session = GeneratePartialMock<FakeWorkingSession>();
			session.Expect(s => s.CallIsAllowed<IFakeRole>()).Return(isAllowed).Repeat.Once();

			// act:
			bool canAchieve = session.CanAchieve<IFakeRole>();

			// assert:
			Assert.AreEqual(isAllowed, canAchieve);
		}

		[Test]
		public void CanAchieve_anAlreadyAchievedRole_dontCallIsAllowedTemplateMethod()
		{
			// arrange:
			FakeRole roleToReturn = new FakeRole();
			FakeWorkingSession session = GeneratePartialMock<FakeWorkingSession>();
			IPrincipal owner = session.Owner;
			session.Expect(s => s.CallIsAllowed<IFakeRole>()).Return(true).Repeat.Once();
			FakeRoleBuilder<IFakeRole, FakeRole> roleBuilder = GeneratePartialMock<FakeRoleBuilder<IFakeRole, FakeRole>>();
			roleBuilder.Expect(b => b.CallCreateRoleFor(owner)).Return(roleToReturn).Repeat.Once();
			session.Expect(s => s.CallGetRoleBuilder<IFakeRole>()).Return(roleBuilder).Repeat.Once();
			IFakeRole achievedRole1 = null;
			session.Achieve<IFakeRole>(out achievedRole1);

			// act:
			bool canAchieve = session.CanAchieve<IFakeRole>();

			// assert:
			Assert.IsTrue(canAchieve);
		}

		
		#endregion CanAchieve
		
		#region Achieve
		
		[Test]
		public void Achieve_newAllowedRole_delegateToRoleBuilder()
		{
			// arrange:
			FakeRole roleToReturn = new FakeRole();
			FakeWorkingSession session = GeneratePartialMock<FakeWorkingSession>();
			IPrincipal owner = session.Owner;
			session.Expect(s => s.CallIsAllowed<IFakeRole>()).Return(true).Repeat.Once();
			FakeRoleBuilder<IFakeRole, FakeRole> roleBuilder = GeneratePartialMock<FakeRoleBuilder<IFakeRole, FakeRole>>();
			roleBuilder.Expect(b => b.CallCreateRoleFor(owner)).Return(roleToReturn).Repeat.Once();
			session.Expect(s => s.CallGetRoleBuilder<IFakeRole>()).Return(roleBuilder).Repeat.Once();
			IFakeRole achievedRole = null;

			// act:
			session.Achieve<IFakeRole>(out achievedRole);

			// assert:
			Assert.AreSame(roleToReturn, achievedRole);
		}
		
		[Test]
		public void Achieve_notAllowedRole_throwsInvalidOperationException()
		{
			// arrange:
			FakeWorkingSession session = GeneratePartialMock<FakeWorkingSession>();
			session.Expect(s => s.CallIsAllowed<IFakeRole>()).Return(false).Repeat.Once();
			IFakeRole achievedRole = null;

			// assert:
			Assert.Throws<InvalidOperationException>(delegate {
				session.Achieve<IFakeRole>(out achievedRole);
			});
			Assert.IsNull(achievedRole);
		}
		
		[Test]
		public void Achieve_anAlreadyAchievedRole_returnPreviousInstance()
		{
			// arrange:
			FakeRole roleToReturn = new FakeRole();
			FakeWorkingSession session = GeneratePartialMock<FakeWorkingSession>();
			IPrincipal owner = session.Owner;
			session.Expect(s => s.CallIsAllowed<IFakeRole>()).Return(true).Repeat.Once();
			FakeRoleBuilder<IFakeRole, FakeRole> roleBuilder = GeneratePartialMock<FakeRoleBuilder<IFakeRole, FakeRole>>();
			roleBuilder.Expect(b => b.CallCreateRoleFor(owner)).Return(roleToReturn).Repeat.Once();
			session.Expect(s => s.CallGetRoleBuilder<IFakeRole>()).Return(roleBuilder).Repeat.Once();
			IFakeRole achievedRole1 = null;
			session.Achieve<IFakeRole>(out achievedRole1);
			IFakeRole achievedRole2 = null;

			// act:
			session.Achieve<IFakeRole>(out achievedRole2);

			// assert:
			Assert.AreSame(achievedRole1, achievedRole2);
		}
		
		#endregion Achieve
		
		#region Leave
		
		[Test]
		public void Leave_withoutRole_throwsArgumentNullException()
		{
			// arrange:
			FakeWorkingSession session = GeneratePartialMock<FakeWorkingSession>();
			IFakeRole achievedRole = null;

			// assert:
			Assert.Throws<ArgumentNullException>(delegate {
				session.Leave<IFakeRole>(ref achievedRole);
			});
		}
		
		[Test]
		public void Leave_withUnknownRole_throwsArgumentException()
		{
			// arrange:
			FakeWorkingSession session = GeneratePartialMock<FakeWorkingSession>();
			IFakeRole achievedRole = GenerateStrictMock<IFakeRole>();

			// assert:
			Assert.Throws<ArgumentException>(delegate {
				session.Leave<IFakeRole>(ref achievedRole);
			});
		}
		
		[Test]
		public void Leave_withAchievedRole_disposeRole()
		{
			// arrange:
			FakeRole roleToReturn = GeneratePartialMock<FakeRole>();
			roleToReturn.Expect(r => r.Dispose()).Repeat.Once();
			FakeWorkingSession session = GeneratePartialMock<FakeWorkingSession>();
			IPrincipal owner = session.Owner;
			session.Expect(s => s.CallIsAllowed<IFakeRole>()).Return(true).Repeat.Once();
			FakeRoleBuilder<IFakeRole, FakeRole> roleBuilder = GeneratePartialMock<FakeRoleBuilder<IFakeRole, FakeRole>>();
			roleBuilder.Expect(b => b.CallCreateRoleFor(owner)).Return(roleToReturn).Repeat.Once();
			session.Expect(s => s.CallGetRoleBuilder<IFakeRole>()).Return(roleBuilder).Repeat.Once();
			IFakeRole achievedRole1 = null;
			session.Achieve<IFakeRole>(out achievedRole1);

			// act:
			session.Leave<IFakeRole>(ref achievedRole1);

			// assert:
			Assert.IsNull(achievedRole1);
		}
		
		[TestCase(2)]
		[TestCase(5)]
		[TestCase(100)]
		public void Leave_withRoleAchievedManyTimes_disposeRoleOnlyOnce(int times)
		{
			// arrange:
			FakeRole roleToReturn = GeneratePartialMock<FakeRole>();
			roleToReturn.Expect(r => r.Dispose()).Repeat.Once();  						// Dispose expected only once.
			FakeWorkingSession session = GeneratePartialMock<FakeWorkingSession>();
			IPrincipal owner = session.Owner;
			session.Expect(s => s.CallIsAllowed<IFakeRole>()).Return(true).Repeat.Once();
			FakeRoleBuilder<IFakeRole, FakeRole> roleBuilder = GeneratePartialMock<FakeRoleBuilder<IFakeRole, FakeRole>>();
			roleBuilder.Expect(b => b.CallCreateRoleFor(owner)).Return(roleToReturn).Repeat.Once();
			session.Expect(s => s.CallGetRoleBuilder<IFakeRole>()).Return(roleBuilder).Repeat.Once();
			IFakeRole achievedRole = null;
			for(int i = 0; i < times; ++i)
			{
				IFakeRole achievedRoleI = null;
				session.Achieve<IFakeRole>(out achievedRoleI);
				if(i == 0)
				{
					achievedRole = achievedRoleI;
				}
				else
				{
					Assert.AreSame(achievedRole, achievedRoleI);
				}
			}

			// assert:
			for(int i = 0; i < times; ++i)
			{
				IFakeRole achievedRoleI2 = achievedRole;
				session.Leave<IFakeRole>(ref achievedRoleI2);
				Assert.IsNull(achievedRoleI2);
			}
		}
		
		[Test]
		public void Leave_withUnknownRoleReference_throwsArgumentException()
		{
			// arrange:
			IFakeRole unknownInstance = new FakeRole();
			FakeRole roleToReturn = GeneratePartialMock<FakeRole>();
			FakeWorkingSession session = GeneratePartialMock<FakeWorkingSession>();
			IPrincipal owner = session.Owner;
			session.Expect(s => s.CallIsAllowed<IFakeRole>()).Return(true).Repeat.Once();
			FakeRoleBuilder<IFakeRole, FakeRole> roleBuilder = GeneratePartialMock<FakeRoleBuilder<IFakeRole, FakeRole>>();
			roleBuilder.Expect(b => b.CallCreateRoleFor(owner)).Return(roleToReturn).Repeat.Once();
			session.Expect(s => s.CallGetRoleBuilder<IFakeRole>()).Return(roleBuilder).Repeat.Once();
			IFakeRole achievedRole1 = null;
			session.Achieve<IFakeRole>(out achievedRole1);

			// assert:
			Assert.Throws<ArgumentException>(delegate {
				session.Leave<IFakeRole>(ref unknownInstance);
			});
			Assert.IsNotNull(unknownInstance);
		}
		
		#endregion Leave
	
		#region Dispose
		
		[Test]
		public void Dispose_callsBeforeDisposeTemplateMethod()
		{
			// arrange:
			FakeWorkingSession fakeSession = GeneratePartialMock<FakeWorkingSession>();
			fakeSession.Expect(s => s.CallBeforeDispose()).Repeat.Once();
			IWorkingSession session = fakeSession;
			IFakeRole roleRef = null;
			
			// act:
			fakeSession.Dispose();

			// assert:
			Assert.Throws<ObjectDisposedException>(delegate { session.CanAchieve<IFakeRole>(); });
			Assert.Throws<ObjectDisposedException>(delegate { session.Achieve<IFakeRole>(out roleRef); });
			Assert.IsNull(roleRef);
			Assert.Throws<ObjectDisposedException>(delegate { session.Leave<IFakeRole>(ref roleRef); });
		}
		
		[Test]
		public void Dispose_causeAchievedRolesDisposition()
		{
			// arrange:
			FakeRole roleToReturn = GeneratePartialMock<FakeRole>();
			roleToReturn.Expect(r => r.Dispose()).Repeat.Once();
			FakeWorkingSession fakeSession = GeneratePartialMock<FakeWorkingSession>();
			IPrincipal owner = fakeSession.Owner;
			fakeSession.Expect(s => s.CallIsAllowed<IFakeRole>()).Return(true).Repeat.Once();
			FakeRoleBuilder<IFakeRole, FakeRole> roleBuilder = GeneratePartialMock<FakeRoleBuilder<IFakeRole, FakeRole>>();
			roleBuilder.Expect(b => b.CallCreateRoleFor(owner)).Return(roleToReturn).Repeat.Once();
			fakeSession.Expect(s => s.CallGetRoleBuilder<IFakeRole>()).Return(roleBuilder).Repeat.Once();
			IFakeRole achievedRole = null;
			fakeSession.Achieve<IFakeRole>(out achievedRole);
			fakeSession.Expect(s => s.CallBeforeDispose()).Repeat.Once();
			IWorkingSession session = fakeSession;
			IFakeRole roleRef = null;
			
			// act:
			fakeSession.Dispose();

			// assert:
			Assert.Throws<ObjectDisposedException>(delegate { session.CanAchieve<IFakeRole>(); });
			Assert.Throws<ObjectDisposedException>(delegate { session.Achieve<IFakeRole>(out roleRef); });
			Assert.IsNull(roleRef);
			Assert.Throws<ObjectDisposedException>(delegate { session.Leave<IFakeRole>(ref roleRef); });
		}

		
		#endregion Dispose
	}
}

