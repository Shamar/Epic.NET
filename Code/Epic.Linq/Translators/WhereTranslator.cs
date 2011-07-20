//  
//  WhereTranslator.cs
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
using Epic.Linq.Expressions.Visit;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace Epic.Linq.Translators
{
    public class WhereTranslator<TEntity> : CompositeVisitorBase, ICompositeVisitor<MethodCallExpression>
        where TEntity : class
    {
        private static readonly MethodInfo _queryableWhere = typeof(Queryable).GetMethod("Where", new Type[] { typeof(IQueryable<TEntity>), typeof(Expression<Func<TEntity, bool>>) });
        private readonly CompositeVisitorChain _predicateVisitors;
        
        public WhereTranslator (CompositeVisitorChain chain)
            : base(chain)
        {
            ICompositeVisitor chainEnd = new PlaceholderUntypedWrapper(this);
            
            _predicateVisitors = new CompositeVisitorChain(chainEnd);
            
        }
        
        protected override ICompositeVisitor<TExpression> AsVisitor<TExpression> (TExpression target)
        {
            ICompositeVisitor<TExpression> visitor = base.AsVisitor (target);
            MethodCallExpression callExp = target as MethodCallExpression;
            if(null != visitor && null != callExp)
            {
                if(! callExp.Method.Equals(_queryableWhere))
                {
                    return null;
                }
                else
                {
                    UnaryExpression predicateQuote = (callExp.Arguments[1] as UnaryExpression);
                    if(! predicateQuote.NodeType.Equals(ExpressionType.Quote))
                        return null;
                    else
                    {
                        Expression<Func<TEntity, bool>> predicate = predicateQuote.Operand as Expression<Func<TEntity, bool>>;
                        if(null == predicate)
                            return null;
                        ICompositeVisitor<Expression<Func<TEntity, bool>>> predicateVisitor = _predicateVisitors.GetVisitor<Expression<Func<TEntity, bool>>>(predicate);
                        VisitorWrapperBase pradicateWrapper = predicateVisitor as VisitorWrapperBase;
                        if(null != pradicateWrapper && pradicateWrapper.WrappedVisitor == this)
                            return null;
                    }
                }
            }
            return visitor;
        }
        
        #region ICompositeVisitor[MethodCallExpression] implementation
        public Expression Visit (MethodCallExpression target, IVisitState state)
        {
            IQueryable<TEntity> queryable = target.Arguments[0] as IQueryable<TEntity>;
            UnaryExpression predicateQuote = target.Arguments[1] as UnaryExpression;
            Expression<Func<TEntity, bool>> predicate = predicateQuote.Operand as Expression<Func<TEntity, bool>>;
            
            ICompositeVisitor<Expression<Func<TEntity, bool>>> predicateVisitor = _predicateVisitors.GetVisitor<Expression<Func<TEntity, bool>>>(predicate);
            return predicateVisitor.Visit(predicate);
        }
        #endregion

    }
}

