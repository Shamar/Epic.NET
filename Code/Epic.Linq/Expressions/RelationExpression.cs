//  
//  RelationExpression.cs
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

namespace Epic.Linq.Expressions
{
    public abstract class RelationExpression : VisitableExpression, IEquatable<RelationExpression>
    {
        private readonly string _name;
        public RelationExpression (string name, ExpressionType nodeType, Type type)
            : base(nodeType, type)
        {
            if(string.IsNullOrEmpty(name))
                throw new ArgumentNullException(name);
            _name = name;
        }
        
        public string Name
        {
            get
            {
                return _name;
            }
        }

        #region IEquatable[RelationExpression] implementation
        public abstract bool Equals (RelationExpression other);
        #endregion
        
        public sealed override bool Equals (object obj)
        {
            return Equals (obj as RelationExpression);
        }
        
        public override int GetHashCode ()
        {
            return _name.GetHashCode() ^ Type.GetHashCode();
        }
    }
}

