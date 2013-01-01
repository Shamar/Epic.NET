//  
//  ChangeEventArgs.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2013 Giacomo Tesio
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
namespace Epic.Events
{
	/// <summary>
	/// Change event argument. Indicates the current and the old value.
	/// </summary>
	[Serializable]
	public sealed class ChangeEventArgs<TValue> : EventArgs
	{
		/// <summary>
		/// The old value.
		/// </summary>
		public readonly TValue OldValue;
		
		/// <summary>
		/// The new value.
		/// </summary>
		public readonly TValue NewValue;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Epic.Events.ChangeEventArgs{TValue}"/> class.
		/// </summary>
		/// <param name='oldValue'>
		/// Old value.
		/// </param>
		/// <param name='newValue'>
		/// New value.
		/// </param>
		public ChangeEventArgs (TValue oldValue, TValue newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}

