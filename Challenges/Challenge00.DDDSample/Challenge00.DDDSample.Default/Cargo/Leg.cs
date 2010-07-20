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
using Challenge00.DDDSample.Voyage;
using Challenge00.DDDSample.Location;
namespace Challenge00.DDDSample.Cargo
{
	[Serializable]
	public class Leg : ILeg
	{
		private readonly VoyageNumber _voyage;
		private readonly DateTime _loadTime;
		private readonly DateTime _unloadTime;
		private readonly UnLocode _loadLocation;
		private readonly UnLocode _unloadLocation;
		public Leg (IVoyage voyage, ILocation loadLocation, DateTime loadTime, ILocation unloadLocation, DateTime unloadTime)
		{
			if(null == voyage)
				throw new ArgumentNullException("voyage");
			if(null == loadLocation)
				throw new ArgumentNullException("loadLocation");
			if(loadTime >= unloadTime)
				throw new ArgumentException("Unload time must follow the load time.","unloadTime");
			if(null == loadLocation)
				throw new ArgumentNullException("loadLocation");
			if(null == unloadLocation)
				throw new ArgumentNullException("unloadLocation");
			if(loadLocation.UnLocode.Equals(unloadLocation.UnLocode))
				throw new ArgumentException("The locations must not be differents.", "unloadLocation");
			if(!voyage.WillStopOverAt(loadLocation))
			{
				string message = string.Format("The voyage {0} will not stop over the load location {1}.", voyage.Number, loadLocation.UnLocode);
				throw new ArgumentException(message, "loadLocation");
			}
			if(!voyage.WillStopOverAt(unloadLocation))
			{
				string message = string.Format("The voyage {0} will not stop over the unload location {1}.", voyage.Number, unloadLocation.UnLocode);
				throw new ArgumentException(message, "unloadLocation");
			}
			
			_voyage = voyage.Number;
			_loadLocation = loadLocation.UnLocode;
			_unloadLocation = unloadLocation.UnLocode;
			_loadTime = loadTime;
			_unloadTime = unloadTime;
		}
	

		#region ILeg implementation
		public VoyageNumber Voyage 
		{
			get 
			{
				return _voyage;
			}
		}

		public DateTime LoadTime 
		{
			get 
			{
				return _loadTime;
			}
		}

		public UnLocode LoadLocation 
		{
			get 
			{
				return _loadLocation;
			}
		}

		public DateTime UnloadTime 
		{
			get 
			{
				return _unloadTime;
			}
		}

		public UnLocode UnloadLocation 
		{
			get 
			{
				return _unloadLocation;
			}
		}
		#endregion

		#region IEquatable[Challenge00.DDDSample.Cargo.ILeg] implementation
		public bool Equals (ILeg other)
		{
			if(null == other)
				return false;
			if(object.ReferenceEquals(this, other))
				return true;
			return _voyage.Equals(other.Voyage) && _loadLocation.Equals(other.LoadLocation) && _unloadLocation.Equals(other.UnloadLocation);
		}
		#endregion
		
		public override bool Equals (object obj)
		{
			return Equals (obj as ILeg);
		}
		
		public override int GetHashCode ()
		{
			return _voyage.GetHashCode() ^ _loadLocation.GetHashCode() ^ _unloadLocation.GetHashCode();
		}
	}
}

