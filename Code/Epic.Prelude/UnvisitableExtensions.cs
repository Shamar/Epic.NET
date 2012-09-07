//
//  UnvisitableExtensions.cs
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
using System.Linq.Expressions;

namespace Epic
{
    /// <summary>
    /// Defines a set of extension methods that make some common type hierarchies
    /// suitable for visit by a <see cref="IVisitor{TResult}"/>.
    /// </summary>
    public static class UnvisitableExtensions
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

        /// <summary>
        /// Force <paramref name="eventArgs"/> to accept the specified visitor and context.
        /// </summary>
        /// <param name='eventArgs'>
        /// Event argument that will be routed to <paramref name="visitor"/> with its actual type.
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
        /// <exception cref="ArgumentNullException"><paramref name="eventArgs"/>, <paramref name="visitor"/> or <paramref name="context"/> is <see langword="null"/>.</exception>
        public static TResult Accept<TResult>(this EventArgs eventArgs, IVisitor<TResult> visitor, IVisitContext context)
        {
            if (null == eventArgs)
                throw new ArgumentNullException("eventArgs");
            if (null == visitor)
                throw new ArgumentNullException("visitor");
            if (null == context)
                throw new ArgumentNullException("context");
            return UnvisitableWrapper<EventArgs, TResult>.SimulateAccept(eventArgs, visitor, context);
        }

        /// <summary>
        /// Force <paramref name="expression"/> to accept the specified visitor and context.
        /// </summary>
        /// <param name='expression'>
        /// <see cref="System.Linq.Expressions.Expression"/> that will be routed to <paramref name="visitor"/> with its actual (public) type.
        /// </param>
        /// <param name='visitor'>
        /// Visitor.
        /// </param>
        /// <param name='context'>
        /// Context of the visit.
        /// </param>
        /// <returns>Result of the visit.</returns>
        /// <typeparam name='TResult'>
        /// The type of the result of the visit.
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="expression"/>, <paramref name="visitor"/> or <paramref name="context"/> is <see langword="null"/>.</exception>
        public static TResult Accept<TResult>(this Expression expression, IVisitor<TResult> visitor, IVisitContext context)
        {
            if (null == expression)
                throw new ArgumentNullException("expression");
            if (null == visitor)
                throw new ArgumentNullException("visitor");
            if (null == context)
                throw new ArgumentNullException("context");
            return UnvisitableWrapper<Expression, TResult>.SimulateAccept(expression, visitor, context);
        }

        /// <summary>
        /// Allows the Epic.NET users to define their own Accept extension method for type hierarchies 
        /// that do not implement <see cref="IVisitable"/>.
        /// </summary>
        /// <typeparam name="TResult">Type of the result of the visit.</typeparam>
        /// <typeparam name="TBaseClass">Type of the base class of the hierarchy. It cannot be <see cref="System.Object"/>.</typeparam>
        /// <param name="expression">Instance of <typeparamref name="TBaseClass"/> to visit.</param>
        /// <param name='visitor'>
        /// Visitor.
        /// </param>
        /// <param name='context'>
        /// Context of the visit.
        /// </param>
        /// <returns>
        /// Result of the visit.
        /// </returns>
        /// <example>
        /// The following code shows how to make an existing class hierachy visitable without any change
        /// to the existing code.
        /// <code>
        /// public abstract class Animal { }
        /// 
        /// public class Mammal : Animal { }
        /// 
        /// public sealed class Human : Mammal { }
        /// 
        /// public sealed class Cat : Mammal { }
        /// 
        /// public static class VisitableAnimals
        /// {
        ///     public static TResult Accept&lt;TResult&gt;(this Animal animal, IVisitor&lt;TResult&gt; visitor, IVisitContext context)
        ///     {
        ///         return UnvisitableExtensions.SimulateAcceptFor&lt;TResult, Animal&gt;(animal, visitor, context);
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException"><paramref name="expression"/>, <paramref name="visitor"/> or <paramref name="context"/> is <see langword="null"/>.</exception>
        public static TResult SimulateAcceptFor<TResult, TBaseClass>(TBaseClass expression, IVisitor<TResult> visitor, IVisitContext context)
            where TBaseClass : class
        {
            if (null == expression)
                throw new ArgumentNullException("expression");
            if (null == visitor)
                throw new ArgumentNullException("visitor");
            if (null == context)
                throw new ArgumentNullException("context");
            if (typeof(TBaseClass).IsSealed)
            {
                string message = string.Format("Cannot use {0} as the base class of a visitable hierarchy becouse it is sealed.", typeof(TBaseClass));
                throw new InvalidOperationException(message);
            }
            TResult result;
            try
            {
                result = UnvisitableWrapper<TBaseClass, TResult>.SimulateAccept(expression, visitor, context);
            }
            catch(TypeInitializationException ex)
            {
                throw ex.InnerException;
            }
            return result;
        }
    }
}

