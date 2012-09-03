//
//  Disjunction.cs
//
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
//
//  Copyright (c) 2010-2012 Giacomo Tesio
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

namespace Epic.Specifications
{
    /// <summary>
    /// Specification that is satisfied by any <typeparamref name="TCandidate"/> that is satisfied by at
    /// least one of the specifications.
    /// </summary>
    /// <typeparam name="TCandidate">The type of the objects that can be tested with this specification.</typeparam>
    [Serializable]
    public sealed class Disjunction<TCandidate> : SpecificationBase<Disjunction<TCandidate>, TCandidate>,
                                                  IEquatable<Disjunction<TCandidate>>,
                                                  IEnumerable<ISpecification<TCandidate>>
        where TCandidate : class
    {
        private readonly ISpecification<TCandidate>[] _specifications;

        /// <summary>
        /// Initialize a new <see cref="Disjunction{TCandidate}"/>.
        /// </summary>
        /// <param name="first">First specification.</param>
        /// <param name="second">Second specification.</param>
        /// <exception cref="ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="first"/> and <paramref name="second"/> are equals.</exception>
        public Disjunction(ISpecification<TCandidate> first, ISpecification<TCandidate> second)
        {
            if (null == first)
                throw new ArgumentNullException("first");
            if (null == second)
                throw new ArgumentNullException("second");
            if (first.Equals(second))
                throw new ArgumentException("Cannot create a Disjunction of two equals specification.", "second");

            List<ISpecification<TCandidate>> specifications = new List<ISpecification<TCandidate>>();

            Disjunction<TCandidate> firstOr = first as Disjunction<TCandidate>;
            Disjunction<TCandidate> secondOr = second as Disjunction<TCandidate>;

            if (null == firstOr)
                specifications.Add(first);
            else
                specifications.AddRange(firstOr._specifications);

            if (null == secondOr)
            {
                // No need to check that the second is not included in first, since OrElse already check this.
                specifications.Add(second);
            }
            else if(null == firstOr)
            {
                for(int i = 0; i < secondOr._specifications.Length; ++i)
                {
                    ISpecification<TCandidate> toAdd = secondOr._specifications[i];
                    if(!first.Equals(toAdd))
                    {
                        specifications.Add(toAdd);
                    }
                }
            }
            else
            {
                for(int i = 0; i < secondOr._specifications.Length; ++i)
                {
                    bool alreadyPresent = false;
                    ISpecification<TCandidate> toAdd = secondOr._specifications[i];
                    for(int j = 0; j < firstOr._specifications.Length && !alreadyPresent; ++j)
                    {
                        if(toAdd.Equals(firstOr._specifications[j]))
                        {
                            alreadyPresent = true;
                        }
                    }
                    if (!alreadyPresent)
                    {
                        specifications.Add(toAdd);
                    }
                }
            }

            _specifications = specifications.ToArray();
        }

        #region implemented abstract members of Epic.Specifications.SpecificationBase

        /// <summary>
        /// Returns a new <see cref="Disjunction{TCandidate}"/> that will be satisfied when either
        /// the current specification or the <paramref name="other"/> are satisfied.
        /// </summary>
        /// <returns>
        /// A <see cref="Disjunction{TCandidate}"/>.
        /// </returns>
        /// <param name='other'>
        /// Another specification.
        /// </param>
        protected override ISpecification<TCandidate> OrElse (ISpecification<TCandidate> other)
        {
            Disjunction<TCandidate> otherOr = other as Disjunction<TCandidate>;
            if(null == otherOr)
            {
                for(int i = 0; i < _specifications.Length; ++i)
                {
                    if(other.Equals(_specifications[i]))
                        return this;
                }
            }
            return base.OrElse (other);
        }

        /// <summary>
        /// Determine whether the current <see cref="Disjunction{TCandidate}"/> is equal
        /// to <paramref name="otherSpecification"/>.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if each of disjuncted specification are equal 
        /// to the corresponding one in <paramref name="otherSpecification"/>, <c>false</c> otherwise.
        /// </returns>
        /// <param name='otherSpecification'>
        /// Another specification.
        /// </param>
        protected override bool EqualsA (Disjunction<TCandidate> otherSpecification)
        {
            if(_specifications.Length != otherSpecification._specifications.Length)
                return false;
            for(int i = 0; i < _specifications.Length; ++i)
                if(!_specifications[i].Equals(otherSpecification._specifications[i]))
                    return false;
            return true;
        }

        /// <summary>
        /// Determines whether this specification is satisfied by <paramref name="candidate"/>.
        /// </summary>
        /// <returns>
        /// <c>true</c> if <paramref name="candidate"/> satisfies at least one of 
        /// the disjuncted specifications; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='candidate'>
        /// Candidate.
        /// </param>
        protected override bool IsSatisfiedByA (TCandidate candidate)
        {
            for(int i = 0; i < _specifications.Length; ++i)
                if(_specifications[i].IsSatisfiedBy(candidate))
                    return true;
            return false;
        }

        #endregion

        #region IEnumerable implementation
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return _specifications.GetEnumerator();
        }
        #endregion

        #region IEnumerable implementation
        IEnumerator<ISpecification<TCandidate>> IEnumerable<ISpecification<TCandidate>>.GetEnumerator ()
        {
            return (_specifications as IEnumerable<ISpecification<TCandidate>>).GetEnumerator();
        }
        #endregion
    }
}

