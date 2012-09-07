//  
//  DefaultNormalizer.cs
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

namespace Epic.Query.Linq.Expressions.Normalization
{
    /// <summary>
    /// Default normalizer.
    /// </summary>
    public class DefaultNormalizer : ExpressionNormalizerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Query.Linq.Expressions.Normalization.DefaultNormalizer"/> class.
        /// </summary>
        /// <param name='name'>
        /// Name of the normalizer.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/> or <see cref="String.Empty"/>.</exception>
        public DefaultNormalizer (string name)
            : base(name)
        {
            // WARNING: the order is relevant!

            // Evaluate and returns the value
            new PartialEvaluator(this);

            // Resolve queryable constant and returns the result
            new QueryableConstantResolver(this);

            // Forward to the normalizer the arguments, and return the result of the eventual reduction
            new QueryableMethodsReducer(this);

            // Forward to the normalizer the arguments, and return the result of the eventual reduction
            new EnumerableMethodsReducer(this);

            // Identify closures and FORWARD to the normalizer their value
            new ClosureExpander(this);
        }
    }
}

