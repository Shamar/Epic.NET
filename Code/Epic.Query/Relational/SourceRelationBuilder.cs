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
    public sealed class SourceRelationBuilder
    {
        public readonly Relation _mainRelation;
        public readonly List<Func<Relation, Relation>> _expressionFactory;
        public readonly HashSet<Relation> _relations;
        public SourceRelationBuilder (Relation mainRelation)
        {
            if (mainRelation == null)
                throw new ArgumentNullException ("mainRelation");
            _mainRelation = mainRelation;
            _expressionFactory = new List<Func<Relation, Relation>>();
            _relations = new HashSet<Relation>();
        }

        public void InnerNaturalJoin(Relation relation, params string[] attributes)
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

        public void CrossProduct(Relation relation)
        {
            if (relation == null)
                throw new ArgumentNullException ("relation");
            if(_relations.Add(relation))
            {
                //_expressionFactory.Add(previous => BuildCrossProduct(previous, relation));
            }
        }

        private static Relation BuildInnerNaturalJoin(Relation left, Relation right, string[] attributes)
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
            return new InnerJoin(left, right, predicate, "deleteMe");
        }

        public Relation ToRelation()
        {
            Relation result = _mainRelation;
            foreach(var operation in _expressionFactory)
            {
                result = operation(result);
            }
            return result;
        }
    }
}

