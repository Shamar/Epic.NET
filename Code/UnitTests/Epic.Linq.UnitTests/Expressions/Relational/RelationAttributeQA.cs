//  
//  RelationAttributeQA.cs
//  
//  Author:
//       Marco Veglio <m.veglio@gmail.com>
// 
//  Copyright (c) 2010-2011 Marco Veglio
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
using Epic.Linq.Expressions.Relational;
using NUnit.Framework;
using Rhino.Mocks;

namespace Epic.Linq.Expressions.Relational
{
    [TestFixture()]
    public class RelationAttributeQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutName_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate { new RelationAttribute(null, null); } );
        }
        
        [Test]
        public void Initialize_withoutRelation_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate { new RelationAttribute("test", null); } );
        }
        
        [TestCase("testAttribute", "testRelation")]
        public void Initialize_wihtNameAndRelation_Works(string attrName, string relationName)
        {
            // act:
            BaseRelation relation = new BaseRelation(relationName);
            RelationAttribute attribute = new RelationAttribute(attrName, relation);

            // assert:
            Assert.AreEqual (attribute.Name, attrName);
            Assert.AreEqual (attribute.Relation.Name, relationName);
            Assert.IsTrue (attribute.Relation.Equals(relation));
            
        }
        
        [TestCase("testAttribute", "testRelation")]
        public void Equals_toAttributeWithSameNameAndRelation_isTrue(string attrName, string relationName)
        {
            // arrange:
            BaseRelation relation = new BaseRelation(relationName);
            RelationAttribute attribute1 = new RelationAttribute(attrName, relation);
            RelationAttribute attribute2 = new RelationAttribute(attrName, relation);

            // act:
            bool isTrue = attribute1.Equals(attribute2);

            // assert:
            Assert.IsTrue (isTrue);
        }
        
        [TestCase("testAttribute", "toastAttribute", "testRelation")]
        public void Equals_toAttributeWithDifferentName_isFalse(string attrName, string attrName2, string relationName)
        {
            // arrange:
            BaseRelation relation = new BaseRelation(relationName);
            RelationAttribute attribute1 = new RelationAttribute(attrName, relation);
            RelationAttribute attribute2 = new RelationAttribute(attrName2, relation);

            // act:
            bool isFalse = attribute1.Equals(attribute2);

            // assert:
            Assert.IsFalse (isFalse);
        }        

        [TestCase("testAttribute", "testRelation", "toastRelation")]
        public void Equals_toAttributeWithDifferentRelation_isFalse(string attrName, string relationName, string relationName2)
        {
            // arrange:
            BaseRelation relation = new BaseRelation(relationName);
            BaseRelation relation2 = new BaseRelation(relationName2);
            RelationAttribute attribute1 = new RelationAttribute(attrName, relation);
            RelationAttribute attribute2 = new RelationAttribute(attrName, relation2);

            // act:
            bool isFalse = attribute1.Equals(attribute2);

            // assert:
            Assert.IsFalse (isFalse);
        }
        
        [TestCase("testAttribute", "testRelation")]
        public void Equals_toNull_isFalse(string attrName, string relationName)
        {
            // arrange:
            // arrange:
            BaseRelation relation = new BaseRelation(relationName);
            RelationAttribute attribute1 = new RelationAttribute(attrName, relation);
            RelationAttribute attribute2 = null;            

            // act:
            bool isFalse = attribute1.Equals(attribute2);

            // assert:
            Assert.IsFalse (isFalse);

        }
        
        [Test]
        [Ignore]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            object expectedResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            BaseRelation relation = new BaseRelation("test");
            RelationAttribute attribute = new RelationAttribute("testName", relation);
            IVisitor<object, RelationAttribute> baseAttributeVisitor = GenerateStrictMock<IVisitor<object, RelationAttribute>>();
            baseAttributeVisitor.Expect(v => v.Visit(attribute, context)).Return(expectedResult).Repeat.Once();
            IVisitor<object> visitor = GenerateStrictMock<IVisitor<object>>();
            visitor.Expect(v => v.GetVisitor(attribute)).Return(baseAttributeVisitor).Repeat.Once ();

            // act:
            try
            {
                object result = attribute.Accept(visitor, context);
                // assert:
                Assert.AreSame(expectedResult, result);
            }
            catch (Exception ex) { System.Console.Out.WriteLine (ex);throw;}
        }

    }
}

