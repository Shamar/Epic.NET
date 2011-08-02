//  
//  StringExpression.cs
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

namespace Epic.Linq.Expressions
{
    public sealed class SqlExpression : VisitableExpression
    {
        private static object[] GetArguments(SqlExpression[] arguments)
        {
            if(null == arguments)
                return null;
            object[] results = new object[arguments.Length];
            for(int i = 0; i < arguments.Length; ++i)
            {
                results[i] = arguments[i].Expression;
            }
            return results;
        }
        
        private readonly string _expression;
        
        public SqlExpression (string format)
            : this(format, new object[0])
        {
        }
        
        public SqlExpression (string format, params object[] arguments)
            : base(ExpressionType.Sql, typeof(string))
        {
            if(string.IsNullOrEmpty(format))
                throw new ArgumentNullException("format");
            _expression = string.Format(format, arguments);
        }
        
        public SqlExpression (string format, params SqlExpression[] arguments)
            : this(format, GetArguments(arguments))
        {
        }

        #region implemented abstract members of Epic.Linq.Expressions.VisitableExpression
        public override Expression Accept (ICompositeVisitor visitor, IVisitState state)
        {
            return AcceptAs<SqlExpression>(visitor, state);
        }
        #endregion
            
        public string Expression
        {
            get
            {
                return _expression;
            }
        }
    }
}

