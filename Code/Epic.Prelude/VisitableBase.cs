//  
//  VisitableBase.cs
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
    /// Base class for visitable expressions.
    /// </summary>
    /// <exception cref='InvalidOperationException'>
    /// Is thrown when an operation cannot be performed.
    /// </exception>
    [Serializable]
    public abstract class VisitableBase : IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.VisitableBase"/> class.
        /// </summary>
        protected VisitableBase ()
        {
        }
        
        /// <summary>
        /// Accept the specified visitor (double dispatch).
        /// </summary>
        /// <param name='visitor'>
        /// Visitor.
        /// </param>
        /// <param name='context'>
        /// Context of the visit.
        /// </param>
        /// <returns>Result of the visit.</returns>
        /// <typeparam name='TResult'>
        /// Type of the result of the visit.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="visitor"/> or <paramref name="context"/> is <see langword="null"/>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The <paramref name="visitor"/> can not be used to visit the current instance.
        /// </exception>
        public abstract TResult Accept<TResult>(IVisitor<TResult> visitor, IVisitContext context);
        
        /// <summary>
        /// Utility method. You can implement Accept simply with "return AcceptMe(this, visitor, context);".
        /// </summary>
        /// <returns>
        /// The result of the visit.
        /// </returns>
        /// <param name='visitable'>
        /// The current instance
        /// </param>
        /// <param name='visitor'>
        /// Visitor.
        /// </param>
        /// <param name='context'>
        /// Context of the visit.
        /// </param>
        /// <returns>Result of the visit.</returns>
        /// <typeparam name='TResult'>
        /// Type of the result.
        /// </typeparam>
        /// <typeparam name='TVisitable'>
        /// Type of the visitable (must be a leaf in the hierarchy tree).
        /// </typeparam>
        /// <exception cref="ArgumentNullException">Is thrown when either <paramref name="visitor"/> or <paramref name="context"/> are <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Is thrown when <paramref name="visitable"/> is not the current instance.</exception>
        /// <exception cref='InvalidOperationException'>
        /// Is thrown when called from a non leaf in the hierachy tree.
        /// </exception>
        protected TResult AcceptMe<TResult, TVisitable>(TVisitable visitable, IVisitor<TResult> visitor, IVisitContext context) where TVisitable : class, IVisitable
        {
            if(!object.ReferenceEquals(this, visitable))
            {
                // this would be a bug.
                throw new ArgumentOutOfRangeException("visitable", "You must provide the current instance.");
            }
            if(!typeof(TVisitable).IsInterface && !typeof(TVisitable).Equals(this.GetType()))
            {
                string message = string.Format("VisitableBase.AcceptMe() must be called only from leafs of the hierarchy tree.");
                throw new InvalidOperationException(message);
            }
            if(null == visitor)
                throw new ArgumentNullException("visitor");
            if(null == context)
                throw new ArgumentNullException("context");
            
            var myVisitor = visitor.AsVisitor(visitable);
            return myVisitor.Visit(visitable, context);
        }
    }
}

