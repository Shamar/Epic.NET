//
//  WrongLocationException.cs
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

namespace Challenge00.DDDSample.Location
{
	/// <summary>
	/// Wrong location exception.
	/// </summary>
	[Serializable]
	public sealed class WrongLocationException : ArgumentException
	{
		private readonly UnLocode _expected;
		private readonly UnLocode _actual;
		/// <summary>
		/// Initializes a new instance of the <see cref="Challenge00.DDDSample.WrongLocationException"/> class.
		/// </summary>
		/// <param name="paramName">The name of the parameter that caused the current exception. </param>
		/// <param name="message">The error message that explains the reason for the exception. </param>
		/// <param name="expectedLocation">The expected location.</param>
		/// <param name="actualLocation">The actual location.</param>
		public WrongLocationException (string paramName, string message, UnLocode expectedLocation, UnLocode actualLocation)
			: base(message, paramName)
		{
			_actual = actualLocation;
			_expected = expectedLocation;
		}

		/// <summary>
		/// Gets the expected location.
		/// </summary>
		public UnLocode ExpectedLocation
		{
			get
			{
				return _expected;
			}
		}

		/// <summary>
		/// Gets the actual location.
		/// </summary>
		public UnLocode ActualLocation
		{
			get
			{
				return _actual;
			}
		}
	}
}

