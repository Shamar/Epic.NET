//
//  MyClass.cs
//
//  Author:
//       giacomo <${AuthorEmail}>
//
//  Copyright (c) 2010-2012 giacomo
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
using Epic;
using System.Collections.Generic;
using Challenge00.DDDSample.Location;
using Challenge00.DDDSample.Cargo;
using Challenge00.DDDSample.Voyage;

namespace Challenge00.DDDSample.ACME.UnitTests
{
	[TestFixture]
	public class EnglishExceptionsMessagesQA
	{
		[Test]
		public void Initialize_works ()
		{
			// act:
			EnglishExceptionsMessages toTest = new EnglishExceptionsMessages();

			// assert:
			Assert.IsNotNull(toTest);
		}

		public static IEnumerable<Exception> Visit_anUnknownException_returnsTheDefaultMessage_Data
		{
			get
			{
				yield return new InvalidOperationException();
				yield return new ArgumentNullException();
				yield return new ArgumentException();
				yield return new Exception();
				yield return new RoutingException(new TrackingId("test"), RoutingStatus.Routed); // No routing exception should have such RoutingStatus
			}
		}

		[Test, TestCaseSource("Visit_anUnknownException_returnsTheDefaultMessage_Data")]
		public void Visit_anUnknownException_returnsTheDefaultMessage (Exception toFormat)
		{
			// arrange:
			EnglishExceptionsMessages toTest = new EnglishExceptionsMessages();

			// act:
			string result = toFormat.Accept(toTest, VisitContext.New);

			// assert:
			Assert.AreEqual("An unexpected error occurred. Please contact the administrator.", result);
		}

		[TestCase("UNL23", "UNL34")]
		[TestCase("UNL45", "UNL56")]
		public void Visit_aWrongLocationException_returnsTheRightMessage (string expected, string actual)
		{
			// arrange:
			UnLocode expectedLocation = new UnLocode(expected);
			UnLocode actualLocation = new UnLocode(actual);
			WrongLocationException toFormat = new WrongLocationException("test", "Test message.", expectedLocation, actualLocation);
			EnglishExceptionsMessages toTest = new EnglishExceptionsMessages();

			// act:
			string result = toFormat.Accept(toTest, VisitContext.New);

			// assert:
			StringAssert.Contains(expected, result);
			StringAssert.Contains(actual, result);
		}

		[TestCase("test", RoutingStatus.Misrouted, "Cannot perform the operation requested becouse the cargo '{0}' has been misrouted.")]
		[TestCase("test2", RoutingStatus.NotRouted, "Cannot perform the operation requested becouse the cargo '{0}' is still not routed.")]
		public void Visit_aKnownRoutingException_returnsTheProperMessage (string cargo, RoutingStatus status, string expectedFormat)
		{
			// arrange:
			string expectedMessage = string.Format(expectedFormat, cargo);
			TrackingId id = new TrackingId(cargo);
			RoutingException toFormat = new RoutingException(id, status);
			EnglishExceptionsMessages toTest = new EnglishExceptionsMessages();

			// act:
			string result = toFormat.Accept(toTest, VisitContext.New);

			// assert:
			Assert.AreEqual(expectedMessage, result);
		}

		[TestCase("TEST1")]
		[TestCase("TEST2")]
		public void Visit_anAlreadyClaimedException_returnsTheRightMessage (string trackingId)
		{
			// arrange:
			string expectedMessage = string.Format("Cannot perform the operation requested becouse the cargo '{0}' has been claimed.", trackingId);
			TrackingId cargo = new TrackingId(trackingId);
			AlreadyClaimedException toFormat = new AlreadyClaimedException(cargo, "Test message.");
			EnglishExceptionsMessages toTest = new EnglishExceptionsMessages();

			// act:
			string result = toFormat.Accept(toTest, VisitContext.New);
			
			// assert:
			Assert.AreEqual(expectedMessage, result);
		}

		[TestCase("TEST1")]
		[TestCase("TEST2")]
		public void Visit_aVoyageCompletedException_returnsTheRightMessage (string voyageNumber)
		{
			// arrange:
			string expectedMessage = string.Format("The voyage '{0}' has already reached its own destintation.", voyageNumber);
			VoyageNumber voyage = new VoyageNumber(voyageNumber);
			VoyageCompletedException toFormat = new VoyageCompletedException(voyage, "Test message.");
			EnglishExceptionsMessages toTest = new EnglishExceptionsMessages();
			
			// act:
			string result = toFormat.Accept(toTest, VisitContext.New);
			
			// assert:
			Assert.AreEqual(expectedMessage, result);
		}
	}
}

