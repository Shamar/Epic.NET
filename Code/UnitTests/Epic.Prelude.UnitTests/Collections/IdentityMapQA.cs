//
//  IdentityMapQA.cs
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
using Epic.Math;
using Rhino.Mocks;
using System.IO;

namespace Epic.Collections
{
    [TestFixture()]
    public class IdentityMapQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutAMapping_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new IdentityMap<string, int>(null as IMapping<string, int>);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                new IdentityMap<string, int>(null as Func<string, int>);
            });
        }

        [Test]
        public void Initialize_withAFuncMapping_works()
        {
            // arrange:
            Func<string, int> mapping = s => s.Length;

            // act:
            IdentityMap<string, int> map = new IdentityMap<string, int>(mapping);

            // assert:
            Assert.IsNotNull(map);
        }

        [Test]
        public void Initialize_withAMapping_works()
        {
            // arrange:
            IMapping<string, int> mapping = GenerateStrictMock<IMapping<string, int>>();

            // act:
            IdentityMap<string, int> map = new IdentityMap<string, int>(mapping);

            // assert:
            Assert.IsNotNull(map);
        }

        [Test]
        public void Register_withoutTheEntity_throwsArgumentNullException()
        {
            // arrange:
            IMapping<string, int> mapping = GenerateStrictMock<IMapping<string, int>>();
            IdentityMap<string, int> map = new IdentityMap<string, int>(mapping);

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                map.Register(null);
            });
        }

        [Test]
        public void Register_withAnEntity_works()
        {
            // arrange:
            string entity = "Test";
            IMapping<string, int> mapping = GenerateStrictMock<IMapping<string, int>>();
            mapping.Expect(m => m.ApplyTo(entity)).Return(2).Repeat.Once();
            IdentityMap<string, int> map = new IdentityMap<string, int>(mapping);

            // act:
            map.Register(entity);

            // assert:
            Assert.IsTrue(map.Knows(2));
        }

        [Test]
        public void Register_twiceAnEntity_throwsArgumentException()
        {
            // arrange:
            string entity = "Test";
            IMapping<string, int> mapping = GenerateStrictMock<IMapping<string, int>>();
            mapping.Expect(m => m.ApplyTo(entity)).Return(2).Repeat.Twice();
            IdentityMap<string, int> map = new IdentityMap<string, int>(mapping);

            // act:
            map.Register(entity);

            // assert:
            Assert.Throws<ArgumentException>(delegate {
                map.Register(entity);
            });
        }    

        [Test]
        public void Knows_aRegisteredEntities_isTrue()
        {
            // arrange:
            string entity = "Test";
            IMapping<string, int> mapping = GenerateStrictMock<IMapping<string, int>>();
            mapping.Expect(m => m.ApplyTo(entity)).Return(2).Repeat.Once();
            IdentityMap<string, int> map = new IdentityMap<string, int>(mapping);

            // act:
            Assert.IsFalse(map.Knows(2));
            map.Register(entity);

            // assert:
            Assert.IsTrue(map.Knows(2));
        }

        [Test]
        public void Knows_withoutTheEntity_throwsArgumentNullException()
        {
            // arrange:
            IMapping<string, string> mapping = GenerateStrictMock<IMapping<string, string>>();
            IdentityMap<string, string> map = new IdentityMap<string, string>(mapping);

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                map.Knows(null);
            });
        }

        [Test]
        public void GetItem_withoutAnIdentity_throwsArgumentNullException()
        {
            // arrange:
            IMapping<string, string> mapping = GenerateStrictMock<IMapping<string, string>>();
            IdentityMap<string, string> map = new IdentityMap<string, string>(mapping);
            string result = null;

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                result = map[null];
            });
            Assert.IsNull(result);
        }

        [Test]
        public void GetItem_withAnUnknownEntity_throwsKeyNotFoundException()
        {
            // arrange:
            IMapping<string, int> mapping = GenerateStrictMock<IMapping<string, int>>();
            IdentityMap<string, int> map = new IdentityMap<string, int>(mapping);
            string result = null;

            // assert:
            Assert.Throws<KeyNotFoundException<int>>(delegate {
                result = map[2];
            });
            Assert.IsNull(result);
        }

        [Test]
        public void GetItem_withAKnownEntity_returnTheEntity()
        {
            // arrange:
            string entity = "test";
            IMapping<string, int> mapping = GenerateStrictMock<IMapping<string, int>>();
            mapping.Expect(m => m.ApplyTo(entity)).Return(2).Repeat.Once();
            IdentityMap<string, int> map = new IdentityMap<string, int>(mapping);
            map.Register(entity);
            string result = null;

            // act:
            result = map[2];

            // assert:
            Assert.AreSame(entity, result);
        }

        [Test]
        public void Serialization_works()
        {
            // arrange:
            string entity = "test";
            Func<string, int> mapping = s => s.Length;
            IdentityMap<string, int> map = new IdentityMap<string, int>(mapping);
            map.Register(entity);

            // act:
            Stream stream = SerializationUtilities.Serialize(map);

            // assert:
            Assert.IsNotNull(stream);
        }

        [Test]
        public void Deserialization_works()
        {
            // arrange:
            string entity = "test";
            Func<string, int> mapping = s => s.Length;
            IdentityMap<string, int> map = new IdentityMap<string, int>(mapping);
            map.Register(entity);
            Stream stream = SerializationUtilities.Serialize(map);

            // act:
            IdentityMap<string, int> deserialized = SerializationUtilities.Deserialize<IdentityMap<string, int>>(stream);

            // assert:
            Assert.IsNotNull(deserialized);
            Assert.IsTrue(deserialized.Knows(4));
            Assert.AreEqual(entity, deserialized[4]);
            Assert.IsFalse(deserialized.Knows(5));
            deserialized.Register("test1");
            Assert.IsTrue(deserialized.Knows(5));
            Assert.AreEqual("test1", deserialized[5]);
        }

        [Test]
        public void Dispose_works()
        {
            // arrange:
            string entity = "test";
            Func<string, int> mapping = s => s.Length;
            IdentityMap<string, int> map = new IdentityMap<string, int>(mapping);
            map.Register(entity);
            string result;

            // act:
            map.Dispose();

            // assert:
            Assert.Throws<ObjectDisposedException>(delegate {
                result = map[4];
            });
            Assert.Throws<ObjectDisposedException>(delegate {
                map.Register("test2");
            });
            Assert.Throws<ObjectDisposedException>(delegate {
                map.Knows(5);
            });
            Assert.Throws<ObjectDisposedException>(delegate {
                SerializationUtilities.Serialize(map);
            });
        }
    }
}

