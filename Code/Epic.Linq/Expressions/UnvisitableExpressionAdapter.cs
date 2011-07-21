//  
//  UnvisitableExpressionAdapter.cs
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
using Epic.Linq.Expressions.Visit;
using System.Collections.Generic;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Epic.Linq.Expressions
{
    public sealed class UnvisitableExpressionAdapter : VisitableExpression
    {
        private readonly Expression _expression;
        
        private static Type GetUnvisitableExpressionType (Expression unvisitableExpression)
        {
            if (null == unvisitableExpression)
                throw new ArgumentNullException ("unvisitableExpression");
            if (unvisitableExpression.NodeType.Equals (VisitableExpression.VisitableNodeType))
                throw new ArgumentException ("The expression provided is visitable.");        
            return unvisitableExpression.Type;
        }
        
        public UnvisitableExpressionAdapter (Expression unvisitableExpression)
            : base(GetUnvisitableExpressionType(unvisitableExpression))
        {
            _expression = unvisitableExpression;
        }
        
        #region implemented abstract members of Epic.Linq.Expressions.VisitableExpression
        public override Expression Accept (ICompositeVisitor visitor, IVisitState state)
        {
            if(null == visitor)
                throw new ArgumentNullException("visitor");
            ICompositeVisitor<Expression> expressionVisitor = visitor.GetVisitor<Expression>(_expression);
            return expressionVisitor.Visit(_expression, state);
        }
        #endregion
    }
}

