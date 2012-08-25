//
//  VoyageCompletedException.cs
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

namespace Challenge00.DDDSample.Voyage
{
	/// <summary>
	/// Voyage completed exception.
	/// </summary>
	[Serializable]
	public sealed class VoyageCompletedException : InvalidOperationException
	{
		private readonly VoyageNumber _voyage;
		/// <summary>
		/// Initializes a new instance of the <see cref="Challenge00.DDDSample.Voyage.VoyageCompletedException"/> class.
		/// </summary>
		/// <param name='voyage'>
		/// Voyage.
		/// </param>
		/// <param name='message'>
		/// Message.
		/// </param>
		public VoyageCompletedException (VoyageNumber voyage, string message)
			: base(message)
		{
			_voyage = voyage;
		}

		/// <summary>
		/// Gets the voyage.
		/// </summary>
		public VoyageNumber Voyage
		{
			get
			{
				return _voyage;
			}
		}
	}
}

