//  
//  ExecutedQueryableMethodsReducer.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Epic.Linq.Expressions.Normalization
{
    public sealed class ExecutedQueryableMethodsReducer : CompositeVisitor<Expression>.VisitorBase, IVisitor<Expression, MethodCallExpression>
    {
        public ExecutedQueryableMethodsReducer (CompositeVisitor<Expression> composition)
            : base(composition)
        {
        }

        internal protected override IVisitor<Expression, TExpression> AsVisitor<TExpression> (TExpression target)
        {
            IVisitor<Expression, TExpression> visitor = base.AsVisitor (target);

            if(null != visitor)
            {
                MethodCallExpression callExp = target as MethodCallExpression;
                if(null != callExp.Object  || !callExp.Method.DeclaringType.Equals(typeof(System.Linq.Queryable)))
                    return null;
            }

            return visitor;
        }

        #region IVisitor[Expression,MethodCallExpression] implementation
        public Expression Visit (MethodCallExpression target, IVisitContext state)
        {
            Expression methodSource = VisitInner(target.Arguments[0], state);
            List<Expression> arguments = new List<Expression>();
            if(methodSource.NodeType == System.Linq.Expressions.ExpressionType.Constant)
            {
                ConstantExpression constantSource = methodSource as ConstantExpression;
                Type queryableType = null;
                if(!(constantSource.Value is IQueryable) && Reflection.TryGetItemTypeOfEnumerable(constantSource.Type, out queryableType))
                {
                    arguments.Add(Expression.Constant(constantSource.Value, typeof(IEnumerable<>).MakeGenericType(queryableType)));
                    for(int i = 1; i < target.Arguments.Count; ++i)
                    {
                        Expression arg = VisitInner(target.Arguments[i], state);
                        arguments.Add(arg);
                    }
                    return null; //Expression.Call(Reflection.Queryable.GetEnumerableEquivalent(target.Method), arguments.ToArray());
                }
            }
            arguments.Add(methodSource);
            for(int i = 1; i < target.Arguments.Count; ++i)
            {
                Expression arg = VisitInner(target.Arguments[i], state);
                arguments.Add(arg);
            }
            return Expression.Call(target.Method, arguments.ToArray());
        }
        #endregion
    }
}

