//
//  MockableVisitor.cs
//
//  Author:
//       giacomo <${AuthorEmail}>
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

namespace Epic.Query.Linq.Fakes
{
    public class MockableVisitor<T> : CompositeVisitor<T>.VisitorBase, IVisitor<T, T>
        where T : class
    {
        public MockableVisitor (CompositeVisitor<T> composition)
            : base(composition)
        {
        }

        public virtual IVisitor<T, TExpression> CallAsVisitor<TExpression> (TExpression target) where TExpression : class
        {
            return base.AsVisitor (target);
        }

        protected override IVisitor<T, TExpression> AsVisitor<TExpression> (TExpression target)
        {
            return CallAsVisitor(target);
        }

        #region IVisitor implementation

        public virtual T Visit (T target, IVisitContext context)
        {
            return target;
        }

        #endregion

    }
}

