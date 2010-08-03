//  
//  StringIdentifier.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010 Giacomo Tesio
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
using System.Text.RegularExpressions;
namespace Challenge00.DDDSample.Shared
{
	[Serializable]
	public abstract class StringIdentifier
	{
        protected readonly string _identifier;

		protected StringIdentifier (string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
                throw new ArgumentNullException("identifier");
            _identifier = identifier;
        }

        public override string ToString()
        {
            return _identifier;
        }
		
		public bool Contains(string value)
		{
			return _identifier.Contains(value);
		}
		
		public bool EndsWith(string value)
		{
			return _identifier.EndsWith(value);
		}
		
		public bool StartsWith(string value)
		{
			return _identifier.StartsWith(value);
		}
		
		public bool Matches(Regex expression)
		{
			return expression.IsMatch(_identifier);
		}
	}
	
	[Serializable]
	public abstract class StringIdentifier<TIdentifier> : StringIdentifier, IEquatable<TIdentifier>
		where TIdentifier : StringIdentifier<TIdentifier>
	{
		protected StringIdentifier(string identifier)
			: base(identifier)
		{
		}
		
        public override bool Equals(object obj)
        {
            TIdentifier other = obj as TIdentifier;
            return Equals(other);
        }
		
		public override int GetHashCode()
        {
            return _identifier.GetHashCode();
        }
		
        #region IEquatable<TrackingId> Members

        public bool Equals(TIdentifier other)
        {
            if (object.ReferenceEquals(other, null))
                return false;
			if (object.ReferenceEquals(this, other))
                return true;
            return _identifier.Equals(other._identifier);
        }
		
        #endregion
		
		public static bool operator ==(StringIdentifier<TIdentifier> left, TIdentifier right)
        {
			if(object.ReferenceEquals(left, null))
			{
				if(object.ReferenceEquals(right, null))
					return true; // both null
				else
					return false; // one null
			}
            return left.Equals(right);
        }

        public static bool operator !=(StringIdentifier<TIdentifier> left, TIdentifier right)
        {
			if(object.ReferenceEquals(left, null))
			{
				if(object.ReferenceEquals(right, null))
					return false; // both null
				else
					return true; // one null
			}
            return !left.Equals(right);
        }
	}
}

