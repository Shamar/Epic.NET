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

namespace Epic.Enterprise
{
	[TestFixture]
	public class WorkingSessionBaseQA
	{
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
		}
		
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
	}
}

