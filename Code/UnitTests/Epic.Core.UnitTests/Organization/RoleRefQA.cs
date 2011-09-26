//  
//  RoleRefQA.cs
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
using Rhino.Mocks;
using Epic.Fakes;
using System.IO;

namespace Epic.Organization
{
	[TestFixture]
	public class RoleRefQA
	{
		[Test]
		public void Ctor_withoutRole_throwArgumentNullException()
		{
			// assert:
			Assert.Throws<ArgumentNullException>(delegate {
				new RoleRef(null);
			});
		}
		
		[Test]
		public void Ctor_withRole_works()
		{
			// arrange:
			RoleBase role = MockRepository.GenerateStrictMock<RoleBase>();
			
			// act:
			RoleRef roleRef = new RoleRef(role);
			
			// assert:
			Assert.AreSame(role, roleRef.Role);
		}
		
		[Test]
		public void Increase_increaseTheNumberOfReferences()
		{
			// arrange:
			RoleBase role = MockRepository.GenerateStrictMock<RoleBase>();
			RoleRef roleRef = new RoleRef(role);
			
			// assert:
			for(int i = 0; i < 100; ++i)
			{
				Assert.AreEqual(i+1, roleRef.Increase());
			}
		}
		
		[Test]
		public void Decrease_decreaseTheNumberOfReferences()
		{
			// arrange:
			RoleBase role = MockRepository.GenerateStrictMock<RoleBase>();
			RoleRef roleRef = new RoleRef(role);
			
			// assert:
			for(int i = 0; i < 100; ++i)
			{
				roleRef.Increase();
				Assert.AreEqual(0, roleRef.Decrease());
			}
			for(int i = 0; i < 100; ++i)
				roleRef.Increase();
			for(int i = 0; i < 100; ++i)
			{
				Assert.AreEqual(100 - (i+1), roleRef.Decrease());
			}
		}
		
		[Test]
		public void Decrease_underZero_throwsInvalidOperationException()
		{
			// arrange:
			RoleBase role = MockRepository.GenerateStrictMock<RoleBase>();
			RoleRef roleRef = new RoleRef(role);
			
			// assert:
			Assert.Throws<InvalidOperationException>(delegate{ roleRef.Decrease(); });
		}
		
				
		[Test]
		public void Dispose_willDisposeTheRole()
		{
			// arrange:
			RoleBase role = MockRepository.GenerateStrictMock<RoleBase>();
			role.Expect(r => r.Dispose()).Repeat.Once();
			RoleRef roleRef = new RoleRef(role);
			
			// assert:
			roleRef.Dispose();
		}
		
		[Test]
		public void Serialize_works()
		{
			// arrange:
			RoleBase role = new FakeRole();
			RoleRef roleRef = new RoleRef(role);
			int numRef = 100;
			for(int i = 0; i < numRef; ++i)
				roleRef.Increase();
			
			// act:
			Stream stream = TestUtilities.Serialize(roleRef);
			
			// assert:
			Assert.IsNotNull(stream);
		}
		
		[Test]
		public void Deserialize_works()
		{
			// arrange:
			RoleBase role = new FakeRole();
			RoleRef roleRef = new RoleRef(role);
			int numRef = 100;
			for(int i = 0; i < numRef; ++i)
				roleRef.Increase();
			Stream stream = TestUtilities.Serialize(roleRef);
			
			// act:
			RoleRef deserialized = TestUtilities.Deserialize<RoleRef>(stream);
			int nextRef = deserialized.Increase();
			
			// assert:
			Assert.AreEqual(numRef + 1, nextRef);
		}
	}
}

