//
//  ActiveVisitContext.cs
//
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
//
//  Copyright (c) 2010-2013 Giacomo Tesio
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
    /// Extension methods that enable the execution of custom actions during a visit.
    /// </summary>
    public static class ActiveVisitContext
    {
        private delegate void CompositeAction<TValue>(TValue value);

        /// <summary>
        /// Return a <see cref="IVisitContext"/> that will performs <paramref name="action"/> when a 
        /// <typeparamref name="TValue"/> is provided to <see cref="ActiveVisitContext.ApplyTo{TValue}"/>.
        /// </summary>
        /// <param name="context">Initial context.</param>
        /// <param name="action">Action to perform when a <typeparamref name="TValue"/> is found.</param>
        /// <returns>A new <see cref="IVisitContext"/>.</returns>
        /// <typeparam name="TValue">The type of the object that is required to perform <paramref name="action"/>.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> or <paramref name="action"/> is <see langword="null"/>.</exception>
        public static IVisitContext Perform<TValue>(this IVisitContext context, Action<TValue> action)
        {
            if (null == context)
                throw new ArgumentNullException("context");
            if (null == action)
                throw new ArgumentNullException("extraction");
            CompositeAction<TValue> contextualAction = null;
            if(!context.TryGet(out contextualAction))
            {
                contextualAction = v => action(v);
                return context.With(contextualAction);
            }
            else
            {
                CompositeAction<TValue> newContextualAction = v => { action(v); contextualAction(v); };
                return context.With(newContextualAction);
            }
        }

        /// <summary>
        /// Applies to <paramref name="value"/> the actions that were previously registered in
        /// <paramref name="context"/> by <see cref="ActiveVisitContext.Perform{TValue}"/>.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="value">Value.</param>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        /// <returns><see langword="true"/> if an action was previosly registered in <paramref name="context"/>
        /// for <typeparamref name="TValue"/> by <see cref="ActiveVisitContext.Perform{TValue}"/>, 
        /// <see langword="false"/> otherwise.</returns>
        public static bool ApplyTo<TValue>(this IVisitContext context, TValue value)
        {
            if(null == context)
                throw new ArgumentNullException("context");
            CompositeAction<TValue> action = null;
            if(context.TryGet<CompositeAction<TValue>>(out action))
            {
                action(value);
                return true;
            }
            return false;
        }
    }
}

