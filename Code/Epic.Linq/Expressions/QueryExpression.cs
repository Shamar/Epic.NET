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
using Epic.Linq.Expressions.Visit;

namespace Epic.Linq.Expressions
{
    public sealed class QueryExpression : DomainExpression
    {
        private readonly DomainExpression _domain;
        private readonly PredicateExpression _selection;
        
        public QueryExpression (Type type, string name, DomainExpression domain)
            : base(type, name)
        {
            if(null == domain)
                throw new ArgumentNullException("domain");
            _domain = domain;
        }
        
        private QueryExpression(QueryExpression original, PredicateExpression newPredicate)
            : this(original.Type, original.Name, original._domain)
        {
            if(null == original.Selection)
                _selection = newPredicate;
            else
                _selection = new AndPredicateExpression(original.Selection, newPredicate);
        }
        
        public QueryExpression Select(PredicateExpression predicate)
        {
            if(null == predicate)
                throw new ArgumentNullException("predicate");
            if(! _domain.Equals(predicate.Domain))
                throw new ArgumentException("The predicate's domain does not match the query's one.","predicate");
            return new QueryExpression(this, predicate);
        }
        
        public PredicateExpression Selection
        {
            get
            {
                return _selection;
            }
        }

        #region implemented abstract members of Epic.Linq.Expressions.DomainExpression
        public override bool Equals (DomainExpression other)
        {
            if(object.ReferenceEquals(this, other))
                return true;
            QueryExpression otherQuery = other as QueryExpression;
            if(null == otherQuery)
                return false;
            return _domain.Equals(otherQuery._domain) && _selection.Equals(otherQuery._selection) && Type.Equals(otherQuery.Type);
        }
        #endregion
        
        public override System.Linq.Expressions.Expression Accept (ICompositeVisitor visitor)
        {
            ICompositeVisitor<QueryExpression> queryVisitor = visitor.GetVisitor<QueryExpression>(this);
            return queryVisitor.Visit(this);
        }
    }
}

