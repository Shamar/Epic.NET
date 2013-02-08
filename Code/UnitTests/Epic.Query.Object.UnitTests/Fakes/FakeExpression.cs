//  
//  FakeExpression.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2013 Giacomo Tesio
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
using System.Runtime.Serialization;
using Epic.Query.Object.Expressions;
using System.Collections.Generic;


namespace Epic.Query.Object.UnitTests.Fakes
{
    [Serializable]
    public class FakeExpression<TValue> : Expression<TValue>
    {
        public readonly TValue Value;
        public FakeExpression ()
        {
        }

        public FakeExpression (TValue value)
        {
            Value = value;
        }

        public FakeExpression(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Value = (TValue)info.GetValue("V", typeof(TValue));
        }

        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe(this, visitor, context);
        }

        #region implemented abstract members of Epic.Query.Object.Expressions.Expression
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("V", Value, typeof(TValue));
        }
        #endregion

        public override bool Equals(object obj)
        {
            FakeExpression<TValue> other = obj as FakeExpression<TValue>;
            if(null == other)
                return false;
            return EqualityComparer<TValue>.Default.Equals(Value, other.Value);
        }
        public override int GetHashCode()
        {
            return typeof(TValue).GetHashCode();
        }
    }
}

