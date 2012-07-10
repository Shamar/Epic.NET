//
//  QueryBuilder.cs
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
    public class QueryBuilder
    {
        public readonly Relation _mainRelation;
        public readonly Dictionary<Relation, Predicate> _registeredJoins;
        public QueryBuilder (Relation mainRelation)
        {
            if (mainRelation == null)
                throw new ArgumentNullException ("mainRelation");
            _mainRelation = mainRelation;
            _registeredJoins = new Dictionary<Relation, Predicate>();
        }

        public void InnerNaturalJoin(Relation relation, Predicate joinCondition)
        {
            if (relation == null)
                throw new ArgumentNullException ("relation");
            if (joinCondition == null)
                throw new ArgumentNullException ("joinCondition");
            _registeredJoins[relation] = joinCondition;
        }

        public void Join(QueryBuilder subquery, string name, Predicate joinCondition)
        {
            if (subquery == null)
                throw new ArgumentNullException ("subquery");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (joinCondition == null)
                throw new ArgumentNullException ("joinCondition");
            _registeredJoins[new Rename(subquery.ToRelation(), name)] = joinCondition;
        }

        public Relation ToRelation()
        {
            Relation result = _mainRelation;
            foreach(KeyValuePair<Relation, Predicate> kvp in _registeredJoins)
            {
                result = new InnerJoin(result, kvp.Key, kvp.Value);
            }
            return result;
        }
    }
}

