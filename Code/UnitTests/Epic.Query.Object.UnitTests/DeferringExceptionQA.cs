//  
//  DeferringExceptionQA.cs
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
using NUnit.Framework;
using System;
using Challenge00.DDDSample.Cargo;
using Epic.Query.Object.Expressions;
using System.Collections.Generic;

namespace Epic.Query.Object.UnitTests
{
    [TestFixture()]
    public class DeferringExceptionQA
    {
        private static string DefaultMessage = "An error occurred in the infrastructure.";

        [Test]
        public void Initialize_withMessage_works()
        {
            string message = "Test message.";
            DeferringException e = new DeferringException<ISearch<ICargo, TrackingId>, IEnumerable<ICargo>>(message);
            Assert.IsNotNull(e);
            Assert.AreEqual(message, e.Message);
            Assert.AreEqual(typeof(ISearch<ICargo, TrackingId>), e.DeferredType);
            Assert.AreEqual(typeof(IEnumerable<ICargo>), e.ResultType);
        }
        
        [Test]
        public void Initialize_withoutMessage_works()
        {
            DeferringException e = new DeferringException<ISearch<ICargo, TrackingId>, IEnumerable<ICargo>>(null);
            Assert.IsNotNull(e);
            Assert.AreEqual(DefaultMessage, e.Message);
            Assert.AreEqual(typeof(ISearch<ICargo, TrackingId>), e.DeferredType);
            Assert.AreEqual(typeof(IEnumerable<ICargo>), e.ResultType);
        }
        
        [Test]
        public void Initialize_withoutMessage_withInnerException_works()
        {
            Exception inner = new Exception();
            DeferringException e = new DeferringException<ISearch<ICargo, TrackingId>, IEnumerable<ICargo>>(null, inner);
            Assert.IsNotNull(e);
            Assert.AreEqual(DefaultMessage, e.Message);
            Assert.AreSame(inner, e.InnerException);
            Assert.AreEqual(typeof(ISearch<ICargo, TrackingId>), e.DeferredType);
            Assert.AreEqual(typeof(IEnumerable<ICargo>), e.ResultType);
        }
        
        [Test]
        public void Initialize_withMessage_withInnerException_works()
        {
            string message = "Test message.";
            Exception inner = new Exception();
            DeferringException e = new DeferringException<ISearch<ICargo, TrackingId>, IEnumerable<ICargo>>(message, inner);
            Assert.IsNotNull(e);
            Assert.AreEqual(message, e.Message);
            Assert.AreSame(inner, e.InnerException);
            Assert.AreEqual(typeof(ISearch<ICargo, TrackingId>), e.DeferredType);
            Assert.AreEqual(typeof(IEnumerable<ICargo>), e.ResultType);
        }
        
        [Test]
        public void Serialization_works()
        {
            string message = "Test message.";
            Exception inner = new Exception();
            DeferringException e = new DeferringException<ISearch<ICargo, TrackingId>, IEnumerable<ICargo>>(message, inner);

            System.IO.Stream stream = SerializationUtilities.Serialize(e);
            Assert.IsNotNull(stream);
        }
        
        [Test]
        public void Deserialization_works()
        {
            string innerMessage = "Test inner.";
            string message = "Test message.";
            Exception inner = new Exception(innerMessage);
            DeferringException e = new DeferringException<ISearch<ICargo, TrackingId>, IEnumerable<ICargo>>(message, inner);

            System.IO.Stream stream = SerializationUtilities.Serialize(e);
            DeferringException deserialized = SerializationUtilities.Deserialize<DeferringException<ISearch<ICargo, TrackingId>, IEnumerable<ICargo>>>(stream);
            
            Assert.AreEqual(message, deserialized.Message);
            Assert.AreEqual(innerMessage, deserialized.InnerException.Message);
            Assert.AreEqual(typeof(ISearch<ICargo, TrackingId>), deserialized.DeferredType);
            Assert.AreEqual(typeof(IEnumerable<ICargo>), deserialized.ResultType);
        }
    }
}

