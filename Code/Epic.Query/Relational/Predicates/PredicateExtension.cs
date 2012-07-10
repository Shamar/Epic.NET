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
        public static Not Not(this Predicate predicate)
        {
            return new Not(predicate);
        }

        public static And And(this Predicate predicate, Predicate other)
        {
            return new And(predicate, other);
        }

        public static Or Or(this Predicate predicate, Predicate other)
        {
            return new Or(predicate, other);
        }

        public static Equal Equal(this Scalar scalar, Scalar other)
        {
            return new Equal(scalar, other);
        }

        public static Greater Greater(this Scalar scalar, Scalar other)
        {
            return new Greater(scalar, other);
        }

        public static Less Less(this Scalar scalar, Scalar other)
        {
            return new Less(scalar, other);
        }

    }
}

