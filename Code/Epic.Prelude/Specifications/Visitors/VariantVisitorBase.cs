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

namespace Epic.Specifications.Visitors
{
    /// <summary>
    /// Base class for visitors of <see cref="Variant{FromCandidate, ToCandidate}"/>.
    /// </summary>
    /// <typeparam name="TResult">Type of the result produced from the visits.</typeparam>
    /// <typeparam name="TCandidate">Common ancestor of the candidate types handled by the <see cref="Variant{FromCandidate, ToCandidate}"/>.</typeparam>
    /// <seealso cref="Variant{FromCandidate, ToCandidate}"/>
    public abstract class VariantVisitorBase<TResult, TCandidate> : CompositeVisitor<TResult>.VisitorBase, IVisitor<TResult, IMonadicSpecificationComposition<TCandidate>>
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
            IVisitor<TResult, TExpression> result = base.AsVisitor(target);
            
            if (null != result)
            {
                var monadic = target as IMonadicSpecificationComposition<TCandidate>;
                if(monadic.SpecificationType.GetGenericTypeDefinition().Equals(typeof(Variant<,>)))
                {
                    IVisitor<TResult> typedVisitor = null;
                    if(!_visitors.TryGetValue(typeof(TExpression), out typedVisitor))
                    {
                        // no known visitor: we need one
                        Type[] fromToTypes = monadic.SpecificationType.GetGenericArguments();
                        Type visitorFactory;
                        if(fromToTypes[0].IsAssignableFrom(fromToTypes[1]))
                        {
                            // target is an upcasting specification
                            visitorFactory = typeof(UpcastingVariantVisitor<,>).MakeGenericType(fromToTypes);
                        }
                        else
                        {
                            // target is an upcasting specification
                            visitorFactory = typeof(DowncastingVariantVisitor<,>).MakeGenericType(fromToTypes);
                        }
                        typedVisitor = Activator.CreateInstance(visitorFactory, this) as IVisitor<TResult>;
                        _visitors.TryAdd(typeof(TExpression), typedVisitor);
                    }
                    return typedVisitor as IVisitor<TResult, TExpression>;
                }
            }
            
            return result;
        }

        #region IVisitor implementation

        TResult IVisitor<TResult, IMonadicSpecificationComposition<TCandidate>>.Visit(IMonadicSpecificationComposition<TCandidate> target, IVisitContext context)
        {
            // this should never happen
            Type[] fromToTypes = target.SpecificationType.GetGenericArguments();
            string visitMethod = fromToTypes[0].IsAssignableFrom(fromToTypes[1]) ? "Downcasting" : "Upcasting"; 
            string message = string.Format("The visitor of type {0} in the composition '{1}' has a bug: it reached the Visit() method instead of calling {2}Visitor<{3}, {4}>.",
                                           this.GetType(),
                                           CompositionName,
                                           visitMethod,
                                           fromToTypes[0],
                                           fromToTypes[1]);
            throw new EpicException(message);
        }

        #endregion

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
        private sealed class DowncastingVariantVisitor<FromCandidate, ToCandidate> : IVisitor<TResult, Variant<FromCandidate, ToCandidate>> where FromCandidate : class, TCandidate where ToCandidate : class, TCandidate, FromCandidate
        {
            private readonly VariantVisitorBase<TResult, TCandidate> _composition;
            public DowncastingVariantVisitor(VariantVisitorBase<TResult, TCandidate> composition)
            {
                _composition = composition;
            }
            #region IVisitor implementation
            TResult IVisitor<TResult, Variant<FromCandidate, ToCandidate>>.Visit(Variant<FromCandidate, ToCandidate> target, IVisitContext context)
            {
                return _composition.VisitDowncastingVariant(target, context);
            }
            #endregion
            #region IVisitor implementation
            IVisitor<TResult, TExpression> IVisitor<TResult>.AsVisitor<TExpression>(TExpression target)
            {
                IVisitor<TResult, TExpression> result = this as IVisitor<TResult, TExpression>;
                if (null != result)
                    return result;
                return _composition.AsVisitor(target);
            }
            #endregion
        }

        private sealed class UpcastingVariantVisitor<FromCandidate, ToCandidate> : IVisitor<TResult, Variant<FromCandidate, ToCandidate>>  where FromCandidate : class, TCandidate, ToCandidate where ToCandidate : class, TCandidate
        {
            private readonly VariantVisitorBase<TResult, TCandidate> _composition;
            public UpcastingVariantVisitor(VariantVisitorBase<TResult, TCandidate> composition)
            {
                _composition = composition;
            }
            #region IVisitor implementation
            TResult IVisitor<TResult, Variant<FromCandidate, ToCandidate>>.Visit(Variant<FromCandidate, ToCandidate> target, IVisitContext context)
            {
                return _composition.VisitUpcastingVariant(target, context);
            }
            #endregion
            #region IVisitor implementation
            IVisitor<TResult, TExpression> IVisitor<TResult>.AsVisitor<TExpression>(TExpression target)
            {
                IVisitor<TResult, TExpression> result = this as IVisitor<TResult, TExpression>;
                if (null != result)
                    return result;
                return _composition.AsVisitor(target);
            }
            #endregion
        }

        #endregion nested visitors
    }
}

