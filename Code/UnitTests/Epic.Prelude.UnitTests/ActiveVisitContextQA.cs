//
//  ActiveVisitContextQA.cs
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
using System.Linq;
using System.Collections.Generic;

namespace Epic
{
    [TestFixture()]
    public class ActiveVisitContextQA
    {
        [Test]
        public void Perform_withoutAnyArgument_throwsArgumentNullException()
        {
            // arrange:
            IVisitContext context = VisitContext.New;
            Action<string> action = s => { throw new Exception(s); };

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                context.Perform<string>(null);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                ActiveVisitContext.Perform<string>(null, action);
            });
        }

        [Test]
        public void Perform_aValidActionInAValidContext_provideANewContext()
        {
            // arrange:
            string sToAssign = null;
            IVisitContext context = VisitContext.New;
            Action<string> action = s => sToAssign = s;

            // act:
            IVisitContext result = context.Perform(action);

            // assert:
            Assert.AreNotSame(context, result);
        }

        [Test]
        public void Perform_anyActionMoreThanOnce_alwaysReturnsANewContext()
        {
            // arrange:
            int count = 0;
            IVisitContext context = VisitContext.New;
            Action<string> action = s => ++count;
            List<IVisitContext> contexts = new List<IVisitContext>();
            
            // act:
            for(int i = 0; i < 10; ++i)
                contexts.Add(context.Perform(action));
            
            // assert:
            Assert.IsFalse(contexts.Any(c => object.ReferenceEquals(context, c)));
            Assert.AreNotEqual(1, contexts.Count(c => contexts.Any(c2 => object.ReferenceEquals(c2, c))));
        }

        [Test]
        public void ApplyTo_withoutContext_throwsArgumentNullException()
        {
            // arrange:
            IVisitContext context = null;

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                ActiveVisitContext.ApplyTo(context, "test");
            });
        }
        
        [Test]
        public void ApplyTo_anyObjectWithContextThatHasNoRegisteredActionsThatTypeOfObject_doesNothing()
        {
            // arrange:
            IVisitContext context = VisitContext.New;
            
            // act:
            context.ApplyTo("test");
        }

        
        [Test]
        public void ApplyTo_anyObjectWithContextThatHasRegisteredActionsThatTypeOfObject_executeAllTheActionRegisteredInReverseOrder()
        {
            // arrange:
            int count = 0;
            List<string> collector = new List<string>();
            Action<string> countingAction = s => { ++count; };
            IVisitContext context = VisitContext.New;
            for(int i = 0; i < 10; ++i)
            {
                var current = i;
                context = context
                    .Perform(countingAction)
                    .Perform<string>(s => collector.Add(s + current.ToString()));
            }

            // act:
            context.ApplyTo("test");

            // assert:
            Assert.AreEqual(10, count);
            foreach(var registered in collector)
            {
                Assert.AreEqual("test"+(--count).ToString(), registered);
            }
        }    
    }
}

