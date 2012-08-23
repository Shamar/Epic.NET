//  
//  IVisitor.cs
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
    /// Visitor producing a <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TResult">Type of the result of the visit.</typeparam>
    /// <seealso cref="IVisitor{TResult, TExpression}"/>
    /// <seealso cref="IVisitContext"/>
    public interface IVisitor<out TResult>
    {
        /// <summary>
        /// Enables the visit of <paramref name="target"/> returning the visitor
        /// under an interface ready for the operation.
        /// </summary>
        /// <returns>
        /// The visitor that can visit <paramref name="target"/>.
        /// </returns>
        /// <param name='target'>
        /// Expression to be visited.
        /// </param>
        /// <typeparam name='TExpression'>
        /// Type of the expression that will be visited from the returned visitor.
        /// </typeparam>
        /// <exception cref="InvalidOperationException">
        /// The current visitor can not be used to visit <paramref name="target"/>.
        /// </exception>
        /// <seealso cref="CompositeVisitor{TResult}"/>
        /// <seealso cref="CompositeVisitorBase{TResult, TExpression}"/>
        /// <seealso cref="CompositeVisitor{TResult}.VisitorBase"/>
        IVisitor<TResult, TExpression> AsVisitor<TExpression>(TExpression target) where TExpression : class;
    }
    
    /// <summary>
    /// Visitor that knows how to visit a <typeparamref name="TExpression"/>.
    /// </summary>
    /// <typeparam name="TResult">Type of the result of the visit.</typeparam>
    /// <typeparam name="TExpression">Expression to visit.</typeparam>
    /// <seealso cref="IVisitContext"/>
    public interface IVisitor<out TResult, in TExpression> : IVisitor<TResult>
        where TExpression : class
    {
        /// <summary>
        /// Visit the specified expression.
        /// </summary>
        /// <param name='target'>
        /// Expression to visit.
        /// </param>
        /// <param name='context'>
        /// Visit context. Contains the state produced by previous visitors.
        /// </param>
        /// <returns>Result of the visit.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="target"/> or <paramref name="context"/> is <see langword="null"/>
        /// </exception>
        TResult Visit(TExpression target, IVisitContext context);
    }
}

