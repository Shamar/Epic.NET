//
//  EntitiesExistence.cs
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

namespace Epic.Query
{
    /// <summary>
    /// Knowledge about entities existence.
    /// </summary>
    /// <typeparam name="TIdentity">Type of the identity.</typeparam>
    [Serializable]
    public sealed class EntitiesExistence<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        private static readonly Action<TIdentity> _checkNull;
        static EntitiesExistence()
        {
            if(typeof(TIdentity).IsValueType)
                _checkNull = i => { };
            else
                _checkNull = i => { if (null == i) throw new ArgumentNullException("identity"); };
        }
        private readonly TIdentity[] _existings;
        private readonly TIdentity[] _notExistings;
        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Query.EntitiesExistence{TIdentity}"/> class.
        /// </summary>
        /// <param name='existingEntities'>
        /// Existing entities.
        /// </param>
        /// <param name='notExistingEntities'>
        /// Not existing entities.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="existingEntities"/> or <paramref name="notExistingEntities"/> 
        /// is <see langword="null"/>.</exception>
        public EntitiesExistence(TIdentity[] existingEntities, TIdentity[] notExistingEntities)
        {
            if (null == existingEntities)
                throw new ArgumentNullException("existingEntities");
            if (null == notExistingEntities)
                throw new ArgumentNullException("notExistingEntities");
            _existings = existingEntities;
            _notExistings = notExistingEntities;
        }

        /// <summary>
        /// Determines if the specified identities identifies an existing entity.
        /// </summary>
        /// <param name='identity'>
        /// Identity of interest.
        /// </param>
        /// <exception cref="EpicException">No information is known about the existence of an entity identified by <paramref name="identity"/>.</exception>
        public bool Exists(TIdentity identity)
        {
            _checkNull(identity);
            for(int i = 0; i < _existings.Length; ++i)
                if (identity.Equals(_existings[i]))
                    return true;
            for(int i = 0; i < _notExistings.Length; ++i)
                if (identity.Equals(_notExistings[i]))
                    return false;
            string message = string.Format("No information is known about the existence of an entity identified by '{0}'.", identity);
            throw new EpicException(message);
        }
    }
}

