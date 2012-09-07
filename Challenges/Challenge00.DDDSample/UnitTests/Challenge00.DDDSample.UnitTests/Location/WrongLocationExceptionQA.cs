//
//  WrongLocationExceptionQA.cs
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
using Challenge00.DDDSample.Location;

namespace Contracts.Location
{
	[TestFixture]
	public class WrongLocationExceptionQA
	{
		[Test]
		public void Initialization_works()
		{
			// arrange:
			string message = "Test message.";
			string argument = "test";
			UnLocode expected = new UnLocode("EXP23");
			UnLocode actual = new UnLocode("ACT34");
			
			// act:
			var toTest = new WrongLocationException(argument, message, expected, actual);
			
			// assert:
			Assert.IsTrue(toTest.Message.Contains(message));
			Assert.IsTrue(toTest.Message.Contains(argument));
			Assert.AreEqual(argument, toTest.ParamName);
			Assert.AreSame(expected, toTest.ExpectedLocation);
			Assert.AreSame(actual, toTest.ActualLocation);
		}
	}
}

