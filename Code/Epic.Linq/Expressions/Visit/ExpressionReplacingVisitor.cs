//  
//  ExpressionReplacingVisitor.cs
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
    public sealed class ExpressionReplacingVisitor<TExpression> : CompositeVisitorBase, ICompositeVisitor<TExpression>
        where TExpression : Expression
    {
        private readonly TExpression _toReplace; 
        private readonly Expression _replacement;
        public ExpressionReplacingVisitor (CompositeVisitorChain chain, TExpression toReplace, Expression replacement)
            : base(chain)
        {
            if(null == toReplace)
                throw new ArgumentNullException("toReplace");
            if(null == replacement)
                throw new ArgumentNullException("replacement");
            _toReplace = toReplace;
            _replacement = replacement;
        }
        
        protected override ICompositeVisitor<TRequested> AsVisitor<TRequested> (TRequested target, IVisitState state)
        {
            if(!object.ReferenceEquals(_toReplace, target))
                return null;
            return base.AsVisitor (target, state);
        }
        
        #region ICompositeVisitor[TExpression] implementation
        public System.Linq.Expressions.Expression Visit (TExpression target, IVisitState state)
        {
            return _replacement;
        }
        #endregion

    }
}

