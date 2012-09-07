//  
//  HandlingEventArgs.cs
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
using Challenge00.DDDSample.Location;
namespace Challenge00.DDDSample.Cargo
{
	/// <summary>
	/// Event related to cargo handling.
	/// </summary>
	[Serializable]
	public sealed class HandlingEventArgs : EventArgs
	{
		/// <summary>
		/// New delivery status of the cargo
		/// </summary>
		public readonly IDelivery Delivery;
		
		/// <summary>
		/// Completion date 
		/// </summary>
		public readonly DateTime CompletionDate;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="delivery">
		/// The new cargo <see cref="IDelivery"/> status. Will not be null.
		/// </param>
		/// <param name="completionDate">
		/// The completion <see cref="DateTime"/>
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="delivery"/> is <see langword="null"/>.</exception>
		public HandlingEventArgs (IDelivery delivery, DateTime completionDate)
		{
			if (null == delivery)
				throw new ArgumentNullException ("delivery");
			Delivery = delivery;
			CompletionDate = completionDate;
		}
	}
}

