//
//  EquatableException.cs
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
    [Serializable]
    public class EquatableException : Exception
    {
        public EquatableException(string message)
            : base(message)
        {
        }

        private EquatableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override bool Equals(object obj)
        {
            EquatableException e = obj as EquatableException;
            if(null == e)
                return false;
            return e.Message.Equals(Message);
        }
        public override int GetHashCode()
        {
            return Message.GetHashCode();
        }
    }
}

