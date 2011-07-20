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
    public sealed class EmptyState : IVisitState
    {
        #region IVisitState implementation
        public bool TryGetInstance<TState> (out TState state) where TState : class
        {
            state = null;
            return false;
        }

        public bool TryGetValue<TState> (out TState state) where TState : struct
        {
            state = default(TState);
            return false;
        }
        
        public IVisitState AddInstance<TState> (TState state) where TState : class
        {
            return new InstanceState<TState>(this, state);
        }
        
        public IVisitState AddValue<TState> (TState state) where TState : struct
        {
            return new ValueState<TState>(this, state);
        }
        #endregion
    }

    internal sealed class ValueState<TStoredState> : IVisitState
        where TStoredState : struct
    {
        private readonly IVisitState _next;
        private readonly TStoredState _state;

        public ValueState(IVisitState next, TStoredState state)
        {
            _next = next;
            _state = state;
        }

        #region IVisitState implementation
        public bool TryGetInstance<TState> (out TState state) where TState : class
        {
            return _next.TryGetInstance<TState>(out state);
        }

        public bool TryGetValue<TState> (out TState state) where TState : struct
        {
            if(typeof(TState).IsAssignableFrom(typeof(TStoredState)))
            {
                state = (TState)(object)_state;
                return true;
            }
            return _next.TryGetValue<TState>(out state);
        }
        
        public IVisitState AddInstance<TState> (TState state) where TState : class
        {
            return new InstanceState<TState>(this, state);
        }
        
        public IVisitState AddValue<TState> (TState state) where TState : struct
        {
            return new ValueState<TState>(this, state);
        }
        
        #endregion
    }

    
    internal sealed class InstanceState<TStoredState> : IVisitState
        where TStoredState : class
    {
        private readonly IVisitState _next;
        private readonly TStoredState _state;

        public InstanceState(IVisitState next, TStoredState state)
        {
            _next = next;
            _state = state;
        }

        #region IVisitState implementation
        public bool TryGetInstance<TState> (out TState state) where TState : class
        {
            state = _state as TState;
            if(null != state)
                return true;
            return _next.TryGetInstance<TState>(out state);
        }
  
        public bool TryGetValue<TState> (out TState state) where TState : struct
        {
            return _next.TryGetValue<TState>(out state);
        }
        
        public IVisitState AddInstance<TState> (TState state) where TState : class
        {
            return new InstanceState<TState>(this, state);
        }
        
        public IVisitState AddValue<TState> (TState state) where TState : struct
        {
            return new ValueState<TState>(this, state);
        }
        
        #endregion
    }
}

