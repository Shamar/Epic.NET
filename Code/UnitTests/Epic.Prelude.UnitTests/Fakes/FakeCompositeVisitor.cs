//  
//  FakeCompositeVisitor.cs
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

namespace Epic.Fakes
{
    public class FakeCompositeVisitor<TResult> : CompositeVisitor<TResult>
    {
        public FakeCompositeVisitor (string name)
            : base(name)
        {
        }
    }
    
    public class FakeCompositeVisitor<TResult, TExpression> : CompositeVisitorBase<TResult, TExpression>
        where TExpression : class
    {
        public FakeCompositeVisitor (string name)
            : base(name)
        {
        }

        #region implemented abstract members of Epic.Linq.Expressions.CompositeVisitorBase[TResult,TExpression]
        protected override IVisitContext InitializeVisitContext (TExpression target, IVisitContext context)
        {
            return CallInitializeVisitContext(target, context);
        }
        #endregion
        
        #region templates for tests

        public virtual IVisitContext CallInitializeVisitContext (TExpression target, IVisitContext context)
        {
            return context;
        }
        
        #endregion templates for tests
    }
}

