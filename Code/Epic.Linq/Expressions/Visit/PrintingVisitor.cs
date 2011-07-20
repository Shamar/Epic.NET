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
        
        public PrintingVisitor (CompositeVisitorChain chain)
            : base(chain)
        {
        }
        
        protected override ICompositeVisitor<TExpression> AsVisitor<TExpression> (TExpression target, IVisitState state)
        {
            if(typeof(TExpression).Equals(typeof(Expression)))
                return null;
            Callstack stack;
            bool stackFound = state.TryGet<Callstack>(out stack);
            if(stackFound)
            {
                if(target == stack.Top)
                    return null;
            }
            return new VisitorWrapper<TExpression>(this, this.Display<TExpression>);
        }
        
        private Expression Display<TExpression>(TExpression target, IVisitState state) where TExpression : Expression
        {
            Callstack depth;
            if(!state.TryGet<Callstack>(out depth))
            {
                depth = Callstack.New(target);
                state = state.Add(depth.Next(target));
            }
            for(int i = 0; i < depth.Size; ++i)
            {
                Console.Write("    ");
            }
            Console.WriteLine("{0} - {1}", target.NodeType, target.GetType().FullName);
            if(!object.ReferenceEquals(target, depth.Top))
            {
                depth = depth.Next(target);
                state = state.Add(depth);
            }
            var visitor = GetNextVisitor(target, state);
            if(null == visitor)
                return target;
            Expression visited = visitor.Visit(target, state);
            return visited;
        }
        
        class Callstack
        {
            public readonly int Size;
            public readonly Expression Top;
            
            private Callstack(int previous, Expression top)
            {
                Size = previous + 1;
                Top = top;
            }
            
            private Callstack(Expression top)
            {
                Size = 0;
                Top = top;
            }
            
            public static Callstack New(Expression top)
            {
                return new Callstack(top);
            }
   
            public Callstack Next(Expression e)
            {
                return new Callstack(this.Size, e);
            }
        }

    }
}

