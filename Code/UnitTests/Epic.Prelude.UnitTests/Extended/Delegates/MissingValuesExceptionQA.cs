//
//  MissingValuesExceptionQA.cs
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
using System;
using NUnit.Framework;
using System.IO;

namespace Epic.Extended.Delegates
{
    [TestFixture]
    public class MissingValuesExceptionQA
    {
        [Test]
        public void Initialization_withParamNameAndMessage_works ()
        {
            // arrange:
            string paramName = "paramName";
            string message = "Missing values";

            // act:
            var toTest = new MissingValuesException(paramName, message);

            // assert:
            Assert.AreSame(paramName, toTest.ParamName);
            StringAssert.Contains(message, toTest.Message);
        }

        [Test]
        public void Serialization_works ()
        {
            // arrange:
            string paramName = "paramName";
            string message = "Missing values";
            var toSerialize = new MissingValuesException(paramName, message);
            
            // act:
            Stream stream = SerializationUtilities.Serialize(toSerialize);
            
            // assert:
            Assert.IsNotNull (stream);
        }

        
        [Test]
        public void Deserialization_works ()
        {
            // arrange:
            string paramName = "paramName";
            string message = "Missing values";
            var toSerialize = new MissingValuesException(paramName, message);
            Stream stream = SerializationUtilities.Serialize(toSerialize);

            // act:
            var deserialized = SerializationUtilities.Deserialize<MissingValuesException>(stream);

            // assert:
            Assert.IsNotNull (deserialized);
            Assert.AreEqual(paramName, deserialized.ParamName);
            StringAssert.Contains(message, deserialized.Message);
        }

    }
}

