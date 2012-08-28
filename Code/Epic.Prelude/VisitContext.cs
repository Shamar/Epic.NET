//  
//  VisitContext.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2012 Giacomo Tesio
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

namespace Epic
{
    /// <summary>
    /// Visit context.
    /// </summary>
    /// <exception cref='InvalidOperationException'>
    /// Is thrown when a state can not be found.
    /// </exception>
    public sealed class VisitContext : IVisitContext
    {
        /// <summary>
        /// A new empty context to be filled during each visit.
        /// </summary>
        public static readonly VisitContext New = new VisitContext();
        
        /// <summary>
        /// Private constructor.
        /// </summary>
        private VisitContext ()
        {
        }

        #region IVisitContext implementation
        /// <summary>
        /// Returns the <typeparamref name="TState"/>.
        /// </summary>
        /// <typeparam name='TState'>
        /// Type of the state of interest.
        /// </typeparam>
        /// <returns>
        /// Value of the <typeparamref name="TState"/> that was registered from previous visitors in the context.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown when the <typeparamref name="TState"/> has not been found in the context.</exception>
        public TState Get<TState> ()
        {
            throw new InvalidOperationException ("Can not find any " + typeof(TState).FullName + " in the current context.");
        }

        /// <summary>
        /// Tries to find the <typeparamref name="TState"/> in the context.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> when the <typeparamref name="TState"/> has been found, <see langword="false"/> otherwise.
        /// </returns>
        /// <param name='state'>
        /// <typeparamref name="TState"/> to be used during the visit.
        /// </param>
        /// <typeparam name='TState'>
        /// Type of the state needed.
        /// </typeparam>
        public bool TryGet<TState> (out TState state)
        {
            state = default(TState);
            return false;
        }

        /// <summary>
        /// Create a new context containing the specified <paramref name="state"/>.
        /// </summary>
        /// <param name='state'>
        /// State to use in the visits recieving the resulting context.
        /// </param>
        /// <returns>
        /// A new context containing the <paramref name="state"/>.
        /// </returns>
        /// <typeparam name='TState'>
        /// Type of the state to use.
        /// </typeparam>
        public IVisitContext With<TState> (TState state)
        {
            return new State<TState>(this, state);
        }
        #endregion
        
        /// <summary>
        /// State of a context.
        /// </summary>
        /// <exception cref='InvalidOperationException'>
        /// Is thrown when an operation cannot be performed.
        /// </exception>
        private sealed class State<TValue> : IVisitContext
        {
            private readonly IVisitContext _next;
            private readonly TValue _state;
            
            /// <summary>
            /// Initializes a new instance of the <see cref="Epic.VisitContext.State{TValue}"/> class.
            /// </summary>
            /// <param name='next'>
            /// Next context to be looked for states.
            /// </param>
            /// <param name='state'>
            /// Value of the state hold by this instance.
            /// </param>
            public State(IVisitContext next, TValue state)
            {
                _next = next;
                _state = state;
            }
    
            #region IVisitState implementation
            /// <summary>
            /// Returns the <typeparamref name="TState"/>.
            /// </summary>
            /// <typeparam name='TState'>
            /// Type of the state of interest.
            /// </typeparam>
            /// <returns>
            /// Value of the <typeparamref name="TState"/> that was registered from previous visitors in the context.
            /// </returns>
            /// <exception cref="InvalidOperationException">Thrown when the <typeparamref name="TState"/> has not been found in the context.</exception>
            public TState Get<TState> ()
            {
                TState state = default(TState);
                if(!TryGet<TState>(out state))
                    throw new InvalidOperationException ("Can not find any " + typeof(TState).FullName + " in the current context.");
                return state;
            }
            
            /// <summary>
            /// Tries to find the <typeparamref name="TState"/> in the context.
            /// </summary>
            /// <returns>
            /// <see langword="true"/> when the <typeparamref name="TState"/> has been found, <see langword="false"/> otherwise.
            /// </returns>
            /// <param name='state'>
            /// <typeparamref name="TState"/> to be used during the visit.
            /// </param>
            /// <typeparam name='TState'>
            /// Type of the state needed.
            /// </typeparam>
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
    
            /// <summary>
            /// Return a new context containing the specified <paramref name="state"/>.
            /// </summary>
            /// <param name='state'>
            /// State to use in the visits recieving the resulting context.
            /// </param>
            /// <typeparam name='TState'>
            /// Type of the state to use.
            /// </typeparam>
            public IVisitContext With<TState> (TState state)
            {
                return new State<TState>(this, state);
            }
            #endregion
        }

    }
}

