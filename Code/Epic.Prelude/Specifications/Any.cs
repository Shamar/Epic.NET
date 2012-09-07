//
//  Any.cs
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
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Epic.Specifications
{
    /// <summary>
    /// Specification that is satisfied by any <typeparamref name="TCandidate"/>.
    /// </summary>
    /// <typeparam name="TCandidate">The type of the objects that can be tested with this specification.</typeparam>
    /// <remarks>
    /// This is a serializable singleton: an <see cref="IObjectReference"/> will always return the
    /// instance at <see cref="Specification"/> after deserialization.
    /// </remarks>
    [Serializable]
    public sealed class Any<TCandidate> : StatelessSpecificationBase<Any<TCandidate>, TCandidate>,
                                          IEquatable<Any<TCandidate>>,
                                          ISerializable
        where TCandidate : class
    {
        private Any ()
        {
        }

        /// <summary>
        /// Singleton instance of the specification.
        /// </summary>
        public static Any<TCandidate> Specification = new Any<TCandidate>();

        #region implemented abstract members of Epic.Specifications.SpecificationBase

        /// <summary>
        /// Determines that this specification is satisfied by any candidate.
        /// </summary>
        /// <returns>
        /// Always <c>true</c>, since the base class grant that no <see langword="null"/> candidate will reach this method.
        /// </returns>
        /// <param name='candidate'>
        /// Candidate.
        /// </param>
        protected override bool IsSatisfiedByA (TCandidate candidate)
        {
            return true;
        }

        /// <summary>
        /// Overrides the default behaviour returning <paramref name="other"/>.
        /// </summary>
        /// <returns>
        /// The <paramref name="other"/> specification, since P == True AND P.
        /// </returns>
        /// <param name='other'>
        /// Another specification.
        /// </param>
        protected override ISpecification<TCandidate> AndAlso (ISpecification<TCandidate> other)
        {
            return other;
        }

        /// <summary>
        /// Overrides the default behaviour returning the current instance.
        /// </summary>
        /// <returns>
        /// The current instance, since True == True OR P.
        /// </returns>
        /// <param name='other'>
        /// Another specification.
        /// </param>
        protected override ISpecification<TCandidate> OrElse (ISpecification<TCandidate> other)
        {
            return this;
        }

        /// <summary>
        /// Returns in <paramref name="negation"/> a <see cref="No{TCandidate}"/>.
        /// </summary>
        /// <param name='negation'>
        /// A <see cref="No{TCandidate}"/>. since NOT True == False.
        /// </param>
        protected override void BuildNegation(out ISpecification<TCandidate> negation)
        {
            negation = No<TCandidate>.Specification;
        }

        #endregion

        #region ISerializable implementation
        void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
        {
            Ref.Instance.GetObjectData(info, context);
        }
        #endregion

        [Serializable]
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        private sealed class Ref : IObjectReference, ISerializable
        {
            private Ref()
            {
            }

            public static readonly Ref Instance = new Ref();

            private Ref(SerializationInfo info, StreamingContext context)
            {
            }

            #region ISerializable Members

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.SetType(typeof(Ref));
            }

            #endregion

            #region IObjectReference Members

            public object GetRealObject(StreamingContext context)
            {
                return Any<TCandidate>.Specification;
            }

            #endregion
        }
    }
}

