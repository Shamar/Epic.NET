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
using Challenge00.DDDSample.Cargo;
using System.Linq;
using Epic.Specifications;
using System.Collections.Generic;
using Epic.Environment;

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

        [TestCase("dummy", "dummy")]
        [TestCase("dummy", 1)]
        public void Initialize_withEveryArgument_works<TEntity, TIdentity>(TEntity dummyEntity, TIdentity dummyId) where TEntity : class where TIdentity : IEquatable<TIdentity>
        {
            // arrange:
            IIdentityMap<TEntity, TIdentity> identityMap = GenerateStrictMock<IIdentityMap<TEntity, TIdentity>>();
            IEntityLoader<TEntity, TIdentity> loader = GenerateStrictMock<IEntityLoader<TEntity, TIdentity>>();
            IMapping<TEntity, TIdentity> identification = GenerateStrictMock<IMapping<TEntity, TIdentity>>();
            string deferrerName = "Test";

            // act:
            SearchableRepositoryBase<TEntity, TIdentity> toTest = new Fakes.FakeSearchableRepository<TEntity, TIdentity>(identityMap, loader, identification, deferrerName);
        }

        [Test]
        public void ItemAccess_withoutAKey_throwsArgumentNullException()
        {
            // arrange:
            string deferrerName = "Test";
            IIdentityMap<ICargo, TrackingId> identityMap = GenerateStrictMock<IIdentityMap<ICargo, TrackingId>>();
            IEntityLoader<ICargo, TrackingId> loader = GenerateStrictMock<IEntityLoader<ICargo, TrackingId>>();
            IMapping<ICargo, TrackingId> identification = GenerateStrictMock<IMapping<ICargo, TrackingId>>();
            SearchableRepositoryBase<ICargo, TrackingId> toTest = new Fakes.FakeSearchableRepository<ICargo, TrackingId>(identityMap, loader, identification, deferrerName);
            TrackingId id = null;

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                ICargo c = toTest[id];
            });
        }

        [Test]
        public void GetItem_withUnknownEntity_retrieveTheEntityFromTheLoader()
        {
            // arrange:
            TrackingId id = new TrackingId("TEST");
            ICargo expectedResult = GenerateStrictMock<ICargo>();
            string deferrerName = "Test";
            IMapping<ICargo, TrackingId> identification = GenerateStrictMock<IMapping<ICargo, TrackingId>>();
            IIdentityMap<ICargo, TrackingId> identityMap = GenerateStrictMock<IIdentityMap<ICargo, TrackingId>>();
            identityMap.Expect(m => m.Knows(id)).Return(false).Repeat.Once();
            identityMap.Expect(m => m.Register(expectedResult)).Repeat.Once();
            identityMap.Expect(m => m[id]).Return(expectedResult).Repeat.Once();
            IEntityLoader<ICargo, TrackingId> loader = GenerateStrictMock<IEntityLoader<ICargo, TrackingId>>();
            loader.Expect(l => l.Load(new TrackingId[] { id })).Return(new ICargo[]{expectedResult}).Repeat.Once();
            Fakes.FakeSearchableRepository<ICargo, TrackingId> toTest = GeneratePartialMock<Fakes.FakeSearchableRepository<ICargo, TrackingId>>(identityMap, loader, identification, deferrerName);
            toTest.Expect(r => r.CallInstrument(ref expectedResult)).Repeat.Once();

            // act:
            ICargo result = toTest[id];

            // assert:
            Assert.AreSame(expectedResult, result);
        }

        [Test]
        public void GetItem_withUnknownEntity_rethrowExceptionsFromTheInfrastructure()
        {
            // arrange:
            TrackingId id = new TrackingId("TEST");
            ICargo expectedResult = GenerateStrictMock<ICargo>();
            string deferrerName = "Test";
            Exception loadingException = new Exception();
            IMapping<ICargo, TrackingId> identification = GenerateStrictMock<IMapping<ICargo, TrackingId>>();
            IIdentityMap<ICargo, TrackingId> identityMap = GenerateStrictMock<IIdentityMap<ICargo, TrackingId>>();
            identityMap.Expect(m => m.Knows(id)).Return(false).Repeat.Once();
            IEntityLoader<ICargo, TrackingId> loader = GenerateStrictMock<IEntityLoader<ICargo, TrackingId>>();
            loader.Expect(l => l.Load(new TrackingId[] { id })).Throw(loadingException).Repeat.Once();
            SearchableRepositoryBase<ICargo, TrackingId> toTest = new Fakes.FakeSearchableRepository<ICargo, TrackingId>(identityMap, loader, identification, deferrerName);
            
            // assert:
            Assert.Throws<KeyNotFoundException<TrackingId>>(delegate {
                ICargo c = toTest[id];
            });
        }

        [Test]
        public void GetItem_withAKnownEntity_returnsTheEntityInTheMap()
        {
            // arrange:
            TrackingId id = new TrackingId("TEST");
            ICargo expectedResult = GenerateStrictMock<ICargo>();
            string deferrerName = "Test";
            IMapping<ICargo, TrackingId> identification = GenerateStrictMock<IMapping<ICargo, TrackingId>>();
            IIdentityMap<ICargo, TrackingId> identityMap = GenerateStrictMock<IIdentityMap<ICargo, TrackingId>>();
            identityMap.Expect(m => m.Knows(id)).Return(true).Repeat.Once();
            identityMap.Expect(m => m[id]).Return(expectedResult).Repeat.Once();
            IEntityLoader<ICargo, TrackingId> loader = GenerateStrictMock<IEntityLoader<ICargo, TrackingId>>();
            SearchableRepositoryBase<ICargo, TrackingId> toTest = new Fakes.FakeSearchableRepository<ICargo, TrackingId>(identityMap, loader, identification, deferrerName);
            
            // act:
            ICargo result = toTest[id];
            
            // assert:
            Assert.AreSame(expectedResult, result);
        }

        [Test]
        public void LoadingAsEntityLoader_asAnEntityLoader_loadsOnlyEntitiesUnknownToTheIdentityMapButReturnsAllEntitiesRequired()
        {
            // arrange:
            TrackingId id1 = new TrackingId("TEST1");
            TrackingId id2 = new TrackingId("TEST2");
            TrackingId id3 = new TrackingId("TEST3");
            TrackingId id4 = new TrackingId("TEST4");
            ICargo expectedResult1 = GenerateStrictMock<ICargo>();
            ICargo expectedResult2 = GenerateStrictMock<ICargo>();
            ICargo expectedResult3 = GenerateStrictMock<ICargo>();
            ICargo expectedResult4 = GenerateStrictMock<ICargo>();
            string deferrerName = "Test";
            IMapping<ICargo, TrackingId> identification = GenerateStrictMock<IMapping<ICargo, TrackingId>>();
            IIdentityMap<ICargo, TrackingId> identityMap = GenerateStrictMock<IIdentityMap<ICargo, TrackingId>>();
            identityMap.Expect(m => m.Knows(id1)).Return(true).Repeat.Once();
            identityMap.Expect(m => m[id1]).Return(expectedResult1).Repeat.Once();
            identityMap.Expect(m => m.Knows(id2)).Return(false).Repeat.Once();
            identityMap.Expect(m => m.Register(expectedResult2)).Repeat.Once();
            identityMap.Expect(m => m.Knows(id3)).Return(true).Repeat.Once();
            identityMap.Expect(m => m[id3]).Return(expectedResult3).Repeat.Once();
            identityMap.Expect(m => m.Knows(id4)).Return(false).Repeat.Once();
            identityMap.Expect(m => m.Register(expectedResult4)).Repeat.Once();
            IEntityLoader<ICargo, TrackingId> loader = GenerateStrictMock<IEntityLoader<ICargo, TrackingId>>();
            loader.Expect(l => l.Load(new TrackingId[] { id2, id4 })).Return(new ICargo[]{expectedResult2, expectedResult4}).Repeat.Once();
            Fakes.FakeSearchableRepository<ICargo, TrackingId> searchableRepo = GeneratePartialMock<Fakes.FakeSearchableRepository<ICargo, TrackingId>>(identityMap, loader, identification, deferrerName);
            searchableRepo.Expect(r => r.CallInstrument(ref expectedResult2)).Repeat.Once();
            searchableRepo.Expect(r => r.CallInstrument(ref expectedResult4)).Repeat.Once();
            IEntityLoader<ICargo, TrackingId> toTest = searchableRepo;

            // act:
            ICargo[] result = toTest.Load(new TrackingId[]{id1, id2, id3, id4}).ToArray();

            // assert:
            Assert.AreSame(expectedResult1, result[0]);
            Assert.AreSame(expectedResult2, result[1]);
            Assert.AreSame(expectedResult3, result[2]);
            Assert.AreSame(expectedResult4, result[3]);
        }

        [Test]
        public void LoadingAsEntityLoader_withoutIdentifiers_throwsArgumentNullException()
        {
            // arrange:
            string deferrerName = "Test";
            IMapping<ICargo, TrackingId> identification = GenerateStrictMock<IMapping<ICargo, TrackingId>>();
            IIdentityMap<ICargo, TrackingId> identityMap = GenerateStrictMock<IIdentityMap<ICargo, TrackingId>>();
            IEntityLoader<ICargo, TrackingId> loader = GenerateStrictMock<IEntityLoader<ICargo, TrackingId>>();
            Fakes.FakeSearchableRepository<ICargo, TrackingId> searchableRepo = GeneratePartialMock<Fakes.FakeSearchableRepository<ICargo, TrackingId>>(identityMap, loader, identification, deferrerName);
            IEntityLoader<ICargo, TrackingId> toTest = searchableRepo;
            
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                var r = toTest.Load(null);
            });
        }

        [Test]
        public void LoadingAsEntityLoader_withoutIdentifiers_returnAnEmptyEnumeration()
        {
            // arrange:
            string deferrerName = "Test";
            IMapping<ICargo, TrackingId> identification = GenerateStrictMock<IMapping<ICargo, TrackingId>>();
            IIdentityMap<ICargo, TrackingId> identityMap = GenerateStrictMock<IIdentityMap<ICargo, TrackingId>>();
            IEntityLoader<ICargo, TrackingId> loader = GenerateStrictMock<IEntityLoader<ICargo, TrackingId>>();
            Fakes.FakeSearchableRepository<ICargo, TrackingId> searchableRepo = GeneratePartialMock<Fakes.FakeSearchableRepository<ICargo, TrackingId>>(identityMap, loader, identification, deferrerName);
            IEntityLoader<ICargo, TrackingId> toTest = searchableRepo;
            
            // act:
            var r = toTest.Load();
            
            // assert:
            CollectionAssert.IsEmpty(r);
        }

        [Test]
        public void Identify_withAnyEntity_callsTheIdentificationMapping()
        {
            // arrange:
            string deferrerName = "Test";
            TrackingId id = new TrackingId("TEST");
            ICargo cargo = GenerateStrictMock<ICargo>();
            IMapping<ICargo, TrackingId> identification = GenerateStrictMock<IMapping<ICargo, TrackingId>>();
            identification.Expect(i => i.ApplyTo(cargo)).Return(id).Repeat.Once();
            IIdentityMap<ICargo, TrackingId> identityMap = GenerateStrictMock<IIdentityMap<ICargo, TrackingId>>();
            IEntityLoader<ICargo, TrackingId> loader = GenerateStrictMock<IEntityLoader<ICargo, TrackingId>>();
            Fakes.FakeSearchableRepository<ICargo, TrackingId> toTest = GeneratePartialMock<Fakes.FakeSearchableRepository<ICargo, TrackingId>>(identityMap, loader, identification, deferrerName);

            // act:
            var r = toTest.CallIdentify(cargo);
            
            // assert:
            Assert.AreSame(id, r);
        }

        [Test]
        public void Dispose_withEntitiesInTheMap_callCleanUpOnThem()
        {
            // arrange:
            string deferrerName = "Test";
            TrackingId id1 = new TrackingId("TEST1");
            TrackingId id2 = new TrackingId("TEST2");
            ISpecification<ICargo> specification = GenerateStrictMock<ISpecification<ICargo>>();
            ICargo cargo1 = GenerateStrictMock<ICargo>();
            ICargo cargo2 = GenerateStrictMock<ICargo>();
            IMapping<ICargo, TrackingId> identification = GenerateStrictMock<IMapping<ICargo, TrackingId>>();
            IIdentityMap<ICargo, TrackingId> identityMap = GenerateStrictMock<IIdentityMap<ICargo, TrackingId>>();
            identityMap.Expect(i => i.ForEachKnownEntity(null)).IgnoreArguments().WhenCalled(
                mi => {
                (mi.Arguments[0] as Action<ICargo>)(cargo1);
                (mi.Arguments[0] as Action<ICargo>)(cargo2);
            }
            );
            identityMap.Expect(i => i.Dispose()).Repeat.Once();
            IEntityLoader<ICargo, TrackingId> loader = GenerateStrictMock<IEntityLoader<ICargo, TrackingId>>();
            Fakes.FakeSearchableRepository<ICargo, TrackingId> searchableRepo = GeneratePartialMock<Fakes.FakeSearchableRepository<ICargo, TrackingId>>(identityMap, loader, identification, deferrerName);
            searchableRepo.Expect(r => r.CallCleanUp(cargo1)).Repeat.Once();
            searchableRepo.Expect(r => r.CallCleanUp(cargo2)).Repeat.Once();
            searchableRepo.Expect(r => r.CallDispose()).Repeat.Once();
            IDisposable toTest = searchableRepo;

            // act:
            toTest.Dispose();
            toTest.Dispose(); // Dispose must be idempotent.

            // assert:
            Assert.Throws<ObjectDisposedException>(delegate {
                var r = (searchableRepo as IEntityLoader<ICargo, TrackingId>).Load(new TrackingId[] { id1, id2 });
            });
            Assert.Throws<ObjectDisposedException>(delegate {
                var r = searchableRepo[id1];
            });
            Assert.Throws<ObjectDisposedException>(delegate {
                var r = searchableRepo.Search<ICargo>(specification);
            });
        }

        [Test]
        public void Search_withoutASpecification_throwsArgumentNullException()
        {
            // arrange:
            string deferrerName = "Test";
            IMapping<ICargo, TrackingId> identification = GenerateStrictMock<IMapping<ICargo, TrackingId>>();
            IIdentityMap<ICargo, TrackingId> identityMap = GenerateStrictMock<IIdentityMap<ICargo, TrackingId>>();
            IEntityLoader<ICargo, TrackingId> loader = GenerateStrictMock<IEntityLoader<ICargo, TrackingId>>();
            Fakes.FakeSearchableRepository<ICargo, TrackingId> searchableRepo = GeneratePartialMock<Fakes.FakeSearchableRepository<ICargo, TrackingId>>(identityMap, loader, identification, deferrerName);

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                var r = searchableRepo.Search<ICargo>(null);
            });
        }

        [Test]
        public void Search_withASpecificationOfTheBaseType_returnsADeferredProducedByTheDeferrer()
        {
            // arrange:
            ISpecification<ICargo> specification = GenerateStrictMock<ISpecification<ICargo>>();
            IDeferrer deferrer = GenerateStrictMock<IDeferrer>();
            ISearch<ICargo, TrackingId> expectedResult = GenerateStrictMock<ISearch<ICargo, TrackingId>>();
            deferrer.Expect(d => d.Defer<ISearch<ICargo, TrackingId>, IEnumerable<ICargo>>(null)).IgnoreArguments().Return(expectedResult).Repeat.Once();
            string deferrerName = "Test";
            TestUtilities.ResetEnterprise();
            string appName = "SampleApp";
            EnvironmentBase environment = MockRepository.GenerateStrictMock<EnvironmentBase>();
            environment.Expect(e => e.Get<IDeferrer>(new InstanceName<IDeferrer>(deferrerName))).Return(deferrer).Repeat.Once();
            IOrganization organization = MockRepository.GenerateStrictMock<IOrganization>();
            EnterpriseBase appSingleton = MockRepository.GeneratePartialMock<EnterpriseBase>(appName);
            appSingleton.Expect(a => a.Environment).Return(environment).Repeat.Once();
            appSingleton.Expect(a => a.Organization).Return(organization).Repeat.Once();
            Enterprise.Initialize(appSingleton);
            IMapping<ICargo, TrackingId> identification = GenerateStrictMock<IMapping<ICargo, TrackingId>>();
            IIdentityMap<ICargo, TrackingId> identityMap = GenerateStrictMock<IIdentityMap<ICargo, TrackingId>>();
            IEntityLoader<ICargo, TrackingId> loader = GenerateStrictMock<IEntityLoader<ICargo, TrackingId>>();
            Fakes.FakeSearchableRepository<ICargo, TrackingId> searchableRepo = GeneratePartialMock<Fakes.FakeSearchableRepository<ICargo, TrackingId>>(identityMap, loader, identification, deferrerName);

            // act:
            var result = searchableRepo.Search(specification);

            // assert:
            Assert.AreSame(expectedResult, result);
        }

        [Test]
        public void Search_withASpecificationOfADerivedType_returnsADeferredProducedByTheDeferrer()
        {
            // arrange:
            ISpecification<ISpecializedCargo> specification = GenerateStrictMock<ISpecification<ISpecializedCargo>>();
            IDeferrer deferrer = GenerateStrictMock<IDeferrer>();
            ISearch<ISpecializedCargo, TrackingId> expectedResult = GenerateStrictMock<ISearch<ISpecializedCargo, TrackingId>>();
            deferrer.Expect(d => d.Defer<ISearch<ISpecializedCargo, TrackingId>, IEnumerable<ISpecializedCargo>>(null)).IgnoreArguments().Return(expectedResult).Repeat.Once();
            string deferrerName = "Test";
            TestUtilities.ResetEnterprise();
            string appName = "SampleApp";
            EnvironmentBase environment = MockRepository.GenerateStrictMock<EnvironmentBase>();
            environment.Expect(e => e.Get<IDeferrer>(new InstanceName<IDeferrer>(deferrerName))).Return(deferrer).Repeat.Once();
            IOrganization organization = MockRepository.GenerateStrictMock<IOrganization>();
            EnterpriseBase appSingleton = MockRepository.GeneratePartialMock<EnterpriseBase>(appName);
            appSingleton.Expect(a => a.Environment).Return(environment).Repeat.Once();
            appSingleton.Expect(a => a.Organization).Return(organization).Repeat.Once();
            Enterprise.Initialize(appSingleton);
            IMapping<ICargo, TrackingId> identification = GenerateStrictMock<IMapping<ICargo, TrackingId>>();
            IIdentityMap<ICargo, TrackingId> identityMap = GenerateStrictMock<IIdentityMap<ICargo, TrackingId>>();
            IEntityLoader<ICargo, TrackingId> loader = GenerateStrictMock<IEntityLoader<ICargo, TrackingId>>();
            Fakes.FakeSearchableRepository<ICargo, TrackingId> searchableRepo = GeneratePartialMock<Fakes.FakeSearchableRepository<ICargo, TrackingId>>(identityMap, loader, identification, deferrerName);
            
            // act:
            var result = searchableRepo.Search(specification);
            
            // assert:
            Assert.AreSame(expectedResult, result);
        }
    }
}

