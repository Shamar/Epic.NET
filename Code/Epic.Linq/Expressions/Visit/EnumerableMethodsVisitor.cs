//  
//  EnumerableMethodsVisitor.cs
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
    /// <summary>
    /// Enumerable methods can be in an expression only when they can be partially evaluated or when they enclose a IQueryable
    /// </summary>
    public sealed class EnumerableMethodsVisitor : VisitorsComposition<Expression>.VisitorBase, ICompositeVisitor<Expression, MethodCallExpression>
    {
        public EnumerableMethodsVisitor (VisitorsComposition<Expression> chain)
            : base(chain)
        {
        }
        
        internal protected override ICompositeVisitor<Expression, TExpression> AsVisitor<TExpression> (TExpression target)
        {
            ICompositeVisitor<Expression, TExpression> visitor = base.AsVisitor (target);
            if(null != visitor)
            {
                MethodCallExpression callExp = target as MethodCallExpression;
                if(null != callExp.Object  || !callExp.Method.DeclaringType.Equals(typeof(Enumerable)))
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
                Type enumerableType = null;
                if(constantSource.Value is IQueryable && Reflection.TryGetItemTypeOfEnumerable(constantSource.Type, out enumerableType))
                {
                    arguments.Add(Expression.Constant(constantSource.Value, typeof(IQueryable<>).MakeGenericType(enumerableType)));
                    for(int i = 1; i < target.Arguments.Count; ++i)
                    {
                        Expression arg = ForwardToChain(target.Arguments[i], state);
                        arguments.Add(arg);
                    }
                    return Expression.Call(Reflection.Enumerable.GetQueryableEquivalent(target.Method), arguments.ToArray());
                }
                else
                {
                    arguments.Add(sourceEnum);
                    bool canBeCompiled = true;
                    for (int i = 1; i < target.Arguments.Count; i++) 
                    {
                        Expression arg = ForwardToChain(target.Arguments[i], state);
                        if(!Reflection.CanBeCompiled(arg))
                        {
                            canBeCompiled = false;
                        }
                        arguments.Add(arg);
                    }
                    if(canBeCompiled)
                    {
                        // TODO : optimize
                        LambdaExpression valueExpression = Expression.Lambda(
                                Expression.Call(target.Method, arguments.ToArray())
                            );
                        Delegate valueProv = valueExpression.Compile();
                        object v = valueProv.DynamicInvoke();
                        return Expression.Constant(v, target.Type);
                    }
                    else
                    {
                        return Expression.Call(target.Method, arguments.ToArray());
                    }
                }
            }
            else
            {
                arguments.Add(sourceEnum);
                for(int i = 1; i < target.Arguments.Count; ++i)
                {
                    Expression arg = ForwardToChain(target.Arguments[i], state);
                    arguments.Add(arg);
                }
                return Expression.Call(target.Method, arguments.ToArray());
            }
        }
        #endregion
    }
}

