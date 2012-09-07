//  
//  VisitContextQA.cs
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

namespace Epic
{
    [TestFixture()]
    public class VisitContextQA
    {
        [Test]
        public void aNewVisitContext_doNotThrows()
        {
            // act:
            IVisitContext context = VisitContext.New;
            Assert.IsNotNull(context);
        }
        
        [TestCase("testString")]
        [TestCase(1)]
        public void TryGet_onAContextWithAValue_returnsTheValue<T>(T valueToSet)
        {
            // arrange:
            T valueHolder;
            IVisitContext context = VisitContext.New.With<T>(valueToSet);
            
            // act:
            bool valueHasBeenFound = context.TryGet<T>(out valueHolder);

            // assert:
            Assert.IsTrue(valueHasBeenFound);
            Assert.AreEqual(valueToSet, valueHolder);
        }
        
       
        [TestCase("testString")]
        [TestCase(1)]
        public void TryGet_onAContextWithAValueAndSomethingElse_returnsTheValue<T>(T valueToSet)
        {
            // arrange:
            T valueHolder;
            IVisitContext context = VisitContext.New.With<T>(valueToSet).With<object>(null);
            
            // act:
            bool valueHasBeenFound = context.TryGet<T>(out valueHolder);

            // assert:
            Assert.IsTrue(valueHasBeenFound);
            Assert.AreEqual(valueToSet, valueHolder);
        }
        
        [TestCase("testString")]
        [TestCase(1)]
        public void TryGet_onAContextWithANullValue_returnsTheNullAsNeeded<T>(T valueToSet)
        {
            // arrange:
            object valueHolder;
            IVisitContext context = VisitContext.New.With<object>(null).With<T>(valueToSet);
            
            // act:
            bool valueHasBeenFound = context.TryGet<object>(out valueHolder);

            // assert:
            Assert.IsTrue(valueHasBeenFound);
            Assert.IsNull(valueHolder);
        }

        [TestCase("returned", 1)]
        [TestCase(1, "returned")]
        public void TryGet_onAContextWithOnlyOneValueForEachType_returnsTheRightValue<T,Q>(Q qValue, T tValue)
        {
            // arrange:
            T tValueHolder;
            Q qValueHolder;
            bool tValueFound;
            bool qValueFound;
            IVisitContext context = VisitContext.New.With(qValue)
                                                    .With(tValue);

            // act:
            tValueFound = context.TryGet(out tValueHolder);
            qValueFound = context.TryGet(out qValueHolder);

            // assert:
            Assert.AreEqual(qValue, qValueHolder);
            Assert.AreEqual(tValue, tValueHolder);
            Assert.IsTrue(tValueFound);
            Assert.IsTrue(qValueFound);
        }
        
        [TestCase("ignored", 1, "returned")]
        [TestCase(0, "returned", 1)]
        public void TryGet_onAContextWithMoreValues_returnsTheLastValueRegistered<T,Q>(T valueToOverride, Q qValue, T tValue)
        {
            // arrange:
            T tValueHolder;
            Q qValueHolder;
            bool tValueFound;
            bool qValueFound;
            IVisitContext context = VisitContext.New.With(valueToOverride)
                                                    .With(qValue)
                                                    .With(tValue);

            // act:
            tValueFound = context.TryGet(out tValueHolder);
            qValueFound = context.TryGet(out qValueHolder);

            // assert:
            Assert.AreEqual(qValue, qValueHolder);
            Assert.AreEqual(tValue, tValueHolder);
            Assert.IsTrue(tValueFound);
            Assert.IsTrue(qValueFound);
        }


        [TestCase("testString")]
        [TestCase(1)]
        public void TryGet_anAbsentTypeInAContextWithAValue_returnsFalse<T>(T valueToSet)
        {
            // arrange:
            object valueHolder;
            IVisitContext context = VisitContext.New.With<T>(valueToSet);
            
            // act:
            bool valueHasBeenFound = context.TryGet<object>(out valueHolder);

            // assert:
            Assert.IsFalse(valueHasBeenFound);
            Assert.IsNull(valueHolder);
        }

