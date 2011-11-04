//  
//  RelationAttribute.cs
//  
//  Author:
//       Marco Veglio <m.veglio@gmail.com>
// 
//  Copyright (c) 2010-2011 Marco Veglio
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

namespace Epic.Linq.Expressions.Relational
{
    public sealed class RelationAttribute: VisitableBase, IEquatable<RelationAttribute>
    {
        private readonly string name;
        private readonly Relation relation;
        
        public RelationAttribute (string name, Relation relation)
        {
            if (null == name)
                throw new ArgumentNullException("name");
            if (null == relation)
                throw new ArgumentNullException("relation");
            this.name = name;
            this.relation = relation;
        }
        
        public string Name { get { return this.name; } }
        
        public Relation Relation { get { return this.relation; } }
        
        public override int GetHashCode ()
        {
            return GetType().GetHashCode() ^ this.name.GetHashCode() ^ this.Relation.GetHashCode ();
        }
        
        public bool Equals (RelationAttribute other)
        {
            if (other == null) return false;
            return other.Name == this.Name && other.Relation == this.Relation;
        }
        
        public override bool Equals (object obj)
        {
            Attribute attribute = obj as Attribute;
            if (null != attribute)
                return this.Equals (attribute);
            return false;
        }
        
        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe(this, visitor, context);
        }
    }
}

