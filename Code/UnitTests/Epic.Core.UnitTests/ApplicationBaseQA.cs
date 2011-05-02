//  
//  ApplicationBaseQA.cs
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
namespace Epic
{
	[TestFixture]
	public class ApplicationBaseQA
	{
		class FakeApplication : ApplicationBase
		{
			public FakeApplication(string name)
				: base(name)
			{
			}
			
			public override IEnvironment Environment { get {throw new NotImplementedException();} }
		
			public override IEnterprise Enterprise { get {throw new NotImplementedException();} }

		}
		
		[Test]
		public void Ctor_withNullName_throwsArgumentNullException()
		{
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new FakeApplication(null); });
		}

		[Test]
		public void Ctor_withEmptyName_throwsArgumentNullException()
		{
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new FakeApplication(string.Empty); });
		}
	}
}

