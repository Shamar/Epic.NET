//  
//  Cut.cs
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
    public sealed class Cut<TEntity> : Expression<IEnumerable<TEntity>>
        where TEntity : class
    {
        private readonly Order<TEntity> _source;
        private readonly uint _toTake;
        private readonly uint _toSkip;

        public Cut (Order<TEntity> source, uint toTake)
            : this(source, toTake, 0)
        {
        }

        public Cut (uint toSkip, Order<TEntity> source)
            : this(source, uint.MaxValue, toSkip)
        {
        }

        public Cut (Order<TEntity> source, uint toTake, uint toSkip)
        {
            if (null == source)
                throw new ArgumentNullException ("source");
            if (0 == toTake)
                throw new ArgumentOutOfRangeException ("toTake", "The number of elements to take must be greater than zero.");
            if (uint.MaxValue == toSkip)
                throw new ArgumentOutOfRangeException ("toSkip", string.Format ("The number of elements to skip must be lower than zero.", uint.MaxValue));
            _source = source;
            _toTake = toTake;
            _toSkip = toSkip;
        }

        public Order<TEntity> Source {
            get { return _source; }
        }

        public uint TakingAtMost {
            get { return _toTake; }
        }

        public uint Skipping {
            get { return _toSkip; }
        }

        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe (this, visitor, context);
        }

        protected override void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            info.AddValue ("S", _source, typeof(Order<TEntity>));
            info.AddValue ("L", _toTake);
            info.AddValue ("O", _toSkip);
        }

        private Cut (SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _source = (Order<TEntity>)info.GetValue ("S", typeof(Order<TEntity>));
            _toTake = info.GetUInt32("L");
            _toSkip = info.GetUInt32("O");
        }
    }

}

