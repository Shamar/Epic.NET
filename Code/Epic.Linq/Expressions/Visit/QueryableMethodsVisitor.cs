//  
//  QueryableExpressionVisitor.cs
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
using System.Collections.Generic;

namespace Epic.Linq.Expressions.Visit
{
    public sealed class QueryableMethodsVisitor : VisitorsComposition<Expression>.VisitorBase, ICompositeVisitor<Expression, MethodCallExpression>
    {
        public QueryableMethodsVisitor (VisitorsComposition<Expression> chain)
            : base(chain)
        {
        }
        
        internal protected override ICompositeVisitor<Expression, TExpression> AsVisitor<TExpression> (TExpression target)
        {
            ICompositeVisitor<Expression, TExpression> visitor = base.AsVisitor (target);
            
            if(null == visitor)
            {
                MethodCallExpression callExp = target as MethodCallExpression;
                if(null != callExp.Object  || !callExp.Method.DeclaringType.Equals(typeof(Queryable)))
                    return null;
            }
            
            return visitor;
        }

        #region ICompositeVisitor[Expression,MethodCallExpression] implementation
        public Expression Visit (MethodCallExpression target, IVisitState state)
        {
            Expression sourceEnum = ForwardToChain(target.Arguments[0], state);
            List<Expression> arguments = new List<Expression>();
            if(sourceEnum.NodeType == System.Linq.Expressions.ExpressionType.Constant)
            {
                ConstantExpression constantSource = sourceEnum as ConstantExpression;
                Type queryableType = null;
                if(!(constantSource.Value is IQueryable) && Reflection.TryGetItemTypeOfEnumerable(constantSource.Type, out queryableType))
                {
                    arguments.Add(Expression.Constant(constantSource.Value, typeof(IEnumerable<>).MakeGenericType(queryableType)));
                    for(int i = 1; i < target.Arguments.Count; ++i)
                    {
                        Expression arg = ForwardToChain(target.Arguments[i], state);
                        arguments.Add(arg);
                    }
                    return Expression.Call(Reflection.Queryable.GetEnumerableEquivalent(target.Method), arguments.ToArray());
                }
            }
            arguments.Add(sourceEnum);
            for(int i = 1; i < target.Arguments.Count; ++i)
            {
                Expression arg = ForwardToChain(target.Arguments[i], state);
                arguments.Add(arg);
            }
            return Expression.Call(target.Method, arguments.ToArray());
        }
        #endregion
    }
}

