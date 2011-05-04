//  
//  InstanceName.cs
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
namespace Epic
{/*
	[Serializable]
	public sealed class InstanceName<TObject> : IEquatable<InstanceName<TObject>>
	{
		private readonly string _name;
		[NonSerialized]
		private string _string;
		
		public InstanceName (string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			_name = name;
		}
		
		public override string ToString ()
		{
			if(null == _string)
				_string = string.Format ("{0} named \"{1}\".", typeof(TObject).FullName, _name);
			return _string;
		}
		
		#region IEquatable[InstanceName[TObject]] implementation
		public bool Equals (InstanceName<TObject> other)
		{
			if(object.ReferenceEquals(null, other))
				return false;
			if(object.ReferenceEquals(this, other))
				return true;
			return _name.Equals(other._name);
		}
		#endregion
		
		public override bool Equals (object obj)
		{
			return Equals (obj as InstanceName<TObject>);
		}
		
		public override int GetHashCode ()
		{
			return _name.GetHashCode();
		}
	}*/
}

