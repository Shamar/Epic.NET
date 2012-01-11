//  
//  Constant.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2012 Giacomo Tesio
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
using System.Collections.Generic;

namespace Epic.Linq.Expressions.Relational
{
    /// <summary>
    /// Constant value in a relational predicate.
    /// </summary>
    [Serializable]
    public abstract class Constant : Scalar, IEquatable<Constant>
    {
        internal Constant(): base(ScalarType.Constant)
        {
        }

        #region IEquatable[Constant] implementation
        public abstract bool Equals (Constant other);
        #endregion

        public override bool Equals (Scalar other)
        {
            return Equals (other as Constant);
        }
    }
    
    /// <summary>
    /// Constant value in a relational predicate.
    /// </summary>
    [Serializable]
    public sealed class Constant<TValue> : Constant, IEquatable<Constant<TValue>>
    {
        private readonly TValue _value;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Linq.Expressions.Relational.Constant`1"/> class.
        /// </summary>
        /// <param name='value'>
        /// Value of the constant.
        /// </param>
        public Constant (TValue value)
        {
            _value = value;
        }
  
        /// <summary>
        /// Gets the value of the constant.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public TValue Value
        {
            get
            {
                return _value;
            }
        }
        
        #region implemented abstract members of Epic.Linq.Expressions.VisitableBase
        /// <summary>
        /// Accept the specified visitor and context.
        /// </summary>
        /// <param name='visitor'>
        /// Visitor.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        /// <typeparam name='TResult'>
        /// Type of the result produced from the visitor.
        /// </typeparam>
        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe(this, visitor, context);
        }
        #endregion

        #region IEquatable[Constant[TValue]] implementation
        public bool Equals (Constant<TValue> other)
        {
            if(null == other)
                return false;
            return EqualityComparer<TValue>.Default.Equals(_value, other._value);
        }
        #endregion
        
        #region IEquatable[Constant] implementation
        public override bool Equals (Constant other)
        {
            return Equals(other as Constant<TValue>);
        }
        #endregion
        
        public override int GetHashCode ()
        {
            if(null == (object)_value)
                return 0;
            return _value.GetHashCode();
        }
    }
}

