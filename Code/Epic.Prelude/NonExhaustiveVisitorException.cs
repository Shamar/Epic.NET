//
//  NonExhaustiveVisitorException.cs
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

namespace Epic
{
    /// <summary>
    /// Exception thrown when a visitor composition is can not visit an expression that it's intended to visit.
    /// </summary>
    [Serializable]
    public abstract class NonExhaustiveVisitorException : EpicException
    {
        internal NonExhaustiveVisitorException(string message)
            : base(message)
        {
        }

        internal NonExhaustiveVisitorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Type of the expression that the composition can not handle.
        /// </summary>
        public abstract Type ExpressionType { get; }

        /// <summary>
        /// Name of the visitor composition.
        /// </summary>
        public abstract string VisitorCompositionName { get; }
    }

    /// <summary>
    /// Exception thrown when a visitor composition can not handle an expression that it is intended to visit.
    /// </summary>
    /// <seealso cref="IVisitor{TResult, TExpression}.Visit"/>
    [Serializable]
    public sealed class NonExhaustiveVisitorException<TExpression> : NonExhaustiveVisitorException
        where TExpression : class
    {
        private readonly string _compositionName;
        private readonly TExpression _expression;
        internal NonExhaustiveVisitorException(string compositionName, TExpression expression)
            : base(string.Format("No visitor available for the expression {0} (of type: {2}) in the composition '{1}'.", expression, compositionName, typeof(TExpression)))
        {
            _compositionName = compositionName;
            _expression = expression;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonExhaustiveVisitorException{TExpression}"/> class.
        /// </summary>
        /// <param name="compositionName">Composition name.</param>
        /// <param name="expression">Unexpected expression.</param>
        /// <param name="message">Message.</param>
        public NonExhaustiveVisitorException(string compositionName, TExpression expression, string message)
            : base(message)
        {
            _compositionName = compositionName;
            _expression = expression;
        }

        private NonExhaustiveVisitorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _compositionName = info.GetString("C");
            _expression = (TExpression)info.GetValue("E",typeof(TExpression));
        }

        /// <summary>
        /// Sets the <see cref="SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("C", _compositionName);
            info.AddValue("E", _expression, typeof(TExpression));
        }

        /// <summary>
        /// Type of the expression that the composition can not handle.
        /// </summary>
        public override Type ExpressionType
        {
            get
            {
                return typeof(TExpression);
            }
        }

        /// <summary>
        /// Name of the visitor composition.
        /// </summary>
        public override string VisitorCompositionName
        {
            get
            {
                return _compositionName;
            }
        }

        /// <summary>
        /// Expression that the composition can not visit.
        /// </summary>
        public TExpression UnknownExpression
        {
            get
            {
                return _expression;
            }
        }
    }
}

