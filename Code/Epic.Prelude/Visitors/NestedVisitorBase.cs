//
//  NestedVisitorBase.cs
//
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
//
//  Copyright (c) 2010-2013 Giacomo Tesio
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

namespace Epic.Visitors
{
    /// <summary>
    /// Base class for visitors nested in visitors deriving from <see cref="CompositeVisitor{TResult}.VisitorBase"/>.
    /// </summary>
    public abstract class NestedVisitorBase<TResult, TExpression, TOuterVisitor> : IVisitor<TResult, TExpression> where TExpression : class where TOuterVisitor : CompositeVisitor<TResult>.VisitorBase, IVisitor<TResult>
    {
        private readonly TOuterVisitor _outerVisitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="NestedVisitorBase{TResult, TExpression, TOuterVisitor}"/> class.
        /// </summary>
        /// <param name="outerVisitor">Outer visitor that will be passed to the <see cref="NestedVisitorBase{TResult, TExpression, TOuterVisitor}.Visit"/> method.</param>
        /// <exception cref="ArgumentNullException"><paramref name="outerVisitor"/> is <see langword="null"/></exception>
        protected NestedVisitorBase(TOuterVisitor outerVisitor)
        {
            if(null == outerVisitor)
                throw new ArgumentNullException("outerVisitor");
            _outerVisitor = outerVisitor;
        }

        /// <summary>
        /// Visit the specified <paramref name="target"/> in <paramref name="context"/> on behalf of <paramref name="outerVisitor"/>.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="context">Context.</param>
        /// <param name="outerVisitor">Outer visitor.</param>
        protected abstract TResult Visit(TExpression target, IVisitContext context, TOuterVisitor outerVisitor);

        #region IVisitor implementation
        TResult IVisitor<TResult, TExpression>.Visit(TExpression target, IVisitContext context)
        {
            return Visit(target, context, _outerVisitor);
        }
        #endregion
        #region IVisitor implementation
        IVisitor<TResult, ExpressionToVisit> IVisitor<TResult>.AsVisitor<ExpressionToVisit>(ExpressionToVisit target)
        {
            return _outerVisitor.AsVisitor(target);
        }
        #endregion
    }
}

