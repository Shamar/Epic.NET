//  
//  OrSpecification.cs
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
using Challenge00.DDDSample.Shared;
namespace Challenge00.DDDSample.Shared
{
	[Serializable]
	public sealed class OrSpecification<TCandidate> : AbstractSpecification<TCandidate>
	{
		public readonly ISpecification<TCandidate> First;
		public readonly ISpecification<TCandidate> Second;
		
		public OrSpecification (ISpecification<TCandidate> first, ISpecification<TCandidate> second)
		{
			if(null == first)
				throw new ArgumentNullException("first");
			if(null == second)
				throw new ArgumentNullException("second");
			
			First = first;
			Second = second;
		}
		
		#region implemented abstract members of Challenge00.DDDSample.Shared.AbstractSpecification[TCandidate]
		
		public override bool IsSatisfiedBy (TCandidate candidate)
		{
			return First.IsSatisfiedBy(candidate) || Second.IsSatisfiedBy(candidate);
		}
		
		protected override bool Equals (AbstractSpecification<TCandidate> other)
		{
			OrSpecification<TCandidate> specification = other as OrSpecification<TCandidate>;
			if(null == specification)
				return false;
			return this.First.Equals(specification.First) && this.Second.Equals(specification.Second);
		}
		
		protected override int GetStateHashCode ()
		{
			return First.GetHashCode() ^ Second.GetHashCode();
		}
		
		#endregion
	}
}

