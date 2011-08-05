//  
//  NegatedPredicateVisitor.cs
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
using System.Linq;

namespace Epic.Linq.Expressions.Visit
{
    public sealed class NegatedPredicateVisitor : VisitorsComposition<RelationExpression>.VisitorBase, ICompositeVisitor<RelationExpression, LambdaExpression>
    {
        public NegatedPredicateVisitor (VisitorsComposition<RelationExpression> chain)
            : base(chain)
        {
        }
        
        internal protected override ICompositeVisitor<RelationExpression, TExpression> AsVisitor<TExpression> (TExpression target)
        {
            ICompositeVisitor<RelationExpression, TExpression> visitor = base.AsVisitor (target);
            
            if(null != visitor)
            {
                LambdaExpression lambda = target as LambdaExpression;
                if(lambda.Body.NodeType != System.Linq.Expressions.ExpressionType.Negate)
                {
                    return null;
                }
            }
            
            return visitor;
        }

        #region ICompositeVisitor[Expressions,LambdaExpression] implementation
        public RelationExpression Visit (LambdaExpression target, IVisitState state)
        {
            UnaryExpression notExp = target.Body as UnaryExpression;
            Expression content = notExp.Operand;
            if(content.NodeType == System.Linq.Expressions.ExpressionType.Quote)
            {
                UnaryExpression quoteExp = content as UnaryExpression;
                content = quoteExp.Operand;
            }
            
            LambdaExpression expressionToForward = Expression.Lambda(content, target.Parameters.ToArray());
            
            
            return ForwardToChain(expressionToForward, state.Add(new Promemoria(expressionToForward)));
        }
        #endregion
  
        
        public sealed class Promemoria
        {
            public readonly LambdaExpression ExpressionToNegate;
            public Promemoria(LambdaExpression expressionToNegate)
            {
                ExpressionToNegate = expressionToNegate;
            }
        }
    }
}

