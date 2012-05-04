//  
//  Limit.cs
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
    public sealed class Limits<TEntity> : Expression<IEnumerable<TEntity>>
        where TEntity : class
    {
        private readonly Order<TEntity> _source;
        private readonly uint _limit;
        private readonly uint _offset;

        public Limits (Order<TEntity> source, uint limit)
            : this(source, limit, 0)
        {
        }

        public Limits (Order<TEntity> source, uint limit, uint offset)
        {
            if (null == source)
                throw new ArgumentNullException ("source");
            if (0 == limit)
                throw new ArgumentOutOfRangeException ("limit", "The limit must be greater than zero.");
            _source = source;
            _limit = limit;
            _offset = offset;
        }

        public Order<TEntity> Source {
            get { return _source; }
        }

        public uint Limit {
            get { return _limit; }
        }

        public uint Offset {
            get { return _offset; }
        }

        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe (this, visitor, context);
        }

        protected override void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            info.AddValue ("S", _source, typeof(Order<TEntity>));
            info.AddValue ("L", _limit);
            info.AddValue ("O", _offset);
        }

        private Limits (SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _source = (Order<TEntity>)info.GetValue ("A", typeof(Order<TEntity>));
            _limit = info.GetUInt32("L");
            _offset = info.GetUInt32("O");
        }
    }

}

