//  
//  FakeVisitable.cs
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

namespace Epic.Fakes
{
    /// <summary>
    /// Fake visitable.
    /// </summary>
    public class FakeVisitable<T> : VisitableBase
    {
        #region implemented abstract members of Epic.Linq.Expressions.VisitableBase
        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe(this, visitor, context);
        }
        #endregion
        
        /// <summary>
        /// Try to fool the AcceptMe method.
        /// </summary>
        /// <returns>
        /// The accept me.
        /// </returns>
        /// <param name='visitable'>
        /// Visitable.
        /// </param>
        /// <param name='visitor'>
        /// Visitor.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        public TResult FoolAcceptMe<TVisitable, TResult>(TVisitable visitable, IVisitor<TResult> visitor, IVisitContext context)
            where TVisitable : VisitableBase
        {
            return AcceptMe(visitable, visitor, context);
        }
    }
        
    /// <summary>
    /// This fake do not override Accept. Thus it will throw.
    /// </summary>
    public class DerivedFakeVisitable<T> : FakeVisitable<T>
    {
    }
}

