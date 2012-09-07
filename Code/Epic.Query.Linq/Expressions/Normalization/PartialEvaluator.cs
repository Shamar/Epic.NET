//  
//  PartialEvaluator.cs
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

namespace Epic.Query.Linq.Expressions.Normalization
{
    /// <summary>
    /// Replace tree branches that can be evaluated with their value (in a <see cref="ConstantExpression"/>).
    /// </summary>
    public sealed class PartialEvaluator : CompositeVisitor<Expression>.VisitorBase, 
        IVisitor<Expression, MemberExpression>,
        IVisitor<Expression, MethodCallExpression>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartialEvaluator"/> class.
        /// </summary>
        /// <param name='composition'>
        /// Composition that will own this visitor.
        /// </param>
        public PartialEvaluator (CompositeVisitor<Expression> composition)
            : base(composition)
        {
        }

        #region IVisitor[Expression,MemberExpression] implementation
        /// <summary>
        /// Visits a <see cref="MemberExpression"/> and returns a <see cref="ConstantExpression"/> containing the corresponding value,
        /// or the <paramref name="target"/> itself when it can not be reduced.
        /// </summary>
        /// <param name="target">Expression to visit.</param>
        /// <param name="context">Context of the visit.</param>
        /// <returns>The reduced expression or <paramref name="target"/>, as appropriate.</returns>
        public Expression Visit (MemberExpression target, IVisitContext context)
        {
            Expression owner = null;
            if(null != target.Expression)
                owner = VisitInner(target.Expression, context);
            if(null == owner)
            {
                // static members
                switch(target.Member.MemberType)
                {
                    case MemberTypes.Property:
                        PropertyInfo property = target.Member as PropertyInfo;
                        return Expression.Constant(property.GetValue(null, new object[0]), target.Type);
                    case MemberTypes.Field:
                        FieldInfo field = target.Member as FieldInfo;
                        return Expression.Constant(field.GetValue(null), target.Type);
                }
            }
            else if (owner.NodeType == System.Linq.Expressions.ExpressionType.Constant)
            {
                // instance members
                ConstantExpression constantOwner = owner as ConstantExpression;
                switch(target.Member.MemberType)
                {
                    case MemberTypes.Property:
                        PropertyInfo property = target.Member as PropertyInfo;
                        return Expression.Constant(property.GetValue(constantOwner.Value, new object[0]), target.Type);
                    case MemberTypes.Field:
                        FieldInfo field = target.Member as FieldInfo;
                        return Expression.Constant(field.GetValue(constantOwner.Value), target.Type);
                }
            }
            
            if(owner != target.Expression)
                return Expression.MakeMemberAccess(owner, target.Member);
            return target;
        }
        #endregion IVisitor[Expression,MemberExpression] implementation

        #region IVisitor[Expression,MethodCallExpression] implementation
        /// <summary>
        /// Visits a <see cref="MethodCallExpression"/> and returns a <see cref="ConstantExpression"/> containing the corresponding value,
        /// or the <paramref name="target"/> itself when it can not be reduced.
        /// </summary>
        /// <param name="target">Expression to visit.</param>
        /// <param name="context">Context of the visit.</param>
        /// <returns>The reduced expression or <paramref name="target"/>, as appropriate.</returns>
        public Expression Visit (MethodCallExpression target, IVisitContext context)
        {
            Expression owner = null;
            List<Expression> arguments = null;

            if(null != target.Object)
                owner = VisitInner(target.Object, context);

            bool canBeCompiled = null == owner || owner.NodeType == System.Linq.Expressions.ExpressionType.Constant;
            bool canReturnTarget = owner == target.Object;

            if(target.Arguments.Count > 0)
            {
                arguments = new List<Expression>();
                for(int i = 0; i < target.Arguments.Count; ++i)
                {
                    Expression arg = VisitInner(target.Arguments[i], context);
                    canReturnTarget &= arg == target.Arguments[i];
                    if(arg.NodeType != System.Linq.Expressions.ExpressionType.Constant)
                    {
                        canBeCompiled = false;
                    }
                    arguments.Add(arg);
                }
            }

            if(canBeCompiled)
            {
                if(null != arguments)
                {
                    object[] args = new object[arguments.Count];
                    
                    for(int i = 0; i < arguments.Count; ++i)
                    {
                        args[i] = (arguments[i] as ConstantExpression).Value;
                    }
                    return Expression.Constant(target.Method.Invoke((owner as ConstantExpression).Value, args), target.Type);
                }
                else
                {
                    return Expression.Constant(target.Method.Invoke((owner as ConstantExpression).Value, new object[0]), target.Type);
                }
            }
            else
            {
                if (canReturnTarget)
                    return target;
                if(null == arguments)
                {
                    return Expression.Call(owner, target.Method);
                }
                return Expression.Call(owner, target.Method, arguments);
            }
        }
        #endregion IVisitor[Expression,MemberExpression] implementation
    }
}
