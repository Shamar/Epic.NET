//
//  AlreadyClaimedException.cs
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

namespace Challenge00.DDDSample.Cargo
{
	/// <summary>
	/// Exception thrown when a cargo is already claimed.
	/// </summary>
	[Serializable]
	public sealed class AlreadyClaimedException : InvalidOperationException
	{
		private readonly TrackingId _cargo;

		/// <summary>
		/// Initializes a new instance of the <see cref="Challenge00.DDDSample.Cargo.AlreadyClaimedException"/> class.
		/// </summary>
		/// <param name='cargo'>
		/// Cargo.
		/// </param>
		/// <param name='message'>
		/// Message.
		/// </param>
		public AlreadyClaimedException (TrackingId cargo, string message)
			: base(message)
		{
			_cargo = cargo;
		}

		/// <summary>
		/// Gets the cargo.
		/// </summary>
		public TrackingId Cargo
		{
			get
			{
				return _cargo;
			}
		}
	}
}

