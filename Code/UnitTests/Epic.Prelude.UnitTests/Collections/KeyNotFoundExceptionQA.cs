//
//  KeyNotFoundExceptionTester.cs
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
using Epic.Collections;
using System.IO;

namespace Epic.Collections
{
    [TestFixture()]
    public class KeyNotFoundExceptionQA
    {
        [Test]
        public void Initialize_withKeyAndMessage_works()
        {
            // arrange:
            string message = "Test message.";
            int key = 10;

            // act:
            KeyNotFoundException<int> toTest = new KeyNotFoundException<int>(key, message);

            // assert:
            Assert.AreSame(message, toTest.Message);
            Assert.AreEqual(key, toTest.Key);
        }

        [Test]
        public void Initialize_withKeyAndMessageAndInnerException_works()
        {
            // arrange:
            string message = "Test message.";
            int key = 10;
            Exception inner = new Exception("Inner message");

            // act:
            KeyNotFoundException<int> toTest = new KeyNotFoundException<int>(key, message, inner);

            // assert:
            Assert.AreSame(inner, toTest.InnerException);
            Assert.AreSame(message, toTest.Message);
            Assert.AreEqual(key, toTest.Key);
        }

        [Test]
        public void Serialization_works()
        {
            // arrange:
            string message = "Test message.";
            int key = 10;
            Exception inner = new Exception("Inner message");
            KeyNotFoundException<int> toTest = new KeyNotFoundException<int>(key, message, inner);

            // act:
            Stream stream = SerializationUtilities.Serialize(toTest);

            // assert:
            Assert.IsNotNull(stream);
        }

        [Test]
        public void Deserialization_works()
        {
            // arrange:
            string message = "Test message.";
            int key = 10;
            Exception inner = new Exception("Inner message");
            KeyNotFoundException<int> toTest = new KeyNotFoundException<int>(key, message, inner);
            Stream stream = SerializationUtilities.Serialize(toTest);

            // act:
            KeyNotFoundException<int> deserialized = SerializationUtilities.Deserialize<KeyNotFoundException<int>>(stream);

            // assert:
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(inner.Message, deserialized.InnerException.Message);
            Assert.AreEqual(message, deserialized.Message);
            Assert.AreEqual(key, deserialized.Key);
        }
    }
}

