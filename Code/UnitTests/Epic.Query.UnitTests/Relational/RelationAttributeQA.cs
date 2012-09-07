//  
//  RelationAttributeQA.cs
//  
//  Author:
//       Marco Veglio <m.veglio@gmail.com>
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
using Epic.Query.Relational;
using NUnit.Framework;
using Rhino.Mocks;
using Epic.Query.Fakes;

namespace Epic.Query.Relational
{
    [TestFixture()]
    public class RelationAttributeQA : RhinoMocksFixtureBase
    {
        private RelationalExpression generateRelation()
        {
            RelationalExpression relation = GeneratePartialMock<RelationalExpression>(RelationType.BaseRelation);
            relation.Expect ( r => r.Equals (relation)).Return (true).Repeat.Any ();
            relation.Expect (r => r.Equals (Arg<RelationalExpression>.Is.Anything)).Return(false).Repeat.Any ();
            return relation;
        }
        
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
        
        [TestCase("testAttribute")]
        public void Initialize_wihtNameAndRelation_Works(string attrName)
        {
            // act:
            RelationalExpression relation = generateRelation ();
            RelationAttribute attribute = new RelationAttribute(attrName, relation);

            // assert:
            Assert.AreEqual (attribute.Name, attrName);
            Assert.AreEqual (attribute.Type, ScalarType.Attribute);
            Assert.IsTrue (attribute.Relation.Equals(relation));
        }
        
        [TestCase("testAttribute")]
        public void Equals_toAttributeWithSameNameAndRelation_isTrue(string attrName)
        {
            // arrange:
            RelationalExpression relation = generateRelation ();
            RelationAttribute attribute1 = new RelationAttribute(attrName, relation);
            RelationAttribute attribute2 = new RelationAttribute(attrName, relation);
            Scalar scalar = attribute2;
            // act:
            bool isTrueAttribute = attribute1.Equals(attribute2);
            bool isTrueScalar = attribute1.Equals (scalar);

            // assert:
            Assert.IsTrue (isTrueAttribute);
            Assert.IsTrue (isTrueScalar);
        }
        
        [TestCase("testAttribute", "toastAttribute")]
        public void Equals_toAttributeWithDifferentName_isFalse(string attrName, string attrName2)
        {
            // arrange:
            RelationalExpression relation = generateRelation ();
            RelationAttribute attribute1 = new RelationAttribute(attrName, relation);
            RelationAttribute attribute2 = new RelationAttribute(attrName2, relation);
            Scalar scalar = attribute2;

            // act:
            bool isFalseAttribute = attribute1.Equals(attribute2);
            bool isFalseScalar = attribute1.Equals (scalar);
            // assert:
            Assert.IsFalse (isFalseAttribute);
            Assert.IsFalse (isFalseScalar);
        }        

        [TestCase("testAttribute")]
        public void Equals_toAttributeWithDifferentRelation_isFalse(string attrName)
        {
            // arrange:
            RelationalExpression relation = generateRelation ();
            RelationalExpression relation2 = generateRelation ();
            RelationAttribute attribute1 = new RelationAttribute(attrName, relation);
            RelationAttribute attribute2 = new RelationAttribute(attrName, relation2);

            // act:
            bool isFalse = attribute1.Equals(attribute2);

            // assert:
            Assert.IsFalse (isFalse);
        }
        
        [TestCase("testAttribute")]
        public void Equals_toNull_isFalse(string attrName)
        {
            // arrange:
            // arrange:
            RelationalExpression relation = generateRelation ();
            RelationAttribute attribute1 = new RelationAttribute(attrName, relation);
            RelationAttribute attribute2 = null;            

            // act:
            bool isFalse = attribute1.Equals(attribute2);

            // assert:
            Assert.IsFalse (isFalse);

        }
        
