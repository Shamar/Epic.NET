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
            Type unvisitableType = typeof(TUnvisitable);
            if (typeof(object).Equals(unvisitableType))
            {
                throw new InvalidOperationException("Cannot use System.Object as the base class of a visitable hierarchy. It's too abstract: no client of the domain model should use as a model.");
            }
            if (typeof(IVisitable).IsAssignableFrom(unvisitableType))
            {
                string message = string.Format("The UnvisitableWrapper is for types' hierachies whose root does not implement Epic.IVisitable, but {0} is visitable by itself.", unvisitableType);
                throw new InvalidOperationException(message);
            }
            _acceptAs = typeof(UnvisitableWrapper<TUnvisitable, TResult>).GetMethod("AcceptAs", BindingFlags.Static | BindingFlags.NonPublic);
            _delegates = new ConcurrentDictionary<Type, Func<TUnvisitable, IVisitor<TResult>, IVisitContext, TResult>>();
        }

        private static TResult AcceptAs<TSpecific>(TUnvisitable unvisitable, IVisitor<TResult> visitor, IVisitContext context)
            where TSpecific : class, TUnvisitable
        {
            TSpecific expression = unvisitable as TSpecific;
            IVisitor<TResult, TSpecific> specializedVisitor = visitor.AsVisitor<TSpecific>(expression);
            return specializedVisitor.Visit(expression, context);
        }

        /// <summary>
        /// Gets the first visitable type for an instance of <paramref name="type"/>.
        /// </summary>
        /// <returns>
        /// The first visitable type for <paramref name="type"/>.
        /// </returns>
        /// <param name='type'>
        /// Type of the instance to visit.
        /// </param>
        private static Type GetFirstVisitableTypeFor(Type type)
        {
            if (type.IsPublic || (type.IsNested && type.IsNestedPublic))
                return type;
            while (!type.Equals(typeof(object)) && !(type.IsPublic || (type.IsNested && type.IsNestedPublic)))
            {
                type = type.BaseType;
            }
            if (type.Equals(typeof(object)))
            {
                string message = string.Format("The type {0} is not public and it has no public ancestor up to System.Object in its own type hierarchy. This make it unvisitable.", type.AssemblyQualifiedName);
                throw new ArgumentException(message, "type");
            }
            return type;
        }

        /// <summary>
        /// Simulates the <see cref="IVisitable.Accept{TResult}"/> method on a class
        /// hierarchy that does not provide such interface.
        /// </summary>
        /// <returns>
        /// The result of the visit.
        /// </returns>
        /// <param name='unvisitable'>
        /// Unvisitable to visit.
        /// </param>
        /// <param name='visitor'>
        /// Visitor.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        internal static TResult SimulateAccept(TUnvisitable unvisitable, IVisitor<TResult> visitor, IVisitContext context)
        {
            // no null check here, becouse (being this method internal) a NullReferenceException from here is a bug in Epic.
            Func<TUnvisitable, IVisitor<TResult>, IVisitContext, TResult> del = null;
            if (!_delegates.TryGetValue(unvisitable.GetType(), out del))
            {
                if (unvisitable is IVisitable)
                {
                    // even if the TUnvisitable is not IVisitable, if one of
                    // its specialization implement such an interface we have
                    // to fulfill such a contract.
                    del = (u, v, c) => (u as IVisitable).Accept(v, c);
                }
                else
                {
                    ParameterExpression unvisitableP = Expression.Parameter(typeof(TUnvisitable), "unvisitable");
                    ParameterExpression visitorP = Expression.Parameter(typeof(IVisitor<TResult>), "visitor");
                    ParameterExpression contextP = Expression.Parameter(typeof(IVisitContext), "context");
                    Expression<Func<TUnvisitable, IVisitor<TResult>, IVisitContext, TResult>> delegateBuilder =
                        Expression.Lambda<Func<TUnvisitable, IVisitor<TResult>, IVisitContext, TResult>>(
                            Expression.Call(_acceptAs.MakeGenericMethod(GetFirstVisitableTypeFor(unvisitable.GetType())),
                                            unvisitableP,
                                            visitorP,
                                            contextP),
                            unvisitableP,
                            visitorP,
                            contextP);
                    del = delegateBuilder.Compile();
                }
                _delegates[unvisitable.GetType()] = del;
            }
            return del(unvisitable, visitor, context);
        }
    }
}

