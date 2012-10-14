//
//  ContravariantOrderQA.cs
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
using Challenge00.DDDSample.Cargo;
using System.IO;
using System.Runtime.Serialization;
using System.Linq;
using Rhino.Mocks;

namespace Epic.Query.Object.UnitTests
{
    public interface ISpecializedCargo : ICargo {}
    public interface IMoreSpecializedCargo : ISpecializedCargo {}

    [TestFixture]
    public class ContravariantOrderQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Initialize_withoutACriterionToWrap_throwsArgumentNullException ()
        {
            // assert:
            Assert.Throws<ArgumentNullException> (delegate {
                new ContravariantOrder<ICargo, ISpecializedCargo> (null);
            });
        }
            
        [Test]
        public void Initialize_withACriterionToWrap_works ()
        {
            // arrange:
            OrderCriterion<ICargo> other = new Fakes.FakeCriterion<ICargo> ();
                
            // act:
            new ContravariantOrder<ICargo, ISpecializedCargo> (other);
        }
            
        [Test]
        public void Serialization_works ()
        {
            // arrange:
            OrderCriterion<ICargo> other = new Fakes.FakeCriterion<ICargo> ();
            ContravariantOrder<ICargo, ISpecializedCargo> toSerialize = new ContravariantOrder<ICargo, ISpecializedCargo> (other);
                
            // act:
            Stream stream = SerializationUtilities.Serialize (toSerialize);
                
            // assert:
            Assert.IsNotNull (stream);
        }
            
        [Test]
        public void Deserialization_works ()
        {
            // arrange:
            OrderCriterion<ICargo> other = new Fakes.FakeCriterion<ICargo> (10);
            ContravariantOrder<ICargo, ISpecializedCargo> toSerialize = new ContravariantOrder<ICargo, ISpecializedCargo> (other);
            Stream stream = SerializationUtilities.Serialize (toSerialize);
                
            // act:
            ContravariantOrder<ICargo, ISpecializedCargo> deserialized = SerializationUtilities.Deserialize<ContravariantOrder<ICargo, ISpecializedCargo>> (stream);
                
            // assert:
            Assert.IsNotNull (deserialized);
        }

        [Test]
        public void Serialization_withoutSerializationInfo_throwsArgumentNullException ()
        {
            // arrange:
            OrderCriterion<ICargo> other = new Fakes.FakeCriterion<ICargo> (10);
            ISerializable toTest = new ContravariantOrder<ICargo, ISpecializedCargo> (other);
                
            // assert:
            Assert.Throws<ArgumentNullException> (delegate {
                toTest.GetObjectData (null, default(StreamingContext));
            });
        }

        [Test]
        public void For_aMoreSpecializedEntity_delegateToTheInner ()
        {
            // arrange:
            Fakes.FakeCriterion<ICargo> inner = new Fakes.FakeCriterion<ICargo>(1);
            var toTest = new ContravariantOrder<ICargo, ISpecializedCargo> (inner);

            // act:
            var result = toTest.For<IMoreSpecializedCargo>();

            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ContravariantOrder<ICargo, IMoreSpecializedCargo>>(result);
        }

        [Test]
        public void Reverse_returnsAReversedInstance()
        {
            // arrange:
            Fakes.FakeCriterion<ICargo> inner = new Fakes.FakeCriterion<ICargo>(1);
            var toTest = new ContravariantOrder<ICargo, ISpecializedCargo> (inner);
            
            // act:
            OrderCriterion<ISpecializedCargo> result = toTest.Reverse();
            
            // assert:
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ReverseOrder<ISpecializedCargo>>(result);
        }

        [Test]
        public void Equals_toAContravariantOrderWrappingAnEqualCriterion_isTrue ()
        {
            // arrange:
            OrderCriterion<ICargo> inner1 = new Fakes.FakeCriterion<ICargo> (10);
            OrderCriterion<ICargo> inner2 = new Fakes.FakeCriterion<ICargo> (10);
            Assert.IsTrue (inner1.Equals (inner2) && inner2.Equals (inner1)); // just to be safe.
            OrderCriterion<ISpecializedCargo> toTest = new ContravariantOrder<ICargo, ISpecializedCargo> (inner1);
            OrderCriterion<ISpecializedCargo> other = new ContravariantOrder<ICargo, ISpecializedCargo> (inner2);
                
            // act:
            bool result = toTest.Equals (other);
                
            // assert:
            Assert.IsTrue (result);
        }
            
