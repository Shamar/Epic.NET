using System;
using NUnit.Framework;
using Challenge00.DDDSample.Default.Shared;
namespace Challenge00.DDDSample.Default.UnitTests
{
	public abstract class AbstractSpecificationTester<TSpecification, TCandidate>
		where TSpecification : AbstractSpecification<TCandidate>
	{
		protected abstract void CreateEqualsSpecification(out TSpecification spec1, out TSpecification spec2);
		
		protected abstract void CreateDifferentSpecification(out TSpecification spec1, out TSpecification spec2);
		
		protected abstract TSpecification CreateNewSpecification();
		
		[Test]
		public void Test_Equals_01()
		{
			TSpecification spec = CreateNewSpecification();
			
			Assert.IsTrue(spec.Equals(spec));
		}
		
		[Test]
		public void Test_Equals_02()
		{
			TSpecification spec = CreateNewSpecification();
			
			Assert.IsFalse(spec.Equals(null));
		}
		
		[Test]
		public void Test_Equals_03()
		{
			// arrange:
			TSpecification spec1 = null;
			TSpecification spec2 = null;
			CreateEqualsSpecification(out spec1, out spec2);
		
			// act:
		
			// assert:
			Assert.AreEqual(spec1, spec2);
			Assert.AreEqual(spec2, spec1);
		}
		
		[Test]
		public void Test_Equals_04()
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

