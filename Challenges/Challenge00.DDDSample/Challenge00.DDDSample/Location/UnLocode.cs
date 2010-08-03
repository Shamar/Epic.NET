//  
//  UnLocode.cs
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
using Challenge00.DDDSample.Shared;
namespace Challenge00.DDDSample.Location
{
	/// <summary>
	/// United nations location code.
    /// http://www.unece.org/cefact/locode/ http://www.unece.org/cefact/locode/DocColumnDescription.htm#LOCODE
	/// </summary>
	[Serializable]
	public class UnLocode : StringIdentifier<UnLocode>
	{
        private static readonly Regex _pattern = new Regex("^[a-zA-Z]{2}[a-zA-Z2-9]{3}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		private static string validIdentifier(string identifier)
		{
	        if(string.IsNullOrEmpty(identifier))
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