        [Test]
        public void Equals_toAContravariantOrderWrappingADifferentCriterion_isFalse ()
        {
            // arrange:
            OrderCriterion<ICargo> inner1 = new Fakes.FakeCriterion<ICargo> (10);
            OrderCriterion<ICargo> inner2 = new Fakes.FakeCriterion<ICargo> (1);
            Assert.IsFalse (inner1.Equals (inner2) && inner2.Equals (inner1)); // just to be safe.
            OrderCriterion<ISpecializedCargo> toTest = new ContravariantOrder<ICargo, ISpecializedCargo> (inner1);
            OrderCriterion<ISpecializedCargo> other = new ContravariantOrder<ICargo, ISpecializedCargo> (inner2);
                
            // act:
            bool result = toTest.Equals (other);
                
            // assert:
            Assert.IsFalse (result);
        }
            
        [Test]
        public void Chain_withAnotherCriterion_returnsAnOrderCriteriaWithTheOriginalAsFirst ()
        {
            // arrange:
            OrderCriterion<ISpecializedCargo> other = new Fakes.FakeCriterion<ISpecializedCargo> (5);
            OrderCriterion<ICargo> inner = new Fakes.FakeCriterion<ICargo> (10);
            OrderCriterion<ISpecializedCargo> toTest = new ContravariantOrder<ICargo, ISpecializedCargo> (inner);
                
            // act:
            OrderCriterion<ISpecializedCargo> chain = toTest.Chain (other);
                
            // assert:
            Assert.IsNotNull (chain);
            Assert.IsInstanceOf<OrderCriteria<ISpecializedCargo>> (chain);
            OrderCriteria<ISpecializedCargo> criteria = (OrderCriteria<ISpecializedCargo>)chain;
            Assert.AreEqual (toTest, criteria.ElementAt (0));
            Assert.AreEqual (other, criteria.ElementAt (1));
        }
            
        [Test]
        public void Chain_withoutAnotherCriterion_throwsArgumentNullException ()
        {
            // arrange:
            OrderCriterion<ICargo> inner = new Fakes.FakeCriterion<ICargo> (10);
            OrderCriterion<ISpecializedCargo> toTest = new ContravariantOrder<ICargo, ISpecializedCargo> (inner);
                
            // assert:
            Assert.Throws<ArgumentNullException> (delegate {
                toTest.Chain (null);
            });
        }
            
        [Test]
        public void Compare_withArguments_callTheInner ()
        {
            // arrange:
            int expectedResult = 1;
            ICargo xRecieved = null;
            ICargo yRecieved = null;
            ISpecializedCargo x = GenerateStrictMock<ISpecializedCargo> ();
            ISpecializedCargo y = GenerateStrictMock<ISpecializedCargo> ();
            OrderCriterion<ICargo> inner = new Fakes.FakeCriterion<ICargo> (10, (c1, c2) => {
                xRecieved = c1;
                yRecieved = c2;
                return expectedResult;
            });
            OrderCriterion<ISpecializedCargo> toTest = new ContravariantOrder<ICargo, ISpecializedCargo> (inner);
                
            // act:
            int comparisonResult = toTest.Compare (x, y);
                
            // assert:
            Assert.AreEqual (expectedResult, comparisonResult);
            Assert.AreSame (x, xRecieved);
            Assert.AreSame (y, yRecieved);
        }
            
        [Test]
        public void Accept_withValidArguments_delegateVisitToTheVisitorOfTheInner ()
        {
            // arrange:
            Fakes.FakeCriterion<ICargo> inner = new Fakes.FakeCriterion<ICargo> (10);
            ContravariantOrder<ICargo, ISpecializedCargo> toTest = new ContravariantOrder<ICargo, ISpecializedCargo>(inner);
            object expectedResult = new object ();
            IVisitContext context = GenerateStrictMock<IVisitContext> ();
            IVisitor<object, Fakes.FakeCriterion<ICargo>> expressionVisitor = GenerateStrictMock<IVisitor<object, Fakes.FakeCriterion<ICargo>>> ();
            expressionVisitor.Expect (v => v.Visit (inner, context)).Return (expectedResult).Repeat.Once ();
            IVisitor<object> visitor = GenerateStrictMock<IVisitor<object>> ();
            visitor.Expect (v => v.AsVisitor (inner)).Return (expressionVisitor).Repeat.Once ();
                
            // act:
            object result = toTest.Accept (visitor, context);
                
            // assert:
            Assert.AreSame (expectedResult, result);
        }
    }

}
