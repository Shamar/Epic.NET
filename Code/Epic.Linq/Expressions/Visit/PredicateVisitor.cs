//  
//  PredicateVisitor.cs
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
using Epic.Linq.Expressions.Templates;
using System.Linq.Expressions;

namespace Epic.Linq.Expressions.Visit
{
    // TODO : this is a draft... consider an abstract class instead of "logic"! ! !
    public sealed class PredicateVisitor<TEntity> : VisitorsComposition.VisitorBase, ICompositeVisitor<LambdaExpression>
        where TEntity : class
    {
        private readonly IQueryDataExtractor<Expression<Func<TEntity, bool>>> _extractor;
        private readonly Func<IQuery, RelationExpression, PredicateExpression> _logic;
        public PredicateVisitor (VisitorsComposition chain, IQueryDataExtractor<Expression<Func<TEntity, bool>>> extractor, Func<IQuery, RelationExpression, PredicateExpression> logic)
            : base(chain)
        {
            if(null == extractor)
                throw new ArgumentNullException("extractor");
            if(null == logic)
                throw new ArgumentNullException("logic");
            _extractor = extractor;
            _logic = logic;
        }
        
        internal protected override ICompositeVisitor<TExpression> AsVisitor<TExpression> (TExpression target)
        {
            ICompositeVisitor<TExpression> visitor = base.AsVisitor<TExpression>(target);
            if(null != visitor)
            {
                if(!_extractor.CanParse(target as Expression<Func<TEntity, bool>>))
                    return null;
            }
            return visitor;
        }
        
        public Expression Visit (LambdaExpression target, IVisitState state)
        {
            return Visit(target as Expression<Func<TEntity, bool>>, state);
        }
        
        #region ICompositeVisitor[Expression[Func[TEntity,System.Boolean]]] implementation
        public Expression Visit (Expression<Func<TEntity, bool>> target, IVisitState state)
        {
            RelationExpression relation = null;
            if(!state.TryGet<RelationExpression>(out relation))
                throw new InvalidOperationException();
            IQuery queryData = _extractor.Parse(target);
            return _logic(queryData, relation);
        }
        #endregion

    }
}

