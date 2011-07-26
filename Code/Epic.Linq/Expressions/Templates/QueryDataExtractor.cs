//  
//  QueryDataExtractor.cs
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

namespace Epic.Linq.Expressions.Templates
{
    public sealed class QueryDataExtractor<TExpression>
        where TExpression : Expression
    {
        private readonly List<Func<QueryData, TExpression, bool>> _registrations = new List<Func<QueryData, TExpression, bool>>();
        
        public QueryDataExtractor ()
        {
        }
        
        public IQuery Parse(TExpression expression)
        {
            QueryData data = new QueryData();
            foreach(Func<QueryData, TExpression, bool> register in _registrations)
                if(!register(data, expression))
                    return null;
            return data;
        }
        
        public void Register<TerminalExpression>(string name, Func<TExpression, TerminalExpression> visit)
            where TerminalExpression : Expression
        {
            _registrations.Add((d, e) => Register<TerminalExpression>(name, visit, d, e));
        }
        
        private static bool Register<TerminalExpression>(string name, Func<TExpression, TerminalExpression> visit, QueryData data, TExpression expressionToVisit)
            where TerminalExpression : Expression
        {
            TerminalExpression exp = visit(expressionToVisit);
            if(visit == null)
                return false;
            LambdaExpression expression = Expression.Lambda(typeof(Func<>).MakeGenericType(exp.Type), exp);
            return data.Register(name, expression.Compile());
        }
        
        class QueryData : IQuery
        {
            private readonly Dictionary<string, Delegate> _dataProviders = new Dictionary<string, Delegate>();
            
            #region IQuery implementation
            TValue IQuery.Get<TValue> (string name)
            {
                if(string.IsNullOrEmpty(name))
                    throw new ArgumentException("name");
                Delegate del = _dataProviders[name];
                Func<TValue> getValue = (Func<TValue>)del;
                return getValue();
            }
            #endregion
            
            internal bool Register(string name, Delegate valueProvider)
            {
                if(_dataProviders.ContainsKey(name))
                    return false;
                _dataProviders[name] = valueProvider;
                return true;
            }
        }
    }
}

