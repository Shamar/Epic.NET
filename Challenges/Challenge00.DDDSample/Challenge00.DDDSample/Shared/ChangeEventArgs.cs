//  
//  ChangeEventArgs.cs
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
namespace Challenge00.DDDSample
{
	/// <summary>
	/// Generic event argument that hold a status change.
	/// </summary>
	[Serializable]
	public sealed class ChangeEventArgs<TChanged> : EventArgs
	{
		/// <summary>
		/// Old <typeparamref cref="TChanged"/> value.
		/// </summary>
		public readonly TChanged OldValue;
		
		/// <summary>
		/// New <typeparamref cref="TChanged"/> value.
		/// </summary>
		public readonly TChanged NewValue;

		/// <summary>
		/// Constructor 
		/// </summary>
		/// <param name="oldValue">
		/// The old <see cref="TChanged"/>
		/// </param>
		/// <param name="newValue">
		/// The new <see cref="TChanged"/>
		/// </param>
		public ChangeEventArgs (TChanged oldValue, TChanged newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}

