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
using System.Collections.Generic;

namespace Epic.Linq.Expressions.Visit
{
    public sealed class PredicateStructureVisitor : VisitorsComposition<PredicateExpression>.VisitorBase, ICompositeVisitor<PredicateExpression, LambdaExpression>
    {
        public PredicateStructureVisitor(VisitorsComposition<PredicateExpression> chain)
            : base(chain)
        {
        }
        
        internal protected override ICompositeVisitor<PredicateExpression, TExpression> AsVisitor<TExpression> (TExpression target)
        {
            ICompositeVisitor<PredicateExpression, TExpression> visitor = base.AsVisitor<TExpression>(target);
            if(null != visitor)
            {
                LambdaExpression lambdaExp = target as LambdaExpression;
                if(!lambdaExp.Type.Equals(typeof(bool)) && lambdaExp.Parameters.Count != 1) // Filters has only one parameter
                    return null;
            }
            return visitor;
        }
        
        public PredicateExpression Visit (LambdaExpression target, IVisitState state)
        {
            switch(target.Body.NodeType)
            {
                case System.Linq.Expressions.ExpressionType.AndAlso:
                    return VisitAndAlso(target.Body as BinaryExpression, state, target.Parameters[0]);
                case System.Linq.Expressions.ExpressionType.OrElse:
                    return VisitOrElse(target.Body as BinaryExpression, state, target.Parameters[0]);
                case System.Linq.Expressions.ExpressionType.Not:
                    return VisitNot(target.Body as UnaryExpression, state, target.Parameters[0]);
            }
            
            // TODO : check for outer Selects/SelectManys, as a where out of the outer could have an anonymous type that is not a transparent identifier
            
            if(Reflection.IsAnonymous(target.Parameters[0].Type))
            {
                // a transparent identifier to handle
                ParameterExpression[] usedParameters = Reflection.GetContainedParameters(target.Body); // FIX : WRONG ! ! no parameter will be found, as the transparent identifier is the only parameter used ! !
                LambdaExpression newLambda = Expression.Lambda(target.Body, usedParameters);
                return ForwardToChain(newLambda, state);
            }
            else
            {
                return ForwardToNext(target, state);
            }
        }
        
        
        
        private PredicateExpression VisitNot(UnaryExpression target, IVisitState state, ParameterExpression parameter) // even transparent identifier is a single parameter
        {
            PredicateExpression operand = ForwardToChain(Expression.Lambda(target.Operand, new ParameterExpression[1] { parameter }), state);
            if(operand is NotPredicateExpression)
                return (operand as NotPredicateExpression).Predicate;
            return new NotPredicateExpression(operand);
        }
  
        private PredicateExpression VisitAndAlso(BinaryExpression target, IVisitState state, ParameterExpression parameter)
        {
            PredicateExpression left = ForwardToChain(Expression.Lambda(target.Left, new ParameterExpression[1] { parameter }), state);
            PredicateExpression right = ForwardToChain(Expression.Lambda(target.Right, new ParameterExpression[1] { parameter }), state);
            
            return new AndPredicateExpression(left, right);
        }
        
        private PredicateExpression VisitOrElse(BinaryExpression target, IVisitState state, ParameterExpression parameter)
        {
            PredicateExpression left = ForwardToChain(Expression.Lambda(target.Left, new ParameterExpression[1] { parameter }), state);
            PredicateExpression right = ForwardToChain(Expression.Lambda(target.Right, new ParameterExpression[1] { parameter }), state);
            
            return new OrPredicateExpression(left, right);
        }
    }
    
    internal sealed class TransparentIdentifier
    {
        
    }
    
    
    // TODO : this is a draft... consider an abstract class instead of "logic"! ! !
    public sealed class PredicateVisitor<TEntity> : VisitorsComposition<RelationExpression>.VisitorBase, ICompositeVisitor<RelationExpression, LambdaExpression>
        where TEntity : class
    {
        private readonly IQueryDataExtractor<Expression<Func<TEntity, bool>>> _extractor;
        private readonly Func<IQuery, RelationExpression, RelationExpression> _logic;
        public PredicateVisitor (VisitorsComposition<RelationExpression> chain, IQueryDataExtractor<Expression<Func<TEntity, bool>>> extractor, Func<IQuery, RelationExpression, RelationExpression> logic)
            : base(chain)
        {
            if(null == extractor)
                throw new ArgumentNullException("extractor");
            if(null == logic)
                throw new ArgumentNullException("logic");
            _extractor = extractor;
            _logic = logic;
        }
        
        internal protected override ICompositeVisitor<RelationExpression, TExpression> AsVisitor<TExpression> (TExpression target)
        {
            ICompositeVisitor<RelationExpression, TExpression> visitor = base.AsVisitor<TExpression>(target);
            if(null != visitor)
            {
                if(!_extractor.CanParse(target as Expression<Func<TEntity, bool>>))
                    return null;
            }
            return visitor;
        }
        
        public RelationExpression Visit (LambdaExpression target, IVisitState state)
        {
            return Visit(target as Expression<Func<TEntity, bool>>, state);
        }
        
        #region ICompositeVisitor[Expression[Func[TEntity,System.Boolean]]] implementation
        public RelationExpression Visit (Expression<Func<TEntity, bool>> target, IVisitState state)
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

