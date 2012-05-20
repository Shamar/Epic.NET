//  
//  ExpressionQA.cs
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
using System.Runtime.Serialization;
using Epic.Query.Object.Expressions;
using Challenge00.DDDSample.Cargo;


namespace Epic.Query.Object.UnitTests.Expressions
{
    [TestFixture]
    public class ExpressionQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Deserialization_withoutSerializationInfo_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new Fakes.FakeExpression<ICargo>(null, default(StreamingContext));
            });
        }

        [Test]
        public void Serialization_withoutSerializationInfo_throwsArgumentNullException()
        {
            // arrange:
            ISerializable toTest = new Fakes.FakeExpression<ICargo>();

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                toTest.GetObjectData(null, default(StreamingContext));
            });
        }
    }
}

