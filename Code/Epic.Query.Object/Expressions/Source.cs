//  
//  Source.cs
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
    public sealed class Source<TEntity, TIdentity> : Expression<IEnumerable<TEntity>>
        where TEntity : class
        where TIdentity : IEquatable<TIdentity>
    {
        private readonly IRepository<TEntity, TIdentity> _repository;

        public Source (IRepository<TEntity, TIdentity> repository)
        {
            if (null == repository)
                throw new ArgumentNullException ("repository");
            _repository = repository;
        }

        public IRepository<TEntity, TIdentity> Repository {
            get { return _repository; }
        }

        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe (this, visitor, context);
        }

        protected override void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            info.AddValue ("R", _repository, typeof(IRepository<TEntity, TIdentity>));
        }

        private Source (SerializationInfo info, StreamingContext context)
                : base(info, context)
        {
            _repository = (IRepository<TEntity, TIdentity>)info.GetValue ("R", typeof(IRepository<TEntity, TIdentity>));
        }

    }
}

