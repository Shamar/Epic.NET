//  
//  Expression.cs
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
using System.Runtime.Serialization;
using System.Security.Permissions;


namespace Epic.Query.Object.Expressions
{
    /// <summary>
    /// Expression that represent a <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TValue">
    /// Type of the value that the expression represent.
    /// </typeparam>
    [Serializable]
    public abstract class Expression<TValue> : VisitableBase, ISerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Expression{TValue}"/> class.
        /// </summary>
        protected Expression ()
            : base()
        {
        }

        #region ISerializable implementation

        /// <summary>
        /// Initializes a new instance of the <see cref="Expression{TValue}"/> class after a deserialization.
        /// </summary>
        /// <param name='info'>
        /// Serialization informations.
        /// </param>
        /// <param name='context'>
        /// Streaming context.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="info"/> is <see langword="null"/>.</exception>
        protected Expression (SerializationInfo info, StreamingContext context)
            : this()
        {
            if(null == info)
                throw new ArgumentNullException("info");
        }

        /// <summary>
        /// Register into <paramref name="info"/> the data to be stored in the serialization.
        /// </summary>
        /// <param name='info'>
        /// Serialization informations (will never be <see langword="null"/>).
        /// </param>
        /// <param name='context'>
        /// Streaming context.
        /// </param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected abstract void GetObjectData (SerializationInfo info, StreamingContext context);

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
        {
            if(null == info)
                throw new ArgumentNullException("info");
            GetObjectData(info, context);
        }

        #endregion ISerializable implementation
    }
}

