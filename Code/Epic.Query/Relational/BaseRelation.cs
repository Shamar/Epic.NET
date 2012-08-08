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
    public sealed class BaseRelation : Relation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Query.Relational.BaseRelation"/> class.
        /// </summary>
        /// <param name='name'>
        /// Name of the relation.
        /// </param>
        public BaseRelation (string name)
            : base(RelationType.BaseRelation, name)
        {
        }

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
        private bool Equals(BaseRelation other)
        {
            return Name.Equals(other.Name);
        }
        
        #region implemented abstract members of Epic.Linq.Expressions.Relational.Relation
        /// <summary>
        /// Determines whether the specified <see cref="Epic.Query.Relational.Relation"/> is equal to the current <see cref="Epic.Query.Relational.BaseRelation"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="Epic.Query.Relational.Relation"/> to compare with the current <see cref="Epic.Query.Relational.BaseRelation"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Epic.Query.Relational.Relation"/> is equal to the current
        /// <see cref="Epic.Query.Relational.BaseRelation"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (Relation other)
        {
            if(null == other)
                return false;
            return other.Type == RelationType.BaseRelation && Equals(other as BaseRelation);
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
        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            return AcceptMe(this, visitor, context);
        }
    }
}

