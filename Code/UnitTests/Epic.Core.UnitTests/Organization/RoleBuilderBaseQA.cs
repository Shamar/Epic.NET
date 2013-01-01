//  
//  RoleBuilderBaseQA.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2013 Giacomo Tesio
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
using Rhino.Mocks;
using System.Security.Principal;
using System.IO;
using Epic;

namespace Epic.Organization
{
	[TestFixture]
	public class RoleBuilderBaseQA
	{
		[Test]
		public void TypeCtor_withConcreteRole_throwsInvalidOperationException ()
		{
			// arrange:
			TypeInitializationException exception = null;
			
			// act:
			try
			{
				new FakeRoleBuilder<FakeRole, FakeRole> ();
			}
			catch(TypeInitializationException ex)
			{
				exception = ex;
			}
			
			// assert:
			Assert.IsNotNull(exception);
			Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
		}
		
		[Test]
		public void TypeCtor_withInterfaceRole_works ()
		{
			// assert:
			new FakeRoleBuilder<IFakeRole, FakeRole>();
		}
		
		[Test]
		public void Build_withPrincipal_callTemplateMethod()
		{
			// arrange:
			IPrincipal player = MockRepository.GenerateStrictMock<IPrincipal>();
			FakeRole role = MockRepository.GeneratePartialMock<FakeRole>();
			FakeRoleBuilder<IFakeRole, FakeRole> builder = MockRepository.GeneratePartialMock<FakeRoleBuilder<IFakeRole, FakeRole>>();
			builder.Expect(b => b.CallCreateRoleFor(player)).Return(role).Repeat.Once();
			
			// act:
			IFakeRole returnedRole = builder.Build(player);
			
			// assert:
			Assert.AreSame(role, returnedRole);
		}
		
		[Test]
		public void Build_withoutPrincipal_callTemplateMethod()
		{
			// arrange:
			IPrincipal player = null;
			FakeRole role = MockRepository.GeneratePartialMock<FakeRole>();
			FakeRoleBuilder<IFakeRole, FakeRole> builder = MockRepository.GeneratePartialMock<FakeRoleBuilder<IFakeRole, FakeRole>>();
			builder.Expect(b => b.CallCreateRoleFor(player)).Return(role).Repeat.Once();
			
			// act:
			IFakeRole returnedRole = builder.Build(player);
			
			// assert:
			Assert.AreSame(role, returnedRole);
		}
		
		[Test]
		public void Serialize_works()
		{
			// arrange:
			FakeRoleBuilder<IFakeRole, FakeRole> builder = new FakeRoleBuilder<IFakeRole, FakeRole>();
			
			// act:
			Stream stream = SerializationUtilities.Serialize(builder);
			
			// assert:
			Assert.IsNotNull(stream);
		}
		
		[Test]
		public void Deserialize_works()
		{
			// arrange:
			FakeRoleBuilder<IFakeRole, FakeRole> builder = new FakeRoleBuilder<IFakeRole, FakeRole>();
			Stream stream = SerializationUtilities.Serialize(builder);
			
			// act:
			FakeRoleBuilder<IFakeRole, FakeRole> deserialized = SerializationUtilities.Deserialize<FakeRoleBuilder<IFakeRole, FakeRole>>(stream);
			
			// assert:
			Assert.IsNotNull(deserialized);
		}
	}
}

