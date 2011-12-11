//  
//  FunctionQA.cs
//  
//  Author:
//       Marco <${AuthorEmail}>
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
using Rhino.Mocks;

namespace Epic.Linq.Expressions.Relational
{
    [TestFixture]
    public class FunctionQA: RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutName_throwsArgumentNullException()
        {
            // assert:
           Assert.Throws<ArgumentNullException>(delegate {
                new FakeFunction(null);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                new FakeFunction(string.Empty);
            });
        }

        [Test]
        public void Initialize_withAName_works()
        {
            // arrange:
            string name = "test";

            // assert:
            Function relation = new FakeFunction(name);

            Assert.AreEqual(name, relation.Name);
            Assert.IsTrue(RelationType.Function.Equals(relation.Type));
        }

        [TestCase("test0")]
        [TestCase("test1")]
        [TestCase("test2")]
        public void GetHashCode_areEqualForEqualNames(string name)
        {
            // arrange:
            Function rel1 = new FakeFunction(name);
            Function rel2 = new FakeFunction(name);

            // act:
            int hash1 = rel1.GetHashCode();
            int hash2 = rel2.GetHashCode();

            // assert:
            Assert.AreEqual(hash1, hash2);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ObjectEquals_callFunctionEquals(bool expectedResult)
        {
            // arrange:
            string name = "test";
            Function mockArgument = GeneratePartialMock<Function>(name);
            FakeFunction relation = GeneratePartialMock<FakeFunction>(name);
            relation.Expect(r => r.Equals(mockArgument)).Return(expectedResult).Repeat.Once();

            // act:
            bool result = relation.Equals((object)mockArgument);

            // assert:
            Assert.AreEqual(expectedResult, result);
        }

        [TestCase("testName")]
        public void FunctionEquals_withDifferentTypeRelation_returnsFalse(string name)
        {
            // arrange:
            Function function = new FakeFunction(name);
            Relation relation = new FakeRelation(RelationType.BaseRelation, name);

            // act:
            bool areEqual = function.Equals (relation);
            bool sameHash = function.GetHashCode() == relation.GetHashCode ();

            // assert:
            Assert.IsFalse (areEqual);
            Assert.IsFalse (sameHash);
        }


        [Test]
        public void Serialization_works ()
        {
            // arrange:
            string name = "test";

            // assert:
            Function relation = new FakeFunction (name);

            System.IO.Stream stream = TestUtilities.Serialize (relation);
            Assert.IsNotNull (stream);
        }

        [Test]
        public void Deserialization_works ()
        {
            // arrange:
            string name = "test";

            // assert:
            Function relation = new FakeFunction (name);

            System.IO.Stream stream = TestUtilities.Serialize (relation);
            Function deserialized = TestUtilities.Deserialize<Function> (stream);

            Assert.AreEqual(relation.Name, deserialized.Name);
            Assert.AreEqual(relation.Type, deserialized.Type);
        }
    }
}

