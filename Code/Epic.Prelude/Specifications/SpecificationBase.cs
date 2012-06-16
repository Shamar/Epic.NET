//
//  SpecificationBase.cs
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
using Epic.Math;

namespace Epic.Specifications
{
    /// <summary>
    /// Base class for specifications.
    /// </summary>
    /// <typeparam name="TSpecification">Type of the specification implemented.</typeparam>
    /// <typeparam name="TCandidate">Type of the candidates to satisfy the specification.</typeparam>
    [Serializable]
    public abstract class SpecificationBase<TSpecification, TCandidate> : VisitableBase, 
                                                                          ISpecification<TCandidate>,
                                                                          IMapping<TCandidate, bool>
        where TCandidate : class
        where TSpecification : class, ISpecification<TCandidate>, IEquatable<TSpecification>
    {
        static SpecificationBase()
        {
            if(typeof(object).Equals(typeof(TCandidate)))
            {
                string message = "System.Object is too generic to be a valid candidate for specifications.";
                throw new InvalidOperationException (message);
            }
            if(typeof(ISpecification<TCandidate>).Equals(typeof(TSpecification)))
            {
                string message = string.Format ("ISpecification<{0}> is too generic to be a valid specification type.", typeof(TCandidate));
                throw new InvalidOperationException (message);
            }
        }
        
        protected SpecificationBase ()
        {
            if (!(this is TSpecification)) {
                string message = string.Format ("The specification {0} must implement {1} becouse it extends SpecificationBase<{1}, {2}>.", this.GetType (), typeof(TSpecification), typeof(TCandidate));
                throw new InvalidOperationException (message);
            }
        }

        private static void ThrowIfNull(ISpecification<TCandidate> other)
        {
            if(null == other)
                throw new ArgumentNullException("other");
        }

        #region implemented abstract members of Epic.VisitableBase
        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe<TResult, TSpecification> (this as TSpecification, visitor, context);
        }
        #endregion
        
        #region IEquatable implementation
        public bool Equals (ISpecification<TCandidate> other)
        {
            return Equals (other as TSpecification);
        }
        
        public override bool Equals(object obj)
        {
            return Equals (obj as TSpecification);
        }

        public override int GetHashCode ()
        {
            // The HashCode of the TSpecification should be enough: it's strange 
            // to use a specification as a Key in a dictionary
            return typeof(TSpecification).GetHashCode ();
        }
        #endregion

        #region IEquatable implementation
        public bool Equals (TSpecification other)
        {
            if (null == other)
                return false;
            if (this == other)
                return true;
            if (this.GetType ().Equals (other))
                return false;
            return EqualsA (other);
        }
        
        protected abstract bool EqualsA (TSpecification otherSpecification);
        
        #endregion

        #region ISpecification implementation
        public bool IsSatisfiedBy (TCandidate candidate)
        {
            if (null == candidate)
                return false;
            return IsSatisfiedByA (candidate);
        }
        
        /// <summary>
        /// Determines whether this specification is satisfied by a the specified candidate.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this specification is satisfied by a the specified candidate; otherwise, <c>false</c>.
        /// </returns>
        /// <param name='candidate'>
        /// A valid (not null) candidate.
        /// </param>
        protected abstract bool IsSatisfiedByA (TCandidate candidate);

        public ISpecification<TCandidate> And (ISpecification<TCandidate> other)
        {
            ThrowIfNull(other);
            throw new System.NotImplementedException ();
        }

        public ISpecification<TCandidate> Or (ISpecification<TCandidate> other)
        {
            ThrowIfNull(other);
            throw new System.NotImplementedException ();
        }

        public virtual ISpecification<TCandidate> Negate ()
        {
            throw new System.NotImplementedException ();
        }

        public ISpecification<TOther> OfType<TOther> () where TOther : class
        {
            ISpecification<TOther> other = this as ISpecification<TOther>;
            if (null != other)
                return other;
            return OfAnotherType<TOther>();
        }
        
        /// <summary>
        /// Return a specifications satisfied by <typeparamref name="TOther"/> that
        /// satisfy this specification.
        /// </summary>
        /// <returns>
        /// A specifications satisfied by <typeparamref name="TOther"/> that
        /// satisfy this specification (if not overridden, a <see cref="OfType{TOther, TCandidate}"/>).
        /// </returns>
        /// <typeparam name='TOther'>
        /// Either a specialization or an abstraction of <typeparamref name="TCandidate"/>.
        /// </typeparam>
        /// <exception cref="InvalidCastException"><typeparamref name="TOther"/> is neither an
        /// abstraction nor a specialization of <typeparamref name="TCandidate"/>.</exception>
        protected virtual ISpecification<TOther> OfAnotherType<TOther> () where TOther : class
        {
            return new OfType<TOther, TCandidate>(this);
        }

        public virtual Type CandidateType {
            get {
                return typeof(TCandidate);
            }
        }

        public virtual Type SpecificationType {
            get {
                return typeof(TSpecification);
            }
        }
        #endregion

        #region IMapping implementation
        bool IMapping<TCandidate, bool>.ApplyTo (TCandidate element)
        {
            return IsSatisfiedBy (element);
        }
        #endregion
    }
}

