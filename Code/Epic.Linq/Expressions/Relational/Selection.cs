//  
//  Selection.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2011 Giacomo Tesio
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
    [Serializable]
    public sealed class Selection : Relation
    {
        private readonly Relation _source;
        private readonly Predicate _condition;
        public Selection (string name, Relation source, Predicate condition)
            : base(RelationType.Selection, name)
        {
            if(null == source)
                throw new ArgumentNullException("source");
            if(null == condition)
                throw new ArgumentNullException("condition");
            _source = source;
            _condition = condition;
        }
        
        public Relation Source
        {
            get
            {
                return _source;
            }
        }
        
        public Predicate Condition
        {
            get
            {
                return _condition;
            }
        }
  
        private bool Equals(Selection other)
        {
            return _source.Equals(other._source) && _condition.Equals(other._condition);
        }
        
        #region implemented abstract members of Epic.Linq.Expressions.Relational.Relation
        public override bool Equals (Relation other)
        {
            if(null == other)
                return false;
            return other.Type == RelationType.Selection && Equals(other as Selection);
        }
        #endregion
 
        #region implemented abstract members of Epic.Linq.Expressions.VisitableBase
        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe(this, visitor, context);
        }
        #endregion
    }
}

