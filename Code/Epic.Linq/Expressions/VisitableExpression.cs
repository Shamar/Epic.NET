//  
//  VisitableExpression.cs
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

namespace Epic.Linq.Expressions
{
    public abstract class VisitableExpression : Expression
    {
        protected VisitableExpression(ExpressionType nodeType, Type type)
            : base((System.Linq.Expressions.ExpressionType)nodeType, type)
        {
        }
        
        public abstract Expression Accept(ICompositeVisitor visitor, IVisitState state);
        
        protected Expression AcceptAs<TVisitableExpression>(ICompositeVisitor visitor, IVisitState state) where TVisitableExpression : VisitableExpression
        {
            TVisitableExpression visitable = this as TVisitableExpression;
            if(null == visitable)
            {
                // this would be a bug.
                string message = string.Format("The current expression is of type {0}. Can not be visited as {1}.", this.GetType().FullName, typeof(TVisitableExpression).FullName);
                throw new InvalidOperationException(message);
            }
            var myVisitor = visitor.GetVisitor(visitable);
            return myVisitor.Visit(visitable, state);
        }
    }
}

