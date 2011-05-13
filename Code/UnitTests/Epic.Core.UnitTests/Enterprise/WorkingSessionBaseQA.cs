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
	}
}

