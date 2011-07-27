//  
//  Match.cs
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
using System.Collections.Generic;

namespace Epic.Linq.Expressions.Templates
{
    internal static class Match
    {
        internal static bool UnaryExpression(UnaryExpression expression, UnaryExpression template)
        {
            if(template.IsLifted != expression.IsLifted || template.IsLiftedToNull != expression.IsLiftedToNull)
                return false;
            if (null == template.Method)
                return null == expression.Method;
            return template.Method.Equals(expression.Method);
        }
        
        internal static bool BinaryExpression(BinaryExpression expression, BinaryExpression template)
        {
            if(template.IsLifted != expression.IsLifted || template.IsLiftedToNull != expression.IsLiftedToNull)
                return false;
            if (null == template.Method)
                return null == expression.Method;
            return template.Method.Equals(expression.Method);
        }
        
        internal static bool InvocationExpression(InvocationExpression expression, InvocationExpression template)
        {
            return template.Arguments.Count == expression.Arguments.Count;
        }
        
        internal static bool LambdaExpression(LambdaExpression expression, LambdaExpression template)
        {
            return template.Parameters.Count == expression.Parameters.Count;
        }

        internal static bool MemberExpression(MemberExpression expression, MemberExpression template)
        {
            return template.Member.Equals(expression.Member);
        }

        internal static bool MethodCall(MethodCallExpression expression, MethodCallExpression template)
        {
            if(template.Arguments.Count != expression.Arguments.Count)
                return false;
            if (null == template.Method)
                return null == expression.Method;
            return template.Method.Equals(expression.Method);
        }
        
        internal static bool MemberInit(MemberInitExpression expression, MemberInitExpression template)
        {
            if(template.Bindings.Count != expression.Bindings.Count)
                return false;
            if(!template.NewExpression.Constructor.Equals(expression.NewExpression.Constructor))
                return false;
            
            return true;
        }
        
        internal static bool NewArrayExpression(NewArrayExpression expression, NewArrayExpression template)
        {
            return template.Expressions.Count == expression.Expressions.Count;
        }

        internal static bool ListInitExpression(ListInitExpression expression, ListInitExpression template)
        {
            if(template.Initializers.Count != expression.Initializers.Count)
                return false;
            
            int i = 0;
            while(i < expression.Initializers.Count)
            {
                ElementInit templateInit = template.Initializers[i];
                ElementInit expressionInit = template.Initializers[i];
                if(templateInit.Arguments.Count != expressionInit.Arguments.Count || !templateInit.AddMethod.Equals(expressionInit.AddMethod))
                    return false;
                ++i;
            }
            
            return true;
        }

        internal static bool TypeBinaryExpression(TypeBinaryExpression expression, TypeBinaryExpression template)
        {
            return template.TypeOperand.Equals(expression.TypeOperand);
        }
        
        internal static bool Expression(Expression expression, Expression template)
        {
            return template.NodeType == expression.NodeType && template.Type.Equals(expression.Type);
        }
    }
}

