//  
//  FakeScalar.cs
//  
//  Author:
//       Marco <${AuthorEmail}>
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

namespace Epic.Linq.Fakes
{
    public class FakeScalarFunction: ScalarFunction
    {
        public FakeScalarFunction (string name):base(name)
        {

        }
        #region implemented abstract members of Epic.Linq.Expressions.Relational.ScalarFunction
        public override bool Equals (ScalarFunction other)
        {
            if (other == null) return false;
            return this.Name == other.Name;
        }

        public override int GetHashCode ()
        {
            return GetType ().GetHashCode () ^ this.Name.GetHashCode ();
        }

        public override TResult Accept<TResult> (Epic.Linq.Expressions.IVisitor<TResult> visitor, Epic.Linq.Expressions.IVisitContext context)
        {
            return AcceptMe(this, visitor, context);
        }
        #endregion
    }
}

