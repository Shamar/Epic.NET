using System;
using System.Text.RegularExpressions;
namespace Challenge00.DDDSample.Location
{
	/// <summary>
	/// United nations location code.
    /// http://www.unece.org/cefact/locode/ http://www.unece.org/cefact/locode/DocColumnDescription.htm#LOCODE
	/// </summary>
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
		
		/// <summary>
		/// Constructor 
		/// </summary>
		/// <param name="identifier">
		/// A <see cref="System.String"/>
		/// </param>
		public UnLocode (string identifier)
			: base(validIdentifier(identifier))
		{
		}
	}
}

