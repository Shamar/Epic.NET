//  
//  ClosureVisitor.cs
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

namespace Epic.Linq.Expressions.Visit
{
    /// <summary>
    /// Closure visitor. Replace a closure with its value. Closures are defined as accesses to fields or properties of constants.
    /// </summary>
    public sealed class ClosureVisitor : VisitorsComposition<Expression>.VisitorBase, ICompositeVisitor<Expression, MemberExpression>
    {
        public ClosureVisitor (VisitorsComposition<Expression> chain)
            : base(chain)
        {
        }
        
        internal protected override ICompositeVisitor<Expression, TExpression> AsVisitor<TExpression> (TExpression target)
        {
            ICompositeVisitor<Expression, TExpression> visitor = base.AsVisitor (target);
            
            if(null != visitor)
            {
                MemberExpression expression = target as MemberExpression;
                if( expression.Expression.NodeType != System.Linq.Expressions.ExpressionType.Constant 
                    || (expression.Member.MemberType != MemberTypes.Field && expression.Member.MemberType != MemberTypes.Property))
                {
                    return null;
                }
            }
            
            return visitor;
        }

        #region ICompositeVisitor[MemberExpression] implementation
        public Expression Visit (MemberExpression target, IVisitState state)
        {
            try
            {
                LambdaExpression lambda = Expression.Lambda(typeof(Func<>).MakeGenericType(target.Type), target);
                ConstantExpression constant = Expression.Constant(lambda.Compile().DynamicInvoke(), target.Type);
                
                ICompositeVisitor<Expression, ConstantExpression> visitor = GetVisitor<ConstantExpression>(constant);
                return visitor.Visit(constant, state);
            }
            catch
            {
                ICompositeVisitor<Expression, MemberExpression> visitor = GetNextVisitor<MemberExpression>(target);
                return visitor.Visit(target, state);
            }
        }
        #endregion
    }
}

