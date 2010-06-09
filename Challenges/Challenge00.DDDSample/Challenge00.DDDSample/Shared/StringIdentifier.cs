using System;
using System.Text.RegularExpressions;
namespace Challenge00.DDDSample
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
            return _identifier.Equals(other._identifier);
        }
		
        #endregion
		
		public static bool operator ==(StringIdentifier<TIdentifier> left, TIdentifier right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StringIdentifier<TIdentifier> left, TIdentifier right)
        {
            return !left.Equals(right);
        }
	}
}

