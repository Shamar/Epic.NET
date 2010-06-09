using System;
using System.Linq.Expressions;

namespace Challenge00.DDDSample.Shared
{
	/// <summary>
	/// Specificaiton interface.  
	/// </summary>
	public interface ISpecification<TCandidate> : IEquatable<ISpecification<TCandidate>>
	{
		/// <summary>
		/// Check if the <typeparamref name="TCandidate"> satisfy the specification. 
		/// </summary>
		/// <param name="candidate">
		/// A <see cref="TCandidate"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		bool IsSatisfiedBy(TCandidate candidate);
	}
}

