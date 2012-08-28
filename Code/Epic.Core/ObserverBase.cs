//  
//  ObserverBase.cs
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
using System.Collections.Generic;

namespace Epic
{
    /// <summary>
    /// Base class for entities' observers. 
    /// </summary>
    /// <remarks>
    /// On disposition, all observed entities will be unsubscribed.
    /// </remarks>
    /// <typeparam name="TEntity">
    /// Type of the observed entities.
    /// </typeparam>
    /// <exception cref='ArgumentNullException'>
    /// Is thrown when an argument is null.
    /// </exception>
    public abstract class ObserverBase<TEntity> : IDisposable
        where TEntity : class
    {
        private readonly HashSet<TEntity> _observed = new HashSet<TEntity>();
        private bool _disposed = false;
        
        /// <summary>
        /// Initializes a new instance of the observer.
        /// </summary>
        protected ObserverBase ()
        {
        }
        
        /// <summary>
        /// Subscribe the interesting events of the specified <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name='entity'>
        /// The <typeparamref name="TEntity"/> to observe.
        /// </param>
        protected abstract void Subscribe(TEntity entity);
        
        /// <summary>
        /// Unsubscribe the subscribed events of the specified <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name='entity'>
        /// The <typeparamref name="TEntity"/> to ignore.
        /// </param>
        protected abstract void Unsubscribe(TEntity entity);
        
        /// <summary>
        /// Begin to observe the specified <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name='entity'>
        /// The <typeparamref name="TEntity"/> to observe.
        /// </param>
        /// <exception cref='ArgumentNullException'>
        /// Is thrown when <paramref name="entity"/> is null.
        /// </exception>
        /// <exception cref='ObjectDisposedException'>
        /// Is thrown after the observer has been disposed.
        /// </exception>
        public void Observe(TEntity entity)
        {
            if(_disposed)
                throw new ObjectDisposedException(this.GetType().Name);
            if(null == entity)
                throw new ArgumentNullException("entity");
            if(!_observed.Contains(entity))
            {
                Subscribe(entity);
                _observed.Add(entity);
            }
        }
        
        /// <summary>
        /// Stop observing the specified <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name='entity'>
        /// The <typeparamref name="TEntity"/> to observe.
        /// </param>
        /// <exception cref='ArgumentNullException'>
        /// Is thrown when <paramref name="entity"/> is null.
        /// </exception>
        /// <exception cref='ObjectDisposedException'>
        /// Is thrown after the observer has been disposed.
        /// </exception>
        public void Ignore(TEntity entity)
        {
            if(_disposed)
                throw new ObjectDisposedException(this.GetType().Name);
            if(null == entity)
                throw new ArgumentNullException("entity");
            if(_observed.Contains(entity))
            {
                Unsubscribe(entity);
                _observed.Remove(entity);
            }
        }

        #region IDisposable implementation
        /// <summary>
        /// Unsubscribe all subscriptions done.
        /// </summary>
        /// <remarks>
        /// Call <see cref="Dispose"/> when you are finished using the <see cref="Epic.ObserverBase{TEntity}"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="Epic.ObserverBase{TEntity}"/> in an unusable state. After
        /// calling <see cref="Dispose"/>, you must release all references to the <see cref="Epic.ObserverBase{TEntity}"/> so
        /// the garbage collector can reclaim the memory that the <see cref="Epic.ObserverBase{TEntity}"/> was occupying.
        /// </remarks>
        public void Dispose ()
        {
            _disposed = true;
            foreach(TEntity entity in _observed)
            {
                Unsubscribe(entity);
            }
            _observed.Clear();
        }
        #endregion
    }
}

