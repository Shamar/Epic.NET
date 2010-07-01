//  
//  Leg.cs
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
namespace Challenge00.DDDSample.Cargo
{
	[Serializable]
	public class Leg : ILeg
	{
		public Leg ()
		{
		}
	

		#region ILeg implementation
		public Voyage.VoyageNumber Voyage 
		{
			get 
			{
				throw new NotImplementedException ();
			}
		}

		public DateTime LoadTime 
		{
			get 
			{
				throw new NotImplementedException ();
			}
		}

		public Location.UnLocode LoadLocation 
		{
			get 
			{
				throw new NotImplementedException ();
			}
		}

		public DateTime UnloadTime 
		{
			get 
			{
				throw new NotImplementedException ();
			}
		}

		public Location.UnLocode UnloadLocation 
		{
			get 
			{
				throw new NotImplementedException ();
			}
		}
		#endregion

		#region IEquatable[Challenge00.DDDSample.Cargo.ILeg] implementation
		public bool Equals (ILeg other)
		{
			throw new NotImplementedException ();
		}
		#endregion
	}
}

