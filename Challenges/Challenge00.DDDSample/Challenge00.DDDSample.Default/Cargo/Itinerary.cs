//  
//  Itinerary.cs
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
using System.Collections.Generic;
namespace Challenge00.DDDSample.Cargo
{
	[Serializable]
	public sealed class Itinerary : IItinerary
	{
		private readonly ILeg[] _legs;
		
		public Itinerary ()
			: this(new ILeg[0] { })
		{
		}
		
		private Itinerary (ILeg[] legs)
		{
			if(null == legs)
				throw new ArgumentNullException("legs");
			_legs = legs;
		}

		#region IItinerary implementation
		public IItinerary Append (ILeg leg)
		{
			if(null == leg)
				throw new ArgumentNullException("leg");
			if(_legs.Length > 0)
			{
				if(leg.LoadLocation != _legs[_legs.Length - 1].UnloadLocation)
					throw new ArgumentException("Invalid load location.", "leg");
				if(leg.LoadTime < _legs[_legs.Length - 1].UnloadTime)
					throw new ArgumentException("Invalid load time.", "leg");
			}
			ILeg[] newLegs = new ILeg[_legs.Length + 1];
			Array.Copy(_legs, newLegs, _legs.Length);
			newLegs[_legs.Length] = leg;
			return new Itinerary(newLegs);
		}

		public IItinerary Replace (ILeg fromLeg, ILeg toLeg, IEnumerable<ILeg> legs)
		{
			throw new NotImplementedException ();
		}

		public Location.UnLocode InitialDepartureLocation 
		{
			get 
			{
				if(_legs.Length == 0)
					return null;
				throw new NotImplementedException ();
			}
		}

		public Location.UnLocode FinalArrivalLocation 
		{
			get 
			{
				if(_legs.Length == 0)
					return null;
				throw new NotImplementedException ();
			}
		}
		#endregion

		#region IEnumerable[Challenge00.DDDSample.Cargo.ILeg] implementation
		public IEnumerator<ILeg> GetEnumerator ()
		{
			throw new NotImplementedException ();
		}
		#endregion

		#region IEnumerable[Challenge00.DDDSample.Cargo.ILeg] implementation
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
		#endregion
		
		#region IEquatable[IItinerary] implementation
		public bool Equals(IItinerary other)
		{
			throw new NotImplementedException ();
		}
		#endregion
		
		
	}
}

