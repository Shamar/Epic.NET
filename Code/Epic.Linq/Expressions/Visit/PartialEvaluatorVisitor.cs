//  
//  PartialEvaluatorVisitor.cs
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
using System.Collections.Generic;

namespace Epic.Linq.Expressions.Visit
{
    public sealed class PartialEvaluatorVisitor : VisitorsComposition<Expression>.VisitorBase, 
        ICompositeVisitor<Expression, MemberExpression>,
        ICompositeVisitor<Expression, MethodCallExpression>
    {
        public PartialEvaluatorVisitor (VisitorsComposition<Expression> chain)
            : base(chain)
        {
        }

        #region ICompositeVisitor[Expression,MemberExpression] implementation
        public Expression Visit (MemberExpression target, IVisitState state)
        {
            Expression owner = Continue(target.Expression, state);
            if(owner.NodeType != System.Linq.Expressions.ExpressionType.Constant)
            {
                if(target.Expression != owner)
                {
                    return Expression.MakeMemberAccess(owner, target.Member);
                }
            }
            else
            {
                ConstantExpression constantOwner = owner as ConstantExpression;
                switch(target.Member.MemberType)
                {
                    case MemberTypes.Property:
                        PropertyInfo property = target.Member as PropertyInfo;
                        return Expression.Constant(property.GetValue(constantOwner.Value, new object[0]), target.Type);
                    case MemberTypes.Field:
                        FieldInfo field = target.Member as FieldInfo;
                        return Expression.Constant(field.GetValue(constantOwner.Value), target.Type);
                    default:
                        break;
                }
            }
            if(owner != target.Expression)
                return Expression.MakeMemberAccess(owner, target.Member);
            return target;
        }
        #endregion

        #region ICompositeVisitor[Expression,MethodCallExpression] implementation
        public Expression Visit (MethodCallExpression target, IVisitState state)
        {
            Expression owner = null;
            List<Expression> arguments = null;
			
			
            if(null != target.Object)
                owner = Continue(target.Object, state);
			
			bool canBeCompiled = null == owner || owner.NodeType == System.Linq.Expressions.ExpressionType.Constant;
            
            if(target.Arguments.Count > 0)
            {
                arguments = new List<Expression>();
                for(int i = 0; i < target.Arguments.Count; ++i)
                {
                    Expression arg = Continue(target.Arguments[i], state);
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
                return Expression.Call(owner, target.Method, arguments);
            }
        }
        #endregion
    }
}

