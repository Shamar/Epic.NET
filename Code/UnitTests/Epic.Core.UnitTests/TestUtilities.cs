//  
//  Utilities.cs
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
{
	public static class TestUtilities
	{
		/// <summary>
		/// Reset the application (to be used in TestFixtures' setups.
		/// </summary>
		public static void ResetApplication()
		{
			Application.Reset();
		}
		
		/// <summary>
		/// Serialize an object.
		/// </summary>
		/// <typeparamref name="TObject">
		/// Type of the object to serialize.
		/// </typeparamref>
		/// <param name="target">
		/// A <see cref="TObject"/> to serialize
		/// </param>
		/// <returns>
		/// A <see cref="System.IO.Stream"/> containing a serialization of <paramref name="target"/>.
		/// </returns>
        public static System.IO.Stream Serialize<TObject>(TObject target)
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter serializer =
            new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.MemoryStream memStream = new System.IO.MemoryStream();
            serializer.Serialize(memStream, target);
            return memStream;
        }
		
		/// <summary>
		/// Derialize a <typeparamref name="TObject"> instance from <paramref name="memStream"/>.
		/// </summary>
		/// <typeparamref name="TObject">
		/// Type of the object to deserialize.
		/// </typeparamref>
		/// <param name="memStream">
		/// A <see cref="System.IO.Stream"/> containing a serialization of <paramref name="target"/>.
		/// </param>
		/// <returns>
		/// A <see cref="TObject"/> instance.
		/// </returns>
        public static TObject Deserialize<TObject>(System.IO.Stream memStream)
        {
            memStream.Position = 0;
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter deserializer =
            new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            object newobj = deserializer.Deserialize(memStream);
            memStream.Close();
            return (TObject)newobj;
        }
	}
}

