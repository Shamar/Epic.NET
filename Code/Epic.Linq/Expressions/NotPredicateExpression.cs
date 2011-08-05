//  
//  NotPredicateExpression.cs
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
    public sealed class NotPredicateExpression : PredicateExpression
    {
        private readonly PredicateExpression _negated;
        public NotPredicateExpression (PredicateExpression predicateToNegate)
        {
            if(null == predicateToNegate)
                throw new ArgumentNullException("negated");
            _negated = predicateToNegate;
        }
        
        public PredicateExpression Predicate
        {
            get
            {
                return _negated;
            }
        }
        
        public override TResult Accept<TResult> (ICompositeVisitor<TResult> visitor, IVisitState state)
        {
            return AcceptAs<TResult, NotPredicateExpression>(visitor, state);
        }
        
        private bool Equals(NotPredicateExpression other)
        {
            if(null == other)
                return false;
            return _negated.Equals(other._negated);
        }
        
        public override bool Equals (PredicateExpression other)
        {
            return Equals(other as NotPredicateExpression);
        }
    }
}

