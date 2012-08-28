//  
//  ISpecification.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2012 Giacomo Tesio
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
namespace Epic.Specifications
{
    /// <summary>
    /// Specificaiton interface.  
    /// </summary>
    /// <typeparam name="TCandidate">The (reference) type of the objects 
    /// that can satisfy the specification.</typeparam>
    public interface ISpecification<TCandidate> : IEquatable<ISpecification<TCandidate>>, IVisitable
        where TCandidate : class
    {
        /// <summary>
        /// Gets a <see cref="Type"/> of objects that can satisfy this <see cref="ISpecification{TCandidate}"/>.
        /// It can be an abstraction or a specialization of <typeparamref name="TCandidate"/>.
        /// </summary>
        Type CandidateType { get; }

        /// <summary>
        /// Gets the type of the specification.
        /// </summary>
        Type SpecificationType { get; }

        /// <summary>
        /// Check if the <typeparamref name="TCandidate"/> satisfy the specification. 
        /// </summary>
        /// <param name="candidate">
        /// A <typeparamref name="TCandidate"/>.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> when <paramref name="candidate"/> satisfies the specification, <c>false</c> otherwise.
        /// Note that <see langword="null"/> can not satisfy any specification.
        /// </returns>
        bool IsSatisfiedBy(TCandidate candidate);

        /// <summary>
        /// Create a new <see cref="ISpecification{TCandidate}"/> that evaluates the <paramref name="other"/> 
        /// only if the current specification is satisfied.
        /// </summary>
        /// <param name="other">The other specification to evaluate.</param>
        /// <returns>A new <see cref="ISpecification{TCandidate}"/> that evaluates the <paramref name="other"/> 
        /// only if the current specification is satisfied.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        ISpecification<TCandidate> And(ISpecification<TCandidate> other);
        
        /// <summary>
        /// Create a new <see cref="ISpecification{TCandidate}"/> that evaluates the <paramref name="other"/> 
        /// only if the current specification is not satisfied.
        /// </summary>
        /// <param name="other">The other specification to evaluate.</param>
        /// <returns>A new <see cref="ISpecification{TCandidate}"/> that evaluates the <paramref name="other"/> 
        /// only if the current specification is not satisfied.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        ISpecification<TCandidate> Or(ISpecification<TCandidate> other);

        /// <summary>
        /// Create a new <see cref="ISpecification{TCandidate}"/> that is satisfied if and only if the
        /// current specification is not satisfied.
        /// </summary>
        /// <returns>A new <see cref="ISpecification{TCandidate}"/> that is satisfied if and only if the
        /// current specification is not satisfied.</returns>
        ISpecification<TCandidate> Negate();

        /// <summary>
        /// Create a new <see cref="ISpecification{TOther}"/> that is satisfied by all the <typeparamref name="TOther"/> 
        /// that are <typeparamref name="TCandidate"/> and satisfy the current specification.
        /// </summary>
        /// <returns>
        /// A new <see cref="ISpecification{TOther}"/> that is satisfied by all the <typeparamref name="TOther"/> 
        /// that are <typeparamref name="TCandidate"/> and satisfy the current specification.
        /// </returns>
        /// <typeparam name='TOther'>
        /// The type of objects that we want to check against the current specification. 
        /// It must either abstract or extend <typeparamref name="TCandidate"/>.
        /// </typeparam>
        /// <exception cref="InvalidCastException"><typeparamref name="TOther"/> does not abstract or specialize <typeparamref name="TCandidate"/>.</exception>
        /// <exception cref="InvalidOperationException"><typeparamref name="TOther"/> is <see cref="System.Object"/>.</exception>
        ISpecification<TOther> OfType<TOther>() where TOther : class;
    }
}

