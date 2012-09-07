//  
//  EnumerableMethodsReducer.cs
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
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Epic.Query.Linq.Expressions.Normalization
{
    /// <summary>
    /// Reduce <see cref="MethodCallExpression"/> expressing methods of <see cref="Enumerable"/> to either their equivalent value 
    /// or an equivalent call to the <see cref="Queryable"/> methods (when the source is actually a queryable and we can translate all the arguments).
    /// </summary>
    public sealed class EnumerableMethodsReducer : CompositeVisitor<Expression>.VisitorBase, IVisitor<Expression, MethodCallExpression>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Epic.Query.Linq.Expressions.Normalization.EnumerableMethodsReducer"/> class.
        /// </summary>
        /// <param name='composition'>
        /// Composition that will own the new instance.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="composition"/> is <see langword="null"/>.</exception>
        public EnumerableMethodsReducer (CompositeVisitor<Expression> composition)
            : base(composition)
        {
        }

        /// <summary>
        /// Return the current instance as a visitor of <paramref name="target"/> if it's a <see cref="MethodCallExpression"/>
        /// related to any <see cref="Enumerable"/>'s method.
        /// </summary>
        /// <returns>
        /// The visitor.
        /// </returns>
        /// <param name='target'>
        /// Target.
        /// </param>
        /// <typeparam name='TExpression'>
        /// Type of the expression to be visited.
        /// </typeparam>
        protected override IVisitor<Expression, TExpression> AsVisitor<TExpression>(TExpression target)
        {
            IVisitor<Expression, TExpression> visitor = base.AsVisitor (target);

            if(null != visitor)
            {
                MethodCallExpression callExp = target as MethodCallExpression;
                if(null != callExp.Object  || !callExp.Method.DeclaringType.Equals(typeof(System.Linq.Enumerable)))
                    return null;
            }

            return visitor;
        }

        #region IVisitor[Expression,MethodCallExpression] implementation
        /// <summary>
        /// Visit the specified expression, reducing all <see cref="MethodCallExpression"/> expressing methods of <see cref="Enumerable"/> to either their equivalent value 
        /// or an equivalent call to the <see cref="Queryable"/> methods (when the source is actually a queryable and we can translate all the arguments).
        /// </summary>
        /// <param name='target'>
        /// Expression to visit.
        /// </param>
        /// <param name='context'>
        /// Visit context. Contains the state produced by previous visitors.
        /// </param>
        /// <returns>
        /// Result of the visit.
        /// </returns>
        public Expression Visit (MethodCallExpression target, IVisitContext context)
        {
            MethodInfo targetMethod = target.Method;
            Expression methodSource = VisitInner(target.Arguments[0], context);

            // TODO: evaluate if a new List<Expression>(target.Arguments.Count) or an array can improve performances.
            List<Expression> fallbackArgs = new List<Expression>();

            int i = 0;
            if (methodSource.NodeType == System.Linq.Expressions.ExpressionType.Constant)
            {
                ConstantExpression constantSource = methodSource as ConstantExpression;
                Type enumerableType = null;

                if (constantSource.Value is IQueryable && Reflection.TryGetItemTypeOfEnumerable(constantSource.Type, out enumerableType))
                {
                    // Enumerable methods that insist over an IQueryable constant are translated to Queryable ones.
                    fallbackArgs.Add(Expression.Constant(constantSource.Value, typeof(IQueryable<>).MakeGenericType(enumerableType)));
                    targetMethod = Reflection.Enumerable.GetQueryableEquivalent(target.Method);
                    goto fallback;
                }
                else
                {
                    fallbackArgs.Add(methodSource);
                    List<object> invokeArgs = new List<object>();
                    invokeArgs.Add(constantSource.Value);
                    for (i = 1; i < target.Arguments.Count; i++)
                    {
                        Expression arg = VisitInner(target.Arguments[i], context);
                        fallbackArgs.Add(arg);
                        switch (arg.NodeType)
                        {
                            case ExpressionType.Lambda:
                                try
                                {
                                    invokeArgs.Add(((LambdaExpression)arg).Compile());
                                }
                                catch (InvalidOperationException)
                                {
                                    goto fallback;
                                }
                                break;
                            case ExpressionType.Constant:
                                invokeArgs.Add(((ConstantExpression)arg).Value);
                                break;
                            default:
                                // after a visit, only Constants and LambdaExpression are can be compiled or evaluated.
                                goto fallback;
                        }
                    }
                    return Expression.Constant(targetMethod.Invoke(null, invokeArgs.ToArray()), targetMethod.ReturnType);
                }
            }
            else
            {
                if(typeof(IQueryable).IsAssignableFrom(methodSource.Type))
                {
                    // Enumerable methods that insist over an IQueryable are translated to Queryable ones.
                    targetMethod = Reflection.Enumerable.GetQueryableEquivalent(target.Method);
                }
                fallbackArgs.Add(methodSource);
            }

            fallback:
            for (++i; i < target.Arguments.Count; ++i)
            {
                Expression arg = VisitInner(target.Arguments[i], context);
                if (arg.NodeType == ExpressionType.Lambda && targetMethod != target.Method)
                    arg = Expression.Quote(arg);
                fallbackArgs.Add(arg);
            }
            return Expression.Call(targetMethod, fallbackArgs.ToArray());
        }
        #endregion
    }
}

