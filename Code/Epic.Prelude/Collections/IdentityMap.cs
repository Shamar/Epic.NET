//
//  IdentityMap.cs
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
using System.Runtime.Serialization;
using Epic.Math;

namespace Epic.Collections
{
    /// <summary>
    /// Simple identity map.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity of interest.</typeparam>
    /// <typeparam name="TIdentity">Type of the identity of <typeparamref name="TEntity"/>.</typeparam>
    [Serializable]
    public sealed class IdentityMap<TEntity, TIdentity> : IIdentityMap<TEntity, TIdentity>, ISerializable
        where TEntity : class
        where TIdentity : IEquatable<TIdentity>
    {
        private readonly IMapping<TEntity, TIdentity> _mapping;
        private Dictionary<TIdentity, TEntity> _map;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityMap{TEntity, TIdentity}"/> class.
        /// </summary>
        /// <param name='mapping'>
        /// Mapping from instances of <typeparamref name="TEntity"/> and their identities.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="mapping"/> is <see langword="null"/>.</exception>
        public IdentityMap (IMapping<TEntity, TIdentity> mapping)
        {
            if (null == mapping)
                throw new ArgumentNullException ("mapping");
            _mapping = mapping;
            _map = new Dictionary<TIdentity, TEntity>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityMap{TEntity, TIdentity}"/> class.
        /// </summary>
        /// <param name='mapping'>
        /// Mapping from instances of <typeparamref name="TEntity"/> and their identities.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="mapping"/> is <see langword="null"/>.</exception>
        public IdentityMap (Func<TEntity, TIdentity> mapping)
            : this(new FunctionMapping<TEntity, TIdentity>(mapping))
        {
        }

        private void ThrowIfDisposed()
        {
            if(null == _map)
                throw new ObjectDisposedException(string.Format("IdentityMap<{0}, {1}>", typeof(TEntity), typeof(TIdentity)));
        }
           
        #region IMap implementation

        /// <summary>
        /// Gets the <typeparamref name="TEntity"/> with the specified key.
        /// </summary>
        /// <param name='key'>
        /// Key of interest.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        public TEntity this[TIdentity key]
        {
            get
            {
                ThrowIfDisposed();
                TEntity entity;
                try
                {
                    entity = _map[key];
                }
                catch(ArgumentNullException nullExc)
                {
                    throw nullExc;
                }
                catch(KeyNotFoundException keyExc)
                {
                    throw new KeyNotFoundException<TIdentity>(key, keyExc.Message, keyExc);
                }
                return entity;
            }
        }
        #endregion

        #region ISerializable implementation
        private IdentityMap(SerializationInfo info, StreamingContext context)
        {
            _mapping = (IMapping<TEntity, TIdentity>)info.GetValue("F", typeof(IMapping<TEntity, TIdentity>));
            _map = (Dictionary<TIdentity, TEntity>)info.GetValue("M", typeof(Dictionary<TIdentity, TEntity>));
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ThrowIfDisposed();
            info.AddValue("F", _mapping, typeof(IMapping<TEntity, TIdentity>));
            info.AddValue("M", _map, typeof(Dictionary<TIdentity, TEntity>));
        }
        #endregion

        #region IDisposable implementation
        /// <summary>
        /// Releases all resource used by the <see cref="Epic.Collections.IdentityMap{TKey,TValue}"/> object.
        /// </summary>
        /// <remarks>
        /// The different repositories must call <see cref="Dispose"/> when they are disposed.
        /// </remarks>
        public void Dispose ()
        {
            Dictionary<TIdentity, TEntity> map = _map;
            if(null != map)
                map.Clear();
            _map = null;
        }
        #endregion

        #region IIdentityMap implementation
        /// <summary>
        /// Register in the map the specified entity.
        /// </summary>
        /// <param name='entity'>
        /// Entity to register.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="entity"/> has already been registered.</exception>
        public void Register (TEntity entity)
        {
            ThrowIfDisposed();  
            if(null == entity)
                throw new ArgumentNullException("entity");
            TIdentity identity = _mapping.ApplyTo(entity);
            if(_map.ContainsKey(identity))
            {
                string message = string.Format("The {0} identified by '{1}' is already registered.", typeof(TEntity), typeof(TIdentity));
                throw new ArgumentException(message, "entity");
            }
            _map[identity] = entity;
        }

        /// <summary>
        /// Determines if the map already knows the specified entity.
        /// </summary>
        /// <param name='entity'>
        /// Identity of the entity of interest.
        /// </param>
        /// <returns><c>true</c>if the map knows the specified entity, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <see langword="null"/>.</exception>
        public bool Knows(TIdentity entity)
        {
            ThrowIfDisposed();
            if(null == entity)
                throw new ArgumentNullException("entity");
            return _map.ContainsKey(entity);
        }
        #endregion
    }
}

