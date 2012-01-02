//  
//  RelationType.cs
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

namespace Epic.Linq.Expressions.Relational
{
    /// <summary>
    /// Type of relations in a extended relational algebra.
    /// </summary>
    [Serializable]
    public enum RelationType
    {
        /// <summary>
        /// Base Relation (in SQL, tipically a table).
        /// </summary>
        BaseRelation,
        
        /// <summary>
        /// Selection (in SQL, the relation produced by a WHERE statement).
        /// </summary>
        Selection,
        
        /// <summary>
        /// Cross product (in SQL, a cross join).
        /// </summary>
        CrossProduct,
        
        /// <summary>
        /// Projection (in SQL, the relation produced by a SELECT statement)
        /// </summary>
        Projection,
        
        /// <summary>
        /// Union.
        /// </summary>
        Union,
        
        /// <summary>
        /// Grouped relation (in SQL, it's the effect of a group by)
        /// </summary>
        Grouped,

        /// <summary>
        /// Relation originated by a function.
        /// </summary>
        Function
    }
}

