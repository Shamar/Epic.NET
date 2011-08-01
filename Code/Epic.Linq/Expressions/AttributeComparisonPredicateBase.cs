//  
//  AttributeComparisonPredicate.cs
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

namespace Epic.Linq.Expressions
{
    public abstract class AttributeComparisonPredicateBase : PredicateExpression
    {
        private readonly VisitableExpression _left;
        private readonly VisitableExpression _right;
        
        public AttributeComparisonPredicateBase (RelationExpression relation, VisitableExpression left, VisitableExpression right)
            : base(relation)
        {
            if(null == relation)
                throw new ArgumentNullException("relation");
            if(null == left)
                throw new ArgumentNullException("left");
            if(left.NodeType != (System.Linq.Expressions.ExpressionType)ExpressionType.Attribute && left.NodeType != (System.Linq.Expressions.ExpressionType)ExpressionType.Constant)
            {
                throw new ArgumentException("The left operand node type is neither an Attribute nor a Constant.", "left");
            }
            if(null == right)
                throw new ArgumentNullException("right");
            if(right.NodeType != (System.Linq.Expressions.ExpressionType)ExpressionType.Attribute && right.NodeType != (System.Linq.Expressions.ExpressionType)ExpressionType.Constant)
            {
                throw new ArgumentException("The right operand node type is neither an Attribute nor a Constant.", "left");
            }
            if(!left.Type.Equals(right.Type))
            {
                string message = string.Format("Type mismatch. The type of left operand ({0}) do not match the type of the right one ({1}).", left.Type.FullName, right.Type.FullName);
                throw new ArgumentException(message);
            }
            _left = left;
            _right = right;
        }
  
        private bool Equals(AttributeComparisonPredicateBase other)
        {
            if(null == other)
                return false;
            if(object.ReferenceEquals(this, other))
                return true;
            if(_left.Equals(other._left) && _right.Equals(other._left) && this.Domain.Equals(other.Domain))
                return this.GetType().Equals(other.GetType());
            return false;
        }
        
        #region implemented abstract members of Epic.Linq.Expressions.PredicateExpression
        public override bool Equals (PredicateExpression other)
        {
            return Equals(other as AttributeComparisonPredicateBase);
        }
        #endregion
    }
}

