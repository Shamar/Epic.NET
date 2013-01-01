//
//  MissingValuesException.cs
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

namespace Epic.Extended.Delegates
{
    /// <summary>
    /// Exception thrown when the values provided for a <see cref="Delegate"/> are not enough.
    /// </summary>
    [Serializable]
    public sealed class MissingValuesException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingValuesException"/> class.
        /// </summary>
        /// <param name='paramName'>
        /// Parameter name.
        /// </param>
        /// <param name='message'>
        /// Message.
        /// </param>
        public MissingValuesException (string paramName, string message)
            : base(message, paramName)
        {
        }

        private MissingValuesException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

