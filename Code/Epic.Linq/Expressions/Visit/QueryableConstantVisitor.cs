//  
//  QueryableConstantVisitor.cs
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
using System.Linq;

namespace Epic.Linq.Expressions.Visit
{
    /// <summary>
    /// Queryable constant visitor. It execute the query if the provider is not the same of the current one, or unwrap the expression if it is.
    /// </summary>
    /// <exception cref='InvalidOperationException'>
    /// Is thrown when an operation cannot be performed.
    /// </exception>
    public class QueryableConstantVisitor : VisitorsComposition<Expression>.VisitorBase, ICompositeVisitor<Expression, ConstantExpression>
    {
        public QueryableConstantVisitor (VisitorsComposition<Expression> chain)
            : base(chain)
        {
        }

        #region ICompositeVisitor[ConstantExpression] implementation
        public Expression Visit (ConstantExpression target, IVisitState state)
        {
            IQueryProvider currentProvider = null;
            if(!state.TryGet<IQueryProvider>(out currentProvider))
                throw new InvalidOperationException("Missing the IQueryProvider in the state.");
            IQueryable query = target.Value as IQueryable;
            
            if(object.ReferenceEquals(currentProvider, query.Provider))
            {
                if(query.Expression.NodeType == System.Linq.Expressions.ExpressionType.Constant)
                {
                    // it is a repository
                    ConstantExpression constantExpression = query.Expression as ConstantExpression;
                    return Continue(constantExpression, state);
                }
                return GetVisitor(query.Expression).Visit(query.Expression, state);
            }
            else
            {
                return Expression.Constant(query.Provider.Execute(query.Expression), target.Type);
            }
        }
        #endregion
        
        internal protected override ICompositeVisitor<Expression, TExpression> AsVisitor<TExpression> (TExpression target)
        {
            ICompositeVisitor<Expression, TExpression> visitor = base.AsVisitor (target);
            
            if(null != visitor)
            {
                ConstantExpression expression = target as ConstantExpression;
                IQueryable queryable = expression.Value as IQueryable;
                if(null == queryable)
                    return null;
            }
            
            return visitor;
        }

    }
}

