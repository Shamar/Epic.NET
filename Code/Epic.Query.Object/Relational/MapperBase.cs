//
//  MapperBase.cs
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
using Epic.Math;
using System.Collections.Generic;
using Epic.Query.Relational;
using Epic.Specifications;
using Epic.Query.Relational.Predicates;

namespace Epic.Query.Object.Relational
{
    public abstract class MapperBase<TEntity, TIdentity> : IMapping<TEntity, TIdentity>, 
                                                           IEntityLoader<TEntity, TIdentity>
        where TEntity : class
        where TIdentity : IEquatable<TIdentity>
    {
        private static MapperBase<TEntity, TIdentity> _instance;

        private readonly Relation _mainRelation;
        protected MapperBase (Relation mainRelation)
        {
            if (null == mainRelation)
                throw new ArgumentNullException("mainRelation");
            _mainRelation = mainRelation;
            _instance = this;
        }

        #region IMapping implementation
        TIdentity IMapping<TEntity, TIdentity>.ApplyTo (TEntity element)
        {
            if(null == element)
                throw new ArgumentNullException("element");
            return Identify(element);
        }
        #endregion

        #region IEntityLoader implementation
        IEnumerable<TEntity> IEntityLoader<TEntity, TIdentity>.Load (params TIdentity[] identities)
        {
            if(null == identities || identities.Length == 0)
                throw new ArgumentNullException("identities");
            return Load(identities);
        }
        #endregion
        
        protected abstract TIdentity Identify(TEntity entity);
        
        protected abstract TEntity[] Load (TIdentity[] identities);
        
        protected void Map<TSpecification>(Relation relationToJoin, Func<TSpecification, Predicate> predicateBuilder)
            where TSpecification : class, ISpecification<TEntity>
        {
        }
    }
}

