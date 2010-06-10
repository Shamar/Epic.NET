using System;
using NUnit.Framework;
using Challenge00.DDDSample.Default.Shared;
using Challenge00.DDDSample.Shared;
using Rhino.Mocks;
namespace Challenge00.DDDSample.Default.UnitTests
{
	[TestFixture]
	public class AndSpecificationTester : AbstractSpecificationTester<AndSpecification<object>, object>
	{
		#region implemented abstract members of Challenge00.DDDSample.Default.UnitTests.AbstractSpecificationTester[AndSpecification[System.Object],System.Object]
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
	}
}

