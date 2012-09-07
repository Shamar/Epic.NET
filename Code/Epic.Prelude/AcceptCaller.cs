//
//  AcceptCaller.cs
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
    /// This class is used by <see cref="CompositeVisitor{TResult}"/> to unify the calls to visitors
    /// for both visitable and unvisitable targets. It's designed to be as fast as possible.
    /// </summary>
    /// <typeparam name="TToVisit">Type of the object (as known to <see cref="CompositeVisitor{TResult}"/> when it calls <see cref="CallAccept"/>)
    /// that the <see cref="CompositeVisitor{TResult}"/> want to visit.</typeparam>
    /// <typeparam name="TResult">Type of the result that the <see cref="CompositeVisitor{TResult}"/> is able to produce.</typeparam>
    internal sealed class AcceptCaller<TToVisit, TResult>
        where TToVisit : class
    {
        private static readonly Func<TToVisit, IVisitor<TResult>, IVisitContext, TResult> _accept;
        static AcceptCaller ()
        {
            if(typeof(IVisitable).IsAssignableFrom(typeof(TToVisit)))
            {
                _accept = (toVisit, visitor, context) => (toVisit as IVisitable).Accept(visitor, context);
            }
            else
            {
                _accept = UnvisitableWrapper<TToVisit, TResult>.SimulateAccept;
            }
        }

        /// <summary>
        /// Enable <paramref name="visitor"/> to visit <paramref name="toVisit"/> by either
        /// calling <see cref="IVisitable.Accept{TResult}"/> or calling 
        /// <see cref="UnvisitableWrapper{TUnvisitable, TResult}.SimulateAccept"/>, as appropriate for <typeparamref name="TToVisit"/>.
        /// </summary>
        /// <param name="toVisit">Object to be visited.</param>
        /// <param name="visitor">Visitor.</param>
        /// <param name="context">Context of the visit.</param>
        /// <returns>Result produced by the visit.</returns>
        /// <exception>It does not block any exception produced by the visit.</exception>
        public static TResult CallAccept(TToVisit toVisit, IVisitor<TResult> visitor, IVisitContext context)
        {
            return _accept(toVisit, visitor, context);
        }
    }
}

