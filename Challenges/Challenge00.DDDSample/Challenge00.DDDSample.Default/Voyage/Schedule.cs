//  
//  Schedule.cs
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
using Challenge00.DDDSample.Voyage;
using System.Collections.Generic;
namespace Challenge00.DDDSample.Voyage
{
	[Serializable]
	public sealed class Schedule : ISchedule
	{
		private readonly ICarrierMovement[] _movements;
		public Schedule ()
		{
			_movements = new ICarrierMovement[0];
		}
	
		private Schedule (ICarrierMovement[] movements)
		{
			_movements = movements;
		}

		#region ISchedule implementation
		public ISchedule Append (ICarrierMovement movement)
		{
			if(null == movement)
				throw new ArgumentNullException("movement");
			if(_movements.Length == 0)
				return new Schedule(new ICarrierMovement[1] { movement });
			
			ICarrierMovement lastMovement = _movements[_movements.Length - 1];
			if(!lastMovement.ArrivalLocation.Equals(movement.DepartureLocation))
				throw new Location.WrongLocationException("movement", string.Format("The provided movement do not depart from {0}.", lastMovement.ArrivalLocation), movement.DepartureLocation, lastMovement.ArrivalLocation);
			if(lastMovement.ArrivalTime > movement.DepartureTime)
				throw new ArgumentException(string.Format("The provided movement depart from {0} too early.", lastMovement.ArrivalLocation), "movement");
			
			ICarrierMovement[] movements = new ICarrierMovement[_movements.Length + 1];
			_movements.CopyTo(movements, 0);
			movements[_movements.Length] = movement;
			return new Schedule(movements);
		}

		public int MovementsCount {
			get {
				return _movements.Length;
			}
		}

		public ICarrierMovement this[int index] {
			get {
				return _movements[index];
			}
		}
		#endregion

		#region IEnumerable[Challenge00.DDDSample.Voyage.ICarrierMovement] implementation
		public IEnumerator<ICarrierMovement> GetEnumerator ()
		{
			IEnumerable<ICarrierMovement> movements = _movements;
			return movements.GetEnumerator();
		}
		#endregion
		
		#region IEnumerable implementation
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}
		#endregion
		
		#region IEquatable<ISchedule> implementation
		public bool Equals(ISchedule other)
		{
			if(object.ReferenceEquals(other, null))
				return false;
			if(object.ReferenceEquals(this, other))
				return true;
			ICarrierMovement[] otherMovements = other.ToArray();
			if(_movements.Length != otherMovements.Length)
				return false;
			for(int i = 0; i < _movements.Length; ++i)
				if(!_movements[i].Equals(otherMovements[i]))
					return false;
			return true;
		}
		#endregion

		public override bool Equals (object obj)
		{
			return Equals(obj as ISchedule);
		}
		
		public override int GetHashCode ()
		{
			if(_movements.Length == 0)
				return 0;
			return _movements[0].GetHashCode() ^ _movements[_movements.Length - 1].GetHashCode();
		}
	

	}
}

