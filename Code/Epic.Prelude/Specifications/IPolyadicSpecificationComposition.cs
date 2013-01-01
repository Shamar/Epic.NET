//
//  IPolyadicSpecificationComposition.cs
//
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
//
//  Copyright (c) 2010-2013 Giacomo Tesio
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
using System.Collections.Generic;

namespace Epic.Specifications
{
    /// <summary>
    /// Interface implemented by polyadic composition of specifications.
    /// </summary>
    /// <typeparam name="TCandidate">The type of the entity specified by the composition.</typeparam>
    /// <seealso cref="Conjunction{TCandidate}"/>
    /// <seealso cref="Disjunction{TCandidate}"/>
    /// <seealso cref="IMonadicSpecificationComposition{TCandidate}"/>
    /// <remarks>This interface is intended like a marker for specifications (and thus specializes
    /// <see cref="ISpecification"/>), but it doesn't express a specification by itself.</remarks>
    public interface IPolyadicSpecificationComposition<out TCandidate> : ISpecification
    {
        /// <summary>
        /// The specifications in the composition.
        /// </summary>
        IEnumerable<ISpecification> Operands { get; }
    }
}

