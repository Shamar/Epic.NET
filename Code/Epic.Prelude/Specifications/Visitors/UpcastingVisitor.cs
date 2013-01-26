//
//  UpcastingVisitor.cs
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
    /// This visitor uniforms the specifications it recieves to <typeparamref name="TCandidate"/> (that must be the root of the type hierarchy).
    /// </summary>
    /// <typeparam name="TCandidate">Root of the types of candidates that the visited specifications can handle.</typeparam>
    internal sealed class UpcastingVisitor<TCandidate> : CompositeVisitor<ISpecification<TCandidate>>.VisitorBase, IVisitor<ISpecification<TCandidate>, ISpecification>
        where TCandidate : class
    {
        public static bool SpecificationInterfaceFilter(Type typeObj, Object criteriaObj)
        {
            if (   typeObj.IsGenericType
                && typeof(ISpecification).IsAssignableFrom(typeObj) 
                && typeof(ISpecification<>).Equals(typeObj.GetGenericTypeDefinition())
                && typeof(TCandidate).IsAssignableFrom(typeObj.GetGenericArguments()[0]))
            {
                return true;
            }
            return false;
        }

        private readonly ConcurrentDictionary<Type, IVisitor<ISpecification<TCandidate>>> _visitors = new ConcurrentDictionary<Type, IVisitor<ISpecification<TCandidate>>>();
        public UpcastingVisitor(CompositeVisitor<ISpecification<TCandidate>> composition)
            : base(composition)
        {
        }

        protected override IVisitor<ISpecification<TCandidate>, TExpression> AsVisitor<TExpression>(TExpression target)
        {
            IVisitor<ISpecification<TCandidate>, TExpression> visitor = base.AsVisitor(target);

            if(null != visitor)
            {
                if(target is ISpecification<TCandidate>)
                    return visitor; // here it works like an EchoingVisitor<ISpecification<TCandidate>, ISpecification<TCandidate>> 

                IVisitor<ISpecification<TCandidate>> typedVisitor;
                if(!_visitors.TryGetValue(typeof(TExpression), out typedVisitor))
                {
                    Type[] specificationsImplemented = typeof(TExpression).FindInterfaces(SpecificationInterfaceFilter, null);
                    if(specificationsImplemented.Length > 1)
                    {
                        string message = string.Format("The DNFConverter named '{2}' cannot handle specifications that implements ISpecification<TCandidate> multiple times with type of candidates that belong to the same type hierarchy. You must provide your own DNF converter deriving CompositeVisitor<{1}>.VisitorBase for {0}.", typeof(TExpression), typeof(TCandidate), base.CompositionName);
                        throw new EpicException(message);
                    }
                    typedVisitor = Activator.CreateInstance(typeof(CorrectlyTypedVisitor<>).MakeGenericType(typeof(TCandidate), specificationsImplemented[0].GetGenericArguments()[0]), this) as IVisitor<ISpecification<TCandidate>>;
                    _visitors.TryAdd(typeof(TExpression), typedVisitor);
                }
                return typedVisitor as IVisitor<ISpecification<TCandidate>, TExpression>;
            }

            return visitor;
        }

        #region IVisitor implementation
        
        public ISpecification<TCandidate> Visit(ISpecification target, IVisitContext context)
        {
            IMonadicSpecificationComposition<TCandidate> variant = target is Negation<TCandidate> ? null : target as IMonadicSpecificationComposition<TCandidate>;
            if(null != variant)
            {
                return VisitInner(variant.Operand, context);
            }
            return target as ISpecification<TCandidate>; // here it works like an EchoingVisitor<ISpecification<TCandidate>, ISpecification<TCandidate>> (see AsVisitor implementation)
        }
        
        #endregion

        private struct CorrectlyTypedVisitor<CandidateToUpcast> : IVisitor<ISpecification<TCandidate>, ISpecification<CandidateToUpcast>>
            where CandidateToUpcast : class, TCandidate
        {
            private readonly UpcastingVisitor<TCandidate> _composition;
            public CorrectlyTypedVisitor(UpcastingVisitor<TCandidate> composition)
            {
                _composition = composition;
            }

            #region IVisitor implementation

            public ISpecification<TCandidate> Visit(ISpecification<CandidateToUpcast> target, IVisitContext context)
            {
                ISpecification<TCandidate> result = null;
                Conjunction<CandidateToUpcast> conjunction = target as Conjunction<CandidateToUpcast>;
                Disjunction<CandidateToUpcast> disjunction = target as Disjunction<CandidateToUpcast>;
                Negation<CandidateToUpcast> negation = target as Negation<CandidateToUpcast>;
                IMonadicSpecificationComposition<TCandidate> variant = null != negation ? null : target as IMonadicSpecificationComposition<TCandidate>;
                if (null != conjunction)
                {
                    foreach(ISpecification<CandidateToUpcast> unvisitedInner in conjunction)
                    {
                        ISpecification<TCandidate> inner = _composition.VisitInner(unvisitedInner, context);
                        if(null == result)
                            result = inner;
                        else
                            result = result.And(inner);
                    }
                }
                else if (null != disjunction)
                {
                    foreach(ISpecification<CandidateToUpcast> unvisitedInner in disjunction)
                    {
                        ISpecification<TCandidate> inner = _composition.VisitInner(unvisitedInner, context);
                        if(null == result)
                            result = inner;
                        else
                            result = result.Or(inner);
                    }
                }
                else if (null != negation)
                {
                    ISpecification<TCandidate> inner = _composition.VisitInner(negation.Negated, context);
                    result = inner.Negate();
                }
                else if(null != variant)
                {
                    result = _composition.VisitInner(variant.Operand, context);
                }
                else
                {
                    result = target.OfType<TCandidate>();
                }

                return result; // TODO: tail call here
            }

            #endregion

            #region IVisitor implementation

            public IVisitor<ISpecification<TCandidate>, TExpression> AsVisitor<TExpression>(TExpression target) where TExpression : class
            {
                return (_composition as IVisitor<ISpecification<TCandidate>>).AsVisitor<TExpression>(target);
            }

            #endregion
        }
    }
}

