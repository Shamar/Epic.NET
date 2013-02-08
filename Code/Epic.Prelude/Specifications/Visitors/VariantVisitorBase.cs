//
//  VariantVisitorBase.cs
//
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
//
//  Copyright (c) 2010-2013 Giacomo Tesio
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
using System.Collections.Concurrent;
using Epic.Visitors;

namespace Epic.Specifications.Visitors
{
    /// <summary>
    /// Base class for visitors of <see cref="Variant{FromCandidate, ToCandidate}"/>.
    /// </summary>
    /// <typeparam name="TResult">Type of the result produced from the visits.</typeparam>
    /// <typeparam name="TCandidate">Common ancestor of the candidate types handled by the <see cref="Variant{FromCandidate, ToCandidate}"/>.</typeparam>
    /// <seealso cref="Variant{FromCandidate, ToCandidate}"/>
    public abstract class VariantVisitorBase<TResult, TCandidate> : CompositeVisitor<TResult>.VisitorBase
        where TCandidate : class
    {
        private readonly ConcurrentDictionary<Type, IVisitor<TResult>> _visitors;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariantVisitorBase{TResult, TCandidate}"/> class.
        /// </summary>
        /// <param name="composition">Composition.</param>
        /// <exception cref="ArgumentNullException"><paramref name="composition"/> is <see langword="null"/>.</exception>
        protected VariantVisitorBase(CompositeVisitor<TResult> composition)
            : base(composition)
        {
            _visitors = new ConcurrentDictionary<Type, IVisitor<TResult>>();
        }

        /// <summary>
        /// Returns the current instance if and only if it's able to visit <paramref name="target"/>,
        /// <see langword="null"/> otherwise.
        /// It should be overridden whenever the type of <paramref name="target"/> is not enough 
        /// to choose whether the current instance can visit it or not.
        /// </summary>
        /// <returns>
        /// The current instance or <see langword="null"/>.
        /// </returns>
        /// <param name='target'>
        /// Object to visit.
        /// </param>
        /// <typeparam name='TExpression'>
        /// The type of the object to visit.
        /// </typeparam>
        protected sealed override IVisitor<TResult, TExpression> AsVisitor<TExpression>(TExpression target)
        {
            IMonadicSpecificationComposition<TCandidate> monadic = target as IMonadicSpecificationComposition<TCandidate>;
            if (null != monadic)
            {
                if(monadic.SpecificationType.GetGenericTypeDefinition().Equals(typeof(Variant<,>)))
                {
                    IVisitor<TResult> typedVisitor = null;
                    if(!_visitors.TryGetValue(typeof(TExpression), out typedVisitor))
                    {
                        // no known visitor: we need one
                        Type[] fromToTypes = monadic.SpecificationType.GetGenericArguments();
                        Type visitorFactory;
                        if(fromToTypes[1].IsAssignableFrom(fromToTypes[0]))
                        {
                            // target is an upcasting specification
                            visitorFactory = typeof(UpcastingVariantVisitor<,>).MakeGenericType(typeof(TResult), typeof(TCandidate), fromToTypes[0], fromToTypes[1]);
                        }
                        else
                        {
                            // target is an downcasting specification
                            visitorFactory = typeof(DowncastingVariantVisitor<,>).MakeGenericType(typeof(TResult), typeof(TCandidate), fromToTypes[0], fromToTypes[1]);
                        }
                        typedVisitor = Activator.CreateInstance(visitorFactory, this) as IVisitor<TResult>;
                        _visitors.TryAdd(typeof(TExpression), typedVisitor);
                    }
                    return typedVisitor as IVisitor<TResult, TExpression>;
                }
            }
            
            return null;
        }

        /// <summary>
        /// Visits a downcasting <see cref="Variant{FromCandidate, ToCandidate}"/>.
        /// </summary>
        /// <returns>The result of the visit.</returns>
        /// <param name="target">Target to visit. The caller grants that this argument will never be <see langword="null"/>.</param>
        /// <param name="context">Context. The caller grants that this argument will never be <see langword="null"/>.</param>
        /// <typeparam name="FromCandidate">The wider (more abstract) type. 
        /// It corresponds to the type of objects that satisfies the <see cref="Variant{FromCandidate, ToCandidate}.InnerSpecification"/>.</typeparam>
        /// <typeparam name="ToCandidate">The stricter type, that specializes <typeparamref name="FromCandidate"/>.</typeparam>
        protected abstract TResult VisitDowncastingVariant<FromCandidate, ToCandidate>(Variant<FromCandidate, ToCandidate> target, IVisitContext context) where FromCandidate : class, TCandidate where ToCandidate : class, TCandidate, FromCandidate;

        /// <summary>
        /// Visits an upcasting <see cref="Variant{FromCandidate, ToCandidate}"/>.
        /// </summary>
        /// <returns>The result of the visit.</returns>
        /// <param name="target">Target to visit. The caller grants that this argument will never be <see langword="null"/>.</param>
        /// <param name="context">Context. The caller grants that this argument will never be <see langword="null"/>.</param>
        /// <typeparam name="FromCandidate">The stricter type, that specializes <typeparamref name="ToCandidate"/>.
        /// It corresponds to the type of objects that satisfies the <see cref="Variant{FromCandidate, ToCandidate}.InnerSpecification"/>.</typeparam>
        /// <typeparam name="ToCandidate">The wider (more abstract) type. </typeparam>
        protected abstract TResult VisitUpcastingVariant<FromCandidate, ToCandidate>(Variant<FromCandidate, ToCandidate> target, IVisitContext context) where FromCandidate : class, TCandidate, ToCandidate where ToCandidate : class, TCandidate;

        #region nested visitors
        private sealed class DowncastingVariantVisitor<FromCandidate, ToCandidate> : NestedVisitorBase<TResult, Variant<FromCandidate, ToCandidate>, VariantVisitorBase<TResult, TCandidate>> where FromCandidate : class, TCandidate where ToCandidate : class, TCandidate, FromCandidate
        {
            public DowncastingVariantVisitor(VariantVisitorBase<TResult, TCandidate> composition)
                : base(composition)
            {
            }

            #region implemented abstract members of NestedVisitorBase

            protected override TResult Visit(Variant<FromCandidate, ToCandidate> target, IVisitContext context, VariantVisitorBase<TResult, TCandidate> outerVisitor)
            {
                return outerVisitor.VisitDowncastingVariant(target, context);
            }

            #endregion
        }

        private sealed class UpcastingVariantVisitor<FromCandidate, ToCandidate> : NestedVisitorBase<TResult, Variant<FromCandidate, ToCandidate>, VariantVisitorBase<TResult, TCandidate>>  where FromCandidate : class, TCandidate, ToCandidate where ToCandidate : class, TCandidate
        {
            public UpcastingVariantVisitor(VariantVisitorBase<TResult, TCandidate> composition)
                : base(composition)
            {
            }
            
            #region implemented abstract members of NestedVisitorBase
            
            protected override TResult Visit(Variant<FromCandidate, ToCandidate> target, IVisitContext context, VariantVisitorBase<TResult, TCandidate> outerVisitor)
            {
                return outerVisitor.VisitUpcastingVariant(target, context);
            }
            
            #endregion
        }

        #endregion nested visitors
    }
}

