//
//  DisjunctionVisitor.cs
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
using Epic.Query.Relational;
using Epic.Specifications;
using System.Collections.Generic;
using Epic.Query.Relational.Operations;

namespace Epic.Query.Object.Relational.Visitors
{
    /// <summary>
    /// Visitor of a <see cref="Disjunction{TEntity}"/> that produce a <see cref="RelationalExpression"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    public sealed class DisjunctionVisitor<TEntity> : CompositeVisitor<RelationalExpression>.VisitorBase, 
                                                      IVisitor<RelationalExpression, IPolyadicSpecificationComposition<TEntity>>
        where TEntity : class
    {
        /// <summary>
        /// Initialize a new <see cref="DisjunctionVisitor{TEntity}"/> as part of the <paramref name="composition"/>.
        /// </summary>
        /// <param name="composition">Composite visitor to enhance.</param>
        public DisjunctionVisitor(CompositeVisitor<RelationalExpression> composition)
            : base(composition)
        {
        }

        /// <summary>
        /// Returns the current visitor if the target 
        /// </summary>
        /// <returns>The current visitor or <see langword="null"/> if <paramref name="target"/> is not a disjunction.</returns>
        /// <param name="target">Expression to visit.</param>
        /// <typeparam name="TExpression">The type of the expression to visit.</typeparam>
        protected override IVisitor<RelationalExpression, TExpression> AsVisitor<TExpression>(TExpression target)
        {
            IVisitor<RelationalExpression, TExpression> result = base.AsVisitor(target);
            if (null != result)
            {
                var composition = target as IPolyadicSpecificationComposition<TEntity>;
                if(!composition.SpecificationType.GetGenericTypeDefinition().Equals(typeof(Disjunction<>)))
                    result = null;
            }
            return result;
        }
        
        #region IVisitor implementation
        RelationalExpression IVisitor<RelationalExpression, IPolyadicSpecificationComposition<TEntity>>.Visit (IPolyadicSpecificationComposition<TEntity> target, IVisitContext context)
        {
            RelationalExpression[] relations = new RelationalExpression[target.Operands.Count()];
            int i = 0;
            foreach(ISpecification<TEntity> specification in target.Operands)
            {
                relations[i++] = VisitInner(specification, context);
            }

            RelationalExpression result = null;
            for(int j = relations.Length - 1; j >= 0; --j)
            {
                RelationalExpression toAdd = relations[j];
                if(null != toAdd)
                {
                    if(null == result)
                    {
                        result = toAdd;
                    }
                    else
                    {
                        result = new Union(toAdd, result);
                    }
                }
            }

            return result;
        }
        #endregion
    }
}

