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
		
		#endregion Serialization
	
		
	}
}

