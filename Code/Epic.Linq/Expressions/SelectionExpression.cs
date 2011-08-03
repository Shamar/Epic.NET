//  
//  SelectExpression.cs
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

namespace Epic.Linq.Expressions
{
    public sealed class SelectionExpression : RelationExpression
    {
        private readonly RelationExpression _source;
        private readonly PredicateExpression _predicate;
        public SelectionExpression (RelationExpression source, PredicateExpression predicate)
            : base(source.Name, ExpressionType.Selection, source.Type)
        {
            if(null == source)
                throw new ArgumentNullException("source");
            if(null == predicate)
                throw new ArgumentNullException("predicate");
            
            if(source.NodeType == (System.Linq.Expressions.ExpressionType)ExpressionType.Selection)
            {
                SelectionExpression selectExp = source as SelectionExpression;
                _source = selectExp._source;
                _predicate = new AndPredicateExpression(selectExp._predicate, predicate);
            }
            else
            {
                _source = source;
                _predicate = predicate;
            }
        }
        
        public RelationExpression Source
        {
            get
            {
                return _source;
            }
        }
        
        public PredicateExpression Predicate
        {
            get
            {
                return _predicate;
            }
        }
  
        private bool Equals(SelectionExpression other)
        {
            if(null == other)
                return false;
            return this.Name.Equals(other.Name) && _source.Equals(other._source) && _predicate.Equals(other._predicate);
        }
        
        public override TResult Accept<TResult> (ICompositeVisitor<TResult> visitor, IVisitState state)
        {
            return base.AcceptAs<TResult, SelectionExpression>(visitor, state);
        }
        
        #region implemented abstract members of Epic.Linq.Expressions.RelationExpression
        public override bool Equals (RelationExpression other)
        {
            return Equals(other as SelectionExpression);
        }
        #endregion
    }
}

