//  
//  InstanceName.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2012 Giacomo Tesio
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
{
	/// <summary>
	/// Name to identify any instance of a given type (when the type has no identity
	/// by itself, like an int, a logger or a db connection).
	/// It could be used as a key on a <see cref="System.Collections.Hashtable"/>.
	/// </summary>
	/// <typeparam name="TObject">
	/// Type of the instance to identify.
	/// </typeparam>
	[Serializable]
	public sealed class InstanceName<TObject> : IEquatable<InstanceName<TObject>>
	{
		private readonly string _name;
		[NonSerialized]
		private string _string;
		
		/// <summary>
		/// Constuctor.
		/// </summary>
		/// <param name="name">
		/// The name to use for identify the <typeparamref name="TObject"/>.
		/// </param>
		public InstanceName (string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			_name = name;
		}
		
		/// <summary>
		/// Provide a string containing the type and the name separated by a colon.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> containing the type and the name separated by a colon.
		/// </returns>
		public override string ToString ()
		{
			if(null == _string)
				_string = string.Format ("{0}:{1}", typeof(TObject).FullName, _name);
			return _string;
		}
		
		#region IEquatable[InstanceName[TObject]] implementation
		/// <summary>
		/// Indicates whether the <paramref name="other"/> is semantically equals to this.
		/// </summary>
		/// <param name="other">
		/// A <see cref="InstanceName{TObject}"/>.
		/// </param>
		/// <returns>
		/// <see langword="true"/> when <paramref name="other"/> is semantically equals 
		/// to this, <see langword="false"/> otherwise.
		/// </returns>
		public bool Equals (InstanceName<TObject> other)
		{
			if(object.ReferenceEquals(null, other))
				return false;
			if(object.ReferenceEquals(this, other))
				return true;
			return _name.Equals(other._name);
		}
		#endregion
		
		/// <summary>
		/// Indicates whether <paramref name="obj"/> is a <see cref="InstanceName{TObject}"/>
		/// semantically equal to this.
		/// </summary>
		/// <param name="obj">
		/// A <see cref="System.Object"/>.
		/// </param>
		/// <returns>
		/// <see langword="true"/> when <paramref name="obj"/> is <see cref="InstanceName{TObject}"/> 
		/// semantically equal to this, <see langword="false"/> otherwise.
		/// </returns>
		public override bool Equals (object obj)
		{
			return Equals (obj as InstanceName<TObject>);
		}
		
		/// <summary>
		/// A 32-bit signed integer hash code.
		/// </summary>
		/// <returns>
		/// A hash code.
		/// </returns>
		public override int GetHashCode ()
		{
			return _name.GetHashCode();
		}
	}
}

