//  
//  RepositoryBaseQA.cs
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
using NUnit.Framework;
using System;
using System.Linq.Expressions;

namespace Epic.Linq.UnitTests
{
    [TestFixture()]
    public class RepositoryBaseQA
    {
        [Test]
        public void Initialize_withoutProviderName_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
               new FakeRepository<string,int>(null);
            });
            Assert.Throws<ArgumentNullException>(delegate {
               new FakeRepository<object,string>(string.Empty);
            });
        }
        
        [Test]
        public void Initialize_withProviderName_works()
        {
            // act:
            IRepository<string,int> repository = new FakeRepository<string,int>("test");

            // assert:
            Assert.IsNotNull(repository);
            Assert.AreEqual(typeof(string), repository.ElementType);
            Assert.IsInstanceOf<ConstantExpression>(repository.Expression);
            Assert.AreSame(repository, ((ConstantExpression)repository.Expression).Value);
        }
    }
}

