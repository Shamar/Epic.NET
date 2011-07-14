//  
//  PredicateExpression.cs
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
    public abstract class PredicateExpression : VisitableExpression, IEquatable<PredicateExpression>
    {
        private readonly DomainExpression _domain;
        public PredicateExpression (DomainExpression domain)
            : base(typeof(bool))
        {
            if(null == domain)
                throw new ArgumentNullException("domain");
            _domain = domain;
        }
        
        public DomainExpression Domain
        {
            get
            {
                return _domain;
            }
        }

        #region IEquatable[PredicateExpression] implementation
        
        public abstract bool Equals (PredicateExpression other);
        
        #endregion
        
        public override bool Equals (object obj)
        {
            return this.Equals(obj as PredicateExpression);
        }
        
        public override int GetHashCode ()
        {
            return this.GetType().GetHashCode() ^ _domain.GetHashCode();
        }
    }
}

