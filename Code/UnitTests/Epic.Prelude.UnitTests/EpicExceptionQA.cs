//  
//  EpicExceptionQA.cs
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

namespace Epic
{
	[TestFixture]
    public class EpicExceptionQA : EpicExceptionQABase<EpicException>
	{
		private static string DefaultMessage = "An error occurred in the infrastructure.";
		
		[Test]
		public void Initialize_withEmptyCtor_works()
		{
			EpicException e = new EpicException();
			Assert.IsNotNull(e);
			Assert.AreEqual(DefaultMessage, e.Message);
		}
		
		[Test]
		public void Initialize_withMessage_works()
		{
			string message = "Test message.";
			EpicException e = new EpicException(message);
			Assert.IsNotNull(e);
			Assert.AreEqual(message, e.Message);
		}
		
		[Test]
		public void Initialize_withoutMessage_works()
		{
			EpicException e = new EpicException(null);
			Assert.IsNotNull(e);
			Assert.AreEqual(DefaultMessage, e.Message);
		}
		
		[Test]
		public void Initialize_withoutMessage_withInnerException_works()
		{
			Exception inner = new Exception();
			EpicException e = new EpicException(null, inner);
			Assert.IsNotNull(e);
			Assert.AreEqual(DefaultMessage, e.Message);
			Assert.AreSame(inner, e.InnerException);
		}
		
		[Test]
		public void Initialize_withMessage_withInnerException_works()
		{
			string message = "Test message.";
			Exception inner = new Exception();
			EpicException e = new EpicException(message, inner);
			Assert.IsNotNull(e);
			Assert.AreEqual(message, e.Message);
			Assert.AreSame(inner, e.InnerException);
		}

        #region implemented abstract members of EpicExceptionQABase
        protected override System.Collections.Generic.IEnumerable<EpicException> ExceptionsToSerialize
        {
            get
            {
                string innerMessage = "Test inner.";
                string message = "Test message.";
                Exception inner = new Exception(innerMessage);
                EpicException e = new EpicException(message, inner);
                yield return e; 
            }
        }
        #endregion		
	}
}

