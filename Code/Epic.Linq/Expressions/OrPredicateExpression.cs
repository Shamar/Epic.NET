//  
//  OrPredicateExpression.cs
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
    public sealed class OrPredicateExpression : PredicateExpression
    {
        private readonly PredicateExpression _first;
        private readonly PredicateExpression _second;
        
        public OrPredicateExpression (PredicateExpression first, PredicateExpression second)
        {
            if(null == first)
                throw new ArgumentNullException("first");
            if(null == second)
                throw new ArgumentNullException("second");
            _first = first;
            _second = second;
        }

        #region implemented abstract members of Epic.Linq.Expressions.PredicateExpression
        public override bool Equals (PredicateExpression other)
        {
            if(object.ReferenceEquals(this, other))
                return true;
            OrPredicateExpression otherOr = other as OrPredicateExpression;
            if(null == otherOr)
                return false;
            return _first.Equals(otherOr._first) && _second.Equals(otherOr._second);
        }
        #endregion
        
        public override TResult Accept<TResult>(ICompositeVisitor<TResult> visitor, IVisitState state)
        {
            return AcceptAs<TResult, OrPredicateExpression>(visitor, state);
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

