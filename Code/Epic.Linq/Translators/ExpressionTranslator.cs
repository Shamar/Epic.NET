//  
//  ExpressionTranslator.cs
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
using Epic.Linq.Expressions;

namespace Epic.Linq.Translators
{
    public interface IExpressionTranslator
    {
        bool TryTranslationOf(Expression expression, out QueryExpression builtExpression);
    }
    public abstract class ExpressionTranslator<TQueryExpression> : IExpressionTranslator
        where TQueryExpression : QueryExpression
    {
        public bool TryTranslationOf(Expression expression, out QueryExpression builtExpression)
        {
            foreach(Expression template in GetMatchingExpressions(null))
            {
                IQuery query = null;
                if(TryMatchTemplate(expression, template, out query))
                {
                    builtExpression = Translate(query);
                    return true;
                }
            }
            builtExpression = null;
            return false;
        }
        
        private bool TryMatchTemplate(Expression expression, Expression template, out IQuery query)
        {

        }
        
        protected abstract IEnumerable<Expression> GetMatchingExpressions(IQuery query);
        
        protected abstract TQueryExpression Translate(IQuery query);
    }
}

