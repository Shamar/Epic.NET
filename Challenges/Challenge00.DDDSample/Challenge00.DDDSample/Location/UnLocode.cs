using System;
using System.Text.RegularExpressions;
namespace Challenge00.DDDSample
{
	[Serializable]
	public class UnLocode : StringIdentifier<UnLocode>
	{
        private static readonly Regex _pattern = new Regex("[a-zA-Z]{2}[a-zA-Z2-9]{3}", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		private static string validIdentifier(string identifier)
		{
	        if(identifier == null)
	        {
	            throw new ArgumentNullException("identifier");
	        }
	        if (!_pattern.Match(identifier).Success)
	        {
	            throw new ArgumentException(string.Format("Provided identifier does not comply with a UnLocode pattern ({0})",_pattern), "identifier");
			}
			return identifier;
		}
		
		public UnLocode (string identifier)
			: base(validIdentifier(identifier))
		{
		}
	}
}

