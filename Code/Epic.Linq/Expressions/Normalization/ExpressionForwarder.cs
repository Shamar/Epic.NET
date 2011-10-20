//  
//  ExpressionForwarder.cs
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

namespace Epic.Linq.Expressions.Normalization
{
    internal sealed class ExpressionForwarder : CompositeVisitor<Expression>.VisitorBase, IVisitor<Expression, Expression>
    {
        public ExpressionForwarder (CompositeVisitor<Expression> composition)
            : base(composition)
        {
        }
        
        #region IVisitor[Expression,Expression] implementation
        public Expression Visit (Expression target, IVisitContext context)
        {
            if (target == null)
                return null;

            switch (target.NodeType) {
                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.UnaryPlus:
                    return this.VisitInner ((UnaryExpression)target, context);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Power:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.And:
                case ExpressionType.Or:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.LeftShift:
                case ExpressionType.RightShift:
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                    return this.VisitInner ((BinaryExpression)target, context);
                case ExpressionType.Conditional:
                    return this.VisitInner ((ConditionalExpression)target, context);
                case ExpressionType.Constant:
                    return this.VisitInner ((ConstantExpression)target, context);
                case ExpressionType.Invoke:
                    return this.VisitInner ((InvocationExpression)target, context);
                case ExpressionType.Lambda:
                    return this.VisitInner ((LambdaExpression)target, context);
                case ExpressionType.MemberAccess:
                    return this.VisitInner ((MemberExpression)target, context);
                case ExpressionType.Call:
                    return this.VisitInner ((MethodCallExpression)target, context);
                case ExpressionType.New:
                    return this.VisitInner ((NewExpression)target, context);
                case ExpressionType.NewArrayBounds:
                case ExpressionType.NewArrayInit:
                    return this.VisitInner ((NewArrayExpression)target, context);
                case ExpressionType.MemberInit:
                    return this.VisitInner ((MemberInitExpression)target, context);
                case ExpressionType.ListInit:
                    return this.VisitInner ((ListInitExpression)target, context);
                case ExpressionType.Parameter:
                    return this.VisitInner ((ParameterExpression)target, context);
                case ExpressionType.TypeIs:
                    return this.VisitInner ((TypeBinaryExpression)target, context);
        
                default:
                    string message = string.Format("Unknown expression type {0} (NodeType: {1}).", target.GetType().FullName, target.NodeType.ToString());
                    throw new ArgumentException(message, "expression");
            }
        }
        #endregion
    }
}

