//  
//  FakeQueryable.cs
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
using Epic.Query.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Epic.Query.Linq.Fakes
{
    public class FakeQueryable<T> : IQueryable<T>
    {
        private readonly IQueryProvider _provider;

        private readonly T[] _contents;

        public FakeQueryable()
        {
            _contents = new T[0];
        }

        public FakeQueryable(T[] contents)
        {
            _contents = contents;
        }

        public FakeQueryable(T[] contents, IQueryProvider provider)
        {
            _contents = contents;
            _provider = provider;
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_contents).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IQueryable Members

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public Expression Expression
        {
            get { return Expression.Constant(this); }
        }

        public IQueryProvider Provider
        {
            get { return _provider; }
        }

        #endregion
    }
}
