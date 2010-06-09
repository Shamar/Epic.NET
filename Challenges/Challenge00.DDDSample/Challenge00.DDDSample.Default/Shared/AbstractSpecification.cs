using System;
using System.Linq.Expressions;
using Challenge00.DDDSample.Shared;
namespace Challenge00.DDDSample.Default.Shared
{
	[Serializable]
	public abstract class AbstractSpecification<TCandidate> : ISpecification<TCandidate>
	{
		#region ISpecification<TCandidate> implementation
		
		public abstract bool IsSatisfiedBy (TCandidate candidate);	
		
		#endregion
		
		#region IEquatable<ISpecification<TCandidate>> implementation
		
		public bool Equals (ISpecification<TCandidate> other)
		{
			AbstractSpecification<TCandidate> candidate = other as AbstractSpecification<TCandidate>;
			if(null == candidate)
				return false;
			
			return Equals(candidate);
		}
		
		#endregion
		
		protected abstract bool Equals(AbstractSpecification<TCandidate> other);
		
		public sealed override bool Equals (object obj)
		{
			ISpecification<TCandidate> other = obj as ISpecification<TCandidate>;
			return this.Equals(other);
		}
		
		public sealed override int GetHashCode ()
		{
			return this.GetType().GetHashCode() ^ GetStateHashCode();
		}
		
		/// <summary>
		/// Return the hashcode for the internal state of the concrete specification.
		/// Return <see cref="int.MaxValue"/> for stateless specifications.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		protected abstract int GetStateHashCode();
	}
}

