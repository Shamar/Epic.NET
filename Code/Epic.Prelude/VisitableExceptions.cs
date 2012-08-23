//
//  VisitableExceptions.cs
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
    /// Defines a set of extension methods that makes all exceptions 
    /// suitable for visit by any <see cref="IVisitor{TResult, Exception}"/>.
    /// </summary>
    public static class VisitableExceptions
    {
        /// <summary>
        /// Force <paramref name="exception"/> to accept the specified visitor and context.
        /// </summary>
        /// <param name='exception'>
        /// Exception that will be routed to <paramref name="visitor"/> with its actual type.
        /// </param>
        /// <param name='visitor'>
        /// Visitor.
        /// </param>
        /// <param name='context'>
        /// Context of the visit.
        /// </param>
        /// <typeparam name='TResult'>
        /// The type of the result of the visit.
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="exception"/>, <paramref name="visitor"/> or <paramref name="context"/> is <see langword="null"/>.</exception>
        public static TResult Accept<TResult>(this Exception exception, IVisitor<TResult> visitor, IVisitContext context)
        {
            if (null == exception)
                throw new ArgumentNullException("exception");
            if (null == visitor)
                throw new ArgumentNullException("visitor");
            if (null == context)
                throw new ArgumentNullException("context");
            return UnvisitableWrapper<Exception, TResult>.SimulateAccept(exception, visitor, context);
        }
    }
}

