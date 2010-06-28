//  
//  AbstractSpecification.cs
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
using System.Linq.Expressions;
using Challenge00.DDDSample.Shared;
namespace Challenge00.DDDSample.Shared
{
	[Serializable]
	public abstract class AbstractSpecification<TCandidate> : ISpecification<TCandidate>
	{
		#region ISpecification<TCandidate> implementation
		
		public abstract bool IsSatisfiedBy (TCandidate candidate);	
		
		#endregion
		
		#region IEquatable<ISpecification<TCandidate>> implementation
		
		public bool Equals (ISpecification<TCandidate> other)
		{
			AbstractSpecification<TCandidate> candidate = other as AbstractSpecification<TCandidate>;
			if(null == candidate)
				return false;
			
			return Equals(candidate);
		}
		
		#endregion
		
		protected abstract bool Equals(AbstractSpecification<TCandidate> other);
		
		public sealed override bool Equals (object obj)
		{
			ISpecification<TCandidate> other = obj as ISpecification<TCandidate>;
			return this.Equals(other);
		}
		
		public sealed override int GetHashCode ()
		{
			return this.GetType().GetHashCode() ^ GetStateHashCode();
		}
		
		/// <summary>
		/// Return the hashcode for the internal state of the concrete specification.
		/// Return <see cref="int.MaxValue"/> for stateless specifications.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		protected abstract int GetStateHashCode();
	}
}

