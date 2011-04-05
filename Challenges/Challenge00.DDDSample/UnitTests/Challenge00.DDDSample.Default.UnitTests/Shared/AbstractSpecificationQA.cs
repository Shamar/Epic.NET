//  
//  AbstractSpecificationTester.cs
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
using Challenge00.DDDSample.Shared;
using Rhino.Mocks;
namespace DefaultImplementation.Shared
{
	public abstract class AbstractSpecificationQA<TSpecification, TCandidate>
		where TSpecification : AbstractSpecification<TCandidate>
	{
		protected abstract void CreateEqualsSpecification(out TSpecification spec1, out TSpecification spec2);
		
		protected abstract void CreateDifferentSpecification(out TSpecification spec1, out TSpecification spec2);
		
		protected abstract TSpecification CreateNewSpecification();
		
		[Test]
		public void Equals_01()
		{
			TSpecification spec = CreateNewSpecification();
			
			Assert.IsTrue(spec.Equals(spec));
		}
		
		[Test]
		public void Equals_02()
		{
			TSpecification spec = CreateNewSpecification();
			
			Assert.IsFalse(spec.Equals(null));
			Assert.IsFalse(spec.Equals(MockRepository.GenerateStrictMock<ISpecification<TCandidate>>()));
		}
		
		[Test]
		public void Equals_03()
		{
			// arrange:
			TSpecification spec1 = null;
			TSpecification spec2 = null;
			CreateEqualsSpecification(out spec1, out spec2);
		
			// act:
		
			// assert:
			Assert.AreEqual(spec1, spec2);
			Assert.AreEqual(spec2, spec1);
			Assert.AreEqual(spec1.GetHashCode(), spec2.GetHashCode());
		}
		
		[Test]
		public void Equals_04()
		{
			// arrange:
			TSpecification spec1 = null;
			TSpecification spec2 = null;
			CreateDifferentSpecification(out spec1, out spec2);
			
		
			// act:
		
		
			// assert:
			Assert.AreNotEqual(spec1, spec2);
			Assert.AreNotEqual(spec2, spec1);
		}
	}
}

