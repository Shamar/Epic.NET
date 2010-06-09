using System;
using System.Linq.Expressions;
using Challenge00.DDDSample.Shared;
namespace Challenge00.DDDSample.Default.Shared
{
	[Serializable]
	public sealed class AndSpecification<TCandidate> : AbstractSpecification<TCandidate>
	{
		public readonly ISpecification<TCandidate> First;
		public readonly ISpecification<TCandidate> Second;
		
		public AndSpecification (ISpecification<TCandidate> first, ISpecification<TCandidate> second)
		{
			if(null == first)
				throw new ArgumentNullException("first");
			if(null == second)
				throw new ArgumentNullException("second");
			
			First = first;
			Second = second;
		}
		
		#region implemented abstract members of Challenge00.DDDSample.Shared.AbstractSpecification[TCandidate]
		
		public override bool IsSatisfiedBy (TCandidate candidate)
		{
			return First.IsSatisfiedBy(candidate) && Second.IsSatisfiedBy(candidate);
		}
		
		protected override bool Equals (AbstractSpecification<TCandidate> other)
		{
			AndSpecification<TCandidate> specification = other as AndSpecification<TCandidate>;
			if(null == specification)
				return false;
			return this.First.Equals(specification.First) && this.Second.Equals(specification.Second);
		}
		
		protected override int GetStateHashCode ()
		{
			return First.GetHashCode() ^ Second.GetHashCode();
		}
		
		#endregion
	}
}

