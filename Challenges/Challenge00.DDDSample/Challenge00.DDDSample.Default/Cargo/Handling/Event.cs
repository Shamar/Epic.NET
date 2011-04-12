//  
//  Event.cs
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
namespace Challenge00.DDDSample.Cargo.Handling
{
	[Serializable]
	public class Event : IEvent
	{
		private readonly EventType _type;
		private readonly Username _user;
		private readonly TrackingId _cargo;
		private readonly DateTime _date;
		private readonly Location.UnLocode _location;
		private readonly Voyage.VoyageNumber _voyage;
		
		protected Event (IUser user, ICargo cargo, EventType type, DateTime date)
		{
			if(null == user)
				throw new ArgumentNullException("user");
			if(null == cargo)
				throw new ArgumentNullException("cargo");
			
			_user = user.Username;
			_date = date;
			_cargo = cargo.TrackingId;
			_location = cargo.Delivery.LastKnownLocation;
			_voyage = cargo.Delivery.CurrentVoyage;
			_type = type;
		}

		#region IEvent implementation
		
		public Username User {
			get {
				return _user;
			}
		}
		
		public TrackingId Cargo {
			get {
				return _cargo;
			}
		}
		
		public DateTime Date {
			get {
				return _date;
			}
		}

		public EventType Type {
			get {
				return _type;
			}
		}

		public Location.UnLocode Location {
			get {
				return _location;
			}
		}

		public Voyage.VoyageNumber Voyage {
			get {
				return _voyage;
			}
		}
		
		#endregion
}
}

