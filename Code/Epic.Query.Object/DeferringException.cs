//  
//  DeferringException.cs
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

namespace Epic.Query.Object
{
    /// <summary>
    /// Exception thrown when a request to <see cref="IDeferrer.Defer{TDeferred, TResult}"/> is invalid becouse of the type arguments.
    /// </summary>
    /// <remarks>
    /// The <see cref="DeferringException"/> is designed to be cought only: to throw it all <see cref="IDeferrer"/> implementation
    /// must create an instance of <see cref="DeferringException{TDeferred, TResult}"/> instead.
    /// </remarks>
    /// <seealso cref="DeferringException{TDeferred, TResult}"/>
    [Serializable]
    public abstract class DeferringException : EpicException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeferringException"/> class.
        /// </summary>
        /// <param name='message'>
        /// The message that describes the error. 
        /// </param>
        internal DeferringException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeferringException"/> class.
        /// </summary>
        /// <param name='message'>
        /// The message that describes the error. 
        /// </param>
        /// <param name='inner'>
        /// The exception that is the cause of the current exception. 
        /// If the <paramref name="inner"/> parameter is not a <see langword="null"/> reference, the current 
        /// exception is raised in a catch block that handles the inner exception. 
        /// </param>
        internal DeferringException(string message, Exception inner)
            : base(message, inner)
        {
        }
    
        /// <summary>
        /// Initializes a new instance of the <see cref="DeferringException"/> class. This constructor is needed for serialization.
        /// </summary>
        /// <param name='info'>
        /// Serialization info.
        /// </param>
        /// <param name='context'>
        /// Streaming context.
        /// </param>
        internal DeferringException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        
        /// <summary>
        /// The type of the <see cref="IDeferred{TResult}"/>.
        /// </summary>
        public abstract Type DeferredType { get; }
        
        
        /// <summary>
        /// The type of the expected result of the <see cref="IDeferred{TResult}"/>.
        /// </summary>
        public abstract Type ResultType { get; }
    }
    
    /// <summary>
    /// Exception thrown when a request to <see cref="IDeferrer.Defer{TDeferred, TResult}"/> is invalid becouse of the type arguments.
    /// </summary>
    [Serializable] 
    public sealed class DeferringException<TDeferred, TResult> : DeferringException
        where TDeferred : IDeferred<TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeferringException{TDeferred, TResult}"/> class.
        /// </summary>
        /// <param name='message'>
        /// Message.
        /// </param>
        public DeferringException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeferringException"/> class.
        /// </summary>
        /// <param name='message'>
        /// The message that describes the error. 
        /// </param>
        /// <param name='inner'>
        /// The exception that is the cause of the current exception. 
        /// If the <paramref name="inner"/> parameter is not a <see langword="null"/> reference, the current 
        /// exception is raised in a catch block that handles the inner exception. 
        /// </param>
        public DeferringException(string message, Exception inner)
            : base(message, inner)
        {
        }

        private DeferringException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        
        #region implemented abstract members of Epic.Query.Object.UnknownDeferredException
        /// <summary>
        /// The type of the <see cref="IDeferred{TResult}"/>.
        /// </summary>
        public override Type DeferredType {
            get {
                return typeof(TDeferred);
            }
        }
        
        /// <summary>
        /// The type of the expected result of the <see cref="IDeferred{TResult}"/>.
        /// </summary>
        public override Type ResultType {
            get {
                return typeof(TResult);
            }
        }
        #endregion
    }
}

