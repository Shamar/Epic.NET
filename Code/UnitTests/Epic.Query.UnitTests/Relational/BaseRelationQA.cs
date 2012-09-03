//  
//  BaseRelationQA.cs
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
using NUnit.Framework;
using System;
using Epic.Query.Fakes;
using System.Linq;
using Rhino.Mocks;

namespace Epic.Query.Relational
{
    [TestFixture()]
    public class BaseRelationQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutName_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate { 
                new BaseRelation(null);
            });
            Assert.Throws<ArgumentNullException>(delegate { 
                new BaseRelation(string.Empty);
            });
        }
        
        [TestCase("test1")]
        [TestCase("test2")]
        [TestCase("test3")]
        public void Initialize_withAName_works(string name)
        {
            // act:
            BaseRelation relation = new BaseRelation(name);

            // assert:
            Assert.AreEqual(name, relation.Name);
            Assert.AreEqual(RelationType.BaseRelation, relation.Type);
        }
        
        [TestCase("test1")]
        [TestCase("test2")]
        [TestCase("test3")]
        public void RelationEquals_toBaseRelationWithTheSameName_isTrue(string name)
        {
            // arrange:
            RelationalExpression rel1 = new BaseRelation(name);
            RelationalExpression rel2 = new BaseRelation(name);

            // act:
            bool result = rel1.Equals(rel2);

            // assert:
            Assert.IsTrue(result);
        }
        
        [TestCase("test1", "TEST1")]
        [TestCase("test2", "TEST2")]
        [TestCase("test3", "TEST3")]
        public void RelationEquals_toBaseRelationWithDifferentName_isFalse(string name1, string name2)
        {
            // arrange:
            RelationalExpression rel1 = new BaseRelation(name1);
            RelationalExpression rel2 = new BaseRelation(name2);

            // act:
            bool result = rel1.Equals(rel2);

            // assert:
            Assert.IsFalse(result);
        }
        
        [Test]
        public void Equals_toNull_isFalse()
        {
            // arrange:
            string name = "test";
            RelationalExpression relation = new BaseRelation(name);
            RelationalExpression nullRel = null;

            // act:
            bool result = relation.Equals(nullRel);

            // assert:
            Assert.IsFalse(result);
        }
        
        [Test]
        public void Equals_withDifferentTypeOfRelation_returnFalse()
        {
            // arrange:
            string name = "test";
            RelationalExpression relation = new BaseRelation(name);
            RelationalExpression[] differentRelations = new RelationalExpression[] { 
                new FakeRelation(RelationType.CrossProduct, name),  
                new FakeRelation(RelationType.Grouped, name),
                new FakeRelation(RelationType.Projection, name),
                new FakeRelation(RelationType.Selection, name),
                new FakeRelation(RelationType.Union, name)
            };
            System.Collections.Generic.List<bool> results = new System.Collections.Generic.List<bool>();

            // act:
            for(int i = 0; i < differentRelations.Length; ++i)
            {
                results.Add(relation.Equals(differentRelations[i]));
            }

            // assert:
            Assert.IsTrue(results.All(r => r == false));
        }
        
        [Test]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            object expectedResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            BaseRelation relation = new BaseRelation("test");
            IVisitor<object, BaseRelation> baseRelationVisitor = GenerateStrictMock<IVisitor<object, BaseRelation>>();
            baseRelationVisitor.Expect(v => v.Visit(relation, context)).Return(expectedResult).Repeat.Once();
            IVisitor<object> visitor = GenerateStrictMock<IVisitor<object>>();
            visitor.Expect(v => v.AsVisitor(relation)).Return(baseRelationVisitor).Repeat.Once ();

            // act:
            object result = relation.Accept(visitor, context);

            // assert:
            Assert.AreSame(expectedResult, result);
        }
    }
}

