//
//  GenericSpecificationVisitor.cs
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
using Epic.Specifications;
using Epic.Query.Relational.Predicates;

namespace Epic.Query.Object.Relational.Visitors
{
    /// <summary>
    /// Generic visitor for specifications. 
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    /// <typeparam name="TSpecification">Type of the specification.</typeparam>
    public sealed class GenericSpecificationVisitor<TEntity, TSpecification> : CompositeVisitor<Predicate>.VisitorBase,
    IVisitor<Predicate, TSpecification>
    where TEntity : class
    where TSpecification : class, ISpecification<TEntity>
    {
        private readonly Func<TSpecification, Predicate> _predicateBuilder;
        
        /// <summary>
        /// Initializes a new <see cref="GenericSpecificationVisitor{TEntity, TSpecification}"/> as part of the <paramref name="composition"/>.
        /// </summary>
        /// <param name="composition">Composite visitor to enhance.</param>
        /// <param name="predicateBuilder"></param>
        public GenericSpecificationVisitor (PredicateBuilder composition, Func<TSpecification, Predicate> predicateBuilder)
            : base(composition)
        {
            if (null == predicateBuilder)
                throw new ArgumentNullException("predicateBuilder");
            _predicateBuilder = predicateBuilder;
        }

        #region IVisitor implementation

        Predicate IVisitor<Predicate, TSpecification>.Visit (TSpecification target, IVisitContext context)
        {
            return _predicateBuilder(target);
        }

        #endregion
    }
}

