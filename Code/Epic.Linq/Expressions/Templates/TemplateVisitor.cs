//  
//  TemplateVisitor.cs
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
using System.Linq.Expressions;

namespace Epic.Linq.Expressions.Templates
{
    public sealed class TemplateVisitor : VisitorsComposition.VisitorBase, ICompositeVisitor<MethodCallExpression>
    {
        public TemplateVisitor (VisitorsComposition composition)
            : base(composition)
        {
        }
        
        internal protected override ICompositeVisitor<TExpression> AsVisitor<TExpression> (TExpression target)
        {
            ICompositeVisitor<TExpression> visitor = base.AsVisitor<TExpression>(target);
            if(null == visitor || !IsQueryAccess(target as MethodCallExpression))
                visitor = new VisitorWrapper<TExpression>(this, this.Parse<TExpression>);
            return visitor;
        }
        
        private static bool IsQueryAccess(MethodCallExpression call)
        {
            // the argument "call" can't be null, no need to check
            
            if(null == call.Object)
                return false;
            return typeof(IQuery).Equals(call.Object.Type);
        }
        
        private Expression Parse<TExpression>(TExpression target, IVisitState state) where TExpression : Expression
        {
            return target;
        }

        #region ICompositeVisitor[MethodCallExpression] implementation
        public Expression Visit (MethodCallExpression target, IVisitState state)
        {
            // read argument[0]
            // create the key for the extractor
            // generate extractor from howToGetHere (that should be compiled)
            // store extractor in _extractors
           
            return target;
        }
        #endregion
    }
}

