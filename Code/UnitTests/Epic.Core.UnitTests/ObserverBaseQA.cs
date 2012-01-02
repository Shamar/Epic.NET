//  
//  ObserverBaseQA.cs
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
using Epic.Fakes;
using Challenge00.DDDSample.Cargo;
using Rhino.Mocks;

namespace Epic
{
    [TestFixture()]
    public class ObserverBaseQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Observe_withoutEntity_throwsArgumentNullException()
        {
            // arrange:
            ObserverBase<ICargo> observer = GeneratePartialMock<FakeObserver<ICargo>>();

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                observer.Observe(null);
            });
        }
        
        [Test]
        public void Ignore_withoutEntity_throwsArgumentNullException()
        {
            // arrange:
            ObserverBase<ICargo> observer = GeneratePartialMock<FakeObserver<ICargo>>();

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                observer.Ignore(null);
            });
        }
        
        [Test]
        public void Observe_aNewEntity_callSubscribe()
        {
            // arrange:
            ICargo cargo = GenerateStrictMock<ICargo>();
            FakeObserver<ICargo> observer = GeneratePartialMock<FakeObserver<ICargo>>();
            observer.Expect(o => o.CallSubscribe(cargo)).Repeat.Once();

            // act:
            observer.Observe(cargo);

            // assert:
            // all assertion verified by teardown
        }
        
        [Test]
        public void Observe_anAlreadyObservedEntity_dontCallSubscribe()
        {
            // arrange:
            ICargo cargo = GenerateStrictMock<ICargo>();
            FakeObserver<ICargo> observer = GeneratePartialMock<FakeObserver<ICargo>>();
            observer.Expect(o => o.CallSubscribe(cargo)).Repeat.Once();
            observer.Observe(cargo);

            // act:
            observer.Observe(cargo);
            observer.Observe(cargo);
            observer.Observe(cargo);

            // assert:
            // all assertion verified by teardown
        }
        
        [Test]
        public void Ignore_anObservedEntity_callUnsubscribe()
        {
            // arrange:
            ICargo cargo = GenerateStrictMock<ICargo>();
            FakeObserver<ICargo> observer = GeneratePartialMock<FakeObserver<ICargo>>();
            observer.Expect(o => o.CallSubscribe(cargo)).Repeat.Once();
            observer.Expect(o => o.CallUnsubscribe(cargo)).Repeat.Once();
            observer.Observe(cargo);

            // act:
            observer.Ignore(cargo);

            // assert:
            // all assertion verified by teardown
        }
        
        [Test]
        public void Ignore_anEntityUnknown_dontCallUnsubscribe()
        {
            // arrange:
            ICargo cargo = GenerateStrictMock<ICargo>();
            FakeObserver<ICargo> observer = GeneratePartialMock<FakeObserver<ICargo>>();
            
            // act:
            observer.Ignore(cargo);

            // assert:
            // all assertion verified by teardown
        }
        
        
        [Test]
        public void Ignore_anUnsubscribedEntity_dontCallUnsubscribe()
        {
            // arrange:
            ICargo cargo = GenerateStrictMock<ICargo>();
            FakeObserver<ICargo> observer = GeneratePartialMock<FakeObserver<ICargo>>();
            observer.Expect(o => o.CallSubscribe(cargo)).Repeat.Once();
            observer.Expect(o => o.CallUnsubscribe(cargo)).Repeat.Once();
            observer.Observe(cargo);
            observer.Ignore(cargo);
           
            // act:
            observer.Ignore(cargo);
            observer.Ignore(cargo);
            observer.Ignore(cargo);

            // assert:
            // all assertion verified by teardown
        }
  
        [Test]
        public void Dispose_whileObservingEntities_callUnsubscribeOnEachObservedEntity()
        {
            // arrange:
            ICargo cargo1 = GenerateStrictMock<ICargo>();
            ICargo cargo2 = GenerateStrictMock<ICargo>();
            FakeObserver<ICargo> observer = GeneratePartialMock<FakeObserver<ICargo>>();
            observer.Expect(o => o.CallSubscribe(cargo1)).Repeat.Once();
            observer.Expect(o => o.CallSubscribe(cargo2)).Repeat.Once();
            observer.Expect(o => o.CallUnsubscribe(cargo1)).Repeat.Once();
            observer.Expect(o => o.CallUnsubscribe(cargo2)).Repeat.Once();
            observer.Observe(cargo1);
            observer.Observe(cargo2);
           
            // act:
            observer.Dispose();

            // assert:
            // all assertion verified by teardown
        }
        
        [Test]
        public void Dispose_twice_callUnsubscribeOnEachObservedEntityOnlyOnce()
        {
            // arrange:
            ICargo cargo1 = GenerateStrictMock<ICargo>();
            ICargo cargo2 = GenerateStrictMock<ICargo>();
            FakeObserver<ICargo> observer = GeneratePartialMock<FakeObserver<ICargo>>();
            observer.Expect(o => o.CallSubscribe(cargo1)).Repeat.Once();
            observer.Expect(o => o.CallSubscribe(cargo2)).Repeat.Once();
            observer.Expect(o => o.CallUnsubscribe(cargo1)).Repeat.Once();
            observer.Expect(o => o.CallUnsubscribe(cargo2)).Repeat.Once();
            observer.Observe(cargo1);
            observer.Observe(cargo2);
            observer.Dispose();
           
            // act:
            observer.Dispose();

            // assert:
            // all assertion verified by teardown
        }
        
        [Test]
        public void Observe_afterDisposition_throwsObjectDisposedException()
        {
            // arrange:
            ICargo cargo = GenerateStrictMock<ICargo>();
            FakeObserver<ICargo> observer = GeneratePartialMock<FakeObserver<ICargo>>();
            observer.Dispose();
           
            // assert:
            Assert.Throws<ObjectDisposedException>(delegate {
                observer.Observe(cargo);
            });
        }
        
        [Test]
        public void Ignore_afterDisposition_throwsObjectDisposedException()
        {
            // arrange:
            ICargo cargo = GenerateStrictMock<ICargo>();
            FakeObserver<ICargo> observer = GeneratePartialMock<FakeObserver<ICargo>>();
            observer.Dispose();
           
            // assert:
            Assert.Throws<ObjectDisposedException>(delegate {
                observer.Ignore(cargo);
            });
        }
    }
}

