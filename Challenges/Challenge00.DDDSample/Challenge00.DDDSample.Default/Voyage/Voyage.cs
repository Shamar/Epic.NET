//  
//  Voyage.cs
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
namespace Challenge00.DDDSample.Voyage
{
	[Serializable]
	public class Voyage : IVoyage
	{
		private readonly VoyageNumber _identifier;
		private readonly ISchedule _schedule;
		private Voyage _state;
		public Voyage (VoyageNumber identifier, ISchedule schedule)
			: this(identifier, schedule, null)
		{
		}
	
		protected Voyage (VoyageNumber identifier, ISchedule schedule, VoyageState initialState)
		{
			_identifier = identifier;
			_schedule = schedule;
		}

		#region IVoyage implementation
		public event EventHandler<VoyageEventArgs> Arrived;

		public event EventHandler<VoyageEventArgs> Departed;

		public event EventHandler<VoyageEventArgs> Losing;

		public void Arrive ()
		{
			throw new NotImplementedException ();
		}

		public void Depart ()
		{
			throw new NotImplementedException ();
		}

		public void MarkAsLost (Location.ILocation lastKnownLocation)
		{
			throw new NotImplementedException ();
		}

		public VoyageNumber Number {
			get {
				return _identifier;
			}
		}

		public ISchedule Schedule {
			get {
				return _schedule;
			}
		}

		public Location.UnLocode LastKnownLocation {
			get {
				throw new NotImplementedException ();
			}
		}

		public bool IsMoving {
			get {
				throw new NotImplementedException ();
			}
		}

		public bool IsLost {
			get {
				throw new NotImplementedException ();
			}
		}
		#endregion

		#region IEquatable[Challenge00.DDDSample.Voyage.IVoyage] implementation
		public bool Equals (IVoyage other)
		{
			throw new NotImplementedException ();
		}
		#endregion
}
}

