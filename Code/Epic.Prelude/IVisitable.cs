//  
//  IVisitable.cs
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

namespace Epic
{
    /// <summary>
    /// Visitable objects.
    /// </summary>
    public interface IVisitable
    {
        /// <summary>
        /// Accept the specified visitor (double dispatch) and returns 
        /// the <typeparamref name="TResult"/> that the visit produce.
        /// </summary>
        /// <returns>Result of the visit.</returns>
        /// <param name='visitor'>
        /// Visitor.
        /// </param>
        /// <param name='context'>
        /// Context of the visit.
        /// </param>
        /// <typeparam name='TResult'>
        /// Type of the result of the visit.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="visitor"/> or <paramref name="context"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The <paramref name="visitor"/> can not be used to visit the current instance.
        /// </exception>
        TResult Accept<TResult>(IVisitor<TResult> visitor, IVisitContext context);
    }
}

