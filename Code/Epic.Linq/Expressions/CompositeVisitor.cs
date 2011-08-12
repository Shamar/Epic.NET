//  
//  CompositeVisitor.cs
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
using System.Collections.Generic;

namespace Epic.Linq.Expressions
{
    public abstract class CompositeVisitor<TResult> : IVisitor<TResult>
    {
        private readonly List<VisitorBase> _chain;
        private readonly string _name;
        
        internal CompositeVisitor (string name)
        {
            if(string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            _name = name;
            _chain = new List<VisitorBase>();
        }
                
        private void Register(VisitorBase visitor, out int position)
        {
            if(_chain.Contains(visitor))
            {
                // a visitor can be registered only once
                throw new ArgumentException("Already registered.", "visitor");
            }
            position = _chain.Count;
            _chain.Add(visitor);
        }
  
        private IVisitor<TResult, TExpression> GetNextVisitor<TExpression>(TExpression target, int startingPosition)
        {
            IVisitor<TResult, TExpression> foundVisitor = null;
            
            while(startingPosition > 0)
            {
                --startingPosition;
                VisitorBase visitor = _chain[startingPosition];
                foundVisitor = visitor.AsVisitor<TExpression>(target);
                if(null != foundVisitor)
                    return foundVisitor;
            }
            string message = string.Format("No visitor available for the expression {0}.", target);
            throw new ArgumentException(message);
        }


        #region IVisitor[TResult] implementation
        public virtual IVisitor<TResult, TExpression> GetVisitor<TExpression> (TExpression target)
        {
            return GetNextVisitor<TExpression>(target, _chain.Count);
        }
        #endregion
        
                
        public abstract class VisitorBase : IVisitor<TResult>
        {
            private readonly int _position;
            private readonly CompositeVisitor<TResult> _composition;
            protected VisitorBase(CompositeVisitor<TResult> composition)
            {
                if(null == composition)
                    throw new ArgumentNullException("composition");
                _composition = composition;
                _composition.Register(this, out _position);
            }
            
            private IVisitor<TResult, TExpression> GetNextVisitor<TExpression>(TExpression target)
            {
                return _composition.GetNextVisitor<TExpression>(target, _position);
            }
            
            protected TResult ForwardToNext<TExpression>(TExpression target, IVisitContext context)
            {
                IVisitor<TResult, TExpression> next = GetNextVisitor<TExpression>(target);
                return next.Visit(target, context);
            }
            
            protected TResult ForwardToChain<TExpression>(TExpression target, IVisitContext context)
            {
                IVisitor<TResult, TExpression> next = GetVisitor<TExpression>(target);
                return next.Visit(target, context);
            }

            #region IVisitor[TResult] implementation
            public IVisitor<TResult, TExpression> GetVisitor<TExpression> (TExpression target)
            {
                return _composition.GetVisitor<TExpression>(target);
            }
            #endregion
            
            protected internal virtual IVisitor<TResult, TExpression> AsVisitor<TExpression>(TExpression target)
            {
                return this as IVisitor<TResult, TExpression>;
            }
        }
    }

    public abstract class CompositeVisitor<TResult, TExpression> : CompositeVisitor<TResult>, IVisitor<TResult, TExpression>
    {
        protected CompositeVisitor(string name)
            : base(name)
        {
        }
        
        protected abstract IVisitContext InitializeVisitContext(TExpression target, IVisitContext context);
        
        #region IVisitor[TResult,TExpression] implementation
        public TResult Visit (TExpression target, IVisitContext context)
        {
            IVisitContext initializedContext = InitializeVisitContext(target, context);
            IVisitor<TResult, TExpression> visitor = GetVisitor(target);
            return visitor.Visit(target, initializedContext);
        }
        #endregion
  
        public sealed override IVisitor<TResult, TRequiredExpression> GetVisitor<TExpression> (TRequiredExpression target)
        {
            return this as IVisitor<TResult, TRequiredExpression>;
        }
    }
}

