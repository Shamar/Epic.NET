//  
//  ReflectionQA.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2011 Giacomo Tesio
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

namespace Epic.Linq
{
    public interface ITestEnumerable<T> : IOrderedEnumerable<T>
    {
    }
    
    public interface IClosedTestEnumerable : ITestEnumerable<string>
    {
    }
    
    public sealed class DummyClassWithDifferentMembers
    {
        public DummyClassWithDifferentMembers() { this.SampleEvent += delegate { }; }
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
        
        [Test]
        public void TryGetItemTypeOfEnumerable_withoutEnumerableType_throwsArgumentNullException()
        {
            // arrange:
            Type elementType = null;

            // assert:
            Assert.Throws<ArgumentNullException>(delegate { Reflection.TryGetItemTypeOfEnumerable(null, out elementType); });
        }
        
        [TestCase(typeof(IEnumerable<string>), typeof(string))]
        [TestCase(typeof(IEnumerable<int>), typeof(int))]
        [TestCase(typeof(IEnumerable<IEnumerable<string>>), typeof(IEnumerable<string>))]
        [TestCase(typeof(IEnumerable), typeof(object))]
        [TestCase(typeof(int[]), typeof(int))]
        [TestCase(typeof(IQueryable<string>), typeof(string))]
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
                IQueryable<int> dummyQueryable = null;
                IEnumerable<int> dummyEnumerable = null;
                yield return new TestCaseData( 
                    GetMethodInfo(() => dummyQueryable.Aggregate(0, (i,acc) => i + acc)), 
                    GetMethodInfo(() => dummyEnumerable.Aggregate(0, (i,acc) => i + acc))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => dummyQueryable.Aggregate((i, acc) => i + acc)),
                    GetMethodInfo(() => dummyEnumerable.Aggregate((i, acc) => i + acc))
                    );
                yield return new TestCaseData(
                    GetMethodInfo(() => dummyQueryable.Aggregate(0, (i, acc) => i + acc, acc => acc )),
                    GetMethodInfo(() => dummyEnumerable.Aggregate(0, (i, acc) => i + acc, acc => acc))
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

        #endregion Queryable
    }
}

