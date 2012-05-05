//  
//  Identification.cs
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
using System.Runtime.Serialization;

namespace Epic.Query.Object.Expressions
{
    [Serializable]
    public sealed class Identification<TEntity, TIdentity> : Expression<IEnumerable<TIdentity>>
        where TEntity : class
        where TIdentity : IEquatable<TIdentity>
    {
        private readonly Expression<IEnumerable<TEntity>> _entities;

        public Identification (Expression<IEnumerable<TEntity>> entities)
        {
            if (null == entities)
                throw new ArgumentNullException ("entities");
            _entities = entities;
        }

        public Expression<IEnumerable<TEntity>> Entities {
            get {
                return _entities;
            }
        }

        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe (this, visitor, context);
        }

        protected override void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            info.AddValue ("E", _entities, typeof(Expression<IEnumerable<TEntity>>));
        }

        private Identification (SerializationInfo info, StreamingContext context)
                : base(info, context)
        {
            _entities = (Expression<IEnumerable<TEntity>>)info.GetValue ("E", typeof(Expression<IEnumerable<TEntity>>));
        }
    }
}

