//  
//  PrintingVisitor.cs
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

namespace Epic.Linq.Expressions.Visit
{
    public class PrintingVisitor : CompositeVisitorBase
    {
        private readonly Stack<Expression> _stack;
        
        public PrintingVisitor (CompositeVisitorChain chain)
            : base(chain)
        {
            _stack = new Stack<Expression>();
        }
        
        protected override ICompositeVisitor<TExpression> AsVisitor<TExpression> (TExpression target)
        {
            if(typeof(TExpression).Equals(typeof(Expression)))
                return null;
            if(_stack.Count > 0 && object.ReferenceEquals(target, _stack.Peek()))
            {
                return null;
            }
            return new VisitorWrapper<TExpression>(this, this.Display<TExpression>);
        }
        
        private Expression Display<TExpression>(TExpression target) where TExpression : Expression
        {
            for(int i = 0; i < _stack.Count; ++i)
            {
                Console.Write("    ");
            }
            Console.WriteLine("{0} - {1}", target.NodeType, target.GetType().FullName);
            _stack.Push(target);
            var visitor = GetVisitor(target);
            if(null == visitor)
                return target;
            Expression visited = visitor.Visit(target);
            _stack.Pop();
            return visited;
        }
    }
}

