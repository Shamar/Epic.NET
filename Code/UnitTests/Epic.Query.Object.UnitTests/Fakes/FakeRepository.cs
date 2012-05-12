//  
//  FakeRepository.cs
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
using Epic.Specifications;


namespace Epic.Query.Object.UnitTests.Fakes
{
    [Serializable]
    public class FakeRepository<TEntity, TIdentity> : ISearchableRepository<TEntity, TIdentity>
        where TIdentity : IEquatable<TIdentity>
        where TEntity : class
    {
        public FakeRepository ()
        {
        }

        #region ISearchableRepository implementation
        public virtual ISearch<TSpecializedEntity, TIdentity> Search<TSpecializedEntity> (ISpecification<TSpecializedEntity> satifyingSpecification) where TSpecializedEntity : class, TEntity
        {
            throw new System.NotImplementedException ();
        }
        #endregion

        #region IRepository implementation
        public virtual TEntity this[TIdentity identity] {
            get {
                throw new System.NotImplementedException ();
            }
        }
        #endregion

        #region ISearchableRepository implementation
        ISearch<TSpecializedEntity, TIdentity> ISearchableRepository<TEntity, TIdentity>.Search<TSpecializedEntity> (ISpecification<TSpecializedEntity> satifyingSpecification)
        {
            throw new System.NotImplementedException ();
        }
        #endregion
    }
}

