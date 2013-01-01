//  
//  MissingMethods.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2013 Giacomo Tesio
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
using NUnit.Framework;
using System;
using System.Linq.Expressions;
using System.Linq;

namespace Epic.Query.Linq
{
    [TestFixture()]
    public class MissingMethods : RhinoMocksFixtureBase
    {
        [Test]
        public void QueryProvider_Execute_throwsNotImplementedException()
        {
            // arrange:
            IQueryProvider provider = new QueryProvider("Test");

            // assert:
            Assert.Throws<NotImplementedException>(delegate {
                provider.Execute(Expression.Constant(1));
            });
            Assert.Throws<NotImplementedException>(delegate {
                provider.Execute<int>(Expression.Constant(1));
            });
        }

        [Test]
        public void RepositoryBase_getItem_throwsNotImplementedException()
        {
            // arrange:
            string result = null;
            IQueryableRepository<string,int> repository = new Fakes.FakeRepository<string,int>("test");

            // assert:
            Assert.Throws<NotImplementedException>(delegate {
                result = repository[1];
            });
            Assert.IsNull(result);
        }
    }
}

