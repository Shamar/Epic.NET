//
//  ConjunctionVisitor.cs
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
using System.Linq;
using Epic.Query.Relational.Predicates;
using Epic.Specifications;

namespace Epic.Query.Object.Relational.Visitors
{
    /// <summary>
    /// Visitor of a <see cref="Conjunction{TEntity}"/> that produce a <see cref="Predicate"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    public sealed class ConjunctionVisitor<TEntity> : CompositeVisitor<Predicate>.VisitorBase, 
                                                      IVisitor<Predicate, Conjunction<TEntity>>
        where TEntity : class
    {
        /// <summary>
        /// Initialize a new <see cref="ConjunctionVisitor{TEntity}"/> as part of the <paramref name="composition"/>.
        /// </summary>
        /// <param name="composition">Composite visitor to enhance.</param>
        public ConjunctionVisitor(CompositeVisitor<Predicate> composition)
            : base(composition)
        {
        }

        #region IVisitor implementation
        Predicate IVisitor<Predicate, Conjunction<TEntity>>.Visit (Conjunction<TEntity> target, IVisitContext context)
        {
            Predicate[] predicates = new Predicate[target.Count()];
            int i = 0;
            foreach(ISpecification<TEntity> specification in target)
            {
                predicates[i++] = VisitInner(specification, context);
            }
            Predicate result = null;
            for(int j = predicates.Length - 1; j >= 0; --j)
            {
                Predicate toAdd = predicates[j];
                if(null != toAdd)
                {
                    if(null == result)
                    {
                        result = toAdd;
                    }
                    else
                    {
                        result = new And(toAdd, result);
                    }
                }
            }
            return result;
        }
        #endregion
    }
}

