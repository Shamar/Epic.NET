using System;
using Challenge00.DDDSample.Voyage;
using Challenge00.DDDSample.Location;
namespace Challenge00.DDDSample.Default.Cargo
{
	[Serializable]
	public sealed class CarrierMovement : ICarrierMovement
	{
		private readonly ILocation _departureLocation;
		private readonly DateTime _departureTime;
		private readonly ILocation _arrivalLocation;
		private readonly DateTime _arrivalTime;
		
		public CarrierMovement (ILocation departureLocation, DateTime departureTime, ILocation arrivalLocation, DateTime arrivalTime)
		{
			if(null == departureLocation)
				throw new ArgumentNullException("departureLocation");
			if(null == arrivalLocation)
				throw new ArgumentNullException("arrivalLocation");
			if(departureLocation.Equals(arrivalLocation))
				throw new ArgumentException("Departure location and arrival location must not be equals.");
			if(departureTime >= arrivalTime)
				throw new ArgumentOutOfRangeException("arrivalTime", "Arrival time must follow the departure time.");
			
			_departureLocation = departureLocation;
			_departureTime = departureTime;
			_arrivalLocation = arrivalLocation;
			_arrivalTime = arrivalTime;
		}

		#region ICarrierMovement implementation
		public ILocation ArrivalLocation {
			get {
				return _arrivalLocation;
			}
		}

		public ILocation DepartureLocation {
			get {
				return _departureLocation;
			}
		}

		public DateTime ArrivalTime {
			get {
				return _arrivalTime;
			}
		}

		public DateTime DepartureTime {
			get {
				return _departureTime;
			}
		}
		#endregion

		#region IEquatable[Challenge00.DDDSample.Voyage.ICarrierMovement] implementation
		public bool Equals (ICarrierMovement other)
		{
			if(null == other)
				return false;
			return this._arrivalLocation.Equals(other.ArrivalLocation) 
				&& this._departureLocation.Equals(other.DepartureLocation) 
				&& this._arrivalTime.Equals(other.ArrivalTime)
				&& this._departureTime.Equals(other.DepartureTime);
		}
		#endregion
	}
}

