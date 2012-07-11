//
//  DisjunctionVisitor.cs
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
using Epic.Query.Relational.Predicates;
using Epic.Specifications;

namespace Epic.Query.Object.Relational.Visitors
{
    public sealed class DisjunctionVisitor<TEntity> : CompositeVisitor<Predicate>.VisitorBase, 
                                                      IVisitor<Predicate, Disjunction<TEntity>>
        where TEntity : class
    {
        public DisjunctionVisitor (CompositeVisitor<Predicate> composition)
            : base(composition)
        {
        }
        
        #region IVisitor implementation
        public Predicate Visit (Disjunction<TEntity> target, IVisitContext context)
        {
            Predicate[] predicates = new Predicate[target.NumberOfSpecifications];
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

