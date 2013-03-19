//  
//  ICustomer.cs
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
using System.Linq;
using Challenge00.DDDSample.Cargo.Handling;
using Challenge00.DDDSample.Cargo;
using System.Collections.Generic;
namespace Challenge00.DDDSample.ACME
{
	/// <summary>
	/// A customer. He can keep tracks of his own cargos.
	/// </summary>
	public interface ICustomer : IUser
	{
		/// <summary>
		/// Cargos shipped by the customer. He can't invoke cargo commands, just query them.
		/// </summary>
		IQueryable<ICargo> Cargos { get; }
		
		/// <summary>
		/// Return the history of a cargo.
		/// </summary>
		/// <param name='cargo'>
		/// The cargo of interest.
		/// </param>
		/// <returns>
		/// History of a cargo as an enumeration of events.
		/// </returns>
		IEnumerable<IEvent> GetHistoryOf(ICargo cargo);
	}
}

