//
//  EnglishExceptionsFormatterQA.cs
//
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
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
	public class EnglishExceptionsFormatterQA
	{
		[Test]
		public void Initialize_works ()
		{
			// act:
			EnglishExceptionsFormatter toTest = new EnglishExceptionsFormatter();

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
			}
		}

		[Test, TestCaseSource("Visit_anUnknownException_returnsTheDefaultMessage_Data")]
		public void Visit_anUnknownException_returnsTheDefaultMessage (Exception toFormat)
		{
			// arrange:
			EnglishExceptionsFormatter toTest = new EnglishExceptionsFormatter();

			// act:
			string result = toFormat.Accept(toTest, VisitContext.New);

			// assert:
			Assert.AreEqual("An unexpected error occurred. Please contact the administrator.", result);
		}

		[TestCase("UNL23", "UNL34")]
		[TestCase("UNL45", "UNL56")]
		public void Visit_aWrongLocationException_returnsTheExceptionMessage (string expected, string actual)
		{
			// arrange:
			string expectedMessage = "Test message.";
			UnLocode expectedLocation = new UnLocode(expected);
			UnLocode actualLocation = new UnLocode(actual);
			WrongLocationException toFormat = new WrongLocationException("test", expectedMessage, expectedLocation, actualLocation);
			EnglishExceptionsFormatter toTest = new EnglishExceptionsFormatter();

			// act:
			string result = toFormat.Accept(toTest, VisitContext.New);

			// assert:
			Assert.AreEqual(expectedMessage, result);
		}

		[TestCase("test", RoutingStatus.Misrouted)]
		[TestCase("test2", RoutingStatus.NotRouted)]
		public void Visit_aKnownRoutingException_returnsTheProperMessage (string cargo, RoutingStatus status)
		{
			// arrange:
			TrackingId id = new TrackingId(cargo);
			RoutingException toFormat = new RoutingException(id, status);
			EnglishExceptionsFormatter toTest = new EnglishExceptionsFormatter();

			// act:
			string result = toFormat.Accept(toTest, VisitContext.New);

			// assert:
			Assert.AreEqual(toFormat.Message, result);
		}

		[TestCase("TEST1")]
		[TestCase("TEST2")]
		public void Visit_anAlreadyClaimedException_returnsTheExceptionMessage (string trackingId)
		{
			// arrange:
			string expectedMessage = "Test message.";
			TrackingId cargo = new TrackingId(trackingId);
			AlreadyClaimedException toFormat = new AlreadyClaimedException(cargo, expectedMessage);
			EnglishExceptionsFormatter toTest = new EnglishExceptionsFormatter();

			// act:
			string result = toFormat.Accept(toTest, VisitContext.New);
			
			// assert:
			Assert.AreEqual(expectedMessage, result);
		}

		[TestCase("TEST1")]
		[TestCase("TEST2")]
		public void Visit_aVoyageCompletedException_returnsTheExceptionMessage (string voyageNumber)
		{
			// arrange:
			string expectedMessage = string.Format("The voyage '{0}' has already reached its own destintation.", voyageNumber);
			VoyageNumber voyage = new VoyageNumber(voyageNumber);
			VoyageCompletedException toFormat = new VoyageCompletedException(voyage, expectedMessage);
			EnglishExceptionsFormatter toTest = new EnglishExceptionsFormatter();
			
			// act:
			string result = toFormat.Accept(toTest, VisitContext.New);
			
			// assert:
			Assert.AreEqual(expectedMessage, result);
		}

		[Test]
		public void Visit_aWrappedDomainException_returnsTheInnerVisitResult ()
		{
			// arrange:
			string expectedMessage = "Test message.";
			UnLocode expectedLocation = new UnLocode("UNL23");
			UnLocode actualLocation = new UnLocode("UNL34");
			WrongLocationException inner = new WrongLocationException("test", expectedMessage, expectedLocation, actualLocation);
			InvalidTimeZoneException toFormat = new InvalidTimeZoneException("Message to ignore.", inner);
			EnglishExceptionsFormatter toTest = new EnglishExceptionsFormatter();
			
			// act:
			string result = toFormat.Accept(toTest, VisitContext.New);
			
			// assert:
			Assert.AreEqual(expectedMessage, result);
		}
	}
}

