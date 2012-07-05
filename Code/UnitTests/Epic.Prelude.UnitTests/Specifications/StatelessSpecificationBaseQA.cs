//
//  StatelessSpecificationBaseQA.cs
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

namespace Epic.Specifications
{
    [TestFixture]
    public class StatelessSpecificationBaseQA
    {
        [Test]
        public void Initialization_works ()
        {
            // arrange:
            new Fakes.FakeStatelessSpecification<Fakes.FakeCandidate1>();
        }

        [Test]
        public void Equals_toAnotherInstanceOfTheSameType_isTrue ()
        {
            // arrange:
            var toTest = new Fakes.FakeStatelessSpecification<Fakes.FakeCandidate1>();
            var other = new Fakes.FakeStatelessSpecification<Fakes.FakeCandidate1>();

            // act:
            bool result = toTest.Equals(other);

            // assert:
            Assert.IsTrue(result);
        }
    }
}

