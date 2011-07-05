//  
//  QueryExpression.cs
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
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Epic.Linq.Expressions
{
    public abstract class QueryExpression : Expression
    {
        private readonly IEnumerable<Type> _types;
        protected QueryExpression(params Type[] types)
            : this(ValidateArray(types))
        {
            if(null == types|| types.Length < 1)
                throw new ArgumentNullException();
            _types = types;
        }
        
        protected QueryExpression(QueryExpression mother)
        {
            if(null == mother)
                throw new ArgumentNullException();
            _types = mother.Types;
        }
        
        public IEnumerable<Type> Types
        {
            get { return _types; }
        }
        
        public abstract void Accept(IVisitor visitor);
        
        public abstract QueryExpression MergeWith(QueryExpression expression);
    }
}

