//  
//  QueryableMemberAccessVisitor.cs
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
    public sealed class QueryableMemberAccessVisitor : VisitorsComposition.VisitorBase, ICompositeVisitor<MemberExpression>
    {
        public QueryableMemberAccessVisitor (VisitorsComposition chain)
            : base(chain)
        {
        }

        #region ICompositeVisitor[MemberExpression] implementation
        public System.Linq.Expressions.Expression Visit (MemberExpression target, IVisitState state)
        {
            try
            {
                LambdaExpression lambda = Expression.Lambda(typeof(Func<>).MakeGenericType(target.Type), target);
                IQueryable queryable = lambda.Compile().DynamicInvoke() as IQueryable;
                
                ICompositeVisitor<Expression> visitor = GetVisitor<Expression>(queryable.Expression);
                return visitor.Visit(queryable.Expression, state);
            }
            catch
            {
                ICompositeVisitor<MemberExpression> visitor = GetNextVisitor<MemberExpression>(target);
                return visitor.Visit(target, state);
            }
        }
        #endregion

        internal protected override ICompositeVisitor<TExpression> AsVisitor<TExpression> (TExpression target)
        {
            ICompositeVisitor<TExpression> visitor = base.AsVisitor (target);
            if(null != visitor)
            {
                MemberExpression memberExp = target as MemberExpression;
                if(!(typeof(IQueryable).IsAssignableFrom(memberExp.Type) && memberExp.Expression.NodeType == ExpressionType.Constant))
                {
                    return null;
                }
            }
            return visitor;
        }
    }
}

