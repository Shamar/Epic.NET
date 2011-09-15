//  
//  ObserverBase.cs
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
using System.Collections;

namespace Epic
{
    /// <summary>
    /// Base class for entities' observers. 
    /// </summary>
    /// <remarks>
    /// On disposition, all observed entities will be unsubscribed.
    /// </remarks>
    /// <exception cref='ArgumentNullException'>
    /// Is thrown when an argument is null.
    /// </exception>
    public abstract class ObserverBase<TEntity, TIdentifier> : IDisposable
        where TEntity : class
        where TIdentifier : IEquatable<TIdentifier>
    {
        private readonly Hashtable _observed = new Hashtable();
        private readonly Func<TEntity, TIdentifier> _identityReader;
        private bool _disposed = false;
        
        /// <summary>
        /// Initializes a new instance of the observer.
        /// </summary>
        /// <param name='identityReader'>
        /// A delegate able to extract the identifiers of each entity.
        /// </param>
        /// <exception cref='ArgumentNullException'>
        /// Is thrown when <paramref name="identityReader"/> is null.
        /// </exception>
        protected ObserverBase (Func<TEntity, TIdentifier> identityReader)
        {
            if (null == identityReader)
                throw new ArgumentNullException("identityReader");
            _identityReader = identityReader;
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
            TIdentifier identifier = _identityReader(entity);
            if(!_observed.ContainsKey(identifier))
            {
                Subscribe(entity);
                _observed[identifier] = entity;
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
            TIdentifier identifier = _identityReader(entity);
            if(_observed.ContainsKey(identifier))
            {
                Unsubscribe(entity);
                _observed.Remove(identifier);
            }
        }

        #region IDisposable implementation
        /// <summary>
        /// Unsubscribe all subscriptions done.
        /// </summary>
        /// <remarks>
        /// Call <see cref="Dispose"/> when you are finished using the <see cref="Epic.ObserverBase`2"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="Epic.ObserverBase`2"/> in an unusable state. After
        /// calling <see cref="Dispose"/>, you must release all references to the <see cref="Epic.ObserverBase`2"/> so
        /// the garbage collector can reclaim the memory that the <see cref="Epic.ObserverBase`2"/> was occupying.
        /// </remarks>
        public void Dispose ()
        {
            _disposed = true;
            foreach(TEntity entity in _observed.Values)
            {
                Unsubscribe(entity);
                _observed.Clear();
            }
        }
        #endregion
    }
}

