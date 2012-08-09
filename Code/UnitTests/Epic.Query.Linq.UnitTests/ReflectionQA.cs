//  
//  ReflectionQA.cs
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
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;

namespace Epic.Query.Linq
{
    public interface ITestEnumerable<T> : IOrderedEnumerable<T>
    {
    }
    
    public interface IClosedTestEnumerable : ITestEnumerable<string>
    {
    }
    
    public sealed class DummyClassWithDifferentMembers
    {
        public DummyClassWithDifferentMembers() { this.SampleEvent += delegate { }; SampleEvent(this, null);}
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
        public int IntField = 0;
        public string StringField = string.Empty;
        public int IntMethod() { return IntField; }
        public string StringMethod() { return StringField; }
        public event EventHandler<EventArgs> SampleEvent;
    }
    
    [TestFixture()]
    public class ReflectionQA
    {
        static ReflectionQA()
        {
            DummyClassWithDifferentMembers dummy = new DummyClassWithDifferentMembers();
            var v1 = dummy.IntProperty;
            dummy.IntProperty = v1;
            var v2 = dummy.StringProperty;
            dummy.StringProperty = v2;
            v1 = dummy.IntMethod();
            v2 = dummy.StringMethod();
        }

        [TestFixtureSetUp]
        public void VerifyKnownMethods()
        {
            Assert.AreEqual(14, ReflectionQA.AllEnumerableMethodsThatHaveNoEquivalentInQueryable.Count());
            Assert.AreEqual(161, ReflectionQA.AllEnumerableMethodsThatHaveEquivalentInQueryable.Count());
            Assert.AreEqual(110, ReflectionQA.QueryableEnumerableEquivantMethods.Count());
        }
        
        [Test]
        public void TryGetItemTypeOfEnumerable_withoutEnumerableType_throwsArgumentNullException()
        {
            // arrange:
            Type elementType = null;

            // assert:
            Assert.Throws<ArgumentNullException>(delegate { Reflection.TryGetItemTypeOfEnumerable(null, out elementType); });
        }
        
        [TestCase(typeof(IEnumerable<string>), typeof(string))]
        [TestCase(typeof(IOrderedEnumerable<string>), typeof(string))]
        [TestCase(typeof(IEnumerable<int>), typeof(int))]
        [TestCase(typeof(IEnumerable<IEnumerable<string>>), typeof(IEnumerable<string>))]
        [TestCase(typeof(IEnumerable), typeof(object))]
        [TestCase(typeof(int[]), typeof(int))]
        [TestCase(typeof(IQueryable<string>), typeof(string))]
        [TestCase(typeof(IOrderedQueryable<string>), typeof(string))]
        [TestCase(typeof(IQueryable<int>), typeof(int))]
        [TestCase(typeof(IQueryable<IEnumerable<string>>), typeof(IEnumerable<string>))]
        [TestCase(typeof(IQueryable<IQueryable<string>>), typeof(IQueryable<string>))]
        [TestCase(typeof(ITestEnumerable<DateTime>), typeof(DateTime))]
        [TestCase(typeof(IClosedTestEnumerable), typeof(string))]
        public void TryGetItemTypeOfEnumerable_withEnumerableTypes_returnTheExpectedItem(Type enumerable, Type expectedItemType)
        {
            // arrange:
            Type itemType = null;

            // act:
            bool found = Reflection.TryGetItemTypeOfEnumerable(enumerable, out itemType);

            // assert:
            Assert.IsTrue(found);
            Assert.AreEqual(expectedItemType, itemType);
        }
        
        [TestCase(typeof(int))]
        [TestCase(typeof(DateTime))]
        [TestCase(typeof(object))]
        public void TryGetItemTypeOfEnumerable_withNotEnumerableTypes_returnFalseWithoutSettingTheElementType(Type notEnumerable)
        {
            // arrange:
            Type itemType = null;

            // act:
            bool found = Reflection.TryGetItemTypeOfEnumerable(notEnumerable, out itemType);

            // assert:
            Assert.IsFalse(found);
            Assert.IsNull(itemType);
        }
        
