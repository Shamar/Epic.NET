//  
//  DomainExpression.cs
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
    public sealed class DomainExpression<T> : RelationExpression, IEquatable<RelationExpression>
    {
        public DomainExpression (string name)
            : base(name, ExpressionType.Domain, typeof(T))
        {
        }
        
        private bool Equals(DomainExpression<T> other)
        {
            if(null == other)
                return false;
            return Name.Equals(other.Name);
        }
        
        public override Expression Accept (ICompositeVisitor visitor, IVisitState state)
        {
            ICompositeVisitor<DomainExpression<T>> myVisitor = visitor.GetVisitor<DomainExpression<T>>(this);
            return myVisitor.Visit(this, state);
        }
        
        #region IEquatable[RelationExpression] implementation
        
        public override bool Equals (RelationExpression other)
        {
            return Equals(other as DomainExpression<T>);
        }
        
        #endregion
    }
}

