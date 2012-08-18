//
//  UnvisitableWrapper.cs
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
using System.Collections.Concurrent;
using System.Reflection;
using System.Linq.Expressions;

namespace Epic
{
    internal static class UnvisitableWrapper<TUnvisitable, TResult>
        where TUnvisitable : class
    {
        private static readonly MethodInfo _acceptAs;
        private static readonly ConcurrentDictionary<Type, Func<TUnvisitable, IVisitor<TResult>, IVisitContext, TResult>> _delegates;
        static UnvisitableWrapper()
        {
            _acceptAs = typeof(UnvisitableWrapper<TUnvisitable, TResult>).GetMethod("AcceptAs", BindingFlags.Static|BindingFlags.NonPublic);
            _delegates = new ConcurrentDictionary<Type, Func<TUnvisitable, IVisitor<TResult>, IVisitContext, TResult>>();
        }

        private static TResult AcceptAs<TSpecific>(TUnvisitable unvisitable, IVisitor<TResult> visitor, IVisitContext context)
            where TSpecific : class, TUnvisitable
        {
            TSpecific expression = unvisitable as TSpecific;
            IVisitor<TResult, TSpecific> specializedVisitor = visitor.GetVisitor<TSpecific>(expression);
            return specializedVisitor.Visit(expression, context);
        }

        internal static TResult Accept(TUnvisitable unvisitable, IVisitor<TResult> visitor, IVisitContext context)
        {
            Func<TUnvisitable, IVisitor<TResult>, IVisitContext, TResult> del = null;
            if(!_delegates.TryGetValue(unvisitable.GetType(), out del))
            {
                ParameterExpression unvisitableP = Expression.Parameter(typeof(TUnvisitable), "unvisitable");
                ParameterExpression visitorP = Expression.Parameter(typeof(IVisitor<TResult>), "visitor");
                ParameterExpression contextP = Expression.Parameter(typeof(IVisitContext), "context");
                Expression<Func<TUnvisitable, IVisitor<TResult>, IVisitContext, TResult>> delegateBuilder = 
                    Expression.Lambda<Func<TUnvisitable, IVisitor<TResult>, IVisitContext, TResult>>(
                        Expression.Call(_acceptAs.MakeGenericMethod(unvisitable.GetType ()),
                                        unvisitableP, 
                                        visitorP, 
                                        contextP),
                        unvisitableP,
                        visitorP,
                        contextP);
                del = delegateBuilder.Compile();
                _delegates[unvisitable.GetType()] = del;
            }
            return del(unvisitable, visitor, context);
        }
    }
}

