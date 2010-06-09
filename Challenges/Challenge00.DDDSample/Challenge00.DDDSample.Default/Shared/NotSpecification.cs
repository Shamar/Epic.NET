using System;
using Challenge00.DDDSample.Shared;
namespace Challenge00.DDDSample.Default.Shared
{
	[Serializable]
	public sealed class NotSpecification<TCandidate> : AbstractSpecification<TCandidate>
	{
		public readonly ISpecification<TCandidate> Negated;
		public NotSpecification (ISpecification<TCandidate> specification)
		{
			if(null == specification)
				throw new ArgumentNullException("specification");
			Negated = specification;
		}
		
		#region implemented abstract members of Challenge00.DDDSample.Shared.AbstractSpecification[TCandidate]
		public override bool IsSatisfiedBy (TCandidate candidate)
		{
			return !Negated.IsSatisfiedBy(candidate);
		}
		
		protected override bool Equals (AbstractSpecification<TCandidate> other)
		{
			NotSpecification<TCandidate> s = other as NotSpecification<TCandidate>;
			if(null == s)
				return false;
			return Negated.Equals(s.Negated);
		}
		
		protected override int GetStateHashCode ()
		{
			return Negated.GetHashCode();
		}
		
		#endregion
	}
}

