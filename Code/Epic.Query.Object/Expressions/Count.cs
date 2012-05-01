//  
//  Count.cs
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
using System.Runtime.Serialization;
using System.Collections.Generic;


namespace Epic.Query.Object.Expressions
{
    [Serializable]
    public sealed class Count<TItem> : Expression<uint>
    {
        private readonly Expression<IEnumerable<TItem>> _source;

        public Count (Expression<IEnumerable<TItem>> source)
        {
            if (null == source)
                throw new ArgumentNullException("source");
            _source = source;
        }

        public Expression<IEnumerable<TItem>> Source
        {
            get
            {
                return _source;
            }
        }

        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe(this, visitor, context);
        }

        #region implemented abstract members of Epic.Query.Object.Expressions.Expression
        private Count(SerializationInfo info, StreamingContext context)
        {
            _source = (Expression<IEnumerable<TItem>>)info.GetValue("S", typeof(Expression<IEnumerable<TItem>>));
        }

        protected override void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            info.AddValue("S", _source, typeof(Expression<IEnumerable<TItem>>));
        }
        #endregion
    }
}

