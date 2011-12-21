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
        /// <summary>
        /// Visit <paramref name="target"/> and return its evaluation in a <see cref="ConstantExpression"/>
        /// when its source has been evaluated.
        /// </summary>
        /// <param name='target'>
        /// Call to a method of <see cref="System.Linq.Queryable"/>.
        /// </param>
        /// <param name='context'>
        /// Visit context.
        /// </param>
        public Expression Visit (MethodCallExpression target, IVisitContext context)
        {
            MethodInfo targetMethod = target.Method;
            Expression methodSource = VisitInner(target.Arguments[0], context);

            // TODO: evaluate if a new List<Expression>(target.Arguments.Count) or an array can improve performances.
            List<Expression> fallbackArgs = new List<Expression>();

            fallbackArgs.Add(methodSource);
            int i = 1;
            if(methodSource.NodeType == System.Linq.Expressions.ExpressionType.Constant)
            {
                ConstantExpression constantSource = methodSource as ConstantExpression;
                if(!(constantSource.Value is IQueryable))
                {
                    targetMethod = Reflection.Queryable.GetEnumerableEquivalent(targetMethod);

                    List<object> invokeArgs = new List<object>();
                    invokeArgs.Add(constantSource.Value);
                    for(; i < target.Arguments.Count; ++i)
                    {
                        Expression arg = VisitInner(target.Arguments[i], context);
                        fallbackArgs.Add(AdaptArgumentToEnumerableMethod(arg));
                        switch (arg.NodeType)
                        {
                            case ExpressionType.Quote:
                                try
                                {
                                    invokeArgs.Add(((LambdaExpression)((UnaryExpression)arg).Operand).Compile());
                                }
                                catch(InvalidOperationException)
                                {
                                    goto fallback;
                                }
                            break;
                            case ExpressionType.Constant:
                                invokeArgs.Add(((ConstantExpression)arg).Value);
                            break;
                            default:
                                goto fallback;
                        }
                    }
                    
                    return Expression.Constant(targetMethod.Invoke(null, invokeArgs.ToArray()), targetMethod.ReturnType);
                }
            }

            // Yes, I know. Goto is harmful. Feel free to refactor, but without reducing neither readability nor performance.
            fallback:
            for(++i; i < target.Arguments.Count; ++i)
            {
                Expression arg = VisitInner(target.Arguments[i], context);
                fallbackArgs.Add(AdaptArgumentToEnumerableMethod(arg));
            }
            return Expression.Call(targetMethod, fallbackArgs.ToArray());


        }
        #endregion

        /// <summary>
        /// Adapt an argument produced for a <see cref="System.Linq.Queryable"/> method, 
        /// to an argument valid for a <see cref="System.Linq.Enumerable"/> one.
        /// </summary>
        /// <param name="argument">An argument for a Queryable method call.</param>
        /// <returns>An argument for an Enumerable method call.</returns>
        /// <remarks>
        /// Actually, it replace quotes with their operand.
        /// </remarks>
        private static Expression AdaptArgumentToEnumerableMethod(Expression argument)
        {
            if (null == argument || argument.NodeType != ExpressionType.Quote)
                return argument;
            return ((UnaryExpression)argument).Operand;
        }
    }
}

