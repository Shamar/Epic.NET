//  
//  DeferringException.cs
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
using System.Runtime.Serialization;
using Epic.Query.Object.Expressions;

namespace Epic.Query.Object
{
    /// <summary>
    /// Exception thrown when a request to <see cref="IDeferrer.Evaluate{TResult}"/> fails.
    /// </summary>
    /// <seealso cref="DeferredEvaluationException{TResult}"/>
    [Serializable]
    public abstract class DeferredEvaluationException : EpicException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeferringException"/> class.
        /// </summary>
        /// <param name='message'>
        /// The message that describes the error. 
        /// </param>
        internal DeferredEvaluationException(string message)
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
        internal DeferredEvaluationException(string message, Exception inner)
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
        internal DeferredEvaluationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        
        /// <summary>
        /// The type of the expected result of the <see cref="IDeferred{TResult}"/>.
        /// </summary>
        public abstract Type ResultType { get; }

        /// <summary>
        /// Gets the unevaluated expression.
        /// </summary>
        /// <value>The unevaluated expression.</value>
        public Expression UnevaluatedExpression { get { return GetUnevaluatedExpression(); } }

        internal abstract Expression GetUnevaluatedExpression();
    }
    
    /// <summary>
    /// Exception thrown when a request to <see cref="IDeferrer.Evaluate{TResult}"/> fails.
    /// </summary>
    /// <typeparam name="TResult">Type of the desired result of the evaluation that failed.</typeparam>
    [Serializable] 
    public sealed class DeferredEvaluationException<TResult> : DeferredEvaluationException
    {
        private readonly Expression<TResult> _unevaluatedExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeferringException{TDeferred, TResult}"/> class.
        /// </summary>
        /// <param name="unevaluatedExpression">Expression that the <see cref="IDeferrer"/> was unable to evaluate.</param>
        /// <param name='message'>
        /// Message.
        /// </param>
        internal DeferredEvaluationException(Expression<TResult> unevaluatedExpression, string message)
            : base(message)
        {
            _unevaluatedExpression = unevaluatedExpression;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeferringException"/> class.
        /// </summary>
        /// <param name="unevaluatedExpression">Expression that the <see cref="IDeferrer"/> was unable to evaluate.</param>
        /// <param name='message'>
        /// The message that describes the error. 
        /// </param>
        /// <param name='inner'>
        /// The exception that is the cause of the current exception. 
        /// If the <paramref name="inner"/> parameter is not a <see langword="null"/> reference, the current 
        /// exception is raised in a catch block that handles the inner exception. 
        /// </param>
        internal DeferredEvaluationException(Expression<TResult> unevaluatedExpression, string message, Exception inner)
            : base(message, inner)
        {
        }

        private DeferredEvaluationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        
        #region implemented abstract members of Epic.Query.Object.UnknownDeferredException

        /// <summary>
        /// The type of the expected result of the <see cref="IDeferred{TResult}"/>.
        /// </summary>
        public override Type ResultType {
            get {
                return typeof(TResult);
            }
        }
        #endregion

        internal override Expression GetUnevaluatedExpression()
        {
            return _unevaluatedExpression;
        }

        /// <summary>
        /// Gets the unevaluated expression.
        /// </summary>
        /// <value>The unevaluated expression.</value>
        public new Expression<TResult> UnevaluatedExpression
        {
            get
            {
                return _unevaluatedExpression;
            }
        }
    }
}

