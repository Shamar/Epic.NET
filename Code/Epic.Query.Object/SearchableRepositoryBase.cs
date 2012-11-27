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
using Epic.Collections;
using Epic.Specifications;
using System.Linq;

namespace Epic.Query.Object
{
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
        
        #region IRepository implementation

        public TEntity this[TIdentity identity]
        {
            get
            {
                _checkNull(identity);
                if(!_identityMap.Knows(identity))
                {
                    if(!_loader.Exist(identity))
                    {
                        string message = string.Format("No {0} is identified by {1}.", typeof(TEntity), identity));
                        throw new KeyNotFoundException<TIdentity>(identity, message);
                    }
                    _identityMap.Register(_loader.Load(identity).First());
                }
                return _identityMap[identity];
           }
        }
        
        #endregion
        
        #region ISearchableRepository implementation
        
        public ISearch<TSpecializedEntity, TIdentity> Search<TSpecializedEntity>(ISpecification<TSpecializedEntity> satifyingSpecification) where TSpecializedEntity : class, TEntity
        {
            IDeferrer deferrer = Enterprise.Environment.Get<IDeferrer>(new InstanceName<IDeferrer>(name));
            IRepository<TSpecializedEntity, TIdentity> justThis = this as IRepository<TSpecializedEntity, TIdentity>;
            if(null != justThis)
            {
                var source = new Expressions.Source<TSpecializedEntity, TIdentity>(justThis);
                var selection = new Expressions.Selection<TSpecializedEntity>(source, specification);
                return deferrer.Defer<ISearch<TSpecializedEntity, TIdentity>>(selection);
            }
            var abstractSource = new Expressions.Source<TEntity, TIdentity>(this);
            var concreteSource = new Expressions.SourceDowncast<TEntity, TSpecializedEntity>(abstractSource);
            var concreteSelection = new Expressions.Selection<TSpecializedEntity>(concreteSource, specification);
            return deferrer.Defer<ISearch<TSpecializedEntity, TIdentity>>(concreteSelection);
        }

        #endregion
    }
}

