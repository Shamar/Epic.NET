//
//  FunctionMappingQA.cs
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
using Rhino.Mocks;
using System.IO;

namespace Epic.Math
{
    [TestFixture()]
    public class FunctionMappingQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutAFunction_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new FunctionMapping<string, int>(null);
            });
        }

        [Test]
        public void Initialize_withAFunction_works()
        {
            // arrange:
            Func<string, int> func = s => s.Length;

            // act:
            FunctionMapping<string, int> mapping = new FunctionMapping<string, int>(func); 

            // assert:
            Assert.IsNotNull(mapping);
        }

        [Test]
        public void ApplyTo_aDomainInstance_callTheDelegate()
        {
            // arrange:
            string value = "test";
            object expectedResult = new object();
            Func<string, object> func = s => s == value ? expectedResult : null;
            FunctionMapping<string, object> mapping = new FunctionMapping<string, object>(func); 

            // act:
            object result = mapping.ApplyTo(value);

            // assert:
            Assert.AreSame(expectedResult, result);
        }

        [Test]
        public void Serialization_works()
        {
            // arrange:
            Func<string, int> func = s => s.Length;
            FunctionMapping<string, int> toTest = new FunctionMapping<string, int>(func); 

            // act:
            Stream stream = SerializationUtilities.Serialize(toTest);

            // assert:
            Assert.IsNotNull(stream);
        }

        [Test]
        public void Deserialization_works()
        {
            // arrange:
            Func<string, int> func = s => s.Length;
            FunctionMapping<string, int> toTest = new FunctionMapping<string, int>(func); 
            Stream stream = SerializationUtilities.Serialize(toTest);

            // act:
            FunctionMapping<string, int> deserialized = SerializationUtilities.Deserialize<FunctionMapping<string, int>>(stream);

            // assert:
            Assert.IsNotNull(deserialized);
            Assert.AreEqual("test".Length, deserialized.ApplyTo("test"));
        }    
    }
}

