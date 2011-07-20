//  
//  CompositeVisitorChain.cs
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
using System.Collections.Generic;

namespace Epic.Linq.Expressions.Visit
{
    public class CompositeVisitorChain : ICompositeVisitor
    {
        private readonly ICompositeVisitor _end;
        private List<ICompositeVisitor> _visitors;
        
        public CompositeVisitorChain (ICompositeVisitor chainEnd)
        {
            if(null == chainEnd)
                throw new ArgumentNullException("chainEnd");
            _end = chainEnd;
        }
        
        private IList<ICompositeVisitor> Visitors
        {
            get
            {
                if(null == _visitors)
                {
                    _visitors = new List<ICompositeVisitor>();
                    InitializeVisitors();
                }
                return _visitors;
            }
        }
        
        protected virtual void InitializeVisitors()
        {
        }
        
        internal void Append(ICompositeVisitor visitor)
        {
            if(null == visitor)
                throw new ArgumentNullException("visitor");
            Visitors.Add(visitor);
        }

        #region ICompositeVisitor implementation
        public ICompositeVisitor<TExpression> GetVisitor<TExpression> (TExpression expression, IVisitState state) where TExpression : System.Linq.Expressions.Expression
        {
            Callstack stack;
            if(!state.TryGet<Callstack>(out stack))
            {
                stack = Callstack.New;
            }

            ICompositeVisitor<TExpression> visitor;
            if(stack.Size == Visitors.Count)
            {
                // last visitor called;
                visitor = _end.GetVisitor<TExpression>(expression, state.Add(stack.Next()));
            }
            else
            {
                ICompositeVisitor next = Visitors[stack.Size];
                visitor = next.GetVisitor<TExpression> (expression, state.Add(stack.Next()));
            }
            return visitor;
        }
        #endregion
        
        class Callstack
        {
            public readonly int Size;
            
            private Callstack(Callstack previous)
            {
                Size = previous.Size + 1;
            }
            
            private Callstack()
            {
                Size = 0;
            }
            
            public static Callstack New = new Callstack();
   
            public Callstack Next()
            {
                return new Callstack(this);
            }
        }
    }
}

