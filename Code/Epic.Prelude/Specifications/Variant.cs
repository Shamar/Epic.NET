//
//  Variant.cs
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
    /// Specification that is satisfied by any <typeparamref name="ToCandidate"/> that is a 
    /// <typeparamref name="FromCandidate"/> satisfing the inner specification.
    /// </summary>
    /// <typeparam name="ToCandidate">Type of the objects that can be tested with this specification.</typeparam>
    /// <typeparam name="FromCandidate">Type of the objects that can be tested with inner specification.</typeparam>
    /// <seealso cref="IMonadicSpecificationComposition{TCandidate}"/>
    [Serializable]
    public sealed class Variant<FromCandidate, ToCandidate> : SpecificationBase<Variant<FromCandidate, ToCandidate>, ToCandidate>,
                                                              IEquatable<Variant<FromCandidate, ToCandidate>>,
                                                              IMonadicSpecificationComposition<ToCandidate>
        where ToCandidate : class
        where FromCandidate : class
    {
        private static readonly bool _thisIsAnUpcastingVariant;

        static Variant()
        {
            if (typeof(ToCandidate).Equals(typeof(FromCandidate)))
            {
                string message = string.Format("Cannot create a Variant<{1}, {0}> specification, becouse the two type arguments are equals.", typeof(FromCandidate), typeof(ToCandidate));
                throw new EpicException(message);
            }
            _thisIsAnUpcastingVariant = typeof(ToCandidate).IsAssignableFrom(typeof(FromCandidate));
        }
        
        private readonly ISpecification<FromCandidate> _innerSpecification;

        /// <summary>
        /// The <see cref="ISpecification{TInitial}"/> that must be satisfied by <typeparamref name="ToCandidate"/> for this specification.
        /// </summary>
        public ISpecification<FromCandidate> InnerSpecification
        {
            get
            {
                return _innerSpecification;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Variant{ToCandidate, FromCandidate}"/> class.
        /// </summary>
        /// <param name='innerSpecification'>
        /// Specification that must be satisfied by any <typeparamref name="ToCandidate"/> to satisfy the current specification.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="innerSpecification"/> is <see langword="null"/>.</exception>
        internal Variant(ISpecification<FromCandidate> innerSpecification)
        {
            if (null == innerSpecification)
                throw new ArgumentNullException("innerSpecification");
            _innerSpecification = innerSpecification;
        }

        #region implemented abstract members of Epic.Specifications.SpecificationBase
        
        /// <summary>
        /// Determines whether the specified <see cref="Variant{ToCandidate, FromCandidate}"/> is equal to the current one.
        /// </summary>
        /// <returns>
        /// <see langword="true"/>, if <paramref name="otherSpecification"/> has the same <see cref="InnerSpecification"/> 
        /// of the current instance, <see langword="false"/> otherwise.
        /// </returns>
        /// <param name='otherSpecification'>
        /// Other specification.
        /// </param>
        protected override bool EqualsA(Variant<FromCandidate, ToCandidate> otherSpecification)
        {
            return _innerSpecification.Equals(otherSpecification._innerSpecification);
        }
        
        /// <summary>
        /// Determines whether this specification is satisfied by a the specified <paramref name="candidate"/>.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if <paramref name="candidate"/> is a <typeparamref name="FromCandidate"/> that satisfy 
        /// the <see cref="InnerSpecification"/>; otherwise, <see langword="false"/>.
        /// </returns>
        /// <param name='candidate'>
        /// Candidate.
        /// </param>
        protected override bool IsSatisfiedByA(ToCandidate candidate)
        {
            return _innerSpecification.IsSatisfiedBy(candidate as FromCandidate);
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
        protected override ISpecification<Other> OfAnotherType<Other>()
        {
            if (_thisIsAnUpcastingVariant)
                return _innerSpecification.OfType<Other>();
            return new Variant<FromCandidate, Other>(_innerSpecification);
        }
        
        /// <summary>
        /// <see cref="ISpecification{TInitial}.CandidateType"/>.
        /// </summary>
        protected override Type FirstCandidateType
        {
            get
            {
                return _innerSpecification.CandidateType;
            }
        }
        
        #endregion

        #region IMonadicSpecificationComposition implementation
        
        ISpecification IMonadicSpecificationComposition<ToCandidate>.Operand
        {
            get
            {
                return _innerSpecification;
            }
        }

        #endregion
    }
}

