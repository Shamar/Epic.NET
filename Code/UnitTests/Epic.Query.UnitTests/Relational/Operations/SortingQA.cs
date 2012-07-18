//
//  Sorting.cs
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
using System.Linq;

namespace Epic.Query.Relational.Operations
{
    [TestFixture]
    public class SortingQA
    {
        [Test]
        public void Initialize_withoutAnyArgument_throwsArgumentNullException ()
        {
            // arrange:
            Relation source = new BaseRelation("test");
            Sorting.Direction[] nullDirections = null;
            Sorting.Direction[] emptyDirections = new Sorting.Direction[] { };
            Sorting.Direction[] nonEmptyDirections = new Sorting.Direction[] {
                Sorting.ByAscending(new RelationAttribute("test", source))
            };

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                new Sorting(null, nonEmptyDirections);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                new Sorting(source, nullDirections);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                new Sorting(source, emptyDirections);
            });
        }

        [Test]
        public void Initialize_withValidArguments_works ()
        {
            // arrange:
            Relation source = new BaseRelation("test");
            RelationAttribute attribute1 = new RelationAttribute("attribute1", source);
            RelationAttribute attribute2 = new RelationAttribute("attribute2", source);

            // act:
            Sorting sorting = new Sorting(source, 
                                          Sorting.ByAscending(attribute1),
                                          Sorting.ByDescending(attribute2));

            // assert:
            Assert.AreSame(source, sorting.Relation);
            Assert.AreEqual(2, sorting.Directions.Count());
            Assert.IsInstanceOf<Sorting.Ascending>(sorting.Directions.ElementAt(0));
            Assert.IsInstanceOf<Sorting.Descending>(sorting.Directions.ElementAt(1));
            Assert.AreSame(attribute1, sorting.Directions.OfType<Sorting.Ascending>().Single().Property);
            Assert.AreSame(attribute2, sorting.Directions.OfType<Sorting.Descending>().Single().Property);
        }

        [Test]
        public void Initialize_withTheSamePropertyInTwoDirections_throwsArgumentException ()
        {
            // arrange:
            Relation source = new BaseRelation("test");
            RelationAttribute attribute1 = new RelationAttribute("attribute1", source);
            RelationAttribute attribute2 = new RelationAttribute("attribute2", source);

            // assert:
            Assert.Throws<ArgumentException>(delegate {
                new Sorting(source, 
                            Sorting.ByAscending(attribute1),
                            Sorting.ByDescending(attribute2),
                            Sorting.ByDescending(attribute1)
                           );
            });
        }

        [Test]
        public void ByAscending_withAnAttribute_works ()
        {
            // arrange:
            BaseRelation relation = new BaseRelation("test");
            RelationAttribute attribute = new RelationAttribute("attribute", relation);

            // act:
            var result = Sorting.ByAscending(attribute);

            // assert:
            Assert.IsNotNull(result);
            Assert.AreSame(attribute, result.Property);
        }

        [Test]
        public void ByAscending_withAFunction_works ()
        {
            // arrange:
            ScalarFunction function = new Fakes.FakeScalarFunction("functionName");
            
            // act:
            var result = Sorting.ByAscending(function);
            
            // assert:
            Assert.IsNotNull(result);
            Assert.AreSame(function, result.Property);
        }
        
        [Test]
        public void ByAscending_withoutArgument_throwArgumentNullException ()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                Sorting.ByAscending(null as RelationAttribute);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                Sorting.ByAscending(null as ScalarFunction);
            });
        }

        [Test]
        public void ByDescending_withAnAttribute_works ()
        {
            // arrange:
            BaseRelation relation = new BaseRelation("test");
            RelationAttribute attribute = new RelationAttribute("attribute", relation);
            
            // act:
            var result = Sorting.ByDescending(attribute);
            
            // assert:
            Assert.IsNotNull(result);
            Assert.AreSame(attribute, result.Property);
        }
        
        [Test]
        public void ByDescending_withAFunction_works ()
        {
            // arrange:
            ScalarFunction function = new Fakes.FakeScalarFunction("functionName");
            
            // act:
            var result = Sorting.ByDescending(function);
            
            // assert:
            Assert.IsNotNull(result);
            Assert.AreSame(function, result.Property);
        }

        [Test]
        public void ByDescending_withoutArgument_throwArgumentNullException ()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                Sorting.ByDescending(null as RelationAttribute);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                Sorting.ByDescending(null as ScalarFunction);
            });
        }
    }
}

