//  
//  BaseRelation.cs
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

namespace Epic.Query.Relational
{
    /// <summary>
    /// Base relation (in SQL database a table).
    /// </summary>
    [Serializable]
    public sealed class BaseRelation : RelationalExpression, IEquatable<BaseRelation>
    {
        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Query.Relational.BaseRelation"/> class.
        /// </summary>
        /// <param name='name'>
        /// Name of the relation.
        /// </param>
        public BaseRelation (string name)
            : base(RelationType.BaseRelation)
        {
            if (string.IsNullOrEmpty (name))
                throw new ArgumentNullException("name");
            this._name = name;
        }

        /// <summary>
        /// Gets the name of the relation.
        /// </summary>
        public string Name { get { return this._name; } }
        
        #region implemented abstract members of Epic.Linq.Expressions.Relational.Relation
        /// <summary>
        /// Determines whether the specified <see cref="Epic.Query.Relational.RelationalExpression"/> is equal to the current <see cref="Epic.Query.Relational.BaseRelation"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="RelationalExpression"/> to compare with the current <see cref="Epic.Query.Relational.BaseRelation"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="RelationalExpression"/> is equal to the current
        /// <see cref="Epic.Query.Relational.BaseRelation"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (RelationalExpression other)
        {
            return Equals (other as BaseRelation);
        }
        #endregion

        #region IEquatable implementation
        /// <summary>
        /// Determines whether the specified <see cref="Epic.Query.Relational.BaseRelation"/> is equal to the current <see cref="Epic.Query.Relational.BaseRelation"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="Epic.Query.Relational.BaseRelation"/> to compare with the current <see cref="Epic.Query.Relational.BaseRelation"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Epic.Query.Relational.BaseRelation"/> has the same name of the current
        /// <see cref="Epic.Query.Relational.BaseRelation"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals (BaseRelation other)
        {
            if (null == other) return false;
            return this.Name == other.Name;
        }
        #endregion



        /// <summary>
        /// Accept the specified visitor and context.
        /// </summary>
        /// <param name='visitor'>
        /// Visitor.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        /// <typeparam name='TResult'>
        /// Type of the result produced from the visitor.
        /// </typeparam>
        /// <returns>Result of the visit.</returns>
        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe(this, visitor, context);
        }
    }
}

