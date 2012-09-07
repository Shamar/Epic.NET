//
//  StatelessSpecificationBase.cs
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
    /// Base class for stateless specification.
    /// </summary>
    /// <typeparam name="TSpecification">Type of the specification implemented.</typeparam>
    /// <typeparam name="TCandidate">Type of the candidates to satisfy the specification.</typeparam>
    [Serializable]
    public abstract class StatelessSpecificationBase<TSpecification, TCandidate> : SpecificationBase<TSpecification, TCandidate>
        where TCandidate : class
        where TSpecification : class, ISpecification<TCandidate>, IEquatable<TSpecification>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatelessSpecificationBase{TSpecification, TCandidate}"/> class.
        /// </summary>
        protected StatelessSpecificationBase ()
        {
        }
        
        /// <summary>
        /// Returns <c>true</c> for any <typeparamref name="TSpecification"/>, since its has to be stateless.
        /// </summary>
        /// <returns>
        /// Always <c>true</c>.
        /// </returns>
        /// <param name='otherSpecification'>
        /// Other specification.
        /// </param>
        protected override sealed bool EqualsA (TSpecification otherSpecification)
        {
            return true;
        }
    }
}

