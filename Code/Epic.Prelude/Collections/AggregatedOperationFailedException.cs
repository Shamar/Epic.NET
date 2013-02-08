//
//  AggregatedOperationFailedException.cs
//
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
//
//  Copyright (c) 2010-2013 Giacomo Tesio
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

namespace Epic.Collections
{
    /// <summary>
    /// An operation that should be executed on one or more entities thrown exceptions
    /// on some of them.
    /// </summary>
    /// <seealso cref="IIdentityMap{TEntity, TIdentity}.ForEachKnownEntity"/>
    [Serializable]
    public abstract class AggregatedOperationFailedException : EpicException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Collections.AggregatedOperationFailedException"/> class.
        /// </summary>
        internal AggregatedOperationFailedException()
        {
        }

        internal AggregatedOperationFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// An operation that should be executed on one or more entities identified by <typeparamref name="TIdentity"/>
    /// thrown exceptions on some of them.
    /// </summary>
    /// <typeparam name="TIdentity">Identifier of the entities.</typeparam>
    [Serializable]
    public sealed class AggregatedOperationFailedException<TIdentity> : AggregatedOperationFailedException, IEnumerable<KeyValuePair<TIdentity, Exception>> where TIdentity : IEquatable<TIdentity>
    {
        private readonly IEnumerable<KeyValuePair<TIdentity, Exception>> _exceptions;
        private readonly IEnumerable<TIdentity> _affectedEntities;

        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Collections.AggregatedOperationFailedException"/> class.
        /// </summary>
        public AggregatedOperationFailedException(IEnumerable<KeyValuePair<TIdentity, Exception>> exceptionsOccurred, IEnumerable<TIdentity> affectedEntities)
        {
            _exceptions = exceptionsOccurred;
            _affectedEntities = affectedEntities;
        }

        private AggregatedOperationFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _exceptions = (IEnumerable<KeyValuePair<TIdentity, Exception>>)info.GetValue("E", typeof(IEnumerable<KeyValuePair<TIdentity, Exception>>));
            _affectedEntities = (IEnumerable<TIdentity>)info.GetValue("I", typeof(IEnumerable<TIdentity>));
        }

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">Info.</param>
        /// <param name="context">Context.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("E", _exceptions, typeof(IEnumerable<KeyValuePair<TIdentity, Exception>>));
            info.AddValue("I", _affectedEntities, typeof(IEnumerable<TIdentity>));
        }

        /// <summary>
        /// Enumeraction of the entities that were successfully affected by the operation.
        /// </summary>
        /// <value>The entities successfully affected by the operation.</value>
        public IEnumerable<TIdentity> SuccessfullyAffectedEntities
        {
            get
            {
                return _affectedEntities;
            }
        }

        #region IEnumerable implementation
        /// <summary>
        /// Returns an enumerator that iterates through the exceptions occurred.
        /// </summary>
        /// <returns>
        /// The enumerator that iterates through the <see cref="KeyValuePair{TIdentity, Exception}"/>.
        /// </returns>
        public IEnumerator<KeyValuePair<TIdentity, Exception>> GetEnumerator()
        {
            return _exceptions.GetEnumerator();
        }
        #endregion
        #region IEnumerable implementation
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _exceptions.GetEnumerator();
        }
        #endregion
    }
}

