//
//  IMapping.cs
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

namespace Epic.Math
{
    /// <summary>
    /// Reppresent a function that associate each instance of
    /// <typeparamref name="TDomain"/> to an instance of <typeparamref name="TCodomain"/>.
    /// </summary>
    /// <typeparam name="TDomain">Type of the objects to map (the pre-image of the mapping).</typeparam>
    /// <typeparam name="TCodomain">Type of the objects to map (the image of the mapping).</typeparam>
    public interface IMapping<TDomain, TCodomain>
    {
        /// <summary>
        /// Applies the mapping to <paramref name="element"/>.
        /// </summary>
        /// <returns>
        /// The <typeparamref name="TCodomain"/> element associated with <paramref name="element"/>.
        /// </returns>
        /// <param name='element'>
        /// Element to map.
        /// </param>
        TCodomain ApplyTo(TDomain element);
    }
}

