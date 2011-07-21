//  
//  EmptyState.cs
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

namespace Epic.Linq.Expressions.Visit
{
    public sealed class VisitState : IVisitState
    {
        public static readonly VisitState New = new VisitState();
        
        private VisitState()
        {
        }
        
        #region IVisitState implementation
        public bool TryGet<TState> (out TState state)
        {
            state = default(TState);
            return false;
        }
        
        public IVisitState Add<TState> (TState state)
        {
            return new State<TState>(this, state);
        }
        
        #endregion
        
        sealed class State<TValue> : IVisitState
        {
            private readonly IVisitState _next;
            private readonly TValue _state;
            
            public State(IVisitState next, TValue state)
            {
                _next = next;
                _state = state;
            }
    
            #region IVisitState implementation
            public bool TryGet<TState> (out TState state)
            {
                State<TState> holder = this as State<TState>;
                if(null != holder)
                {
                    state = holder._state;
                    return true;
                }
                return _next.TryGet<TState>(out state);
            }
    
            public IVisitState Add<TState> (TState state)
            {
                return new State<TState>(this, state);
            }
            #endregion
        }
    }
}

