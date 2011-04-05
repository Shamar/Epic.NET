//  
//  AndSpecificationTester.cs
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
	[TestFixture]
	public class AndSpecificationQA : AbstractSpecificationQA<AndSpecification<object>, object>
	{
		#region implemented abstract members of DefaultImplementation.AbstractSpecificationTester[AndSpecification[System.Object],System.Object]
		protected override void CreateEqualsSpecification (out AndSpecification<object> spec1, out AndSpecification<object> spec2)
		{
			ISpecification<object> left = MockRepository.GenerateMock<ISpecification<object>>();
			left.Stub(s => s.Equals(left)).Return(true).Repeat.Any();
			ISpecification<object> right = MockRepository.GenerateMock<ISpecification<object>>();
			right.Stub(s => s.Equals(right)).Return(true).Repeat.Any();
			
			right.Stub(s => s.Equals(left)).Return(false).Repeat.Any();
			left.Stub(s => s.Equals(right)).Return(false).Repeat.Any();
			
			spec1 = new AndSpecification<object>(left, right);
			spec2 = new AndSpecification<object>(left, right);
		}
		
		
		protected override void CreateDifferentSpecification (out AndSpecification<object> spec1, out AndSpecification<object> spec2)
		{
			ISpecification<object> left = MockRepository.GenerateMock<ISpecification<object>>();
			left.Stub(s => s.Equals(left)).Return(true).Repeat.Any();
			ISpecification<object> right = MockRepository.GenerateMock<ISpecification<object>>();
			right.Stub(s => s.Equals(right)).Return(true).Repeat.Any();
			

			spec1 = new AndSpecification<object>(right, right);
			spec2 = new AndSpecification<object>(left, left);
		}
		
		
		protected override AndSpecification<object> CreateNewSpecification ()
		{
			ISpecification<object> left = MockRepository.GenerateMock<ISpecification<object>>();
			left.Stub(s => s.Equals(left)).Return(true).Repeat.Any();
			ISpecification<object> right = MockRepository.GenerateMock<ISpecification<object>>();
			right.Stub(s => s.Equals(right)).Return(true).Repeat.Any();
			
			right.Stub(s => s.Equals(left)).Return(false).Repeat.Any();
			left.Stub(s => s.Equals(right)).Return(false).Repeat.Any();
			
			return new AndSpecification<object>(left, right);
		}
		
		#endregion
		
		[Test]
		public void Ctor_01()
		{
			// arrange:
			ISpecification<object> left = MockRepository.GenerateMock<ISpecification<object>>();
			ISpecification<object> right = MockRepository.GenerateMock<ISpecification<object>>();	
			
			// act:
			AndSpecification<object> spec = new AndSpecification<object>(left,right);
		
			// assert:
			Assert.IsNotNull(spec);
			Assert.AreSame(left, spec.First);
			Assert.AreSame(right, spec.Second);
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Ctor_02 ()
		{
			// arrange:
			ISpecification<object> right = MockRepository.GenerateMock<ISpecification<object>>();	
			
			// act:
			new AndSpecification<object>(null,right);
		}
		
		[Test()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Ctor_03 ()
		{
			// arrange:
			ISpecification<object> left = MockRepository.GenerateMock<ISpecification<object>>();	
			
			// act:
			new AndSpecification<object>(left,null);
		}
		
		[Test]
		public void IsSatisfiedBy_01()
		{
			// arrange:
			object candidate = new object();
			ISpecification<object> left = MockRepository.GenerateMock<ISpecification<object>>();
			left.Expect(s => s.IsSatisfiedBy(candidate)).Return(true);
			ISpecification<object> right = MockRepository.GenerateMock<ISpecification<object>>();	
			right.Expect(s => s.IsSatisfiedBy(candidate)).Return(true);
			
		
			// act:
			ISpecification<object> target = new AndSpecification<object>(left, right);
			bool satisfied = target.IsSatisfiedBy(candidate);
		
			// assert:
			Assert.IsTrue(satisfied);
		 	left.VerifyAllExpectations();
			right.VerifyAllExpectations();
		}
		
		[Test]
		public void IsSatisfiedBy_02()
		{
			// arrange:
			object candidate = new object();
			ISpecification<object> left = MockRepository.GenerateMock<ISpecification<object>>();
			left.Expect(s => s.IsSatisfiedBy(candidate)).Return(true);
			ISpecification<object> right = MockRepository.GenerateMock<ISpecification<object>>();	
			right.Expect(s => s.IsSatisfiedBy(candidate)).Return(false);
			
		
			// act:
			ISpecification<object> target = new AndSpecification<object>(left, right);
			bool satisfied = target.IsSatisfiedBy(candidate);
		
			// assert:
			Assert.IsFalse(satisfied);
		 	left.VerifyAllExpectations();
			right.VerifyAllExpectations();
		}
		
		[Test]
		public void IsSatisfiedBy_03()
		{
			// arrange:
			object candidate = new object();
			ISpecification<object> left = MockRepository.GenerateMock<ISpecification<object>>();
			left.Expect(s => s.IsSatisfiedBy(candidate)).Return(false);
			ISpecification<object> right = MockRepository.GenerateMock<ISpecification<object>>();	
			
		
			// act:
			ISpecification<object> target = new AndSpecification<object>(left, right);
			bool satisfied = target.IsSatisfiedBy(candidate);
		
			// assert:
			Assert.IsFalse(satisfied);
		 	left.VerifyAllExpectations();
			right.AssertWasNotCalled(s => s.IsSatisfiedBy(candidate));
		}
		
		[Test]
		public void IsSatisfiedBy_04()
		{
			// arrange:
			object candidate = new object();
			ISpecification<object> left = MockRepository.GenerateMock<ISpecification<object>>();
			left.Expect(s => s.IsSatisfiedBy(candidate)).Return(false);
			ISpecification<object> right = MockRepository.GenerateMock<ISpecification<object>>();	
			right.Expect(s => s.IsSatisfiedBy(candidate)).Return(false);
			
		
			// act:
			ISpecification<object> target = new AndSpecification<object>(left, right);
			bool satisfied = target.IsSatisfiedBy(candidate);
		
			// assert:
			Assert.IsFalse(satisfied);
		 	left.VerifyAllExpectations();
            right.AssertWasNotCalled(s => s.IsSatisfiedBy(candidate));
        }
	}
}

