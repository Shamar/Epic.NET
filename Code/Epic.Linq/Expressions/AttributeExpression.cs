//  
//  AttributeExpression.cs
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
using Epic.Linq.Expressions.Visit;

namespace Epic.Linq.Expressions
{
    public sealed class AttributeExpression<T> : VisitableExpression, IEquatable<AttributeExpression<T>>
    {
        private readonly RelationExpression _relation;
        private readonly string _name;
        public AttributeExpression (RelationExpression relation, string name)
            : base(ExpressionType.Attribute, typeof(T))
        {
            if(null == relation)
                throw new ArgumentNullException("relation");
            if(string.IsNullOrEmpty("name"))
                throw new ArgumentNullException("name");
            _relation = relation;
            _name = name;
        }
        
        #region implemented abstract members of Epic.Linq.Expressions.VisitableExpression
        public override Expression Accept (ICompositeVisitor visitor, IVisitState state)
        {
            return AcceptAs<AttributeExpression<T>>(visitor, state);
        }
        #endregion

        #region IEquatable[AttributeExpression[T]] implementation
        public bool Equals (AttributeExpression<T> other)
        {
            if(null == other)
                return false;
            return _name.Equals(other._name) && _relation.Equals(other._relation);
        }
        #endregion
        
        public override bool Equals (object obj)
        {
            return Equals (obj as AttributeExpression<T>);
        }
        
        public override int GetHashCode ()
        {
            return _name.GetHashCode();
        }
    }
}

