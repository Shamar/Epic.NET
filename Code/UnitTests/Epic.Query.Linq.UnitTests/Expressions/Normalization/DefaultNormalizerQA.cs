//  
//  DefaultNormalizerQA.cs
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
using Epic.Environment;
using System.Linq;
using Epic.Query.Linq.Fakes;
using Rhino.Mocks;
using Challenge00.DDDSample.Cargo;
using Challenge00.DDDSample.Location;
using Challenge00.DDDSample.Voyage;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Epic.Query.Linq.Expressions.Normalization
{
    [TestFixture]
    public class DefaultNormalizerQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initalize_withoutAName_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new DefaultNormalizer(null);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                new DefaultNormalizer(string.Empty);
            });
        }

        [Test]
        public void Visit_CargoVisitLocationWithPrintingVisitor_works()
        {
            // arrange:
            string providerName = "test";
            EnvironmentBase env = GeneratePartialMock<EnvironmentBase>();
            InstanceName<IQueryProvider> instanceName = new InstanceName<IQueryProvider>(providerName);
            IQueryProvider mockProvider = new QueryProvider(providerName);
            env.Expect(e => e.Get<IQueryProvider>(Arg<InstanceName<IQueryProvider>>.Matches(n => n.Equals(instanceName)))).Return(mockProvider).Repeat.Any();
            EnterpriseBase enterprise = new Epic.Fakes.FakeEnterprise(env, null);
            Enterprise.Initialize(enterprise); // plumbing for RepositoryBase

            IQueryableRepository<ICargo, TrackingId> cargos = new FakeRepository<ICargo, TrackingId>(providerName);
            IQueryable<ICargo> movingCargos = from c in cargos
                                              where c.Delivery.TransportStatus == TransportStatus.OnboardCarrier 
                                              select c;
            IQueryableRepository<ILocation, UnLocode> locations = new FakeRepository<ILocation, UnLocode>(providerName);
            IQueryableRepository<IVoyage, VoyageNumber> voyages = new FakeRepository<IVoyage, VoyageNumber>(providerName);
            IEnumerable<IVoyage> movingVoyages = voyages.Where(v => v.IsMoving); // an iqueryable to unwrap from an ienumerable
            IQueryable<ILocation> locationsTraversedFromVoyagesEndingToday = 
                    from c in movingCargos
                    from l in locations
                    from v in movingVoyages
                    where   c.Delivery.CurrentVoyage == v.Number
                        &&  v.WillStopOverAt(l)
                        &&  c.Itinerary.FinalArrivalDate == DateTime.Today
                    select l;

            IVisitContext context = VisitContext.New.With(mockProvider);
            DefaultNormalizer normalizer = new DefaultNormalizer("test");

            // act:
            Expression returnedExpression = normalizer.Visit(locationsTraversedFromVoyagesEndingToday.Expression, context);
            
            // assert:
            // TODO: complete the checks
            Verify.That(returnedExpression).IsA<MethodCallExpression>()
                .WithA (e => e.Method, that => Assert.AreEqual("Select", that.Name));
        }
    }
}

