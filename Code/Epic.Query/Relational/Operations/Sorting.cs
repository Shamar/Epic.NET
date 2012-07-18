//
//  Sorting.cs
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
using System.Collections.Generic;

namespace Epic.Query.Relational.Operations
{
    [Serializable]
    public sealed class Sorting : Relation, IEquatable<Sorting>
    {
        [Serializable]
        public abstract class Direction : VisitableBase, IEquatable<Direction>
        {
            private readonly Scalar _property;
            internal Direction(Scalar property)
            {
                if (null == property)
                    throw new ArgumentNullException("property");
                _property = property;
            }

            public Scalar Property
            {
                get
                {
                    return _property;
                }
            }

            #region IEquatable implementation
            public bool Equals (Direction other)
            {
                if(null == other)
                    return false;
                if(this == other)
                    return true;
                return _property.Equals(other._property) && this.GetType().Equals(other.GetType());
            }
            #endregion
        }

        [Serializable]
        public sealed class Ascending : Direction
        {
            internal Ascending(Scalar property)
                : base(property)
            {
            }

            public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
            {
                return AcceptMe(this, visitor, context);
            }
        }

        [Serializable]
        public sealed class Descending : Direction
        {
            internal Descending(Scalar property)
                : base(property)
            {
            }
            
            public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
            {
                return AcceptMe(this, visitor, context);
            }
        }

        public static Ascending ByAscending(RelationAttribute attribute)
        {
            return new Ascending(attribute);
        }

        public static Descending ByDescending(RelationAttribute attribute)
        {
            return new Descending(attribute);
        }

        public static Ascending ByAscending(ScalarFunction function)
        {
            return new Ascending(function);
        }
        
        public static Descending ByDescending(ScalarFunction function)
        {
            return new Descending(function);
        }
        
        private readonly Relation _relation;

        private readonly Direction[] _directions;

        public Sorting (Relation relation, params Direction[] directions)
            : base(ThrowOrReturn(relation, r => r.Type), ThrowOrReturn(relation, r => r.Name))
        {
            if (null == directions || directions.Length == 0)
                throw new ArgumentNullException("directions");
            if (directions.Length > 1)
            {
                for(int i = 0; i < directions.Length - 1; ++i)
                {
                    for(int j = i + 1; j < directions.Length; ++j)
                    {
                        if (directions[i].Property.Equals(directions[j].Property))
                        {
                            string message = string.Format("The direction at {0} and the direction {1} use the same attribute {2}.", i, j, directions[i].Property);
                            throw new ArgumentException(message, "directions");
                        }
                    }
                }
            }
            _relation = relation;
            _directions = directions;
        }

        public Relation Relation
        {
            get
            {
                return _relation;
            }
        }

        public IEnumerable<Direction> Directions
        {
            get
            {
                return _directions;
            }
        }

        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe(this, visitor, context);
        }

        #region IEquatable implementation
        public override bool Equals (Relation other)
        {
            return Equals(other as Sorting);
        }

        public bool Equals (Sorting other)
        {
            if(null == other)
                return false;
            if(this == other)
                return true;
            if(!_relation.Equals(other._relation) || _directions.Length != other._directions.Length)
                return false;
            for(int i = 0; i < _directions.Length; ++i)
            {
                if(!_directions[i].Equals(other._directions[i]))
                    return false;
            }
            return true;
        }
        #endregion

        private static T ThrowOrReturn<T>(Relation relation, Func<Relation, T> readingDelegate)
        {
            if (null == relation)
                throw new ArgumentNullException("relation");
            return readingDelegate(relation);
        }
    }
}

