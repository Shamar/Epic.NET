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

namespace Epic.Query.Object
{
    /// <summary>
    /// Base class for searchable repositories.
    /// </summary>
    [Serializable]
    public class SearchableRepositoryBase<TEntity, TIdentity> : ISearchableRepository<TEntity, TIdentity>, IRepository<TEntity, TIdentity> where TEntity : class where TIdentity : IEquatable<TIdentity>
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
        private readonly string _deferrerName;

        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Query.Object.SearchableRepositoryBase{TEntity, TIdentity}"/> class.
        /// </summary>
        /// <param name='identityMap'>
        /// Identity map.
        /// </param>
        /// <param name='loader'>
        /// Loader.
        /// </param>
        /// <param name='deferrerName'>
        /// <see cref="IDeferrer"/> name.
        /// </param>
        public SearchableRepositoryBase(IIdentityMap<TEntity, TIdentity> identityMap, IEntityLoader<TEntity, TIdentity> loader, string deferrerName)
        {
            if(null == identityMap) 
                throw new ArgumentNullException("identityMap");
            if(null == loader) 
                throw new ArgumentNullException("loader");
            if(string.IsNullOrEmpty(deferrerName)) 
                throw new ArgumentNullException("deferrerName");
            _identityMap = identityMap;
            _loader = loader;
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
        /// <remarks>The caller grants that <paramref name="identity"/> is not <see langword="null"/>.</remarks>
        protected virtual KeyNotFoundException<TIdentity> CreateEntityNotFoundException(TIdentity identity)
        {
            string message = string.Format("No {0} is identified by {1}.", typeof(TEntity), identity);
            return new KeyNotFoundException<TIdentity>(identity, message);
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
                    if(!_loader.Exist(identity).Exists(identity))
                    {
                        throw CreateEntityNotFoundException(identity);
                    }
                    _identityMap.Register(_loader.Load(identity).First());
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
    }
}

