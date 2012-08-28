//  
//  CompositeVisitorBase.cs
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
using System.Diagnostics;

namespace Epic
{
    /// <summary>
    /// Visitors' composition base class. This class must be derived from concrete compositions.
    /// </summary>
    /// <typeparam name="TResult">The type of the result produced by the visit.</typeparam>
    /// <typeparam name="TExpression">The type of the expression to visit.</typeparam>
    [DebuggerDisplay("CompositeVisitor<{typeof(TResult).Name, nq}, {typeof(TExpression).Name, nq}> {_name} ({_chain.Count} visitors)")]
    public abstract class CompositeVisitorBase<TResult, TExpression> : CompositeVisitor<TResult>, IVisitor<TResult, TExpression>
        where TExpression : class
    {
        /// <summary>
        /// Initializes a new instance of the visitors' composition.
        /// </summary>
        /// <param name='name'>
        /// Name of the compositions.
        /// </param>
        /// <exception cref="ArgumentNullException">Is thrown when <paramref name="name"/> is <see langword="null"/> or <see cref="String.Empty"/>.</exception>
        protected CompositeVisitorBase(string name)
            : base(name)
        {
        }
        
        /// <summary>
        /// Initializes the context of the visit.
        /// </summary>
        /// <returns>
        /// The <see cref="IVisitContext"/> to be used in the current visit.
        /// </returns>
        /// <param name='target'>
        /// Object to visit (will never be <see langword="null"/>).
        /// </param>
        /// <param name='context'>
        /// Context recieved from the caller (will never be <see langword="null"/>).
        /// </param>
        protected abstract IVisitContext InitializeVisitContext(TExpression target, IVisitContext context);
        
        #region IVisitor[TResult,TExpression] implementation
        
        /// <summary>
        /// Visit the specified target.
        /// </summary>
        /// <param name='target'>
        /// Object to visit.
        /// </param>
        /// <param name='context'>
        /// Context of the visit.
        /// </param>
        /// <returns>
        /// The result of the visit.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown when either <paramref name="target"/> or <paramref name="context"/> are <see langword="null"/>.
        /// </exception>
        public TResult Visit (TExpression target, IVisitContext context)
        {
            if(null == target)
                throw new ArgumentNullException("target");
            if(null == context)
                throw new ArgumentNullException("context");
            IVisitContext initializedContext = InitializeVisitContext(target, context);
            IVisitor<TResult, TExpression> visitor = GetFirstVisitor(target);
            return visitor.Visit(target, initializedContext);
        }
        #endregion
    }
}

