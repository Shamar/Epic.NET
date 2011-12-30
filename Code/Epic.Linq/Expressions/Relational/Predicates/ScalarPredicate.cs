//  
//  ScalarPredicate.cs
//  
//  Author:
//       Marco Veglio <m.veglio@gmail.com>
// 
//  Copyright (c) 2010-2011 Giacomo Tesio
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

namespace Epic.Linq.Expressions.Relational
{
    [Serializable]
    public abstract class ScalarPredicate<TScalar1, TScalar2>: Predicate, IEquatable<ScalarPredicate<TScalar1, TScalar2>>
        where TScalar1: Scalar
        where TScalar2: Scalar
    {
        TScalar1 _left;
        TScalar2 _right;

        internal protected ScalarPredicate (TScalar1 leftOperand, TScalar2 rightOperand)
        {
            this._left = leftOperand;
            this._right = rightOperand;
        }

        public TScalar1 Left { get { return this._left; } }

        public TScalar2 Right { get { return this._right; } }

        public abstract bool Equals(ScalarPredicate<TScalar1, TScalar2> other);

        public override bool Equals (Predicate other)
        {
            return Equals(other as ScalarPredicate<TScalar1, TScalar2>);
        }

        public override int GetHashCode ()
        {
            return _left.GetHashCode() ^ _right.GetHashCode () ^ GetType ().GetHashCode ();
        }
    }
}

