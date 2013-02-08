//
//  EpicExceptionQABase.cs
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
using System.Runtime.Serialization;
using System.Reflection;
using System.Collections.Generic;

namespace Epic
{
    /// <summary>
    /// Test fixture for exceptions deriving <seealso cref="EpicException"/>.
    /// </summary>
    [TestFixture]
    public abstract class EpicExceptionQABase<TException> where TException : EpicException
    {
        [Test]
        public void Exception_isMarkedAsSerializable()
        {
            // arrange:
            Type eType = typeof(TException);

            // assert:
            Assert.IsTrue(eType.IsSerializable);
        }

        [Test]
        public void Exception_thatHaveAnyField_overridesGetObjectData()
        {
            // arrange:
            Type eType = typeof(TException);
            FieldInfo[] fields = eType.GetFields(BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.DeclaredOnly);

            // act:
            MethodInfo method = null;
            if(fields.Length > 0)
            {
                method = eType.GetMethod("GetObjectData", 
                                         BindingFlags.Instance | BindingFlags.Public| BindingFlags.NonPublic);
            }

            // assert:
            Assert.IsTrue(fields.Length == 0 || method != null);
        }

        protected abstract IEnumerable<TException> ExceptionsToSerialize { get; }

        [Test, TestCaseSource("ExceptionsToSerialize")]
        public void Serialization_works(TException exception)
        {
            System.IO.Stream stream = SerializationUtilities.Serialize(exception);
            Assert.IsNotNull(stream);
        }
        
        [Test, TestCaseSource("ExceptionsToSerialize")]
        public void Deserialization_works(TException exception)
        {
            // arrange:
            System.IO.Stream stream = SerializationUtilities.Serialize(exception);

            // act:
            EpicException deserialized = SerializationUtilities.Deserialize<EpicException>(stream);

            // assert:
            Assert.AreEqual(exception.Message, deserialized.Message);
            if(exception.InnerException != null)
                Assert.AreEqual(exception.InnerException.Message, deserialized.InnerException.Message);
            foreach(FieldInfo field in typeof(TException).GetFields(BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.DeclaredOnly))
            {
                Assert.AreEqual(field.GetValue(exception), field.GetValue(deserialized), string.Format("After deserialization the values of the field {0} differs.", field.Name));
            }
        }

    }
}

