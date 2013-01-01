//  
//  FakeRelationFunction.cs
//  
//  Author:
//       Marco Veglio <m.veglio@gmail.com>
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
using Epic.Query.Relational;

namespace Epic.Query.Fakes
{
    [Serializable]
    public class FakeRelationFunction: RelationFunction
    {
        public FakeRelationFunction (string name): base(name)
        {
        }

        public override bool Equals (RelationFunction other)
        {
            if (null == other) return false;
            return this.Name == other.Name;
        }

        public override TResult Accept<TResult> (Epic.IVisitor<TResult> visitor, Epic.IVisitContext context)
        {
            return AcceptMe(this, visitor, context);
        }

    }
}

