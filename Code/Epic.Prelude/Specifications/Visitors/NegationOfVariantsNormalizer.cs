//
//  NegationOfVariantsNormalizer.cs
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
    /// Translates any <see cref="Negation{TCandidate}"/> of a <see cref="Variant{FromCandidate,ToCandidate}"/> 
    /// to a <see cref="Variant{FromCandidate,ToCandidate}"/> of a <see cref="Negation{TCandidate}"/>.
    /// </summary>
    /// <typeparam name="TCandidate">Common ancestor of the candidate types handled by the <see cref="Variant{FromCandidate, ToCandidate}"/>.</typeparam>
    /// <seealso cref="Variant{FromCandidate, ToCandidate}"/>
    /// <seealso cref="Negation{TCandidate}"/>
    internal sealed class NegationOfVariantsNormalizer<TCandidate> : CompositeVisitor<ISpecification<TCandidate>>.VisitorBase, IVisitor<ISpecification<TCandidate>, IMonadicSpecificationComposition<TCandidate>>
        where TCandidate : class
    {
        private readonly ConcurrentDictionary<Type, IVisitor<ISpecification<TCandidate>>> _visitors = new ConcurrentDictionary<Type, IVisitor<ISpecification<TCandidate>>>();
        /// <summary>
        /// Initializes a new instance of the <see cref="VariantVisitorBase{TResult, TCandidate}"/> class.
        /// </summary>
        /// <param name="composition">Composition.</param>
        /// <exception cref="ArgumentNullException"><paramref name="composition"/> is <see langword="null"/>.</exception>
        public NegationOfVariantsNormalizer(CompositeVisitor<ISpecification<TCandidate>> composition)
            : base(composition)
        {
        }

        protected override IVisitor<ISpecification<TCandidate>, TExpression> AsVisitor<TExpression>(TExpression target)
        {
            IVisitor<ISpecification<TCandidate>, TExpression> result = base.AsVisitor(target);
            
            if (null != result)
            {
                var monadic = target as IMonadicSpecificationComposition<TCandidate>;
                if(   !(monadic.Operand is IMonadicSpecificationComposition<TCandidate>)                // if this is a Negation, the monadic inside can only be a Variant
                   || !monadic.SpecificationType.GetGenericTypeDefinition().Equals(typeof(Negation<>)))
                {
                    result = null;
                }
                else
                {
                    IVisitor<ISpecification<TCandidate>> typedVisitor = null;
                    if(!_visitors.TryGetValue(typeof(TExpression), out typedVisitor))
                    {
                        // no known visitor: we need one
                        Type[] fromToTypes = monadic.Operand.SpecificationType.GetGenericArguments();
                        Type visitorFactory;
                        if(fromToTypes[0].IsAssignableFrom(fromToTypes[1]))
                        {
                            // target is an upcasting variant
                            visitorFactory = typeof(NegationOfUpcastVisitor<,>).MakeGenericType(fromToTypes);
                        }
                        else
                        {
                            // target is an downcasting variant
                            visitorFactory = typeof(NegationOfDowncastVisitor<,>).MakeGenericType(fromToTypes);
                        }
                        typedVisitor = Activator.CreateInstance(visitorFactory, this) as IVisitor<ISpecification<TCandidate>>;
                        _visitors.TryAdd(typeof(TExpression), typedVisitor);
                    }
                    return typedVisitor as IVisitor<ISpecification<TCandidate>, TExpression>;
                }
            }
            
            return result;
        }

        #region IVisitor implementation

        ISpecification<TCandidate> IVisitor<ISpecification<TCandidate>, IMonadicSpecificationComposition<TCandidate>>.Visit(IMonadicSpecificationComposition<TCandidate> target, IVisitContext context)
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

        #region nested visitors
        private struct NegationOfUpcastVisitor<FromCandidate, ToCandidate> : IVisitor<ISpecification<TCandidate>, Negation<ToCandidate>> where FromCandidate : class, TCandidate, ToCandidate where ToCandidate : class, TCandidate
        {
            private readonly NegationOfVariantsNormalizer<TCandidate> _composition;
            public NegationOfUpcastVisitor(NegationOfVariantsNormalizer<TCandidate> composition)
            {
                _composition = composition;
            }
            #region IVisitor implementation
            ISpecification<TCandidate> IVisitor<ISpecification<TCandidate>, Negation<ToCandidate>>.Visit(Negation<ToCandidate> target, IVisitContext context)
            {
                Variant<FromCandidate, ToCandidate> upcasting = target.Negated as Variant<FromCandidate, ToCandidate>;
                if(upcasting.InnerSpecification is Any<FromCandidate>)
                {
                    // to be coherent with NegationOfDowncastVisitor
                    return upcasting.InnerSpecification.OfType<TCandidate>().Negate();
                }
                return _composition.VisitInner(upcasting.InnerSpecification.Negate(), context);
            }
            #endregion
            #region IVisitor implementation
            IVisitor<ISpecification<TCandidate>, TExpression> IVisitor<ISpecification<TCandidate>>.AsVisitor<TExpression>(TExpression target)
            {
                return (_composition as IVisitor<ISpecification<TCandidate>>).AsVisitor<TExpression>(target);
            }
            #endregion
        }

        private struct NegationOfDowncastVisitor<FromCandidate, ToCandidate> : IVisitor<ISpecification<TCandidate>, Negation<ToCandidate>> where FromCandidate : class, TCandidate where ToCandidate : class, TCandidate, FromCandidate
        {
            private readonly NegationOfVariantsNormalizer<TCandidate> _composition;
            public NegationOfDowncastVisitor(NegationOfVariantsNormalizer<TCandidate> composition)
            {
                _composition = composition;
            }
            #region IVisitor implementation
            ISpecification<TCandidate> IVisitor<ISpecification<TCandidate>, Negation<ToCandidate>>.Visit(Negation<ToCandidate> target, IVisitContext context)
            {
                Variant<FromCandidate, ToCandidate> downcasting = target.Negated as Variant<FromCandidate, ToCandidate>;
                var innerNegated = _composition.VisitInner(downcasting.InnerSpecification.Negate(), context);
                return Any<ToCandidate>.Specification.OfType<TCandidate>().Negate().Or(innerNegated); // by De Morgan's laws
            }
            #endregion
            #region IVisitor implementation
            IVisitor<ISpecification<TCandidate>, TExpression> IVisitor<ISpecification<TCandidate>>.AsVisitor<TExpression>(TExpression target)
            {
                return (_composition as IVisitor<ISpecification<TCandidate>>).AsVisitor<TExpression>(target);
            }
            #endregion
        }
        #endregion nested visitors
    }
}

