//
//  DeMorganLaws.cs
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
    /// Applies De Morgan laws to obtain a Negation normal form to a <seealso cref="ISpecification{TCandidate}"/>.
    /// </summary>
    /// <typeparam name="TCandidate">Type of the candidate.</typeparam>
    /// <seealso href="http://en.wikipedia.org/wiki/Negation_normal_form"/>
    internal sealed class DeMorganLaws<TCandidate> : CompositeVisitor<ISpecification<TCandidate>>.VisitorBase, IVisitor<ISpecification<TCandidate>, IMonadicSpecificationComposition<TCandidate>>
        where TCandidate : class
    {
        private readonly ConcurrentDictionary<Type, IVisitor<ISpecification<TCandidate>>> _orDistributors = new ConcurrentDictionary<Type, IVisitor<ISpecification<TCandidate>>>();
        private readonly ConcurrentDictionary<Type, IVisitor<ISpecification<TCandidate>>> _andDistributors = new ConcurrentDictionary<Type, IVisitor<ISpecification<TCandidate>>>();
        public DeMorganLaws(CompositeVisitor<ISpecification<TCandidate>> composition)
            : base(composition)
        {
        }

        protected override IVisitor<ISpecification<TCandidate>, TExpression> AsVisitor<TExpression>(TExpression target)
        {
            IVisitor<ISpecification<TCandidate>, TExpression> result = base.AsVisitor(target);

            if (null != result)
            {
                var monadic = target as IMonadicSpecificationComposition<TCandidate>;
                if(!(monadic.Operand is IPolyadicSpecificationComposition<TCandidate>) || !monadic.SpecificationType.GetGenericTypeDefinition().Equals(typeof(Negation<>)))
                    result = null;
                ConcurrentDictionary<Type, IVisitor<ISpecification<TCandidate>>> distributorsToLookup = null;
                Type[] candidateTypes = monadic.SpecificationType.GetGenericArguments();
                if(monadic.Operand.SpecificationType.GetGenericTypeDefinition().Equals(typeof(Disjunction<>)))
                {
                    distributorsToLookup = _orDistributors;
                }
                else
                {
                    distributorsToLookup = _andDistributors;
                }
                IVisitor<ISpecification<TCandidate>> visitor = null;
                if(!distributorsToLookup.TryGetValue(typeof(TExpression), out visitor))
                {
                    if(distributorsToLookup == _orDistributors)
                    {
                        visitor = Activator.CreateInstance(typeof(NegatedDisjunctionNormalizer<>).MakeGenericType(candidateTypes), this) as IVisitor<ISpecification<TCandidate>>;
                    }
                    else
                    {
                        visitor = Activator.CreateInstance(typeof(NegatedConjunctionNormalizer<>).MakeGenericType(candidateTypes), this) as IVisitor<ISpecification<TCandidate>>;
                    }
                    distributorsToLookup.TryAdd(typeof(TExpression), visitor);
                }
                return visitor as IVisitor<ISpecification<TCandidate>, TExpression>;
            }

            return result;
        }


        #region IVisitor implementation
        ISpecification IVisitor<ISpecification<TCandidate>, IMonadicSpecificationComposition<TCandidate>>.Visit(IMonadicSpecificationComposition<TCandidate> target, IVisitContext context)
        {
            // this should never happen
            Type[] fromToTypes = target.SpecificationType.GetGenericArguments();
            string visitMethod = fromToTypes[0].IsAssignableFrom(fromToTypes[1]) ? "Downcasting" : "Upcasting"; 
            string message = string.Format("The composition '{1}' has a bug: it reached the DeMorganLaws<{0}>.Visit() method instead of calling the correct normalizing visitor.",
                                           typeof(TCandidate),
                                           CompositionName);
            throw new EpicException(message);
        }
        #endregion

        #region nested visitors
        private struct NegatedConjunctionNormalizer<Candidate> : IVisitor<ISpecification<TCandidate>, Negation<Candidate>> where Candidate : class, TCandidate
        {
            private readonly DeMorganLaws<TCandidate> _composition;
            public NegatedConjunctionNormalizer(DeMorganLaws<TCandidate> composition)
            {
                _composition = composition;
            }

            #region IVisitor implementation
            public ISpecification<TCandidate> Visit(Negation<Candidate> target, IVisitContext context)
            {
                Conjunction<Candidate> operand = target.Negated as Conjunction<Candidate>;
                ISpecification<Candidate> result = null;
                foreach(ISpecification<Candidate> specificationToVisit in operand)
                {
                    ISpecification<TCandidate> specification = _composition.VisitInner(specificationToVisit, context);
                    if(null == result)
                    {
                        result = specification.Negate();
                    }
                    else
                    {
                        result = result.Or(specification.Negate());
                    }
                }
                return result;
            }
            #endregion
            #region IVisitor implementation
            public IVisitor<ISpecification<TCandidate>, TExpression> AsVisitor<TExpression>(TExpression target) where TExpression : class
            {
                return (_composition as IVisitor<ISpecification<TCandidate>>).AsVisitor(target);
            }
            #endregion
        }

        private struct NegatedDisjunctionNormalizer<Candidate> : IVisitor<ISpecification<TCandidate>, Negation<Candidate>> where Candidate : class, TCandidate
        {
            private readonly DeMorganLaws<TCandidate> _composition;
            public NegatedDisjunctionNormalizer(DeMorganLaws<TCandidate> composition)
            {
                _composition = composition;
            }
            
            #region IVisitor implementation
            public ISpecification<TCandidate> Visit(Negation<Candidate> target, IVisitContext context)
            {
                var operand = target.Negated as Disjunction<Candidate>;
                ISpecification<Candidate> result = null;
                foreach(ISpecification<Candidate> specificationToVisit in operand)
                {
                    ISpecification<TCandidate> specification = _composition.VisitInner(specificationToVisit, context);
                    if(null == result)
                    {
                        result = specification.Negate();
                    }
                    else
                    {
                        result = result.And(specification.Negate());
                    }
                }
                return result;
            }
            #endregion
            #region IVisitor implementation
            public IVisitor<ISpecification<TCandidate>, TExpression> AsVisitor<TExpression>(TExpression target) where TExpression : class
            {
                return (_composition as IVisitor<ISpecification<TCandidate>>).AsVisitor(target);
            }
            #endregion
        }
        #endregion nested visitors
    }
}

