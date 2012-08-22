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
    internal sealed class AcceptCaller<TToVisit, TResult>
        where TToVisit : class
    {
        private static readonly Func<TToVisit, IVisitor<TResult, TToVisit>, IVisitContext, TResult> _accept;
        static AcceptCaller ()
        {
            if(typeof(IVisitable).IsAssignableFrom(typeof(TToVisit)))
            {
                _accept = (toVisit, visitor, context) => (toVisit as IVisitable).Accept(visitor, context);
            }
            else
            {
                _accept = UnvisitableWrapper<TToVisit, TResult>.Accept;
            }
        }

        public static TResult CallAccept(TToVisit toVisit, IVisitor<TResult, TToVisit> visitor, IVisitContext context)
        {
            if (null == toVisit)
                return default(TResult);
            return _accept(toVisit, visitor, context);
        }
    }
}

