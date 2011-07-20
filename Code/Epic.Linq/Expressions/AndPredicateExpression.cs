//  
//  AndPredicateExpression.cs
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

namespace Epic.Linq.Expressions
{
    public sealed class AndPredicateExpression : PredicateExpression
    {
        // TODO: move on static helper
        private static DomainExpression ValidateAndGetDomain(PredicateExpression first, PredicateExpression second)
        {
            if(null == first)
                throw new ArgumentNullException("first");
            if(null == second)
                throw new ArgumentNullException("second");
            if(!first.Domain.Equals(second.Domain))
                throw new ArgumentException("The predicates do not share the same domain.");
            return first.Domain;
        }
        
        private readonly PredicateExpression _first;
        private readonly PredicateExpression _second;
        
        public AndPredicateExpression (PredicateExpression first, PredicateExpression second)
            : base(ValidateAndGetDomain(first, second))
        {
            _first = first;
            _second = second;
        }

        #region implemented abstract members of Epic.Linq.Expressions.PredicateExpression
        public override bool Equals (PredicateExpression other)
        {
            if(object.ReferenceEquals(this, other))
                return true;
            AndPredicateExpression otherAnd = other as AndPredicateExpression;
            if(null == otherAnd)
                return false;
            return _first.Equals(otherAnd._first) && _second.Equals(otherAnd._second);
        }
        #endregion
        
        public override System.Linq.Expressions.Expression Accept (ICompositeVisitor visitor, IVisitState state)
        {
            ICompositeVisitor<AndPredicateExpression> queryVisitor = visitor.GetVisitor<AndPredicateExpression>(this, state);
            return queryVisitor.Visit(this, state);
        }
        
        public PredicateExpression First
        {
            get
            {
                return _first;
            }
        }
        
        public PredicateExpression Second
        {
            get
            {
                return _second;
            }
        }
    }
}

