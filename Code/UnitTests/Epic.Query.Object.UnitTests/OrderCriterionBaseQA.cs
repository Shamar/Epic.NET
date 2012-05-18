//  
//  OrderCriterionBaseQA.cs
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
using Challenge00.DDDSample.Cargo;
using System.IO;

namespace Epic.Query.Object.UnitTests
{
    [TestFixture]
    public class OrderCriterionBaseQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_aCriterionOfAWrongType_throwsEpicException()
        {
            // assert:
            Assert.Throws<EpicException>(delegate {
                new Fakes.WrongFakeCriterion<ICargo>();
            });
        }

        [Test]
        public void Initialize_aCriterionWithTheRightType_works()
        {
            // act:
            OrderCriterion<ICargo> criterion = new Fakes.FakeCriterion<ICargo>();

            // assert:
            Assert.IsNotNull(criterion);
        }

        [Test]
        public void Serialization_works()
        {
            // arrange:
            OrderCriterion<ICargo> toSerialize = new Fakes.FakeCriterion<ICargo>(10);

            // act:
            Stream stream = SerializationUtilities.Serialize(toSerialize);

            // assert:
            Assert.IsNotNull (stream);
        }

        [Test]
        public void Deserialization_works()
        {
            // arrange:
            OrderCriterion<ICargo> toSerialize = new Fakes.FakeCriterion<ICargo>(10);
            Stream stream = SerializationUtilities.Serialize(toSerialize);

            // act:
            Fakes.FakeCriterion<ICargo> deserialized = SerializationUtilities.Deserialize<Fakes.FakeCriterion<ICargo>>(stream);

            // assert:
            Assert.IsNotNull (deserialized);
            Assert.AreEqual (10, deserialized.Identity);
        }

    }
}

