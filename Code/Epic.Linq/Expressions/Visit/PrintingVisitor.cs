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
    public sealed class PrintingVisitor : VisitorsComposition.VisitorBase
    {
        public PrintingVisitor (VisitorsComposition chain)
            : base(chain)
        {
        }
        
        protected internal override ICompositeVisitor<TExpression> AsVisitor<TExpression> (TExpression target)
        {
            if(typeof(TExpression).Equals(typeof(Expression)))
                return null;
            return new VisitorWrapper<TExpression>(this, this.Display<TExpression>);
        }
        
        private Expression Display<TExpression>(TExpression target, IVisitState state) where TExpression : Expression
        {
            Callstack depth;
            if(!state.TryGet<Callstack>(out depth))
            {
                depth = Callstack.New;          // The current depth is 0
            }
            for(int i = 0; i < depth.Size; ++i)
            {
                Console.Write("    ");
            }
            Console.WriteLine("{0} - {1}", target.NodeType, target.GetType().FullName);
            
            state = state.Add(depth.Next());      // add the printed expression to the state's stack
            
            ICompositeVisitor<TExpression> visitor = GetNextVisitor(target);
            Expression visited = visitor.Visit(target, state);
            return visited;
        }
        
        struct Callstack
        {
            public readonly int Size;
            
            private Callstack(int previous)
            {
                Size = previous + 1;
            }
            
            public static readonly Callstack New = new Callstack();
   
            public Callstack Next()
            {
                return new Callstack(this.Size);
            }
        }

    }
}

