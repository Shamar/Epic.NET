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
        /// Check if the <typeparamref name="TCandidate"> satisfy the specification. 
        /// </summary>
        /// <param name="candidate">
        /// A <see cref="TCandidate"/>.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> when <paramref name="candidate"/> satisfies the specification, <c>false</c> otherwise.
        /// Note that <c>null</c> can not satisfy any specification.
        /// </returns>
        bool IsSatisfiedBy(TCandidate candidate);
    }

}

