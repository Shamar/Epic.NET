//  
//  VisitorWrapper.cs
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

namespace Epic.Linq.Expressions.Visit
{
    public class VisitorWrapper<TExpression> : ICompositeVisitor<TExpression>
        where TExpression : Expression
    {
        private readonly ICompositeVisitor _wrapped;
        private readonly Func<TExpression, Expression> _visit;
        public VisitorWrapper (ICompositeVisitor wrappedVisitor, Func<TExpression, Expression> visit)
        {
            if(null == wrappedVisitor)
                throw new ArgumentNullException("wrappedVisitor");
            if(null == visit)
                throw new ArgumentNullException("visit");
            if(!object.ReferenceEquals(wrappedVisitor, visit.Target) || visit.Target == null)
                throw new ArgumentException("The visit must be either static or a method of the wrapped visitor.");
            _wrapped = wrappedVisitor;
            _visit = visit;
        }

        #region ICompositeVisitor[TExpression] implementation
        public System.Linq.Expressions.Expression Visit (TExpression target)
        {
            return _visit(target);
        }
        #endregion

        #region ICompositeVisitor implementation
        public ICompositeVisitor<TRequiredExpression> GetVisitor<TRequiredExpression> (TRequiredExpression target) where TRequiredExpression : System.Linq.Expressions.Expression
        {
            return _wrapped.GetVisitor<TRequiredExpression>(target);
        }
        #endregion
    }
}

