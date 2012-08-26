//  
//  QueryableConstantResolver.cs
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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace Epic.Query.Linq.Expressions.Normalization
{
    /// <summary>
    /// Replace the <see cref="IQueryable"/> wrapped into <see cref="ConstantExpression"/> with either the 
    /// corresponding expression or the result of the execution of the query itself.
    /// </summary>
    public sealed class QueryableConstantResolver  : CompositeVisitor<Expression>.VisitorBase, IVisitor<Expression, ConstantExpression>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Query.Linq.Expressions.Normalization.QueryableConstantResolver"/> class.
        /// </summary>
        /// <param name='composition'>
        /// Composition that will own this visitor.
        /// </param>
        public QueryableConstantResolver(CompositeVisitor<Expression> composition)
            : base(composition)
        {
        }

        #region IVisitor<Expression,ConstantExpression> Members

        /// <summary>
        /// Visits a <see cref="ConstantExpression"/> containing an <see cref="IQueryable"/> and returns 
        /// its Expression or the result of the execution. 
        /// Requires a <see cref="IQueryProvider"/> in the <paramref name="context"/>.
        /// </summary>
        /// <param name="target">Expression to visit.</param>
        /// <param name="context">Context of the visit.</param>
        /// <returns>The query's expression or its execution result, as appropriate.</returns>
        public Expression Visit(ConstantExpression target, IVisitContext context)
        {
            IQueryProvider currentProvider = context.Get<IQueryProvider>();
            IQueryable query = target.Value as IQueryable;

            IQueryProvider provider = query.Provider;
            Expression expression = query.Expression;

            if (object.ReferenceEquals(currentProvider, provider))
            {
                if (expression.NodeType == System.Linq.Expressions.ExpressionType.Constant)
                {
                    // this ConstantExpression contains a queryable whose expression contains a ConstantExpression: its a repository!
                    return target;
                }
                return VisitInner(expression, context);
            }
            else
            {
                Type newType = typeof(IEnumerable<>).MakeGenericType(query.ElementType);
                return Expression.Constant(provider.Execute(expression), newType);
            }
        }

        #endregion

        /// <summary>
        /// Return itself as a visitor for <paramref name="target"/> if it is 
        /// a <see cref="ConstantExpression"/> containing a <see cref="IQueryable"/>.
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
        protected override IVisitor<Expression, TExpression> AsVisitor<TExpression>(TExpression target)
        {
            IVisitor<Expression, TExpression> visitor = base.AsVisitor(target);

            if (null != visitor)
            {
                ConstantExpression expression = target as ConstantExpression;
                IQueryable queryable = expression.Value as IQueryable;
                if (null == queryable)
                    return null;
            }

            return visitor;
        }
    }
}
