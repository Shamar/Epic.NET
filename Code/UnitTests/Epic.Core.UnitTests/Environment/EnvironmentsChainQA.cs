//  
//  EnvironmentsChainQA.cs
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
using System.Collections.Generic;
namespace Epic.Environment
{
	[TestFixture]
	public class EnvironmentsChainQA : EnvironmentBaseQA
	{
		protected override EnvironmentBase CreateNewEnvironmentToTest ()
		{
			EnvironmentChainLinkBase firstMockLink;
			EnvironmentChainLinkBase secondMockLink;
			return CreateNewEnvironmentChainToTest(out firstMockLink, out secondMockLink);
		}
		
		[Test]
		public void Ctor_withoutLinks_throwsArgumentNullException()
		{
			// assert:
			Assert.Throws<ArgumentNullException>(delegate { new EnvironmentsChain(); });
			Assert.Throws<ArgumentNullException>(delegate { new EnvironmentsChain(null); });
		}
		
		public EnvironmentsChain CreateNewEnvironmentChainToTest(out EnvironmentChainLinkBase firstMockLink, out EnvironmentChainLinkBase secondMockLink)
		{
			firstMockLink = MockRepository.GeneratePartialMock<EnvironmentChainLinkBase>();
			secondMockLink = MockRepository.GeneratePartialMock<EnvironmentChainLinkBase>();
			return new EnvironmentsChain(firstMockLink, secondMockLink);
		}
		
		[TestCase("test", (int)1)]
		[TestCase("test", "dummyString")]
		public void Get_unknownInformation_throwsKeyNotFoundException<TObject>(string iName, TObject dummyObject)
		{
			// arrange:
			List<object> mocks = new List<object>();
			InstanceName<TObject> name = new InstanceName<TObject>(iName);
			EnvironmentChainLinkBase firstMockLink;
			EnvironmentChainLinkBase secondMockLink;
			EnvironmentsChain underTest = CreateNewEnvironmentChainToTest(out firstMockLink, out secondMockLink);
			mocks.Add(firstMockLink);
			mocks.Add(secondMockLink);
			TObject varPlaceHolder;
			firstMockLink.Expect(l => l.TryGet<TObject>(name, out varPlaceHolder)).Return(false).Repeat.Once();
			secondMockLink.Expect(l => l.TryGet<TObject>(name, out varPlaceHolder)).Return(false).Repeat.Once();
			
			// assert:
			Assert.Throws<KeyNotFoundException>(delegate { underTest.Get<TObject>(name); });
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[TestCase("test", (int)1)]
		[TestCase("test", "dummyString")]
		public void Get_anInformationKnownToFirstLink_dontCallOtherLinksAndReturnTheValue<TObject>(string iName, TObject valueToReturn)
		{
			// arrange:
			List<object> mocks = new List<object>();
			InstanceName<TObject> name = new InstanceName<TObject>(iName);
			EnvironmentChainLinkBase firstMockLink;
			EnvironmentChainLinkBase secondMockLink;
			EnvironmentsChain underTest = CreateNewEnvironmentChainToTest(out firstMockLink, out secondMockLink);
			mocks.Add(firstMockLink);
			mocks.Add(secondMockLink);
			TObject varPlaceHolder;
			firstMockLink.Expect(l => l.TryGet<TObject>(name, out varPlaceHolder)).Return(true).OutRef(valueToReturn).Repeat.Once();
			TObject objectProvided;

			// act:
			objectProvided = underTest.Get<TObject>(name);
			
			// assert:
			Assert.AreEqual(valueToReturn, objectProvided);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
		
		[TestCase("test", (int)1)]
		[TestCase("test", "dummyString")]
		public void Get_anInformationKnownToSecondLink_returnTheValue<TObject>(string iName, TObject valueToReturn)
		{
			// arrange:
			List<object> mocks = new List<object>();
			InstanceName<TObject> name = new InstanceName<TObject>(iName);
			EnvironmentChainLinkBase firstMockLink;
			EnvironmentChainLinkBase secondMockLink;
			EnvironmentsChain underTest = CreateNewEnvironmentChainToTest(out firstMockLink, out secondMockLink);
			mocks.Add(firstMockLink);
			mocks.Add(secondMockLink);
			TObject varPlaceHolder;
			firstMockLink.Expect(l => l.TryGet<TObject>(name, out varPlaceHolder)).Return(false).Repeat.Once();
			secondMockLink.Expect(l => l.TryGet<TObject>(name, out varPlaceHolder)).Return(true).OutRef(valueToReturn).Repeat.Once();
			TObject objectProvided;

			// act:
			objectProvided = underTest.Get<TObject>(name);
			
			// assert:
			Assert.AreEqual(valueToReturn, objectProvided);
			foreach(object mock in mocks)
				mock.VerifyAllExpectations();
		}
	}
}

