//  
//  CompositeVisitorBase.cs
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
    public abstract class CompositeVisitorBase : ICompositeVisitor
    {
        private readonly ICompositeVisitor _next;
        
        protected CompositeVisitorBase (CompositeVisitorChain chain)
        {
            if(null == chain)
                throw new ArgumentNullException("chain");
            chain.Append(this);
            _next = chain;
        }

        #region ICompositeVisitor implementation
        public ICompositeVisitor<TExpression> GetVisitor<TExpression> (TExpression target) where TExpression : System.Linq.Expressions.Expression
        {
            ICompositeVisitor<TExpression> visitor = this.AsVisitor<TExpression>(target);
            if(null != visitor)
                return visitor;
            return GetNextVisitor<TExpression> (target);
        }
        #endregion
        
        protected virtual ICompositeVisitor<TExpression> AsVisitor<TExpression>(TExpression target) where TExpression : System.Linq.Expressions.Expression
        {
            return this as ICompositeVisitor<TExpression>;
        }
        
        protected ICompositeVisitor<TExpression> GetNextVisitor<TExpression> (TExpression target) where TExpression : System.Linq.Expressions.Expression
        {
            return _next.GetVisitor<TExpression>(target);
        }
    }
}

