//  
//  FakeVisitor.cs
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

namespace Epic.Fakes
{
    public class FakeVisitor<TResult, TExpression> : CompositeVisitor<TResult>.VisitorBase, IVisitor<TResult, TExpression>
        where TExpression : class
    {
        public FakeVisitor (CompositeVisitor<TResult> composition)
            : base(composition)
        {
        }
        
        #region IVisitor[TResult,TExpression] implementation
        public virtual TResult Visit (TExpression target, IVisitContext context)
        {
            return default(TResult);
        }
        #endregion IVisitor[TResult,TExpression] implementation
        
        protected override IVisitor<TResult, TRequested> AsVisitor<TRequested> (TRequested target)
        {
            return CallToVisitor(target);
        }
        
        #region templates for tests
        
        public TResult CallContinueVisit<TRequested>(TRequested target, IVisitContext context) where TRequested : class
        {
            return base.ContinueVisit(target, context);
        }

        public TResult CallVisitInner<TRequested>(TRequested target, IVisitContext context) where TRequested : class
        {
            return base.VisitInner(target, context);
        }
        
        public virtual IVisitor<TResult, TRequested> CallToVisitor<TRequested> (TRequested target) where TRequested : class
        {
            return base.AsVisitor (target);
        }
        
        #endregion templates for tests
    }
}

