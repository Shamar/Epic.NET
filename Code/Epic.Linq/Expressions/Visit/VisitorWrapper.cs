//  
//  VisitorWrapper.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2011 Giacomo Tesio
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

namespace Epic.Linq.Expressions.Visit
{
    public abstract class VisitorWrapperBase<TResult> : ICompositeVisitor<TResult>
    {
        private readonly ICompositeVisitor<TResult> _wrapped;
        protected VisitorWrapperBase(ICompositeVisitor<TResult> wrappedVisitor)
        {
            if(null == wrappedVisitor)
                throw new ArgumentNullException("wrappedVisitor");
            _wrapped = wrappedVisitor;
        }
        
        public ICompositeVisitor<TResult> WrappedVisitor
        {
            get
            {
                return _wrapped;
            }
        }

        #region ICompositeVisitor implementation
        public abstract ICompositeVisitor<TResult, TExpression> GetVisitor<TExpression> (TExpression target) where TExpression : Expression;
        #endregion
    }
    
    public sealed class VisitorWrapper<TResult, TExpression> : VisitorWrapperBase<TResult>, ICompositeVisitor<TResult, TExpression>
        where TExpression : Expression
    {
        private readonly Func<TExpression, IVisitState, TResult> _visit;
        public VisitorWrapper (ICompositeVisitor<TResult> wrappedVisitor, Func<TExpression, IVisitState, TResult> visit)
            : base(wrappedVisitor)
        {
            if(null == visit)
                throw new ArgumentNullException("visit");
            if(!(object.ReferenceEquals(WrappedVisitor, visit.Target) || visit.Target == null))
                throw new ArgumentException("The visit must be either static or a method of the wrapped visitor.");
            _visit = visit;
        }

        #region ICompositeVisitor[TExpression] implementation
        public TResult Visit (TExpression target, IVisitState state)
        {
            return _visit(target, state);
        }
        #endregion

        #region ICompositeVisitor implementation
        public override ICompositeVisitor<TResult, TRequiredExpression> GetVisitor<TRequiredExpression> (TRequiredExpression target)
        {
            return WrappedVisitor.GetVisitor<TRequiredExpression>(target);
        }
        #endregion
    }
}

