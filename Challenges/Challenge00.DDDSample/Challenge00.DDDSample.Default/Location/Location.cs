//  
//  Location.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2011 Giacomo Tesio
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
namespace Challenge00.DDDSample.Location
{
	[Serializable]
	public class Location : ILocation
	{
		private	readonly UnLocode _identifier;
		private readonly string _name;
		
		public Location (UnLocode identifier, string name)
		{
			if(null == identifier)
				throw new ArgumentNullException("identifier");
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			_identifier = identifier;
			_name = name;
		}

		#region ILocation implementation
		public string Name 
		{
			get 
			{
				return _name;
			}
		}

		public UnLocode UnLocode 
		{
			get 
			{
				return _identifier;
			}
		}
		#endregion
	}
}

