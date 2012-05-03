//  
//  SourceCast.cs
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
using Epic.Specifications;

namespace Epic.Query.Object.Expressions
{
    [Serializable]
    public sealed class SourceDowncast<TAbstraction, TEntity> : Expression<IEnumerable<TEntity>>
        where TAbstraction : class
        where TEntity : class, TAbstraction
    {
        private readonly Expression<IEnumerable<TAbstraction>> _source;

        public SourceDowncast (Expression<IEnumerable<TAbstraction>> source)
        {
            if (null == source)
                throw new ArgumentNullException ("source");
            _source = source;
        }

        public Expression<IEnumerable<TAbstraction>> Source {
            get { return _source; }
        }

        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe (this, visitor, context);
        }

        protected override void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            info.AddValue (
                "R",
                _source,
                typeof(Expression<IEnumerable<TAbstraction>>)
            );
        }

        private SourceDowncast (SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _source = (Expression<IEnumerable<TAbstraction>>)info.GetValue (
                "R",
                typeof(Expression<IEnumerable<TAbstraction>>)
            );
        }
    }
}

