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
using System.Reflection;

namespace Epic.Linq.Expressions.Normalization
{
    /// <summary>
    /// Reduces any <see cref="MethodCallExpression"/> that would operate over
    /// an <see cref="IQueryable"/> that has been evaluated.
    /// </summary>
    public sealed class ExecutedQueryableMethodsReducer : CompositeVisitor<Expression>.VisitorBase, IVisitor<Expression, MethodCallExpression>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Epic.Linq.Expressions.Normalization.ExecutedQueryableMethodsReducer"/> class.
        /// </summary>
        /// <param name='composition'>
        /// Composition that will own this visitor.
        /// </param>
        public ExecutedQueryableMethodsReducer (CompositeVisitor<Expression> composition)
            : base(composition)
        {
        }

        /// <summary>
        /// Return itself as a visitor for <paramref name="target"/> if it is 
        /// a <see cref="MethodCallExpression"/> containing a method of <see cref="System.Linq.Queryable"/>.
        /// </summary>
        /// <returns>
        /// This visitor.
        /// </returns>
        /// <param name='target'>
        /// The expression that should be visited.
        /// </param>
        /// <typeparam name='TExpression'>
        /// The 1st type parameter.
        /// </typeparam>
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
            Console.WriteLine();
            Console.WriteLine(target.Arguments[0].GetType());
            Console.WriteLine(target.Arguments[0].ToString());
            Console.WriteLine();
            Expression methodSource = VisitInner(target.Arguments[0], state);
            if(methodSource.NodeType == System.Linq.Expressions.ExpressionType.Constant)
            {
                ConstantExpression constantSource = methodSource as ConstantExpression;
                Type queryableType = null;
                if(!(constantSource.Value is IQueryable) && Reflection.TryGetItemTypeOfEnumerable(constantSource.Type, out queryableType))
                {
                    List<object> invokeArgs = new List<object>();
                    invokeArgs.Add(constantSource.Value);
                    for(int i = 1; i < target.Arguments.Count; ++i)
                    {
                        Expression arg = VisitInner(target.Arguments[i], state);
                        switch(arg.NodeType)
                        {
                            case ExpressionType.Quote:
                                invokeArgs.Add(((LambdaExpression)((UnaryExpression)arg).Operand).Compile());
                            break;
                            case ExpressionType.Constant:
                                invokeArgs.Add(((ConstantExpression)arg).Value);
                            break;
                            default:
                                throw new NotImplementedException("TODO");
                        }
                    }

                    MethodInfo enumerableEquivalent = Reflection.Queryable.GetEnumerableEquivalent(target.Method);
                    return Expression.Constant(enumerableEquivalent.Invoke(null, invokeArgs.ToArray()), enumerableEquivalent.ReturnType);
                }
            }
            List<Expression> arguments = new List<Expression>();
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