        [Test]
        public void GetMemberReturnType_withoutMember_throwsArgumentNullException()
        {
            // arrange:
            Type result = null;         

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                result = Reflection.GetMemberReturnType(null);
            });
            Assert.IsNull(result);
        }
        
        [TestCase("StringProperty", "dummyValue")]
        [TestCase("IntProperty", 0)]
        public void GetMemberReturnType_ofAProperty_returnTheRightType<T>(string name, T dummyValue)
        {
            // arrange:
            PropertyInfo property = typeof(DummyClassWithDifferentMembers).GetProperty(name);

            // act:
            Type propertyType = Reflection.GetMemberReturnType(property);

            // assert:
            Assert.AreSame(typeof(T), propertyType);
        }
        
        [TestCase("StringField", "dummyValue")]
        [TestCase("IntField", 0)]
        public void GetMemberReturnType_ofAField_returnTheRightType<T>(string name, T dummyValue)
        {
            // arrange:
            FieldInfo property = typeof(DummyClassWithDifferentMembers).GetField(name);

            // act:
            Type fieldType = Reflection.GetMemberReturnType(property);

            // assert:
            Assert.AreSame(typeof(T), fieldType);
        }
        
        [TestCase("StringMethod", "dummyValue")]
        [TestCase("IntMethod", 0)]
        public void GetMemberReturnType_ofAMethod_returnTheRightType<T>(string name, T dummyValue)
        {
            // arrange:
            MethodInfo property = typeof(DummyClassWithDifferentMembers).GetMethod(name);

            // act:
            Type methodType = Reflection.GetMemberReturnType(property);

            // assert:
            Assert.AreSame(typeof(T), methodType);
        }
        
        [Test]
        public void GetMemberReturnType_ofAType_throwsArgumentException()
        {
            // arrange:
            Type result = null;

            // assert:
            Assert.Throws<ArgumentException>(delegate {
                result = Reflection.GetMemberReturnType(typeof(DummyClassWithDifferentMembers));
            });
            Assert.IsNull(result);
        }
        
        [Test]
        public void GetMemberReturnType_ofAnEvent_throwsArgumentException()
        {
            // arrange:
            EventInfo eventInfo = typeof(DummyClassWithDifferentMembers).GetEvent("SampleEvent");
            Type result = null;

            // assert:
            Assert.Throws<ArgumentException>(delegate {
                result = Reflection.GetMemberReturnType(eventInfo);
            });
            Assert.IsNull(result);
        }

        public static IEnumerable<TestCaseData> QueryableEnumerableEquivantMethods
        {
            get
            {
                IEnumerable<object> dummyEnumerable = null;
                IQueryable<object> q = null;
                IEnumerable<object> e = null;
                IQueryable<decimal?> qDecimalN = null;
                IEnumerable<decimal?> eDecimalN = null;
                IQueryable<decimal> qDecimal = null;
                IEnumerable<decimal> eDecimal = null;
                IQueryable<int> qInt = null;
                IEnumerable<int> eInt = null;
                IQueryable<int?> qIntN = null;
                IEnumerable<int?> eIntN = null;
                IQueryable<long> qLong = null;
                IEnumerable<long> eLong = null;
                IQueryable<long?> qLongN = null;
                IEnumerable<long?> eLongN = null;
                IQueryable<double> qDouble = null;
                IEnumerable<double> eDouble = null;
                IQueryable<double?> qDoubleN = null;
                IEnumerable<double?> eDoubleN = null;
                IQueryable<float> qFloat = null;
                IEnumerable<float> eFloat = null;
                IQueryable<float?> qFloatN = null;
                IEnumerable<float?> eFloatN = null;

                yield return new TestCaseData(
                    GetMethodInfo(() => q.Aggregate(0, (i, acc) => i.GetHashCode() + acc.GetHashCode())),
                    GetMethodInfo(() => e.Aggregate(0, (i, acc) => i.GetHashCode() + acc.GetHashCode()))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Aggregate((i, acc) => i.GetHashCode() + acc.GetHashCode())),
                    GetMethodInfo(() => e.Aggregate((i, acc) => i.GetHashCode() + acc.GetHashCode()))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Aggregate(0, (i, acc) => i.GetHashCode() + acc.GetHashCode(), acc => acc.GetHashCode())),
                    GetMethodInfo(() => e.Aggregate(0, (i, acc) => i.GetHashCode() + acc.GetHashCode(), acc => acc.GetHashCode()))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.All(i => i.GetHashCode() > 0)),
                    GetMethodInfo(() => e.All(i => i.GetHashCode() > 0))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.Any()),
                    GetMethodInfo(() => e.Any())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Any(i => i.GetHashCode() > 0)),
                    GetMethodInfo(() => e.Any(i => i.GetHashCode() > 0))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => qInt.Average()),
                    GetMethodInfo(() => eInt.Average())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => qIntN.Average()),
                    GetMethodInfo(() => eIntN.Average())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => qDecimal.Average()),
                    GetMethodInfo(() => eDecimal.Average())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => qDecimalN.Average()),
                    GetMethodInfo(() => eDecimalN.Average())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => qLong.Average()),
                    GetMethodInfo(() => eLong.Average())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => qLongN.Average()),
                    GetMethodInfo(() => eLongN.Average())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => qDouble.Average()),
                    GetMethodInfo(() => eDouble.Average())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => qDoubleN.Average()),
                    GetMethodInfo(() => eDoubleN.Average())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => qFloat.Average()),
                    GetMethodInfo(() => eFloat.Average())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => qFloatN.Average()),
                    GetMethodInfo(() => eFloatN.Average())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Average(i => Convert.ToDecimal(i))),
                    GetMethodInfo(() => e.Average(i => Convert.ToDecimal(i)))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Average(i => i.GetHashCode() > 0 ? new Nullable<decimal>(Convert.ToDecimal(i)) : null)),
                    GetMethodInfo(() => e.Average(i => i.GetHashCode() > 0 ? new Nullable<decimal>(Convert.ToDecimal(i)) : null))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Average(i => i.GetHashCode())),
                    GetMethodInfo(() => e.Average(i => i.GetHashCode()))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Average(i => i.GetHashCode() > 0 ? new Nullable<int>(i.GetHashCode()) : null)),
                    GetMethodInfo(() => e.Average(i => i.GetHashCode() > 0 ? new Nullable<int>(i.GetHashCode()) : null))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Average(i => Convert.ToInt64(i))),
                    GetMethodInfo(() => e.Average(i => Convert.ToInt64(i)))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Average(i => i.GetHashCode() > 0 ? new Nullable<long>(Convert.ToInt64(i)) : null)),
                    GetMethodInfo(() => e.Average(i => i.GetHashCode() > 0 ? new Nullable<long>(Convert.ToInt64(i)) : null))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Average(i => Convert.ToDouble(i))),
                    GetMethodInfo(() => e.Average(i => Convert.ToDouble(i)))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Average(i => i.GetHashCode() > 0 ? new Nullable<double>(Convert.ToDouble(i)) : null)),
                    GetMethodInfo(() => e.Average(i => i.GetHashCode() > 0 ? new Nullable<double>(Convert.ToDouble(i)) : null))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Average(i => (float)i)),
                    GetMethodInfo(() => e.Average(i => (float)i))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Average(i => i.GetHashCode() > 0 ? new Nullable<float>((float)i) : null)),
                    GetMethodInfo(() => e.Average(i => i.GetHashCode() > 0 ? new Nullable<float>((float)i) : null))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.Cast<decimal>()),
                    GetMethodInfo(() => e.Cast<decimal>())
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.Concat(null)),
                    GetMethodInfo(() => e.Concat(null))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.Contains(1)),
                    GetMethodInfo(() => e.Contains(1))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Contains(1, EqualityComparer<object>.Default)),
                    GetMethodInfo(() => e.Contains(1, EqualityComparer<object>.Default))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.Count()),
                    GetMethodInfo(() => e.Count())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Count(i => true)),
                    GetMethodInfo(() => e.Count(i => true))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.DefaultIfEmpty()),
                    GetMethodInfo(() => e.DefaultIfEmpty())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.DefaultIfEmpty(5)),
                    GetMethodInfo(() => e.DefaultIfEmpty(5))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.Distinct()),
                    GetMethodInfo(() => e.Distinct())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Distinct(EqualityComparer<object>.Default)),
                    GetMethodInfo(() => e.Distinct(EqualityComparer<object>.Default))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.ElementAt(1)),
                    GetMethodInfo(() => e.ElementAt(1))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.ElementAtOrDefault(1)),
                    GetMethodInfo(() => e.ElementAtOrDefault(1))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.Except(dummyEnumerable)),
                    GetMethodInfo(() => e.Except(dummyEnumerable))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Except(dummyEnumerable, EqualityComparer<object>.Default)),
                    GetMethodInfo(() => e.Except(dummyEnumerable, EqualityComparer<object>.Default))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.First()),
                    GetMethodInfo(() => e.First())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.First(i => true)),
                    GetMethodInfo(() => e.First(i => true))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.FirstOrDefault()),
                    GetMethodInfo(() => e.FirstOrDefault())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.FirstOrDefault(i => false)),
                    GetMethodInfo(() => e.FirstOrDefault(i => false))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.GroupBy(i => i.GetType())),
                    GetMethodInfo(() => e.GroupBy(i => i.GetType()))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.GroupBy(i => i.GetType(), i => i.ToString())),
                    GetMethodInfo(() => e.GroupBy(i => i.GetType(), i => i.ToString()))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.GroupBy(i => i.GetType(), (i, g) => g.Count())),
                    GetMethodInfo(() => e.GroupBy(i => i.GetType(), (i, g) => g.Count()))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.GroupBy(i => i.GetType(), (i, g) => g.Count(), EqualityComparer<object>.Default)),
                    GetMethodInfo(() => e.GroupBy(i => i.GetType(), (i, g) => g.Count(), EqualityComparer<object>.Default))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.GroupBy(i => i.GetType(), EqualityComparer<Type>.Default)),
                    GetMethodInfo(() => e.GroupBy(i => i.GetType(), EqualityComparer<Type>.Default))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.GroupBy(i => i.GetType(), i => i.ToString(), EqualityComparer<Type>.Default)),
                    GetMethodInfo(() => e.GroupBy(i => i.GetType(), i => i.ToString(), EqualityComparer<Type>.Default))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.GroupBy(i => i.GetType(), i => i.ToString(), (k, g) => g.Count())),
                    GetMethodInfo(() => e.GroupBy(i => i.GetType(), i => i.ToString(), (k, g) => g.Count()))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.GroupBy(i => i.GetType(), i => i.ToString(), (k, g) => g.Count(), EqualityComparer<object>.Default)),
                    GetMethodInfo(() => e.GroupBy(i => i.GetType(), i => i.ToString(), (k, g) => g.Count(), EqualityComparer<object>.Default))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.GroupJoin(dummyEnumerable, i => i, j => j, (k, g) => g.Count())),
                    GetMethodInfo(() => e.GroupJoin(dummyEnumerable, i => i, j => j, (k, g) => g.Count()))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.GroupJoin(dummyEnumerable, i => i, j => j, (k, g) => g.Count(), EqualityComparer<object>.Default)),
                    GetMethodInfo(() => e.GroupJoin(dummyEnumerable, i => i, j => j, (k, g) => g.Count(), EqualityComparer<object>.Default))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.Intersect(dummyEnumerable)),
                    GetMethodInfo(() => e.Intersect(dummyEnumerable))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Intersect(dummyEnumerable, EqualityComparer<object>.Default)),
                    GetMethodInfo(() => e.Intersect(dummyEnumerable, EqualityComparer<object>.Default))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.Join(dummyEnumerable, i => i, j => j, (k, g) => k.GetHashCode() + g.GetHashCode())),
                    GetMethodInfo(() => e.Join(dummyEnumerable, i => i, j => j, (k, g) => k.GetHashCode() + g.GetHashCode()))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Join(dummyEnumerable, i => i, j => j, (k, g) => k.GetHashCode() + g.GetHashCode(), EqualityComparer<object>.Default)),
                    GetMethodInfo(() => e.Join(dummyEnumerable, i => i, j => j, (k, g) => k.GetHashCode() + g.GetHashCode(), EqualityComparer<object>.Default))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.Last()),
                    GetMethodInfo(() => e.Last())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Last(i => true)),
                    GetMethodInfo(() => e.Last(i => true))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.LastOrDefault()),
                    GetMethodInfo(() => e.LastOrDefault())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.LastOrDefault(i => false)),
                    GetMethodInfo(() => e.LastOrDefault(i => false))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.LongCount()),
                    GetMethodInfo(() => e.LongCount())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.LongCount(i => true)),
                    GetMethodInfo(() => e.LongCount(i => true))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.Max()),
                    GetMethodInfo(() => e.Max())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Max(i => i.GetHashCode())),
                    GetMethodInfo(() => System.Linq.Enumerable.Max<object, int>(e, i => i.GetHashCode()))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.Min()),
                    GetMethodInfo(() => e.Min())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Min(i => i.GetHashCode())),
                    GetMethodInfo(() => System.Linq.Enumerable.Min<object, int>(e, i => i.GetHashCode()))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.OfType<string>()),
                    GetMethodInfo(() => e.OfType<string>())
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.OrderBy(i => i.GetHashCode())),
                    GetMethodInfo(() => e.OrderBy(i => i.GetHashCode()))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.OrderBy(i => i.GetHashCode(), Comparer<int>.Default)),
                    GetMethodInfo(() => e.OrderBy(i => i.GetHashCode(), Comparer<int>.Default))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.OrderByDescending(i => i.GetHashCode())),
                    GetMethodInfo(() => e.OrderByDescending(i => i.GetHashCode()))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.OrderByDescending(i => i.GetHashCode(), Comparer<int>.Default)),
                    GetMethodInfo(() => e.OrderByDescending(i => i.GetHashCode(), Comparer<int>.Default))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.OrderBy(i => i.GetHashCode()).ThenBy(i => i.GetHashCode())),
                    GetMethodInfo(() => e.OrderBy(i => i.GetHashCode()).ThenBy(i => i.GetHashCode()))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.OrderBy(i => i.GetHashCode()).ThenBy(i => i.GetHashCode(), Comparer<int>.Default)),
                    GetMethodInfo(() => e.OrderBy(i => i.GetHashCode()).ThenBy(i => i.GetHashCode(), Comparer<int>.Default))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.OrderBy(i => i.GetHashCode()).ThenByDescending(i => i.GetHashCode())),
                    GetMethodInfo(() => e.OrderBy(i => i.GetHashCode()).ThenByDescending(i => i.GetHashCode()))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.OrderBy(i => i.GetHashCode()).ThenByDescending(i => i.GetHashCode(), Comparer<int>.Default)),
                    GetMethodInfo(() => e.OrderBy(i => i.GetHashCode()).ThenByDescending(i => i.GetHashCode(), Comparer<int>.Default))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.Reverse()),
                    GetMethodInfo(() => e.Reverse())
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.Select(i => i.GetType())),
                    GetMethodInfo(() => e.Select(i => i.GetType()))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Select((i, p) => i.GetHashCode() % p)),
                    GetMethodInfo(() => e.Select((i, p) => i.GetHashCode() % p))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.SelectMany(i => new int[] { i.GetHashCode() })),
                    GetMethodInfo(() => e.SelectMany(i => new int[] { i.GetHashCode() }))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.SelectMany((i, p) => new int[] { i.GetHashCode(), p })),
                    GetMethodInfo(() => e.SelectMany((i, p) => new int[] { i.GetHashCode(), p }))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.SelectMany(i => new int[] { i.GetHashCode() }, (i, c) => i.GetHashCode() + c)),
                    GetMethodInfo(() => e.SelectMany(i => new int[] { i.GetHashCode() }, (i, c) => i.GetHashCode() + c))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.SelectMany((i, p) => new int[] { i.GetHashCode(), p }, (i, c) => i.GetHashCode() + c)),
                    GetMethodInfo(() => e.SelectMany((i, p) => new int[] { i.GetHashCode(), p }, (i, c) => i.GetHashCode() + c))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.SequenceEqual(dummyEnumerable)),
                    GetMethodInfo(() => e.SequenceEqual(dummyEnumerable))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.SequenceEqual(dummyEnumerable, EqualityComparer<object>.Default)),
                    GetMethodInfo(() => e.SequenceEqual(dummyEnumerable, EqualityComparer<object>.Default))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.Single()),
                    GetMethodInfo(() => e.Single())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Single(i => true)),
                    GetMethodInfo(() => e.Single(i => true))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.SingleOrDefault()),
                    GetMethodInfo(() => e.SingleOrDefault())
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.SingleOrDefault(i => true)),
                    GetMethodInfo(() => e.SingleOrDefault(i => true))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.Skip(1)),
                    GetMethodInfo(() => e.Skip(1))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.SkipWhile(i => false)),
                    GetMethodInfo(() => e.SkipWhile(i => false))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.SkipWhile((i, p) => false)),
                    GetMethodInfo(() => e.SkipWhile((i, p) => false))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.Sum(i => Convert.ToDecimal(i))),
                    GetMethodInfo(() => e.Sum(i => Convert.ToDecimal(i)))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Sum(i => i.GetHashCode() > 0 ? new Nullable<decimal>(Convert.ToDecimal(i)) : null)),
                    GetMethodInfo(() => e.Sum(i => i.GetHashCode() > 0 ? new Nullable<decimal>(Convert.ToDecimal(i)) : null))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Sum(i => i.GetHashCode())),
                    GetMethodInfo(() => e.Sum(i => i.GetHashCode()))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Sum(i => i.GetHashCode() > 0 ? new Nullable<int>(i.GetHashCode()) : null)),
                    GetMethodInfo(() => e.Sum(i => i.GetHashCode() > 0 ? new Nullable<int>(i.GetHashCode()) : null))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Sum(i => Convert.ToInt64(i))),
                    GetMethodInfo(() => e.Sum(i => Convert.ToInt64(i)))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Sum(i => i.GetHashCode() > 0 ? new Nullable<long>(Convert.ToInt64(i)) : null)),
                    GetMethodInfo(() => e.Sum(i => i.GetHashCode() > 0 ? new Nullable<long>(Convert.ToInt64(i)) : null))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Sum(i => Convert.ToDouble(i))),
                    GetMethodInfo(() => e.Sum(i => Convert.ToDouble(i)))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Sum(i => i.GetHashCode() > 0 ? new Nullable<double>(Convert.ToDouble(i)) : null)),
                    GetMethodInfo(() => e.Sum(i => i.GetHashCode() > 0 ? new Nullable<double>(Convert.ToDouble(i)) : null))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Sum(i => (float)i)),
                    GetMethodInfo(() => e.Sum(i => (float)i))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Sum(i => i.GetHashCode() > 0 ? new Nullable<float>((float)i) : null)),
                    GetMethodInfo(() => e.Sum(i => i.GetHashCode() > 0 ? new Nullable<float>((float)i) : null))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.Take(1)),
                    GetMethodInfo(() => e.Take(1))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.TakeWhile(i => true)),
                    GetMethodInfo(() => e.TakeWhile(i => true))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.TakeWhile((i, p) => true)),
                    GetMethodInfo(() => e.TakeWhile((i, p) => true))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.Union(dummyEnumerable)),
                    GetMethodInfo(() => e.Union(dummyEnumerable))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Union(dummyEnumerable, EqualityComparer<object>.Default)),
                    GetMethodInfo(() => e.Union(dummyEnumerable, EqualityComparer<object>.Default))
                    );

                yield return new TestCaseData(
                    GetMethodInfo(() => q.Where(i => true)),
                    GetMethodInfo(() => e.Where(i => true))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => q.Where((i, p) => true)),
                    GetMethodInfo(() => e.Where((i, p) => true))
                    );
            }
        }

        private static MethodInfo GetMethodInfo<T>(Expression<Func<T>> expression)
        {
            return ((MethodCallExpression)expression.Body).Method;
        }

        #region Queryable

        [Test, TestCaseSource("QueryableEnumerableEquivantMethods")]
        public void GetEnumerableEquivalent_ofAQueryableMethod_returnsTheRightMethod(MethodInfo queryableMethod, MethodInfo enumerableMethod)
        {
            // act:
            MethodInfo enumerableEquivalent = Reflection.Queryable.GetEnumerableEquivalent(queryableMethod);

            // assert:
            Assert.AreSame(enumerableMethod, enumerableEquivalent);
        }

        [Test]
        public void GetEnumerableEquivalent_ofNull_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                Reflection.Queryable.GetEnumerableEquivalent(null);
            });
        }

        [Test]
        public void GetEnumerableEquivalent_ofAMethodThatDontBelongToQueryable_throwsArgumentException()
        {
            // arrange:
            MethodInfo[] sampleMethods = new MethodInfo[]
            {
                GetMethodInfo(() => int.Parse("1")),
                GetMethodInfo(() => string.Join(",", new string[0]))
            };

            // assert:
            foreach (MethodInfo method in sampleMethods)
            {
                Assert.Throws<ArgumentException>(delegate {
                    Reflection.Queryable.GetEnumerableEquivalent(method);
                });
            }
        }

        [Test, TestCaseSource("QueryableEnumerableEquivantMethods")]
        public void GetEnumerableEquivalent_ofAMethodThatBelongToEnumerable_throwsArgumentException(object dummyVar, MethodInfo enumerableMethod)
        {
            // assert:
            Assert.Throws<ArgumentException>(delegate
            {
                Reflection.Queryable.GetEnumerableEquivalent(enumerableMethod);
            });
        }

        #endregion Queryable

        #region Enumerable

        private static IEnumerable<MethodInfo> AllEnumerableMethodsThatHaveEquivalentInQueryable
        {
            get
            {
                IEnumerable<MethodInfo> queryableMethods = typeof(Queryable).GetMethods();
                List<MethodInfo> methodsToTest = new List<MethodInfo>();
                foreach (MethodInfo method in typeof(Enumerable).GetMethods())
                {
                    if (method.DeclaringType.Equals(typeof(Enumerable)) &&
                        method.GetParameters().Length > 0 &&
                        typeof(IEnumerable).IsAssignableFrom(method.GetParameters()[0].ParameterType) &&
                        queryableMethods.Any(qm => qm.Name == method.Name))
                    {
                        if (method.IsGenericMethodDefinition)
                        {
                            List<Type> args = new List<Type>();
                            for (int i = 0; i < method.GetGenericArguments().Length; ++i)
                            {
                                switch (i)
                                {
                                    case 0:
                                        args.Add(typeof(string)); 
                                        break;
                                    case 1: 
                                        args.Add(typeof(Type)); 
                                        break;
                                    case 2: 
                                        args.Add(typeof(object)); 
                                        break;
                                    case 3:
                                        args.Add(typeof(int));
                                        break;
                                }
                            }
                            methodsToTest.Add(method.MakeGenericMethod(args.ToArray()));
                        }
                        else
                        {
                            methodsToTest.Add(method);
                        }
                    }
                }
                return methodsToTest;
            }
        }

        private static IEnumerable<MethodInfo> AllEnumerableMethodsThatHaveNoEquivalentInQueryable
        {
            get
            {
                IEnumerable<MethodInfo> queryableMethods = typeof(Queryable).GetMethods();
                List<MethodInfo> methodsToTest = new List<MethodInfo>();
                foreach (MethodInfo method in typeof(Enumerable).GetMethods())
                {
                    if (method.DeclaringType.Equals(typeof(Enumerable)) &&
                        !queryableMethods.Any(qm => qm.Name == method.Name))
                    {
                        if (method.IsGenericMethodDefinition)
                        {
                            List<Type> args = new List<Type>();
                            for (int i = 0; i < method.GetGenericArguments().Length; ++i)
                            {
                                switch (i)
                                {
                                    case 0:
                                        args.Add(typeof(string)); 
                                        break;
                                    case 1: 
                                        args.Add(typeof(Type)); 
                                        break;
                                    case 2: 
                                        args.Add(typeof(object)); 
                                        break;
                                    case 3:
                                        args.Add(typeof(int));
                                        break;
                                }
                            }
                            methodsToTest.Add(method.MakeGenericMethod(args.ToArray()));
                        }
                        else
                        {
                            methodsToTest.Add(method);
                        }
                    }
                }
                return methodsToTest;
            }
        }

        [Test, TestCaseSource("AllEnumerableMethodsThatHaveEquivalentInQueryable")]
        public void GetQueryableEquivalent_forAnyEnumerableExtensionMethod_returnsAMethod(MethodInfo method)
        {
            // arrange:
            Type expectedQueryableType = typeof(IQueryable);
            if(method.GetParameters()[0].ParameterType.IsGenericType)
            {
                expectedQueryableType = typeof(IQueryable<>).MakeGenericType(method.GetParameters()[0].ParameterType.GetGenericArguments());
            }

            // act:
            MethodInfo result = Reflection.Enumerable.GetQueryableEquivalent(method);

            // assert:
            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(Queryable), result.DeclaringType);
            Assert.IsTrue(expectedQueryableType.IsAssignableFrom(result.GetParameters()[0].ParameterType));
        }

        [Test, TestCaseSource("AllEnumerableMethodsThatHaveNoEquivalentInQueryable")]
        public void GetQueryableEquivalent_forAnyEnumerableMethodThatHaveNoEquivalentInQueryable_throwsKeyNotFoundException(MethodInfo method)
        {
            // assert:
            Assert.Throws<KeyNotFoundException>(delegate {
                Reflection.Enumerable.GetQueryableEquivalent(method);
            });
        }

        [Test, TestCaseSource("QueryableEnumerableEquivantMethods")]
        public void GetQueryableEquivalent_ofAQueryableMethod_returnsTheRightMethod(MethodInfo queryableMethod, MethodInfo enumerableMethod)
        {
            // act:
            MethodInfo queryableEquivalent = Reflection.Enumerable.GetQueryableEquivalent(enumerableMethod);

            // assert:
            Assert.AreSame(queryableMethod, queryableEquivalent);
        }

        [Test]
        public void GetQueryableEquivalent_ofNull_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate
            {
                Reflection.Enumerable.GetQueryableEquivalent(null);
            });
        }

        [Test]
        public void GetQueryableEquivalent_ofAMethodThatDontBelongToEnumerable_throwsArgumentException()
        {
            // arrange:
            MethodInfo[] sampleMethods = new MethodInfo[]
            {
                GetMethodInfo(() => int.Parse("1")),
                GetMethodInfo(() => string.Join(",", new string[0]))
            };

            // assert:
            foreach (MethodInfo method in sampleMethods)
            {
                Assert.Throws<ArgumentException>(delegate
                {
                    Reflection.Enumerable.GetQueryableEquivalent(method);
                });
            }
        }

        #endregion Enumerable
    }
}

