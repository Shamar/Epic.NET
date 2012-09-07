//
//  EchoingVisitor.cs
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
    /// Visitor that returns the <typeparamref name="TEcho"/> that visit.
    /// </summary>
    /// <remarks>
    /// This visitor is designed for syntax tree normalization and optimization.
    /// Indeed such kind of visitors both visit and returns the same type of object.
    /// Thus by registering the <see cref="EchoingVisitor{TEcho}"/> as the 
    /// first one in such kind of compositions all subsequent visitors will be able
    /// to transform their own nodes or to simply ContinueVisit
    /// on those node that they don't treat.
    /// </remarks>
    /// <typeparam name="TEcho">Type of the objects that this visitor will recieve and return.</typeparam>
    public sealed class EchoingVisitor<TEcho> : CompositeVisitor<TEcho>.VisitorBase, IVisitor<TEcho, TEcho>
        where TEcho : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EchoingVisitor{TEcho}"/> class.
        /// </summary>
        /// <param name='composition'>
        /// Composition (typically a normalizer or an optimizer).
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="composition"/> is <see langword="null"/>.</exception>
        public EchoingVisitor (CompositeVisitorBase<TEcho, TEcho> composition)
            : base(composition)
        {
        }

        #region IVisitor implementation

        /// <summary>
        /// Returns the <paramref name="target"/>.
        /// </summary>
        /// <param name='target'>
        /// Expression to visit that will be returned.
        /// </param>
        /// <param name='context'>
        /// Visit context. Contains the state produced by previous visitors.
        /// </param>
        /// <returns>
        /// The <paramref name="target"/>.
        /// </returns>
        TEcho IVisitor<TEcho, TEcho>.Visit (TEcho target, IVisitContext context)
        {
            return target;
        }

        #endregion
    }
}

