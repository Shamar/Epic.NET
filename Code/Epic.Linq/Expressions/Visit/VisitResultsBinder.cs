//  
//  VisitResultsBinder.cs
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
    public sealed class VisitResultsBinder<TResult, TIntermediate, TExpression> : ICompositeVisitor<TResult, TExpression>
        where TExpression : Expression
        where TIntermediate : Expression
    {
        private readonly ICompositeVisitor<TIntermediate> _firstVisitor;
        private readonly ICompositeVisitor<TResult> _secondVisitor;
        
        public VisitResultsBinder (ICompositeVisitor<TIntermediate> firstVisitor, ICompositeVisitor<TResult> secondVisitor)
        {
            _firstVisitor = firstVisitor;
            _secondVisitor = secondVisitor;
        }

        #region ICompositeVisitor[TResult,TExpression] implementation
        public TResult Visit (TExpression target, IVisitState state)
        {
            ICompositeVisitor<TIntermediate, TExpression> v1 = _firstVisitor.GetVisitor<TExpression>(target);
            TIntermediate intermediate = v1.Visit(target, state);
            ICompositeVisitor<TResult, TIntermediate> v2 = _secondVisitor.GetVisitor<TIntermediate>(intermediate);
            return v2.Visit(intermediate, state);
        }
        #endregion

        #region ICompositeVisitor[TResult] implementation
        public ICompositeVisitor<TResult, TExpressionRequired> GetVisitor<TExpressionRequired> (TExpressionRequired target) where TExpressionRequired : System.Linq.Expressions.Expression
        {
            return this as ICompositeVisitor<TResult, TExpressionRequired>;
        }
        #endregion
    }
}

