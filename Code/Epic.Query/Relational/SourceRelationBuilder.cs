//
//  SourceRelationBuilder.cs
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
using System.Collections.Generic;
using Epic.Query.Relational.Operations;

namespace Epic.Query.Relational
{
    /// <summary>
    /// Builder of the source relation in a relational query designed to extract entities. 
    /// </summary>
    public sealed class SourceRelationBuilder
    {
        private readonly RelationalExpression _mainRelation;
        private readonly List<Func<RelationalExpression, RelationalExpression>> _expressionFactory;
        private readonly HashSet<RelationalExpression> _relations;
        
        /// <summary>
        /// Initializes a new <see cref="SourceRelationBuilder"/>.
        /// </summary>
        /// <param name="mainRelation">Main <see cref="RelationalExpression"/> that holds at least the identity (and
        /// the type) of the entity.</param>
        public SourceRelationBuilder (RelationalExpression mainRelation)
        {
            if (mainRelation == null)
                throw new ArgumentNullException ("mainRelation");
            _mainRelation = mainRelation;
            _expressionFactory = new List<Func<RelationalExpression, RelationalExpression>>();
            _relations = new HashSet<RelationalExpression>();
        }

        /// <summary>
        /// Defines an inner natural join between the current source and the <paramref name="relation"/>.
        /// </summary>
        /// <param name="relation">Relation to join.</param>
        /// <param name="attributes">Join attributes.</param>
        public void InnerNaturalJoin(RelationalExpression relation, params string[] attributes)
        {
            if (relation == null)
                throw new ArgumentNullException ("relation");
            if (attributes == null || attributes.Length == 0)
                throw new ArgumentNullException ("attributes");
            if(_relations.Add(relation))
            {
                _expressionFactory.Add(previous => BuildInnerNaturalJoin(previous, relation, attributes));
            }
        }

        /// <summary>
        /// Defines a cross product between the current source and <paramref name="relation"/>.
        /// </summary>
        /// <param name="relation">Relation to cross product.</param>
        public void CrossProduct(RelationalExpression relation)
        {
            if (relation == null)
                throw new ArgumentNullException ("relation");
            if(_relations.Add(relation))
            {
                //_expressionFactory.Add(previous => BuildCrossProduct(previous, relation));
            }
        }

        private static RelationalExpression BuildInnerNaturalJoin(RelationalExpression left, RelationalExpression right, string[] attributes)
        {
            Predicate predicate = null;
            for(int i = 0; i < attributes.Length; ++i)
            {
                string attributeName = attributes[i];
                if(null == predicate)
                {
                    predicate = new Equal(new RelationAttribute(attributeName, left),
                                          new RelationAttribute(attributeName, right));
                }
                else
                {
                    predicate = new And(predicate, new Equal(new RelationAttribute(attributeName, left),
                                                             new RelationAttribute(attributeName, right))
                                       );
                }
            }
            return new ThetaJoin(left, right, predicate);
        }

        /// <summary>
        /// Produces the <see cref="RelationalExpression"/> that express the source relation of a query.
        /// </summary>
        /// <returns>The <see cref="RelationalExpression"/> that express the source relation of a query.</returns>
        public RelationalExpression ToRelation()
        {
            RelationalExpression result = _mainRelation;
            foreach(var operation in _expressionFactory)
            {
                result = operation(result);
            }
            return result;
        }
    }
}

