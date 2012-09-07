//  
//  IVisitContext.cs
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
    /// Context of a visit.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Epic's visitors are immutable objects that can be used in a multithread environment.
    /// </para>
    /// <para>
    /// The state of each visit is passed as an argument between visitors.
    /// </para>
    /// </remarks>
    public interface IVisitContext
    {
        /// <summary>
        /// Returns the <typeparamref name="TState"/>.
        /// </summary>
        /// <typeparam name='TState'>
        /// Type of the state of interest.
        /// </typeparam>
        /// <returns>
        /// Value of the <typeparamref name="TState"/> that was registered from previous visitors in the context.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the <typeparamref name="TState"/> has not been found in the context.
        /// </exception>
        TState Get<TState>();
        
        /// <summary>
        /// Tries to find the <typeparamref name="TState"/> in the context.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> when the <typeparamref name="TState"/> has been found, <see langword="false"/> otherwise.
        /// </returns>
        /// <param name='state'>
        /// <typeparamref name="TState"/> to be used during the visit.
        /// </param>
        /// <typeparam name='TState'>
        /// Type of the state needed.
        /// </typeparam>
        bool TryGet<TState>(out TState state);
        
        /// <summary>
        /// Create a new context containing the specified <paramref name="state"/>.
        /// </summary>
        /// <param name='state'>
        /// State to use in the visits recieving the resulting context.
        /// </param>
        /// <returns>
        /// A new context containing the <paramref name="state"/>.
        /// </returns>
        /// <typeparam name='TState'>
        /// Type of the state to use.
        /// </typeparam>
        IVisitContext With<TState>(TState state);
    }
}