        [TestCase("testAttribute")]
        public void Equals_ToRelationAttributeObject_Works(string attrName)
        {
            // arrange:
            RelationalExpression relation = generateRelation ();
            RelationAttribute attribute1 = new RelationAttribute(attrName, relation);
            object testObj = new RelationAttribute(attrName, relation);

            // act:
            bool isTrue = attribute1.Equals(testObj);

            // assert:
            Assert.IsTrue (isTrue);
        }
        
        [TestCase("testAttribute")]
        public void Equals_ToAnyObject_IsFalse(string attrName)
        {
            // arrange:
            RelationalExpression relation = generateRelation ();
            RelationAttribute attribute1 = new RelationAttribute(attrName, relation);
            Object testObj = new Object();

            // act:
            bool isTrue = attribute1.Equals (testObj);
            
            // assert:
            Assert.IsFalse (isTrue);

        }
        
        [TestCase("testAttribute")]
        public void GetHashCode_toAttributeWithSameNameAndRelation_AreEqual(string attrName)
        {
            // arrange:
            RelationalExpression relation = generateRelation ();
            RelationAttribute attribute1 = new RelationAttribute(attrName, relation);
            RelationAttribute attribute2 = new RelationAttribute(attrName, relation);

            // act:
            int hash1 = attribute1.GetHashCode ();
            int hash2 = attribute2.GetHashCode ();
            // assert:
            Assert.AreEqual(hash1, hash2);                
        }
        
        [TestCase("testAttribute", "toastAttribute")]
        public void GetHashCode_toAttributeWithDifferentName_areNotEqual(string attrName, string attrName2)
        {
            // arrange:
            RelationalExpression relation = generateRelation ();
            RelationAttribute attribute1 = new RelationAttribute(attrName, relation);
            RelationAttribute attribute2 = new RelationAttribute(attrName2, relation);

            // act:
            int hash1 = attribute1.GetHashCode ();
            int hash2 = attribute2.GetHashCode ();
            // assert:
            Assert.AreNotEqual(hash1, hash2);                
        }        

        
        [Test]
        public void Accept_withValidArguments_delegateVisitToTheRightVisitor()
        {
            // arrange:
            object expectedResult = new object();
            IVisitContext context = GenerateStrictMock<IVisitContext>();
            RelationalExpression relation = generateRelation ();
            RelationAttribute attribute = new RelationAttribute("testName", relation);
            IVisitor<object, RelationAttribute> relationAttributeVisitor = GenerateStrictMock<IVisitor<object, RelationAttribute>>();
            relationAttributeVisitor.Expect(v => v.Visit(attribute, context)).Return(expectedResult).Repeat.Once();
            
            IVisitor<object> visitor = GenerateStrictMock<IVisitor<object>>();
            visitor.Expect (v => v.AsVisitor(attribute)).Return(relationAttributeVisitor).Repeat.Once();

            // act:
            object result = attribute.Accept(visitor, context);
            // assert:
            Assert.AreSame(expectedResult, result);
        }
        
        [TestCase("testAttribute", "testRelation")]
        public void Serialize_Works(string attributeName, string relationName)
        {
            // arrange:
            RelationalExpression relation = new FakeRelation (RelationType.BaseRelation, relationName);
            RelationAttribute attribute = new RelationAttribute(attributeName, relation);

            // act:
            System.IO.Stream stream = SerializationUtilities.Serialize (attribute);
            
            // assert:
            Assert.IsNotNull (stream);

        }
        
        [TestCase("testAttribute", "testRelation")]
        public void Deserialize_Works(string attributeName, string relationName)
        {
            // arrange:
            RelationalExpression relation = new FakeRelation (RelationType.BaseRelation, relationName);
            RelationAttribute attribute = new RelationAttribute(attributeName, relation);

            // act:
            System.IO.Stream stream = SerializationUtilities.Serialize (attribute);
            RelationAttribute deserialized = SerializationUtilities.Deserialize<RelationAttribute>(stream);
            // assert:
   
            Assert.IsTrue (deserialized.Equals (attribute));
        }
    }
}

