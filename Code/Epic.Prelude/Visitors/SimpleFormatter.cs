//
//  SimpleFormatter.cs
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

namespace Epic.Visitors
{
    /// <summary>
    /// Visitor that can produce a <see cref="System.String"/> out of a <typeparamref name="TTarget"/>.
    /// </summary>
    /// <typeparam name="TTarget">The type of the target to format.</typeparam>
    public sealed class SimpleFormatter<TTarget> : CompositeVisitor<string>.VisitorBase, IVisitor<string, TTarget>
        where TTarget : class
    {
        private readonly Func<TTarget, string> _format;
        private readonly Func<TTarget, bool> _shouldAccept;


        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleFormatter{TTarget}"/> class.
        /// </summary>
        /// <param name="composition">The composition of formatters that this <see cref="SimpleFormatter{TTarget}"/> will belong to.</param>
        /// <param name="format">A function that produce a string out of a <typeparamref name="TTarget"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="composition"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <see langword="null"/>.</exception>
        public SimpleFormatter(CompositeVisitor<string> composition, Func<TTarget, string> format)
			: this(composition, format, e => true)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleFormatter{TTarget}"/> class.
        /// </summary>
        /// <param name="composition">The composition of formatters that this <see cref="SimpleFormatter{TTarget}"/> will belong to.</param>
        /// <param name="format">A function that produce a string out of a <typeparamref name="TTarget"/>.</param>
        /// <param name="acceptRule">A predicate of <typeparamref name="TTarget"/> to determine when to this 
        /// instance should apply the <paramref name="format"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="composition"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="acceptRule"/> is <see langword="null"/>.</exception>
        public SimpleFormatter(CompositeVisitor<string> composition, Func<TTarget, string> format, Func<TTarget, bool> acceptRule)
            : base(composition)
        {
            if (null == format)
                throw new ArgumentNullException("format");
            if (null == acceptRule)
                throw new ArgumentNullException("acceptRule");
            _format = format;
            _shouldAccept = acceptRule;
        }

        /// <summary>
        /// Return the current instance if the <paramref name="target"/> match the accept rule provided to
        /// </summary>
        /// <typeparam name="TExpression">Type of the expression to visit.</typeparam>
        /// <param name="target">Target to visit.</param>
        /// <returns>The current <see cref="SimpleFormatter{TTarget}"/> if <paramref name="target"/> is a 
        /// <typeparamref name="TTarget"/> that matches the accept rule provided at initialization.</returns>
        protected sealed override IVisitor<string, TExpression> AsVisitor<TExpression>(TExpression target)
        {
            IVisitor<string, TExpression> visitor = base.AsVisitor(target);
            TTarget myTarget = target as TTarget;
            if (null == visitor || null == myTarget || !_shouldAccept(myTarget))
                return null;

            return visitor;
        }

        string IVisitor<string, TTarget>.Visit(TTarget target, IVisitContext context)
        {
            return _format(target);
        }
    }
}
