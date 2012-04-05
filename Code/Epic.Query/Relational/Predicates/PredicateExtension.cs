//  
//  PredicateExtension.cs
//  
//  Author:
//       Marco Veglio <m.veglio@gmail.com>
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

namespace Epic.Query.Relational.Predicates
{
    public static class PredicateExtension
    {
        public static Not<TPredicate> Not<TPredicate>(this TPredicate predicate) where TPredicate: Predicate
        {
            return new Not<TPredicate>(predicate);
        }

        public static And<TPredicate1, TPredicate2> And<TPredicate1, TPredicate2>(this TPredicate1 predicate, TPredicate2 other)
            where TPredicate1: Predicate where TPredicate2: Predicate
        {
            return new And<TPredicate1, TPredicate2>(predicate, other);
        }

        public static Or<TPredicate1, TPredicate2> Or<TPredicate1, TPredicate2>(this TPredicate1 predicate, TPredicate2 other)
            where TPredicate1: Predicate where TPredicate2: Predicate
        {
            return new Or<TPredicate1, TPredicate2>(predicate, other);
        }

        public static Equal<TScalar1, TScalar2> Equal<TScalar1, TScalar2>(this TScalar1 scalar, TScalar2 other)
            where TScalar1: Scalar
            where TScalar2: Scalar
        {
            return new Equal<TScalar1, TScalar2>(scalar, other);
        }

        public static Greater<TScalar1, TScalar2> Greater<TScalar1, TScalar2>(this TScalar1 scalar, TScalar2 other)
            where TScalar1: Scalar
            where TScalar2: Scalar
        {
            return new Greater<TScalar1, TScalar2>(scalar, other);
        }

        public static Less<TScalar1, TScalar2> Less<TScalar1, TScalar2>(this TScalar1 scalar, TScalar2 other)
            where TScalar1: Scalar
            where TScalar2: Scalar
        {
            return new Less<TScalar1, TScalar2>(scalar, other);
        }

    }
}

