//  
//  ConstantExpression.cs
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
using System.Collections.Generic;

namespace Epic.Linq.Expressions
{
    public sealed class ConstantExpression<T> : VisitableExpression, IEquatable<ConstantExpression<T>>
    {
        private readonly T _value;
        public ConstantExpression (T value)
            : base(ExpressionType.Constant, typeof(T))
        {
            _value = value;
        }
        
        public T Value
        {
            get
            {
                return _value;
            }
        }

        #region implemented abstract members of Epic.Linq.Expressions.VisitableExpression
        public override Expression Accept (ICompositeVisitor visitor, IVisitState state)
        {
            return AcceptAs<ConstantExpression<T>>(visitor, state);
        }
        #endregion

        #region IEquatable[ConstantExpression[T]] implementation
        public bool Equals (ConstantExpression<T> other)
        {
            if(null == other)
                return false;
            return EqualityComparer<T>.Default.Equals(_value, other.Value);
        }
        #endregion
        
        public override bool Equals (object obj)
        {
            return Equals (obj as ConstantExpression<T>);
        }
        
        public override int GetHashCode ()
        {
            return Type.GetHashCode ();
        }
    }
}

