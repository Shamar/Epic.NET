//  
//  SelectionQA.cs
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
using Rhino.Mocks;
using Epic.Linq.Fakes;
using System.Linq;

namespace Epic.Linq.Expressions.Relational
{
    [TestFixture()]
    public class SelectionQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutName_throwsArgumentNullException()
        {
            // arrange:
            Relation source = GeneratePartialMock<Relation>(RelationType.BaseRelation, "test");
            Predicate condition = GeneratePartialMock<Predicate>();

            // assert:
            Assert.Throws<ArgumentNullException>(delegate { 
                new Selection(null, source, condition);
            });
            Assert.Throws<ArgumentNullException>(delegate { 
                new Selection(string.Empty, source, condition);
            });
        }
        
        [Test]
        public void Initialize_withoutSourceRelation_throwsArgumentNullException()
        {
            // arrange:
            string name = "test";
            Predicate condition = GeneratePartialMock<Predicate>();

            // assert:
            Assert.Throws<ArgumentNullException>(delegate { 
                new Selection(name, null, condition);
            });
        }
        
        [Test]
        public void Initialize_withoutPredicate_throwsArgumentNullException()
        {
            // arrange:
            string name = "test";
            Relation source = GeneratePartialMock<Relation>(RelationType.BaseRelation, "testRelation");

            // assert:
            Assert.Throws<ArgumentNullException>(delegate { 
                new Selection(name, source, null);
            });
        }
        
        [Test]
        public void Initialize_withValidArguments_works()
        {
            // arrange:
            string name = "test";
            Relation source = GeneratePartialMock<Relation>(RelationType.BaseRelation, "testRelation");
            Predicate condition = GeneratePartialMock<Predicate>();

            // act:
            Selection selection = new Selection(name, source, condition);

            // assert:
            Assert.AreEqual(name, selection.Name);
            Assert.AreEqual(RelationType.Selection, selection.Type);
            Assert.AreSame(source, selection.Source);
            Assert.AreSame(condition, selection.Condition);
        }
        
        [Test]
        public void Equals_toNull_isFalse()
        {
            // arrange:
            string name = "test";
            Relation source = GeneratePartialMock<Relation>(RelationType.BaseRelation, "testRelation");
            Predicate condition = GeneratePartialMock<Predicate>();
            Relation relation = new Selection(name, source, condition);
            Relation nullRel = null;

            // act:
            bool result = relation.Equals(nullRel);

            // assert:
            Assert.IsFalse(result);
        }
        
        [TestCase("test1", "TEST2")]
        [TestCase("test2", "TEST3")]
        [TestCase("test3", "test3")]
        public void RelationEquals_toSelectionWithEqualConditionAndPredicate_isTrue(string name1, string name2)
        {
            // arrange:
            Relation source1 = GeneratePartialMock<Relation>(RelationType.BaseRelation, "testRelation");
            Relation source2 = GeneratePartialMock<Relation>(RelationType.BaseRelation, "testRelation");
            source1.Expect(r => r.Equals(source2)).Return(true).Repeat.Once();
            Predicate condition1 = GeneratePartialMock<Predicate>();
            Predicate condition2 = GeneratePartialMock<Predicate>();
            condition1.Expect(c => c.Equals(condition2)).Return(true).Repeat.Once();
            Relation rel1 = new Selection(name1, source1, condition1);
            Relation rel2 = new Selection(name2, source2, condition2);

            // act:
            bool result = rel1.Equals(rel2);

            // assert:
            Assert.IsTrue(result);
        }
        
        [TestCase("test1", "TEST2")]
        [TestCase("test2", "TEST3")]
        [TestCase("test3", "test3")]
        public void RelationEquals_toSelectionWithDifferentRelations_isFalse(string name1, string name2)
        {
            // arrange:
            Relation source1 = GeneratePartialMock<Relation>(RelationType.BaseRelation, "testRelation");
            Relation source2 = GeneratePartialMock<Relation>(RelationType.BaseRelation, "testRelation");
            source1.Expect(r => r.Equals(source2)).Return(false).Repeat.Once();
            Predicate condition1 = GeneratePartialMock<Predicate>();
            Predicate condition2 = GeneratePartialMock<Predicate>();
            Relation rel1 = new Selection(name1, source1, condition1);
            Relation rel2 = new Selection(name2, source2, condition2);

            // act:
            bool result = rel1.Equals(rel2);

            // assert:
            Assert.IsFalse(result);
        }
        
        [TestCase("test1", "TEST2")]
        [TestCase("test2", "TEST3")]
        [TestCase("test3", "test3")]
        public void RelationEquals_toSelectionWithDifferentPredicates_isFalse(string name1, string name2)
        {
            // arrange:
            Relation source1 = GeneratePartialMock<Relation>(RelationType.BaseRelation, "testRelation");
            Relation source2 = GeneratePartialMock<Relation>(RelationType.BaseRelation, "testRelation");
            source1.Expect(r => r.Equals(source2)).Return(true).Repeat.Once();
            Predicate condition1 = GeneratePartialMock<Predicate>();
            Predicate condition2 = GeneratePartialMock<Predicate>();
            condition1.Expect(c => c.Equals(condition2)).Return(false).Repeat.Once();
            Relation rel1 = new Selection(name1, source1, condition1);
            Relation rel2 = new Selection(name2, source2, condition2);

            // act:
            bool result = rel1.Equals(rel2);

            // assert:
            Assert.IsFalse(result);
        }
        
        [Test]
        public void Equals_withDifferentTypeOfRelation_returnFalse()
        {
            // arrange:
            string name = "test";
            Relation source = GeneratePartialMock<Relation>(RelationType.BaseRelation, "testRelation");
            Predicate condition = GeneratePartialMock<Predicate>();
            Relation relation = new Selection(name, source, condition);
            Relation[] differentRelations = new Relation[] { 
                new FakeRelation(RelationType.CrossProduct, name),  
                new FakeRelation(RelationType.Grouped, name),
                new FakeRelation(RelationType.Projection, name),
                new FakeRelation(RelationType.BaseRelation, name),
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
            string name = "test";
            Relation source = GeneratePartialMock<Relation>(RelationType.BaseRelation, "testRelation");
            source.Expect(r => r.Equals(source)).Return(true).Repeat.Any();                                 // Needed from Rhino
            Predicate condition = GeneratePartialMock<Predicate>();
            condition.Expect(c => c.Equals(condition)).Return(true).Repeat.Any();                           // Needed from Rhino
            Selection relation = new Selection(name, source, condition);
            IVisitor<object, Selection> selectionVisitor = GenerateStrictMock<IVisitor<object, Selection>>();
            selectionVisitor.Expect(v => v.Visit(relation, context)).Return(expectedResult).Repeat.Once();
            IVisitor<object> visitor = GenerateStrictMock<IVisitor<object>>();
            visitor.Expect(v => v.GetVisitor<Selection>(relation)).Return(selectionVisitor).Repeat.Once();

            // act:
            object result = relation.Accept(visitor, context);

            // assert:
            Assert.AreSame(expectedResult, result);
        }
    }
}

