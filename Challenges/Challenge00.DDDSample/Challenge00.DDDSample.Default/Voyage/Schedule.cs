using System;
using System.Linq;
using Challenge00.DDDSample.Voyage;
using System.Collections.Generic;
namespace Challenge00.DDDSample.Default.Voyage
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
			if(null == movements)
				throw new ArgumentNullException("movements");
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
				throw new ArgumentException(string.Format("The provided movement do not depart from {0}.", lastMovement.ArrivalLocation.Name), "movement");
			if(lastMovement.ArrivalTime > movement.DepartureTime)
				throw new ArgumentException(string.Format("The provided movement depart from {0} too early.", lastMovement.ArrivalLocation.Name), "movement");
			
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
			if(null == other)
				return false;
			if(this == other)
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

