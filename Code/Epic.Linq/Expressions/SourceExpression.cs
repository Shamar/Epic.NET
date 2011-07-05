//  
//  SourceExpression.cs
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

namespace Epic.Linq.Expressions
{
    public class SourceExpression<T> : QueryExpression
    {
        public SourceExpression ()
            : base(typeof(T))
        {
        }
        
        protected SourceExpression(SourceExpression<T> mother, FilterExpression filter)
            : base(this)
        {
            if(null == filter)
                throw new ArgumentNullException();
            _sorter = mother.Sorter;
            _filter = filter;
        }
        
        protected SourceExpression(SourceExpression<T> mother, SortExpression sorter)
            : base(this)
        {
            if(null == sorter)
                throw new ArgumentNullException();
            _sorter = sorter;
            _filter = mother.Filter;
        }
        
        private readonly SortExpression _sorter;
        public SortExpression Sorter
        {
            get
            {
                return _sorter;
            }
        }
        
        private readonly FilterExpression _filter;
        public FilterExpression Filter
        {
            get
            {
                return _filter;
            }
        }

        #region implemented abstract members of Epic.Linq.Expressions.QueryExpression
        public override void Accept (IVisitor visitor)
        {
            throw new NotImplementedException ();
        }

        public override QueryExpression MergeWith (QueryExpression expression)
        {
            FilterExpression filter = expression as FilterExpression;
            if(null != filter)
            {
                if(null != _filter)
                {
                    filter = _filter.MergeWith(filter);
                }
                return new SourceExpression<T>(this, filter);
            }
            
            SortExpression sorter = expression as SortExpression;
            if(null != sorter)
            {
                if(null != _sorter)
                {
                    sorter = _sorter.MergeWith(sorter);
                }
                return new SourceExpression<T>(this, sorter);
            }
        }
        #endregion
    }
}

