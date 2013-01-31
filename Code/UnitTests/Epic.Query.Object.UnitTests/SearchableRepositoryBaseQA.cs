//
//  SearchableRepositoryBaseQA.cs
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
using System;
using NUnit.Framework;
using Rhino.Mocks;
using Epic.Collections;
using Epic.Math;

namespace Epic.Query.Object.UnitTests
{
    [TestFixture]
    public class SearchableRepositoryBaseQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutAnyArgument_throwsArgumentNullException()
        {
            // arrange:
            IIdentityMap<string, int> identityMap = GenerateStrictMock<IIdentityMap<string, int>>();
            IEntityLoader<string, int> loader = GenerateStrictMock<IEntityLoader<string, int>>();
            IMapping<string, int> identification = GenerateStrictMock<IMapping<string, int>>();
            string deferrerName = "Test";

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new Fakes.FakeSearchableRepository<string, int>(null, loader, identification, deferrerName);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                new Fakes.FakeSearchableRepository<string, int>(identityMap, null, identification, deferrerName);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                new Fakes.FakeSearchableRepository<string, int>(identityMap, loader, null, deferrerName);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                new Fakes.FakeSearchableRepository<string, int>(identityMap, loader, identification, null);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                new Fakes.FakeSearchableRepository<string, int>(identityMap, loader, identification, string.Empty);
            });
        }

        [Test]
        public void Initialize_withEveryArgument_works()
        {
            // arrange:
            IIdentityMap<string, string> identityMap = GenerateStrictMock<IIdentityMap<string, string>>();
            IEntityLoader<string, string> loader = GenerateStrictMock<IEntityLoader<string, string>>();
            IMapping<string, string> identification = GenerateStrictMock<IMapping<string, string>>();
            string deferrerName = "Test";

            // act:
            SearchableRepositoryBase<string, string> toTest = new Fakes.FakeSearchableRepository<string, string>(identityMap, loader, identification, deferrerName);
        }
    }
}

