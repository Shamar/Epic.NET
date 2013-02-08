//
//  AggregatedOperationFailedExceptionQA.cs
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
using System.Collections.Generic;

namespace Epic.Collections
{
    [TestFixture()]
    public class AggregatedOperationFailedExceptionQA : EpicExceptionQABase<AggregatedOperationFailedException<string>>
    {
        #region implemented abstract members of EpicExceptionQABase

        protected override System.Collections.Generic.IEnumerable<AggregatedOperationFailedException<string>> ExceptionsToSerialize
        {
            get
            {
                var exceptions = new EquatableDictionary<string, Exception>();
                exceptions["failed"] = new EquatableException("message");
                var successful = new EquatableEnumerable<string>("success");
                yield return new AggregatedOperationFailedException<string>(exceptions, successful);
            }
        }

        #endregion
    }
}