        [TestCase("testString")]
        [TestCase(1)]
        public void TryGet_onANewVisitContext_returnsFalse<T>(T ignoredValue)
        {
            // arrange:
            T dummyVar;
            IVisitContext context = VisitContext.New;

            // act:
            bool valueHasBeenFound = context.TryGet<T>(out dummyVar);

            // assert:
            Assert.IsFalse(valueHasBeenFound);
            Assert.AreEqual(default(T), dummyVar);
        }

        
        [TestCase("testString")]
        [TestCase(1)]
        public void Get_onANewVisitContext_throwsInvalidOperationException<T>(T ignoredValue)
        {
            // arrange:
            IVisitContext context = VisitContext.New;

            // assert:
            Assert.Throws<InvalidOperationException>(delegate {
                context.Get<T>();
            });
        }
        
        [TestCase("testString")]
        [TestCase(1)]
        public void Get_onAContextWithoutTheNeededObject_throwsInvalidOperationException<T>(T ignoredValue)
        {
            // arrange:
            IVisitContext context = VisitContext.New.With(new object());

            // assert:
            Assert.Throws<InvalidOperationException>(delegate {
                context.Get<T>();
            });
        }

        [TestCase("testString")]
        [TestCase(1)]
        public void Get_anAbsentTypeInAContextWithAValue_throwsInvalidOperationException<T>(T valueToSet)
        {
            // arrange:
            IVisitContext context = VisitContext.New
                                        .With<T>(valueToSet);
            
            // assert:
            Assert.Throws<InvalidOperationException>(delegate {
                context.Get<object>();
            });
        }

        
        [TestCase("testString")]
        [TestCase(1)]
        public void Get_onAContextWithAValue_returnsTheValue<T>(T valueToSet)
        {
            // arrange:
            T valueHolder;
            IVisitContext context = VisitContext.New;
            context = context.With<T>(valueToSet);

            // act:
            valueHolder = context.Get<T>();

            // assert:
            Assert.AreEqual(valueToSet, valueHolder);
        }

        [TestCase("ignored", 1, "returned")]
        [TestCase(0, "returned", 1)]
        public void Get_onAContextWithMoreValues_returnsTheLastValueRegistered<T,Q>(T valueToOverride, Q qValue, T tValue)
        {
            // arrange:
            T tValueHolder;
            Q qValueHolder;
            IVisitContext context = VisitContext.New.With(valueToOverride)
                                                    .With(qValue)
                                                    .With(tValue);

            // act:
            tValueHolder = context.Get<T>();
            qValueHolder = context.Get<Q>();

            // assert:
            Assert.AreEqual(qValue, qValueHolder);
            Assert.AreEqual(tValue, tValueHolder);
        }
        
        [TestCase("ignored", 1)]
        [TestCase(1, "returned")]
        public void Get_onAContextWithOnlyOneValueForEachType_returnsTheRightValue<T,Q>(T tValue, Q qValue)
        {
            // arrange:
            T tValueHolder;
            Q qValueHolder;
            IVisitContext context = VisitContext.New.With(qValue)
                                                    .With(tValue);

            // act:
            tValueHolder = context.Get<T>();
            qValueHolder = context.Get<Q>();

            // assert:
            Assert.AreEqual(qValue, qValueHolder);
            Assert.AreEqual(tValue, tValueHolder);
        }
  
        [TestCase("testString")]
        [TestCase(1)]
        public void Get_onAContextWithAValueAndSomethingElse_returnsTheValue<T>(T valueToSet)
        {
            // arrange:
            T valueHolder;
            IVisitContext context = VisitContext.New.With<T>(valueToSet).With<object>(null);
            
            // act:
            valueHolder = context.Get<T>();

            // assert:
            Assert.AreEqual(valueToSet, valueHolder);
        }
        
        [TestCase("testString")]
        [TestCase(1)]
        public void Get_onAContextWithANullValue_returnsTheNullAsNeeded<T>(T valueToSet)
        {
            // arrange:
            object valueHolder;
            IVisitContext context = VisitContext.New.With<object>(null).With<T>(valueToSet);
            
            // act:
            valueHolder = context.Get<object>();

            // assert:
            Assert.IsNull(valueHolder);
        }        
    }
}

