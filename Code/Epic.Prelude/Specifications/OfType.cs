//
//  OfType.cs
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

namespace Epic.Specifications
{
    /// <summary>
    /// Specification that is satisfied by any <typeparamref name="TCandidate"/> that is a 
    /// <typeparamref name="TInitial"/> satisfing the inner specification.
    /// </summary>
    /// <typeparam name="TCandidate">Type of the objects that can be tested with this specification.</typeparam>
    /// <typeparam name="TInitial">Type of the objects that can be tested with inner specification.</typeparam>
    [Serializable]
    public sealed class OfType<TCandidate, TInitial> : SpecificationBase<OfType<TCandidate, TInitial>, TCandidate>,
                                                       IEquatable<OfType<TCandidate, TInitial>>
        where TCandidate : class
        where TInitial : class
    {
        static OfType ()
        {
            if (!typeof(TCandidate).IsAssignableFrom (typeof(TInitial)) && !typeof(TInitial).IsAssignableFrom (typeof(TCandidate))) {
                string message = string.Format ("Cannot cast neither from {0} to {1} nor from {1} to {0}.", typeof(TInitial), typeof(TCandidate));
                throw new InvalidCastException (message);
            }
            if (typeof(TCandidate).Equals(typeof(TInitial)))
            {
                string message = string.Format ("Cannot create an OfType<{1}, {0}>, becouse the two type arguments are equals.", typeof(TInitial), typeof(TCandidate));
                throw new InvalidCastException (message);
            }
        }
        
        private readonly ISpecification<TInitial> _innerSpecification;

        /// <summary>
        /// The <see cref="ISpecification{TInitial}"/> that must be satisfied by <typeparamref name="TCandidate"/> for this specification.
        /// </summary>
        public ISpecification<TInitial> InnerSpecification
        {
            get
            {
                return _innerSpecification;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfType{TCandidate, TInitial}"/> class.
        /// </summary>
        /// <param name='innerSpecification'>
        /// Specification that must be satisfied by any <typeparamref name="TCandidate"/> to satisfy the current specification.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="innerSpecification"/> is <c>null</c>.</exception>
        public OfType (ISpecification<TInitial> innerSpecification)
        {
            if (null == innerSpecification)
                throw new ArgumentNullException ("innerSpecification");
            _innerSpecification = innerSpecification;
        }

        #region implemented abstract members of Epic.Specifications.SpecificationBase
        
        /// <summary>
        /// Determines whether the specified <see cref="OfType{TCandidate, TInitial}"/> is equal to the current one.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if <paramref name="otherSpecification"/> has the same <see cref="InnerSpecification"/> 
        /// of the current instance, <c>false</c> otherwise.
        /// </returns>
        /// <param name='otherSpecification'>
        /// Other specification.
        /// </param>
        protected override bool EqualsA (OfType<TCandidate, TInitial> otherSpecification)
        {
            return _innerSpecification.Equals (otherSpecification._innerSpecification);
        }
        
        /// <summary>
        /// Determines whether this specification is satisfied by a the specified <paramref name="candidate"/>.
        /// </summary>
        /// <returns>
        /// <c>true</c> if <paramref name="candidate"/> is a <typeparamref name="TInitial"/> that satisfy 
        /// the <see cref="InnerSpecification"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='candidate'>
        /// Candidate.
        /// </param>
        protected override bool IsSatisfiedByA (TCandidate candidate)
        {
            return _innerSpecification.IsSatisfiedBy (candidate as TInitial);
        }
        
        /// <summary>
        /// Return the specification produced by the <see cref="ISpecification{TInitial}.OfType{Other}"/> method
        /// of the <see cref="InnerSpecification"/>.
        /// </summary>
        /// <returns>
        /// A specifications satisfied by <typeparamref name="Other"/> that
        /// satisfy this specification.
        /// </returns>
        /// <typeparam name='Other'>
        /// One of types admitted to satisfy the <see cref="InnerSpecification"/>.
        /// </typeparam>
        /// <exception cref="InvalidCastException">No instance of <typeparamref name="Other"/> can 
        /// satisfy the <see cref="InnerSpecification"/>.</exception>
        protected override ISpecification<Other> OfAnotherType<Other> ()
        {
            return _innerSpecification.OfType<Other> ();
        }
        
        /// <summary>
        /// <see cref="ISpecification{TInitial}.CandidateType"/>.
        /// </summary>
        protected override Type FirstCandidateType {
            get {
                return _innerSpecification.CandidateType;
            }
        }
        #endregion
    }
}

