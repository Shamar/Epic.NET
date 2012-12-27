//
//  SearchableRepositoryBase.cs
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
using System.Linq;
using Epic.Collections;
using Epic.Specifications;
using Epic.Query.Object.Expressions;
using System.Collections.Generic;
using Epic.Math;

namespace Epic.Query.Object
{
    /// <summary>
    /// Base class for searchable repositories.
    /// </summary>
    [Serializable]
    public abstract class SearchableRepositoryBase<TEntity, TIdentity> : ISearchableRepository<TEntity, TIdentity>, IEntityLoader<TEntity, TIdentity>, IDisposable where TEntity : class where TIdentity : IEquatable<TIdentity>
    {
        private static readonly Action<TIdentity> _checkNull;
        static SearchableRepositoryBase()
        {
            if(typeof(TIdentity).IsValueType)
                _checkNull = i => { };
            else
                _checkNull = i => { if (null == i) { throw new ArgumentNullException("identity"); } };
        }

        private readonly IIdentityMap<TEntity, TIdentity> _identityMap;
        private readonly IEntityLoader<TEntity, TIdentity> _loader;
        private readonly IMapping<TEntity, TIdentity> _identification;
        private readonly string _deferrerName;
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchableRepositoryBase{TEntity, TIdentity}"/> class.
        /// </summary>
        /// <param name='identityMap'>
        /// Identity map.
        /// </param>
        /// <param name='loader'>
        /// Loader of entities.
        /// </param>
        /// <param name='identification'>
        /// Mapping between an entity and its own identifier.
        /// </param>
        /// <param name='deferrerName'>
        /// <see cref="IDeferrer"/> name.
        /// </param>
        protected SearchableRepositoryBase(IIdentityMap<TEntity, TIdentity> identityMap, 
                                           IEntityLoader<TEntity, TIdentity> loader,
                                           IMapping<TEntity, TIdentity> identification,
                                           string deferrerName)
        {
            if(null == identityMap) 
                throw new ArgumentNullException("identityMap");
            if(null == loader) 
                throw new ArgumentNullException("loader");
            if(null == identification)
                throw new ArgumentNullException("identification");
            if(string.IsNullOrEmpty(deferrerName)) 
                throw new ArgumentNullException("deferrerName");
            _identityMap = identityMap;
            _loader = loader;
            _identification = identification;
            _deferrerName = deferrerName;
        }

        /// <summary>
        /// Creates the <see cref="KeyNotFoundException{TIdentity}"/> to throw.
        /// </summary>
        /// <returns>
        /// The exception to throw when no entity is identified by <paramref name="identity"/>.
        /// </returns>
        /// <param name='identity'>
        /// Identity of interest.
        /// </param>
        /// <param name="inner">
        /// Inner exception.
        /// </param>
        /// <remarks>The caller grants that <paramref name="identity"/> is not <see langword="null"/>.</remarks>
        protected virtual KeyNotFoundException<TIdentity> CreateEntityNotFoundException(TIdentity identity, Exception inner)
        {
            string message = string.Format("No {0} is identified by {1}.", typeof(TEntity), identity);
            return new KeyNotFoundException<TIdentity>(identity, message, inner);
        }

        /// <summary>
        /// Identify the specified entity.
        /// </summary>
        /// <param name='entity'>
        /// Entity to identify.
        /// </param>
        /// <returns>
        /// Returns the identifier of <paramref name="entity"/>.
        /// </returns>
        protected TIdentity Identify(TEntity entity)
        {
            return _identification.ApplyTo(entity);
        }

        #region IRepository implementation

        /// <summary>
        /// Returns the <typeparamref name="TEntity"/> identified by the specified identity.
        /// </summary>
        /// <param name='identity'>
        /// Identity of the entity of interest.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="identity"/> is <see langword="null"/></exception>
        public TEntity this[TIdentity identity]
        {
            get
            {
                _checkNull(identity);
                if(!_identityMap.Knows(identity))
                {
                    try
                    {
                        TEntity entity = _loader.Load(identity).Single();
                        _identityMap.Register(Instrument(entity));
                    }
                    catch (Exception e)
                    {
                        throw CreateEntityNotFoundException(identity, e);
                    }
                }
                return _identityMap[identity];
           }
        }
        
        #endregion
        
        #region ISearchableRepository implementation
        
