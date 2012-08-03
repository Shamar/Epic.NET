//
//  KeyNotFoundException.cs
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

namespace Epic.Collections
{
    /// <summary>
    /// The exception that is thrown when the key specified for 
    /// accessing an element in a <see cref="IMap{TKey,TValue}"/> does not match any 
    /// key in the map.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the map.</typeparam>
    [Serializable]
    public sealed class KeyNotFoundException<TKey> : KeyNotFoundException
        where TKey : IEquatable<TKey>
    {
        private readonly TKey _key;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyNotFoundException{Key}"/> class
        /// with the specified error message and a reference to the inner exception that is 
        /// the cause of this exception.
        /// </summary>
        /// <param name='notFoundKey'>
        /// Not found key.
        /// </param>
        /// <param name='message'>
        /// The message that describes the error.
        /// </param>
        public KeyNotFoundException(TKey notFoundKey, string message)
            : base(message)
        {
            _key = notFoundKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyNotFoundException{Key}"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name='notFoundKey'>
        /// Not found key.
        /// </param>
        /// <param name='message'>
        /// The message that describes the error.
        /// </param>
        /// <param name='innerException'>
        /// Inner exception.
        /// </param>
        public KeyNotFoundException(TKey notFoundKey, string message, Exception innerException)
            : base(message, innerException)
        {
            _key = notFoundKey;
        }

        private KeyNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _key = (TKey)info.GetValue("K", typeof(TKey));
        }

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name='info'>
        /// Info.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        public override void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("K", _key, typeof(TKey));
        }

        /// <summary>
        /// Gets the missing key.
        /// </summary>
        public TKey Key
        {
            get
            {
                return _key;
            }
        }
    }
}

