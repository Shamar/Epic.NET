//  
//  NotSpecification.cs
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
	public sealed class NotSpecification<TCandidate> : AbstractSpecification<TCandidate>
	{
		public readonly ISpecification<TCandidate> Negated;
		public NotSpecification (ISpecification<TCandidate> specification)
		{
			if(null == specification)
				throw new ArgumentNullException("specification");
			Negated = specification;
		}
		
		#region implemented abstract members of Challenge00.DDDSample.Shared.AbstractSpecification[TCandidate]
		public override bool IsSatisfiedBy (TCandidate candidate)
		{
			return !Negated.IsSatisfiedBy(candidate);
		}
		
		protected override bool Equals (AbstractSpecification<TCandidate> other)
		{
			NotSpecification<TCandidate> s = other as NotSpecification<TCandidate>;
			if(null == s)
				return false;
			return Negated.Equals(s.Negated);
		}
		
		protected override int GetStateHashCode ()
		{
			return Negated.GetHashCode();
		}
		
		#endregion
	}
}

