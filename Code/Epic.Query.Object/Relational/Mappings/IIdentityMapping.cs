//
//  IIdentityMapping.cs
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
using Epic.Math;
using Epic.Query.Relational;
using Epic.Query.Relational.Predicates;
using System.Collections.Generic;

namespace Epic.Query.Object.Relational.Mappings
{
    /// <summary>
    /// Mapping of an <typeparamref name="TIdentity"/> to the relational persistence layer.
    /// </summary>
    /// <typeparam name="TIdentity">Type of the identity.</typeparam>
    public interface IIdentityMapping<TIdentity> : IMapping<TIdentity, Predicate>,
                                                   IMapping<object[], TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        /// <summary>
        /// Relational attributes whose values map to an instance of 
        /// <typeparamref name="TIdentity"/>, in the relational persistence layer.
        /// </summary>
        IEnumerable<RelationAttribute> Attributes { get; }
    }
}

