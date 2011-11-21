//  
//  ClosureExpander.cs
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
using System.Reflection;

namespace Epic.Linq.Expressions.Normalization
{
    /// <summary>
    /// Closure expander. Replace a "closure" with its value. 
    /// Closures are defined _here_ as accesses to fields or properties of a <seealso cref="ConstantExpression"/>.
    /// </summary>
    /// <seealso cref="http://stackoverflow.com/questions/4722562/heuristic-for-this-and-closures-ok-expression-trees"/>
    public sealed class ClosureExpander : CompositeVisitor<Expression>.VisitorBase, IVisitor<Expression, MemberExpression>
    {
        public ClosureExpander  (CompositeVisitor<Expression> composition)
            : base(composition)
        {
        }
        
        /// <summary>
        /// Return itself as a visitor for <paramref name="target"/> if it is 
        /// a <see cref="MemberExpression"/> related to a field or property 
        /// of a <see cref="ConstantExpression"/>.
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
                MemberExpression expression = target as MemberExpression;
                if( expression.Expression.NodeType != System.Linq.Expressions.ExpressionType.Constant )
                {
                    return null;
                }
            }
            
            return visitor;
        }

        #region IVisitor[Expression,MemberExpression] implementation
        public Expression Visit (MemberExpression target, IVisitContext context)
        {
            try
            {
                LambdaExpression lambda = Expression.Lambda(typeof(Func<>).MakeGenericType(target.Type), target);
                ConstantExpression constant = Expression.Constant(lambda.Compile().DynamicInvoke(), target.Type);
                
                return VisitInner(constant, context);
            }
            catch
            {
                return ContinueVisit(target, context);
            }
        }
        #endregion
    }
}

