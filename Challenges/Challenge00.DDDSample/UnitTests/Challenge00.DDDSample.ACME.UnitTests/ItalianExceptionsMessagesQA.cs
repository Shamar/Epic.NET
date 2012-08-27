//
//  ItalianExceptionsFormatterQA.cs
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
	public class ItalianExceptionsFormatterQA
	{
		[Test]
		public void Initialize_works ()
		{
			// act:
			ItalianExceptionsFormatter toTest = new ItalianExceptionsFormatter();

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
			ItalianExceptionsFormatter toTest = new ItalianExceptionsFormatter();

			// act:
			string result = toFormat.Accept(toTest, VisitContext.New);

			// assert:
            Assert.AreEqual("Si è verificato un errore imprevisto. Contattare l'amministratore del sistema.", result);
		}

		[TestCase("UNL23", "UNL34")]
		[TestCase("UNL45", "UNL56")]
		public void Visit_aWrongLocationException_returnsTheRightMessage (string expected, string actual)
		{
			// arrange:
			UnLocode expectedLocation = new UnLocode(expected);
			UnLocode actualLocation = new UnLocode(actual);
			WrongLocationException toFormat = new WrongLocationException("test", "Test message.", expectedLocation, actualLocation);
			ItalianExceptionsFormatter toTest = new ItalianExceptionsFormatter();

			// act:
			string result = toFormat.Accept(toTest, VisitContext.New);

			// assert:
			StringAssert.Contains(expected, result);
			StringAssert.Contains(actual, result);
		}

        [TestCase("test", RoutingStatus.Misrouted, "Non è possibile effettuare l'operazione perché il cargo '{0}' è stato diretto su un percorso sbagliato.")]
        [TestCase("test2", RoutingStatus.NotRouted, "Non è possibile effettuare l'operazione perché non è ancora stato assegnato un percorso al cargo '{0}'.")]
		public void Visit_aKnownRoutingException_returnsTheProperMessage (string cargo, RoutingStatus status, string expectedFormat)
		{
			// arrange:
			string expectedMessage = string.Format(expectedFormat, cargo);
			TrackingId id = new TrackingId(cargo);
			RoutingException toFormat = new RoutingException(id, status);
			ItalianExceptionsFormatter toTest = new ItalianExceptionsFormatter();

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
            string expectedMessage = string.Format("Non è possibile effettuare l'operazione perché il cargo '{0}' è già stato ritirato.", trackingId);
			TrackingId cargo = new TrackingId(trackingId);
			AlreadyClaimedException toFormat = new AlreadyClaimedException(cargo, "Test message.");
			ItalianExceptionsFormatter toTest = new ItalianExceptionsFormatter();

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
            string expectedMessage = string.Format("Il viaggio '{0}' ha già raggiunto la propria destinazione.", voyageNumber);
			VoyageNumber voyage = new VoyageNumber(voyageNumber);
			VoyageCompletedException toFormat = new VoyageCompletedException(voyage, "Test message.");
			ItalianExceptionsFormatter toTest = new ItalianExceptionsFormatter();
			
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
			ItalianExceptionsFormatter toTest = new ItalianExceptionsFormatter();
			
			// act:
			string result = toFormat.Accept(toTest, VisitContext.New);
			
			// assert:
			StringAssert.Contains("UNL23", result);
			StringAssert.Contains("UNL34", result);
		}
	}
}

