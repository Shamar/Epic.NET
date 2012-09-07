//  
//  Function.cs
//  
//  Author:
//       Marco Veglio <m.veglio@gmail.com>
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

namespace Epic.Query.Relational
{
    /// <summary>
    /// Models a function which has a <see cref="RelationalExpression"/> as output.
    /// </summary>
    [Serializable]
    public abstract class RelationFunction: RelationalExpression
    {
        private readonly string _name;
        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Query.Relational.RelationFunction"/> class.
        /// </summary>
        /// <param name='name'>
        /// The function name.
        /// </param>
        public RelationFunction (string name): base(RelationType.Function)
        {
            if (string.IsNullOrEmpty (name)) throw new ArgumentNullException("name");
            _name = name;
        }

        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        public string Name { get { return this._name; } }

        /// <summary>
        /// Determines whether the specified <see cref="RelationFunction"/> is equal to the current <see cref="Epic.Query.Relational.RelationFunction"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="RelationFunction"/> to compare with the current <see cref="Epic.Query.Relational.RelationFunction"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="RelationFunction"/> is equal to the current
        /// <see cref="Epic.Query.Relational.RelationFunction"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref='NotImplementedException'>
        /// Is thrown when the not implemented exception.
        /// </exception>
        public abstract bool Equals (RelationFunction other);

        /// <summary>
        /// Determines whether the specified <see cref="RelationalExpression"/> is equal to the current <see cref="RelationFunction"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="RelationalExpression"/> to compare with the current <see cref="RelationFunction"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="RelationalExpression"/> is equal to the current
        /// <see cref="RelationFunction"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (RelationalExpression other)
        {
            RelationFunction function = other as RelationFunction;
            if (null != function) return this.Equals (function);
            return false;
        }
    }
}

