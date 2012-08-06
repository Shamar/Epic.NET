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
    /// <remarks>
    /// <para>
    /// The constructor grant that the instance produced is assignable from <typeparamref name="TSpecification"/>.
    /// </para>
    /// <para>Moreover, the type initializer throws <see cref="InvalidOperationException"/> if 
    /// either <typeparamref name="Candidate"/> is <see cref="System.Object"/> or <typeparamref name="TSpecification"/> is <see cref="ISpecification{Candidate}"/></para>
    /// </remarks> 
    /// <typeparam name="TSpecification">Type of the specification implemented.</typeparam>
    /// <typeparam name="Candidate">Type of the candidates to satisfy the specification.</typeparam>
    [Serializable]
    public abstract class SpecificationBase<TSpecification, Candidate> : VisitableBase, 
    ISpecification<Candidate>,
    IMapping<Candidate, bool>
        where Candidate : class
        where TSpecification : class, ISpecification<Candidate>, IEquatable<TSpecification>
        {
            static SpecificationBase ()
            {
                if (typeof(object).Equals (typeof(Candidate))) {
                    string message = "System.Object is too generic to be a valid candidate for specifications.";
                    throw new InvalidOperationException (message);
                }
                if (typeof(ISpecification<Candidate>).Equals (typeof(TSpecification))) {
                    string message = string.Format ("ISpecification<{0}> is too generic to be a valid specification type.", typeof(Candidate));
                    throw new InvalidOperationException (message);
                }
            }
            
            /// <summary>
            /// Initializes a new instance of the <see cref="Epic.Specifications.SpecificationBase{TSpecification, Candidate}"/> class.
            /// </summary>
            protected SpecificationBase ()
            {
                if (!(this is TSpecification)) {
                    string message = string.Format ("The specification {0} must implement {1} becouse it extends SpecificationBase<{1}, {2}>.", this.GetType (), typeof(TSpecification), typeof(Candidate));
                    throw new InvalidOperationException (message);
                }
            }
            
            /// <summary>
            /// Throws <see cref="ArgumentNullException"/> if <paramref name="other"/> is <c>null</c>.
            /// </summary>
            /// <param name='other'>
            /// Other.
            /// </param>
            /// <typeparam name='T'>
            /// The 1st type parameter.
            /// </typeparam>
            protected static void ThrowIfNull<T> (ISpecification<T> other)
                where T : class
            {
                if (null == other)
                    throw new ArgumentNullException ("other");
            }
            
            #region implemented abstract members of Epic.VisitableBase
            /// <summary>
            /// Accept the specified visitor and context as a <typeparamref name="TSpecification"/>.
            /// </summary>
            /// <param name='visitor'>
            /// Visitor.
            /// </param>
            /// <param name='context'>
            /// Context.
            /// </param>
            /// <typeparam name='TResult'>
            /// The type of the visit result.
            /// </typeparam>
            /// <exception cref="ArgumentNullException"><paramref name="visitor"/> or <paramref name="context"/> is <c>null</c>.</exception>
            public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
            {
                return AcceptMe<TResult, TSpecification> (this as TSpecification, visitor, context);
            }
#endregion
            
            #region IEquatable implementation
            /// <summary>
            /// Determines whether the specified <see cref="Epic.Specifications.ISpecification{Candidate}"/> is equal to the
            /// current <see cref="Epic.Specifications.SpecificationBase{TSpecification,Candidate}"/>.
            /// </summary>
            /// <param name='other'>
            /// The <see cref="Epic.Specifications.ISpecification{Candidate}"/> to compare with the current instance.
            /// </param>
            /// <returns>
            /// <c>true</c> if the specified <see cref="Epic.Specifications.ISpecification{Candidate}"/> is equal to the
            /// current <see cref="Epic.Specifications.SpecificationBase{TSpecification,Candidate}"/>; otherwise, <c>false</c>.
            /// </returns>
            public bool Equals (ISpecification<Candidate> other)
            {
                return Equals (other as TSpecification);
            }
            
            /// <summary>
            /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="SpecificationBase{TSpecification,Candidate}"/>.
            /// </summary>
            /// <param name='obj'>
            /// The <see cref="System.Object"/> to compare with the current <see cref="SpecificationBase{TSpecification,Candidate}"/>.
            /// </param>
            /// <returns>
            /// <c>true</c> if the specified <see cref="System.Object"/> is equal to the current
            /// <see cref="SpecificationBase{TSpecification,Candidate}"/>; otherwise, <c>false</c>.
            /// </returns>
            public override bool Equals (object obj)
            {
                return Equals (obj as TSpecification);
            }
            
            /// <summary>
            /// Serves as a hash function for a <see cref="SpecificationBase{TSpecification,Candidate}"/> object.
            /// </summary>
            /// <returns>
            /// The hash code of <c>typeof(<typeparamref name="TSpecification"/>)</c>.
            /// </returns>
            public override int GetHashCode ()
            {
                // The HashCode of the TSpecification should be enough: it's strange 
                // to use a specification as a Key in a dictionary
                return typeof(TSpecification).GetHashCode ();
            }
#endregion
            
            #region IEquatable implementation
            /// <summary>
            /// Determines whether the specified <typeparamref name="TSpecification"/> is equal
            /// to the current <see cref="SpecificationBase{TSpecification,Candidate}"/>.
            /// </summary>
            /// <param name='other'>
            /// The <typeparamref name="TSpecification"/> to compare with the current <see cref="SpecificationBase{TSpecification,Candidate}"/>.
            /// </param>
            /// <returns>
            /// <c>true</c> if the specified <typeparamref name="TSpecification"/> is equal to
            /// the current <see cref="SpecificationBase{TSpecification,Candidate}"/>; otherwise, <c>false</c>.
            /// </returns>
            public bool Equals (TSpecification other)
            {
                if (null == other)
                    return false;
                if (this == other)
                    return true;
                if (!this.GetType ().Equals (other.GetType ()))
                    return false;
                return EqualsA (other);
            }
            
            /// <summary>
            /// Determines whether the specified <typeparamref name="TSpecification"/> is equal
            /// to the current <see cref="SpecificationBase{TSpecification,Candidate}"/>.
            /// </summary>
            /// <returns>
            /// <c>true</c>, if this is equal to <paramref name="otherSpecification"/>, <c>false</c> otherwise.
            /// </returns>
            /// <param name='otherSpecification'>
            /// Another specification instance, that is not <c>null</c> and has the same type
            /// of this.
            /// </param>
            protected abstract bool EqualsA (TSpecification otherSpecification);
            
#endregion
            
            #region ISpecification implementation
            /// <summary>
            /// Check if the <typeparamref name="Candidate"/> satisfy the specification. 
            /// </summary>
            /// <param name="candidate">
            /// A <typeparamref name="Candidate"/>.
            /// </param>
            /// <returns>
            /// Returns <c>true</c> when <paramref name="candidate"/> satisfies the specification, <c>false</c> otherwise.
            /// Note that <c>null</c> can not satisfy any specification.
            /// </returns>
            public bool IsSatisfiedBy (Candidate candidate)
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
            protected abstract bool IsSatisfiedByA (Candidate candidate);
            
            /// <summary>
            /// Create a new <see cref="ISpecification{Candidate}"/> that evaluates the <paramref name="other"/> 
            /// only if the current specification is satisfied.
            /// </summary>
            /// <param name="other">The other specification to evaluate.</param>
            /// <returns>A new <see cref="ISpecification{Candidate}"/> that evaluates the <paramref name="other"/> 
            /// only if the current specification is satisfied.</returns>
            /// <remarks>This method calls <see cref="AndAlso"/> that can be overridden.</remarks>
            /// <exception cref="ArgumentNullException"><paramref name="other"/> is <value>null</value>.</exception>
            public ISpecification<Candidate> And (ISpecification<Candidate> other)
            {
                ThrowIfNull (other);
                if (other is No<Candidate> || this.Equals (other))
                    return other;
                if (other is Any<Candidate>)
                    return this;
                return AndAlso (other);
            }
            
            /// <summary>
            /// Create a new <see cref="Conjunction{Candidate}"/> that evaluates the <paramref name="other"/> 
            /// only if the current specification is satisfied.
            /// </summary>
            /// <param name="other">The other specification to evaluate (that will not be <c>null</c>).</param>
            /// <returns>A new <see cref="Conjunction{Candidate}"/> that evaluates the <paramref name="other"/> 
            /// only if the current specification is satisfied.</returns>
            protected virtual ISpecification<Candidate> AndAlso (ISpecification<Candidate> other)
            {
                return new Conjunction<Candidate> (this, other);
            }
            
            /// <summary>
            /// Create a new <see cref="ISpecification{Candidate}"/> that evaluates the <paramref name="other"/> 
            /// only if the current specification is not satisfied.
            /// </summary>
            /// <param name="other">The other specification to evaluate.</param>
            /// <returns>A new <see cref="ISpecification{Candidate}"/> that evaluates the <paramref name="other"/> 
            /// only if the current specification is not satisfied.</returns>
            /// <remarks>This method calls <see cref="OrElse"/> that can be overridden.</remarks>
            /// <exception cref="ArgumentNullException"><paramref name="other"/> is <value>null</value>.</exception>
            public ISpecification<Candidate> Or (ISpecification<Candidate> other)
            {
                ThrowIfNull (other);
                if (other is No<Candidate> || this.Equals (other))
                    return this;
                if (other is Any<Candidate>)
                    return other;
                return OrElse (other);
            }
            
            /// <summary>
            /// Create a new <see cref="Disjunction{Candidate}"/> that evaluates the <paramref name="other"/> 
            /// only if the current specification is not satisfied.
            /// </summary>
            /// <param name="other">The other specification to evaluate (that will not be <c>null</c>).</param>
            /// <returns>A new <see cref="Disjunction{Candidate}"/> that evaluates the <paramref name="other"/> 
            /// only if the current specification is not satisfied.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="other"/> is <value>null</value>.</exception>
            protected virtual ISpecification<Candidate> OrElse (ISpecification<Candidate> other)
            {
                return new Disjunction<Candidate> (this, other);
            }
            
            ISpecification<Candidate> ISpecification<Candidate>.Negate ()
            {
                return NegateFirstCandidate();
            }
            
            protected virtual ISpecification<Candidate> NegateFirstCandidate()
            {
                return new Negation<Candidate> (this);
            }
            
            public ISpecification<TOther> OfType<TOther> () where TOther : class
            {
                ISpecification<TOther> other = this as ISpecification<TOther>;
                if (null != other)
                    return other;
                return OfAnotherType<TOther> ();
            }
            
            /// <summary>
            /// Return a specifications satisfied by <typeparamref name="TOther"/> that
            /// satisfy this specification.
            /// </summary>
            /// <returns>
            /// A specifications satisfied by <typeparamref name="TOther"/> that
            /// satisfy this specification (if not overridden, a <see cref="OfType{TOther, Candidate}"/>).
            /// </returns>
            /// <typeparam name='TOther'>
            /// Either a specialization or an abstraction of <typeparamref name="Candidate"/>.
            /// </typeparam>
            /// <exception cref="InvalidCastException"><typeparamref name="TOther"/> is neither an
            /// abstraction nor a specialization of <typeparamref name="Candidate"/>.</exception>
            protected virtual ISpecification<TOther> OfAnotherType<TOther> () where TOther : class
            {
                return new OfType<TOther, Candidate> (this);
            }
            
            protected virtual Type FirstCandidateType {
                get {
                    return typeof(Candidate);
                }
            }
            
            Type ISpecification<Candidate>.CandidateType {
                get {
                    return FirstCandidateType;
                }
            }
            
            public virtual Type SpecificationType {
                get {
                    return typeof(TSpecification);
                }
            }
#endregion
            
            #region IMapping implementation
            bool IMapping<Candidate, bool>.ApplyTo (Candidate element)
            {
                return IsSatisfiedBy (element);
            }
#endregion
        }
    
    [Serializable]
    public abstract class SpecificationBase<TSpecification, Candidate1, Candidate2> : 
        SpecificationBase<TSpecification, Candidate1>,
        ISpecification<Candidate2>,
        IMapping<Candidate2, bool>
            where Candidate1 : class
            where Candidate2 : class
            where TSpecification : class, ISpecification<Candidate1>, 
            ISpecification<Candidate2>, 
            IEquatable<TSpecification>
            {
                protected virtual ISpecification<Candidate2> NegateSecondCandidate()
                {
                    return new Negation<Candidate2> (this);
                }
                
                #region IEquatable implementation
                public bool Equals (ISpecification<Candidate2> other)
                {
                    return base.Equals(other as TSpecification);
                }
#endregion
                
                #region ISpecification implementation
                public bool IsSatisfiedBy (Candidate2 candidate)
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
                protected abstract bool IsSatisfiedByA (Candidate2 candidate);
                
                public ISpecification<Candidate2> And (ISpecification<Candidate2> other)
                {
                    ThrowIfNull (other);
                    if (other is No<Candidate2> || this.Equals (other))
                        return other;
                    if (other is Any<Candidate2>)
                        return this;
                    return AndAlso (other);
                }
                
                protected virtual ISpecification<Candidate2> AndAlso (ISpecification<Candidate2> other)
                {
                    return new Conjunction<Candidate2> (this, other);
                }
                
                public ISpecification<Candidate2> Or (ISpecification<Candidate2> other)
                {
                    ThrowIfNull (other);
                    if (other is No<Candidate2> || this.Equals (other))
                        return this;
                    if (other is Any<Candidate2>)
                        return other;
                    return OrElse (other);
                }
                
                protected virtual ISpecification<Candidate2> OrElse (ISpecification<Candidate2> other)
                {
                    return new Disjunction<Candidate2> (this, other);
                }
                
                ISpecification<Candidate2> ISpecification<Candidate2>.Negate ()
                {
                    return NegateSecondCandidate();
                }
                
                Type ISpecification<Candidate2>.CandidateType {
                    get {
                        return typeof(Candidate2);
                    }
                }
                
                #endregion ISpecification implementation
                
                #region IMapping implementation
                bool IMapping<Candidate2, bool>.ApplyTo (Candidate2 candidate)
                {
                    return IsSatisfiedBy(candidate);
                }
#endregion
            }
}

