//  
//  DefaultTransformer.cs
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
using System.Linq.Expressions;

namespace Epic.Linq.Expressions.Normalization
{
    /// <summary>
    /// Default visitor that normalize an expression tree.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This composition include a set of visitors that are designed to normalize
    /// an expression tree before its translation.
    /// </para>
    /// <para>
    /// It can be extended with the inclusion of more visitors that would be engadged
    /// in reverse order than they are initialized.
    /// </para>
    /// </remarks>
    public class DefaultNormalizer : CompositeVisitorBase<Expression, Expression>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Linq.Expressions.Normalization.DefaultNormalizer"/> class.
        /// </summary>
        public DefaultNormalizer (string name)
            : base(name)
        {
            new ExpressionForwarder(this);
            new ExpressionsInspector(this);
        }

        #region implemented abstract members of Epic.Linq.Expressions.CompositeVisitorBase[Expression,Expression]
        /// <summary>
        /// Initializes the visit context.
        /// </summary>
        /// <returns>
        /// The visit context.
        /// </returns>
        /// <param name='target'>
        /// Target.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        protected override IVisitContext InitializeVisitContext (Expression target, IVisitContext context)
        {
            return context;
        }
        #endregion
    }
}

