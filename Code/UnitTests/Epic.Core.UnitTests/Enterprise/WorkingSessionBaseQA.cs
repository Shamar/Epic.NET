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
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new FakeWorkingSession(null); });
			Assert.Throws<ArgumentNullException>(delegate { new FakeWorkingSession(string.Empty); });
		}
		
		[Test]
		public void Ctor_withIdentifier_works()
		{
			// arrange:
			string identifier = "test";
			
			// act:
			IWorkingSession session = new FakeWorkingSession(identifier);
			
			// assert:
			Assert.AreSame(identifier, session.Identifier);
			Assert.IsEmpty(session.Owner);
		}
		
		#endregion Constructor
		
		#region Serialization
		
		[TestCase("Test1")]
		[TestCase("Test2")]
		[TestCase("Test3")]
		public void Serialize_works(string identifier)
		{
			// arrange:
			IWorkingSession session = new FakeWorkingSession(identifier);
			
			// act:
			Stream stream = TestUtilities.Serialize(session);
			
			// assert:
			Assert.IsNotNull(stream);
		}
		
		[TestCase("Test1", "Owner1")]
		[TestCase("Test2", "Owner2")]
		[TestCase("Test3", "Owner3")]
		public void Serialize_works_afterAssignTo(string identifier, string ownerId)
		{
			// arrange:
			string[] roles = new string[] { "First", "Second" };
			IWorkingSession session = new FakeWorkingSession(identifier);
			IPrincipal owner = new GenericPrincipal(new GenericIdentity(ownerId), roles);
			session.AssignTo(owner);
			
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
			IWorkingSession session = new FakeWorkingSession(identifier);
			Stream stream = TestUtilities.Serialize(session);
			
			// act:
			IWorkingSession deserialized = TestUtilities.Deserialize<IWorkingSession>(stream);
			
			// assert:
			Assert.IsNotNull(deserialized);
			Assert.AreEqual(identifier, deserialized.Identifier);
		}
		
		
		[TestCase("Test1", "Owner1")]
		[TestCase("Test2", "Owner2")]
		[TestCase("Test3", "Owner3")]
		public void Deserialize_works_afterAssignTo(string identifier, string ownerId)
		{
			// arrange:
			string[] roles = new string[] { "First", "Second" };
			IPrincipal owner = new GenericPrincipal(new GenericIdentity(ownerId), roles);
			IWorkingSession session = new FakeWorkingSession(identifier);
			session.AssignTo(owner);
			Stream stream = TestUtilities.Serialize(session);
			
			// act:
			IWorkingSession deserialized = TestUtilities.Deserialize<IWorkingSession>(stream);
			
			// assert:
			Assert.IsNotNull(deserialized);
			Assert.AreEqual(ownerId, deserialized.Owner);
		}
		
		#endregion Serialization
		
		#region AssignTo
		
		[Test]
		public void AssignTo_nullPrincipal_throwsArgumentNullException()
		{
			// arrange:
			IWorkingSession session = new FakeWorkingSession();
		
			// assert:
			Assert.Throws<ArgumentNullException>(delegate {
				session.AssignTo(null);
			});
		}
		
		[Test]
		public void AssignTo_anAllowedPrincipal_works()
		{
			// arrange:
			string ownerId = "TestPrincipal";
			IPrincipal owner = new GenericPrincipal(new GenericIdentity(ownerId), new string[0]);
			FakeWorkingSession session = GeneratePartialMock<FakeWorkingSession>();
			IPrincipal outRef = null;
			session.Expect(s => s.CallAllowNewOwner(owner, out outRef)).OutRef(owner).Return(true).Repeat.Once();
			
			// act:
			session.AssignTo(owner);

			// assert:
			Assert.AreEqual(ownerId, ((IWorkingSession)session).Owner);
			Assert.AreSame(owner, session.CurrentOwner);
		}
		
		[Test]
		public void AssignTo_anAllowedPrincipal_raiseOwnerChanged()
		{
			// arrange:
			string ownerId = "TestPrincipal";
			IPrincipal owner = new GenericPrincipal(new GenericIdentity(ownerId), new string[0]);
			FakeWorkingSession session = GeneratePartialMock<FakeWorkingSession>();
			IPrincipal outRef = null;
			session.Expect(s => s.CallAllowNewOwner(owner, out outRef)).OutRef(owner).Return(true).Repeat.Once();
			object eventSender = null;
			ChangeEventArgs<string> changeEventArgs = null;
			session.OwnerChanged += (sender, e) => { eventSender = sender; changeEventArgs = e; };
			
			// act:
			session.AssignTo(owner);

			// assert:
			Assert.AreSame(session, eventSender);
			Assert.IsNotNull(changeEventArgs);
			Assert.IsNullOrEmpty(changeEventArgs.OldValue);
			Assert.AreEqual(ownerId, changeEventArgs.NewValue);
		}
		
		[Test]
		public void AssignTo_aDifferentAllowedPrincipal_raiseOwnerChanged()
		{
			// arrange:
			string ownerId1 = "TestPrincipal1";
			IPrincipal owner1 = new GenericPrincipal(new GenericIdentity(ownerId1), new string[0]);
			string ownerId2 = "TestPrincipal2";
			IPrincipal owner2 = new GenericPrincipal(new GenericIdentity(ownerId2), new string[0]);
			FakeWorkingSession session = GeneratePartialMock<FakeWorkingSession>();
			IPrincipal outRef = null;
			session.Expect(s => s.CallAllowNewOwner(owner1, out outRef)).OutRef(owner1).Return(true).Repeat.Once();
			session.AssignTo(owner1);
			object eventSender = null;
			ChangeEventArgs<string> changeEventArgs = null;
			session.OwnerChanged += (sender, e) => { eventSender = sender; changeEventArgs = e; };
			
			// act:
			session.AssignTo(owner2);

			// assert:
			Assert.AreSame(session, eventSender);
			Assert.IsNotNull(changeEventArgs);
			Assert.AreEqual(ownerId1, changeEventArgs.OldValue);
			Assert.AreEqual(ownerId2, changeEventArgs.NewValue);
		}
		
		[Test]
		public void AssignTo_aPrincipal_dontCallUnsubscribedHandlersOf_OwnerChanged()
		{
			// arrange:
			string ownerId1 = "TestPrincipal1";
			IPrincipal owner1 = new GenericPrincipal(new GenericIdentity(ownerId1), new string[0]);
			string ownerId2 = "TestPrincipal2";
			IPrincipal owner2 = new GenericPrincipal(new GenericIdentity(ownerId2), new string[0]);
			FakeWorkingSession session = GeneratePartialMock<FakeWorkingSession>();
			IPrincipal outRef = null;
			session.Expect(s => s.CallAllowNewOwner(owner1, out outRef)).OutRef(owner1).Return(true).Repeat.Once();
			session.AssignTo(owner1);
			object eventSender = null;
			ChangeEventArgs<string> changeEventArgs = null;
			EventHandler<ChangeEventArgs<string>> handler = (sender, e) => { eventSender = sender; changeEventArgs = e; };
			
			// act:
			session.OwnerChanged += handler;
			session.OwnerChanged -= handler;
			session.AssignTo(owner2);

			// assert:
			Assert.IsNull(eventSender);
			Assert.IsNull(changeEventArgs);
		}
		
		[Test]
		public void AssignTo_aPrincipal_wrapExceptionsFromDerivedClasses()
		{
			// arrange:
			Exception exception = new Exception();
			string ownerId = "TestPrincipal";
			IPrincipal owner = new GenericPrincipal(new GenericIdentity(ownerId), new string[0]);
			FakeWorkingSession session = GeneratePartialMock<FakeWorkingSession>();
			IPrincipal outRef = null;
			session.Expect(s => s.CallAllowNewOwner(owner, out outRef)).Throw(exception).Repeat.Once();
			InvalidOperationException cought = null;
			
			// act:
			try
			{
				session.AssignTo(owner);
			}
			catch(InvalidOperationException e)
			{
				cought = e;
			}

			// assert:
			Assert.IsEmpty(((IWorkingSession)session).Owner);
			Assert.IsNull(session.CurrentOwner);
			Assert.IsNotNull(cought);
			Assert.AreSame(exception, cought.InnerException);
		}
		
		[Test]
		public void AssignTo_aPrincipal_dontWrapInvalidOperationExceptionsFromDerivedClasses()
		{
			// arrange:
			Exception exception = new InvalidOperationException();
			string ownerId = "TestPrincipal";
			IPrincipal owner = new GenericPrincipal(new GenericIdentity(ownerId), new string[0]);
			FakeWorkingSession session = GeneratePartialMock<FakeWorkingSession>();
			IPrincipal outRef = null;
			session.Expect(s => s.CallAllowNewOwner(owner, out outRef)).Throw(exception).Repeat.Once();
			InvalidOperationException cought = null;
			
			// act:
			try
			{
				session.AssignTo(owner);
			}
			catch(InvalidOperationException e)
			{
				cought = e;
			}

			// assert:
			Assert.IsEmpty(((IWorkingSession)session).Owner);
			Assert.IsNull(session.CurrentOwner);
			Assert.AreSame(exception, cought);
		}

		[Test]
		public void AssignTo_aPrincipalNotAllowed_throwsInvalidOperationException()
		{
			// arrange:
			string ownerId = "TestPrincipal";
			IPrincipal owner = new GenericPrincipal(new GenericIdentity(ownerId), new string[0]);
			FakeWorkingSession session = GeneratePartialMock<FakeWorkingSession>();
			IPrincipal outRef = null;
			session.Expect(s => s.CallAllowNewOwner(owner, out outRef)).Return(false).Repeat.Once();

			// assert:
			Assert.Throws<InvalidOperationException>(delegate { session.AssignTo(owner); });
			Assert.IsEmpty(((IWorkingSession)session).Owner);
			Assert.IsNull(session.CurrentOwner);
		}

		[Test]
		public void AssignTo_aDifferentAllowedPrincipal_whileAchievingAnyRole_throwsInvalidOperationException()
		{
			// arrange:
			string ownerId1 = "TestPrincipal";
			IPrincipal owner1 = new GenericPrincipal(new GenericIdentity(ownerId1), new string[0]);
			string ownerId2 = "TestPrincipal2";
			FakeRole role = new FakeRole();
			IPrincipal owner2 = new GenericPrincipal(new GenericIdentity(ownerId2), new string[0]);
			FakeRoleBuilder<IFakeRole, FakeRole> roleBuilder = GeneratePartialMock<FakeRoleBuilder<IFakeRole, FakeRole>>();
			roleBuilder.Expect(b => b.CallCreateRoleFor(owner1)).Return(role).Repeat.Once();
			FakeWorkingSession session = GeneratePartialMock<FakeWorkingSession>();
			IPrincipal outRef = null;
			session.Expect(s => s.CallAllowNewOwner(owner1, out outRef)).OutRef(owner1).Return(true).Repeat.Once();
			session.Expect(s => s.CallAllowNewOwner(owner2, out outRef)).OutRef(owner2).Return(true).Repeat.Any();
			session.Expect(s => s.CallIsAllowed<IFakeRole>()).Return(true).Repeat.Once();
			session.Expect(s => s.CallGetRoleBuilder<IFakeRole>()).Return(roleBuilder).Repeat.Once();
			session.AssignTo(owner1);
			IFakeRole outRoleRef = null;
			session.Achieve<IFakeRole>(out outRoleRef);

			// assert:
			Assert.Throws<InvalidOperationException>(delegate { session.AssignTo(owner2); });
			Assert.AreEqual(ownerId1, ((IWorkingSession)session).Owner);
			Assert.AreSame(owner1, session.CurrentOwner);
		}
		
		#endregion AssignTo
	}
}

