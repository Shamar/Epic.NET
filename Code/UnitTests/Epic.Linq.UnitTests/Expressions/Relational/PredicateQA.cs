//  
//  PredicateQA.cs
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
using Rhino.Mocks;
using Epic.Linq.Fakes;
using Epic.Linq.Expressions.Relational.Predicates;

namespace Epic.Linq.Expressions.Relational
{
    [TestFixture()]
    public class PredicateQA : RhinoMocksFixtureBase
    {
        /// <summary>
        /// Test that the type of the predicate define the hashcode
        /// </summary>
        [Test]
        public void GetHashCode_followTheType()
        {
            // act:
            FakePredicate p1 = new FakePredicate(1);
            FakePredicate p2 = new FakePredicate(1);
            FakePredicate p3 = new DerivedFakePredicate(1);

            // assert:
            Assert.AreEqual(p1.GetHashCode(), p2.GetHashCode());
            Assert.AreNotEqual(p1.GetHashCode(), p3.GetHashCode());
            Assert.AreNotEqual(p2.GetHashCode(), p3.GetHashCode());
        }
        
        [TestCase(true)]
        [TestCase(false)]
        public void ObjectEqual_Scenario_callPredicateEqual(bool expectedResult)
        {
            // arrange:
            Predicate mockArgument = GeneratePartialMock<Predicate>();
            Predicate predicate = GeneratePartialMock<Predicate>();
            predicate.Expect(p => p.Equals(mockArgument)).Return(expectedResult).Repeat.Once();

            // act:
            bool result = predicate.Equals((object)mockArgument);

            // assert:
            Assert.AreEqual(expectedResult, result);
        }
        
        [Test]
        public void Serialization_works ()
        {
            Predicate predicate = new FakePredicate ();
    
            System.IO.Stream stream = SerializationUtilities.Serialize (predicate);
            Assert.IsNotNull (stream);
        }
        
        [Test]
        public void Deserialization_works ()
        {
            Predicate predicate = new FakePredicate ();

            System.IO.Stream stream = SerializationUtilities.Serialize (predicate);
            Predicate deserialized = SerializationUtilities.Deserialize<Predicate> (stream);
         
            Assert.AreEqual(predicate.GetType(), deserialized.GetType());
        }
    }
}

