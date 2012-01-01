//  
//  FakePredicate.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
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
using Epic.Linq.Expressions.Relational;
using Epic.Linq.Expressions.Relational.Predicates;

namespace Epic.Linq.Fakes
{
    [Serializable]
    public class FakePredicate : Predicate
    {
        private static int counter = 0;
        private int id;

        public FakePredicate ()
        {
         id = ++counter;
        }

        public FakePredicate(int id)
        {
            this.id = id;
        }
        #region implemented abstract members of Epic.Linq.Expressions.Relational.Predicate
        public override bool Equals (Predicate other)
        {
            return Equals (other as FakePredicate);
        }
        #endregion

        public bool Equals(FakePredicate other)
        {
            if (other == null) return false;
            return this.id == other.id;
        }
        
        public override TResult Accept<TResult> (Epic.Linq.Expressions.IVisitor<TResult> visitor, Epic.Linq.Expressions.IVisitContext context)
        {
            return AcceptMe(this, visitor, context);
        }

        public override int GetHashCode ()
        {
            return id ^ GetType ().GetHashCode ();
        }
    }
    
    public class DerivedFakePredicate : FakePredicate
    {
        public DerivedFakePredicate (): base()
        {

        }

        public DerivedFakePredicate (int id): base(id)
        {

        }
    }
}

