//  
//  CompositeVisitorQA.cs
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
using Epic.Linq.Fakes;
using System.Linq.Expressions;

namespace Epic.Linq.Expressions
{
    [TestFixture()]
    public class CompositeVisitorQA
    {
        [Test]
        public void Initialize_withoutName_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new FakeCompositeVisitor<int>(null);
            });
            
            Assert.Throws<ArgumentNullException>(delegate {
                new FakeCompositeVisitor<object>(string.Empty);
            });
        }
        
        [Test]
        public void Initialize_withAName_works()
        {
            // arrange:
            string name = "test";
            Expression<Func<string, int>> target = s => s.Length;

            // act:
            CompositeVisitor<object> visitor = new FakeCompositeVisitor<object>(name);

            // assert:
            Assert.IsNotNull(visitor);
            Assert.IsNull (visitor.GetVisitor<Expression>(target));
        }
        
        [Test]
        public void GetFirstVisitor_forAnUnknowType_throwsArgumentException()
        {
            // arrange:
            string name = "test";
            Expression<Func<string, int>> target = s => s.Length;
            CompositeVisitor<object> visitor = new FakeCompositeVisitor<object>(name);

            // assert:
            Assert.Throws<ArgumentException>(delegate { 
                visitor.GetFirstVisitor(target);
            }, "No visitor available for the expression {0} in the composition \"{1}\".", target, name);
        }
    }
}

