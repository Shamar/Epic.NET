//  
//  ScalarType.cs
//  
//  Author:
//       Marco Veglio <m.veglio@gmail.com>
// 
//  Copyright (c) 2010-2011 Giacomo Tesio
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

namespace Epic.Linq.Expressions.Relational
{
    /// <summary>
    /// Types of scalar allowed.
    /// </summary>
    public enum ScalarType
    {
        /// <summary>
        /// The referenced <see cref="Scalar"/> object is a <see cref="Constant"/>.
        /// </summary>
        Constant,

        /// <summary>
        /// The referenced <see cref="Scalar"/> object is a Function.
        /// </summary>
        Function,

        /// <summary>
        /// The referenced <see cref="Scalar"/> object is a <see cref="RelationAttribute"/>.
        /// </summary>
        Attribute
    }
}

