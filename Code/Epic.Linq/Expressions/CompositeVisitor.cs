//  
//  CompositeVisitor.cs
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
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Epic.Linq.Expressions
{
    /// <summary>
    /// Visitors' composition. It uses composition to handle the visit of any type of expression tree.
    /// </summary>
    /// <seealso cref="CompositeVisitorBase"/>
    public abstract class CompositeVisitor<TResult> : IVisitor<TResult>
    {
        private readonly List<VisitorBase> _chain;
        private readonly string _name;

        
        /// <summary>
        /// Initializes a new instance of a composition.
        /// </summary>
        /// <remarks>
        /// This constructor is internal because clients have to derive <see cref="CompositeVisitorBase"/>.
        /// </remarks>
        /// <param name='name'>
        /// Name of the composition.
        /// </param>
        /// <exception cref='ArgumentNullException'>
        /// Is thrown when <paramref name="name"/> is <value>null</value> or <value>string.Empty</value>.
        /// </exception>
        internal CompositeVisitor (string name)
        {
            if(string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            _name = name;
            _chain = new List<VisitorBase>();
        }
        
        /// <summary>
        /// Register the specified visitor.
        /// </summary>
        /// <param name='visitor'>
        /// Visitor to register in the composition.
        /// </param>
        /// <param name='position'>
        /// Position of the registered visitor.
        /// </param>
        /// <exception cref='ArgumentException'>
        /// Is thrown when the visitor is already registered.
        /// </exception>
        private void Register(VisitorBase visitor, out int position)
        {
            position = _chain.Count;
            _chain.Add(visitor);
        }

        /// <summary>
        /// Returns the next visitor able to visit <paramref name="target"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The visitors will be returned in the reverse order than they have been registered.
        /// This way new visitors can be added by extending previous compositions (ensuring the Open-Closed principle).
        /// </para>
        /// </remarks>
        /// <returns>
        /// The next visitor that can visit <paramref name="target"/>.
        /// </returns>
        /// <param name='target'>
        /// Object to visit.
        /// </param>
        /// <param name='callerPosition'>
        /// Position assigned to the caller visitor.
        /// </param>
        /// <typeparam name='TExpression'>
        /// Type of the object to visit.
        /// </typeparam>
        /// <exception cref='ArgumentException'>
        /// Is thrown when no registered visitor is able to visit <paramref name="target"/>.
        /// </exception>
        private IVisitor<TResult, TExpression> GetNextVisitor<TExpression>(TExpression target, int callerPosition)
        {
            IVisitor<TResult, TExpression> foundVisitor = null;
            
            while(callerPosition > 0)
            {
                --callerPosition;
                VisitorBase visitor = _chain[callerPosition];
                foundVisitor = visitor.AsVisitor<TExpression>(target);
                if(null != foundVisitor)
                    return foundVisitor;
            }
            string message = string.Format("No visitor available for the expression {0} in the composition \"{1}\".", target, _name);
            throw new ArgumentException(message);
        }
  
        /// <summary>
        /// Returns the first visitor in the composition that can visit <paramref name="target"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is internal since it has to be visible to <seealso cref="CompositeVisitorBase{TResult, TExpression}"/>.
        /// However no other derived class should need use this method.
        /// </para>
        /// </remarks>
        /// <returns>
        /// The first visitor that can visit <paramref name="target"/> (the last registered one).
        /// </returns>
        /// <param name='target'>
        /// Object to visit.
        /// </param>
        /// <typeparam name='TExpression'>
        /// Type of the object to visit.
        /// </typeparam>
        internal IVisitor<TResult, TExpression> GetFirstVisitor<TExpression> (TExpression target)
        {
            return GetNextVisitor<TExpression>(target, _chain.Count);
        }

        #region IVisitor[TResult] implementation
        
        /// <summary>
        /// Returns the current instance as <seealso cref="IVisitor{TResult, TExpression}"/> 
        /// (or <value>null</value> when the current composition is not intended to visit <typeparamref name="TExpression"/>).
        /// </summary>
        /// <returns>
        /// The current instance or <value>null</value>.
        /// </returns>
        /// <param name='target'>
        /// Object to visit.
        /// </param>
        /// <typeparam name='TExpression'>
        /// Type of the object to visit.
        /// </typeparam>
        public IVisitor<TResult, TExpression> GetVisitor<TExpression> (TExpression target)
        {
            return this as IVisitor<TResult, TExpression>;
        }

        #endregion
        
        /// <summary>
        /// Base class for composable visitors.
        /// </summary>
        [DebuggerDisplay("{GetType().Name} at position {_position} in {_composition._name}")]
        public abstract class VisitorBase : IVisitor<TResult>
        {
            private readonly int _position;
            private readonly CompositeVisitor<TResult> _composition;
            
            /// <summary>
            /// Initializes a new instance of the visitor and register it in the <paramref name="composition"/> provided.
            /// </summary>
            /// <param name='composition'>
            /// Composition that will own the new visitor.
            /// </param>
            /// <exception cref='ArgumentNullException'>
            /// Is thrown when the <paramref name="composition"/> is <value>null</value>.
            /// </exception>
            protected VisitorBase(CompositeVisitor<TResult> composition)
            {
                if(null == composition)
                    throw new ArgumentNullException("composition");
                _composition = composition;
                _composition.Register(this, out _position);
            }
            
            /// <summary>
            /// Continues the visit of an <typeparamref name="TExpression"/> created (or at least already visited) by the current visitor.
            /// The expression provided will be given to the next appropriate visitor in the composition (when this exists).
            /// </summary>
            /// <returns>
            /// The <typeparamref name="TResult"/> generated from the visit of the expression.
            /// </returns>
            /// <param name='target'>
            /// Target.
            /// </param>
            /// <param name='context'>
            /// Context.
            /// </param>
            /// <typeparam name='TExpression'>
            /// The 1st type parameter.
            /// </typeparam>
            /// <exception cref='ArgumentException'>
            /// Is thrown when no registered visitor is able to visit <paramref name="target"/>.
            /// </exception>
            protected TResult ContinueVisit<TExpression>(TExpression target, IVisitContext context)
            {
                IVisitor<TResult, TExpression> next = _composition.GetNextVisitor<TExpression>(target, _position);
                return next.Visit(target, context);
            }
            
            /// <summary>
            /// Visits an inner expression. This should be called to delegate the transformation to the chain for an unvisited node.
            /// </summary>
            /// <returns>
            /// The <typeparamref name="TResult"/> generated from the visit of the inner expression.
            /// </returns>
            /// <param name='target'>
            /// Object to visit.
            /// </param>
            /// <param name='context'>
            /// Context of the visit.
            /// </param>
            /// <typeparam name='TExpression'>
            /// The type of the object to visit.
            /// </typeparam>
            /// <exception cref='ArgumentException'>
            /// Is thrown when no registered visitor is able to visit <paramref name="target"/>.
            /// </exception>
            protected TResult VisitInner<TExpression>(TExpression target, IVisitContext context)
            {
                IVisitor<TResult, TExpression> next = _composition.GetFirstVisitor<TExpression>(target);
                return next.Visit(target, context);
            }

            #region IVisitor[TResult] implementation
            /// <summary>
            /// Return the visitor that can visit <paramref name="target">.
            /// </summary>
            /// <returns>
            /// The visitor.
            /// </returns>
            /// <param name='target'>
            /// Expression to be visited.
            /// </param>
            /// <typeparam name='TExpression'>
            /// Type of the expression that will be visited from the provided visitor.
            /// </typeparam>
            public IVisitor<TResult, TExpression> GetVisitor<TExpression> (TExpression target)
            {
                return _composition.GetFirstVisitor<TExpression>(target);
            }
            #endregion
            
            /// <summary>
            /// Returns the current instance if and only if it's able to visit <paramref name="target"/>,
            /// <value>null</value> otherwise.
            /// It should be overridden whenever the type of <paramref name="target"/> is not enough 
            /// to choose whether the current instance can visit it or not.
            /// </summary>
            /// <returns>
            /// The current instance or <value>null</value>.
            /// </returns>
            /// <param name='target'>
            /// Object to visit.
            /// </param>
            /// <typeparam name='TExpression'>
            /// The type of the object to visit.
            /// </typeparam>
            protected internal virtual IVisitor<TResult, TExpression> AsVisitor<TExpression>(TExpression target)
            {
                return this as IVisitor<TResult, TExpression>;
            }
        }
    }

}