        /// <summary>
        /// Search the <typeparamref name="TSpecializedEntity"/> that satify the specification provided.
        /// </summary>
        /// <param name='satifyingSpecification'>
        /// Specification to satisfy.
        /// </param>
        /// <typeparam name='TSpecializedEntity'>
        /// The type of the entities of interest.
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="satifyingSpecification"/> is <see langword="null"/>.</exception>
        public ISearch<TSpecializedEntity, TIdentity> Search<TSpecializedEntity>(ISpecification<TSpecializedEntity> satifyingSpecification) where TSpecializedEntity : class, TEntity
        {
            IDeferrer deferrer = Enterprise.Environment.Get<IDeferrer>(new InstanceName<IDeferrer>(_deferrerName));
            IRepository<TSpecializedEntity, TIdentity> justThis = this as IRepository<TSpecializedEntity, TIdentity>;
            if(null != justThis)
            {
                var source = new Source<TSpecializedEntity, TIdentity>(justThis);
                var selection = new Selection<TSpecializedEntity>(source, satifyingSpecification);
                return deferrer.Defer<ISearch<TSpecializedEntity, TIdentity>, IEnumerable<TSpecializedEntity>>(selection);
            }
            var abstractSource = new Source<TEntity, TIdentity>(this);
            var concreteSource = new SourceDowncast<TEntity, TSpecializedEntity>(abstractSource);
            var concreteSelection = new Selection<TSpecializedEntity>(concreteSource, satifyingSpecification);
            return deferrer.Defer<ISearch<TSpecializedEntity, TIdentity>, IEnumerable<TSpecializedEntity>>(concreteSelection);
        }

        #endregion

        /// <summary>
        /// Instrument the specified entity. 
        /// </summary>
        /// <remarks>
        /// This method can be used to subscribe
        /// entity's events or to return a proxy wrapping the original entity.
        /// It will be called just befor registering the entity in the identity map.
        /// </remarks>
        /// <param name='entity'>
        /// Entity to instrument.
        /// </param>
        /// <returns>Either <paramref name="entity"/> or an "instrumented" copy.</returns>
        /// <seealso cref="CleanUp"/>
        protected abstract TEntity Instrument(TEntity entity);

        /// <summary>
        /// Cleans up the instrumentation out of <paramref name="entity"/>.
        /// </summary>
        /// <remarks>
        /// This method should revert the operations executed by <see cref="Instrument"/>
        /// </remarks>
        /// <param name='entity'>
        /// Entity instrumented entity.
        /// </param>
        /// <seealso cref="Instrument"/>
        protected abstract void CleanUp(TEntity entity);

        /// <summary>
        /// Releases all resource used by the object derived by <see cref="SearchableRepositoryBase{TEntity,TIdentity}"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <see cref="SearchableRepositoryBase{TEntity,TIdentity}"/> implements <see cref="IDisposable"/>
        /// to allow the explicit disposition of its responsibility.
        /// </para>
        /// <para>
        /// On disposition, all disposable object provided to the constructor will be disposed and all known
        /// entities will be <see cref="CleanUp"/>.
        /// </para>
        /// <para>
        /// This abstract method should be used to clean other resources used by the concrete implementation
        /// of the respository.
        /// </para>
        /// </remarks>
        protected abstract void Dispose();

        #region IEntityLoader<TEntity, TIdentity> implementation

        IEnumerable<TEntity> IEntityLoader<TEntity, TIdentity>.Load(params TIdentity[] identities)
        {
            if(null == identities)
                throw new ArgumentNullException("identities");
            Dictionary<TIdentity, int> toLoad = new Dictionary<TIdentity, int>();
            TEntity[] results = new TEntity[identities.Length];
            for(int i = 0; i < identities.Length; ++i)
            {
                var id = identities[i];
                if(_identityMap.Knows(id))
                {
                    results[i] = _identityMap[id];
                }
                else
                {
                    toLoad[id] = i;
                }
            }
            if(toLoad.Count > 0)
            {
                TIdentity[] ids = toLoad.Keys.ToArray();
                TEntity[] loadedEntities = _loader.Load(ids).ToArray();
                for(int i = 0; i < ids.Length; ++i)
                {
                    var id = ids[i];
                    var e = Instrument(loadedEntities[i]);
                    _identityMap.Register(e);
                    results[toLoad[id]] = e;
                }
            }
            return results;
        }

        #endregion IEntityLoader<TEntity, TIdentity> implementation

        #region IDisposable implementation

        void IDisposable.Dispose()
        {
            if(!_disposed)
            {
                _disposed = true;
                Dispose();
                _identityMap.ForEachKnownEntity(this.CleanUp);
                _identityMap.Dispose();
            }
        }

        #endregion IDisposable implementation
    }
}

