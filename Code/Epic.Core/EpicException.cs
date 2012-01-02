//  
//  EpicException.cs
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
using System.Runtime.Serialization;

namespace Epic
{
	/// <summary>
	/// Base ancestor for all the infrastructural exceptions in the Epic's ecosystem.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Whenever an Epic's client will catch this exception (or any derived one) 
	/// it will know that the problem occurred was related to the infrastructure.
	/// </para>
	/// <para>
	/// <see cref="EpicException"/> extends <see cref="InvalidOperationException"/>
	/// becouse when the infrastructure has a problem, it's always due to the combined 
	/// state of the infrastructural code itself (where infrastructural is everything 
	/// but the domain model itself).
	/// </para>
	/// </remarks>
	[Serializable]
	public class EpicException : InvalidOperationException
	{
		private static string MessageOrDefault(string message)
		{
			if(string.IsNullOrEmpty(message))
				return "An error occurred in the infrastructure.";
			return message;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Epic.EpicException"/> class.
		/// </summary>
	    public EpicException()
			: this(string.Empty)
	    {
	    }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Epic.EpicException"/> class.
		/// </summary>
		/// <param name='message'>
		/// The message that describes the error. 
		/// </param>
	    public EpicException(string message)
			: this(message, null)
	    {
	    }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Epic.EpicException"/> class.
		/// </summary>
		/// <param name='message'>
		/// The message that describes the error. 
		/// </param>
		/// <param name='inner'>
		/// The exception that is the cause of the current exception, or a null reference if no inner exception is specified. 
		/// </param>
		public EpicException(string message, Exception inner)
			: base(MessageOrDefault(message), inner)
	    {
	    }
	
		/// <summary>
		/// Initializes a new instance of the <see cref="Epic.EpicException"/> class. This constructor is needed for serialization.
		/// </summary>
		/// <param name='info'>
		/// Serialization info.
		/// </param>
		/// <param name='context'>
		/// Streaming context.
		/// </param>
		protected EpicException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}

