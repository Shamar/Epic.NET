//  
//  Samples.cs
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
using System.Linq.Expressions;
using System.Collections.Generic;
using NUnit.Framework;
using System.Reflection;
using System.Linq;
using System.Collections;

namespace Epic.Linq.Expressions
{
    public static class Samples
    {
        private static MethodCallExpression GetMethodCallExpression<T>(IQueryable<T> queryable)
        {
            return (System.Linq.Expressions.MethodCallExpression)queryable.Expression;
        }

        private static MethodCallExpression GetMethodCallExpression<T>(Expression<Func<string, T>> expression)
        {
            return (System.Linq.Expressions.MethodCallExpression)expression.Body;
        }

        public static IEnumerable<TestCaseData> ReduceableEnumerableMethodCallExpressions
        {
            get
            {
                IEnumerable<string> originalStrings = new string[] {
                    "test-A.1", "test-B.1", "test-B.2",
                    "sample-A.2", "sample-B.1", "sample-C.32"
                };
                string closure = "test";

                yield return
                    new TestCaseData(
                    GetMethodCallExpression(p => originalStrings.Where(s => s.StartsWith("test"))),
                    originalStrings,
                    originalStrings.Where(s => s.StartsWith("test"))
                    );
                yield return new TestCaseData(
                    GetMethodCallExpression(p => originalStrings.Where(s => s.StartsWith(closure))),
                    originalStrings,
                    originalStrings.Where(s => s.StartsWith(closure))
                    );
                yield return new TestCaseData(
                    GetMethodCallExpression(p => originalStrings.Take(3)),
                    originalStrings,
                    originalStrings.Take(3)
                    );
                yield return new TestCaseData(
                    GetMethodCallExpression(p => originalStrings.Skip(2)),
                    originalStrings,
                    originalStrings.Skip(2)
                    );
                yield return new TestCaseData(
                    GetMethodCallExpression(p => originalStrings.SkipWhile(s => s.Contains("1"))),
                    originalStrings,
                    originalStrings.SkipWhile(s => s.Contains("1"))
                    );
                yield return new TestCaseData(
                    GetMethodCallExpression(p => originalStrings.Cast<object>()),
                    originalStrings,
                    originalStrings.Cast<object>()
                    );
            }
        }

        public static IEnumerable<MethodCallExpression> NotReduceableEnumerableMethodCallExpressions
        {
            get
            {
                IEnumerable<string> originalStrings = new string[] {
                    "test-A.1", "test-B.1", "test-B.2",
                    "sample-A.2", "sample-B.1", "sample-C.3"
                };

                Expression<Func<int, IEnumerable<string>>> func = i => originalStrings.Where(s => s.Length > i);
                yield return (MethodCallExpression)func.Body;

                func = i => originalStrings.Skip(i);
                yield return (MethodCallExpression)func.Body;

                func = i => originalStrings.GroupBy(s => s + i.ToString(), (s, e) => e.Count().ToString());
                yield return (MethodCallExpression)func.Body;

                Expression<Func<string, IEnumerable<string>>> funcS = s => originalStrings.Skip(s.Length);
                yield return GetMethodCallExpression(funcS);
            }
        }

        public static IEnumerable<TestCaseData> ReduceableQueryableMethodCallExpressions
        {
            get
            {
                IEnumerable<string> originalStrings = new string[] {
                    "test-A.1", "test-B.1", "test-B.2",
                    "sample-A.2", "sample-B.1", "sample-C.32"
                };
                string closure = "test";
                IQueryable<string> queryableString = originalStrings.AsQueryable().Where(s => false); // this where simulate a deeper tree

                yield return new TestCaseData(
                    GetMethodCallExpression(queryableString.Where(s => s.StartsWith("test"))),
                    originalStrings,
                    originalStrings.Where(s => s.StartsWith("test"))
                    );
                yield return new TestCaseData(
                    GetMethodCallExpression(queryableString.Where(s => s.StartsWith(closure))),
                    originalStrings,
                    originalStrings.Where(s => s.StartsWith(closure))
                    );
                yield return new TestCaseData(
                    GetMethodCallExpression(queryableString.Take(closure.Length)),
                    originalStrings,
                    originalStrings.Take(closure.Length)
                    );
                yield return new TestCaseData(
                    GetMethodCallExpression(queryableString.Take(3)),
                    originalStrings,
                    originalStrings.Take(3)
                    );
                yield return new TestCaseData(
                    GetMethodCallExpression(queryableString.Skip(2)),
                    originalStrings,
                    originalStrings.Skip(2)
                    );
                yield return new TestCaseData(
                    GetMethodCallExpression(queryableString.SkipWhile(s => s.Contains("1"))),
                    originalStrings,
                    originalStrings.SkipWhile(s => s.Contains("1"))
                    );
                yield return new TestCaseData(
                    GetMethodCallExpression(queryableString.Cast<object>()),
                    originalStrings,
                    originalStrings.Cast<object>()
                    );
            }
        }

        public static IEnumerable<TestCaseData> NotReduceableQueryableMethodCallExpressions
        {
            get
            {
                IEnumerable<string> originalStrings = new string[] {
                    "test-A.1", "test-B.1", "test-B.2",
                    "sample-A.2", "sample-B.1", "sample-C.3"
                };
                IQueryable<string> queryableString = originalStrings.AsQueryable().Where(s => true); // this where simulate a deeper tree

                Expression<Func<int, IQueryable<string>>> func = i => queryableString.Where(s => s.Length > i);
                Expression<Func<int, IEnumerable<string>>> func2 = i => originalStrings.Where(s => s.Length > i);
                yield return new TestCaseData(
                    (MethodCallExpression)func.Body,
                    ((MethodCallExpression)func2.Body).Method
                    );

                func = i => queryableString.Skip(i);
                func2 = i => originalStrings.Skip(i);
                yield return new TestCaseData(
                    (MethodCallExpression)func.Body,
                    ((MethodCallExpression)func2.Body).Method
                    );

                func = i => queryableString.GroupBy(s => s + i.ToString(), (s, e) => e.Count().ToString());
                func2 = i => originalStrings.GroupBy(s => s + i.ToString(), (s, e) => e.Count().ToString());
                yield return new TestCaseData(
                    (MethodCallExpression)func.Body,
                    ((MethodCallExpression)func2.Body).Method
                    );

                Expression<Func<string, IQueryable<string>>> funcS = s => queryableString.Skip(s.Length);
                Expression<Func<string, IEnumerable<string>>> funcS2 = s => originalStrings.Skip(s.Length);
                yield return new TestCaseData(
                    (MethodCallExpression)funcS.Body,
                    ((MethodCallExpression)funcS2.Body).Method
                    );
            }
        }

        public static IEnumerable<Expression> BinaryExpressions
        {
            get
            {
                yield return Expression.Add(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.AddChecked(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.Divide(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.Modulo(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.Multiply(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.MultiplyChecked(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.Power(Expression.Constant(2.0), Expression.Constant(2.0));
                yield return Expression.Subtract(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.SubtractChecked(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.And(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.Or(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.ExclusiveOr(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.LeftShift(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.RightShift(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.AndAlso(Expression.Constant(true), Expression.Constant(false));
                yield return Expression.OrElse(Expression.Constant(true), Expression.Constant(false));
                yield return Expression.Equal(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.NotEqual(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.GreaterThanOrEqual(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.GreaterThan(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.LessThan(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.LessThanOrEqual(Expression.Constant(1), Expression.Constant(2));
                yield return Expression.Coalesce(Expression.Parameter(typeof(string), "p"), Expression.Constant("test"));
                yield return Expression.ArrayIndex(Expression.Constant(new int[1]), Expression.Constant(0));
            }
        }
        
        public static IEnumerable<TestCaseData> DifferentBinaryExpressionsFromTheSameFactory
        {
            get
            {
                yield return new TestCaseData( Expression.Add(Expression.Constant(1), Expression.Constant(2)), Expression.Add(Expression.Constant(3), Expression.Constant(4)) );
                yield return new TestCaseData( Expression.AddChecked(Expression.Constant(1), Expression.Constant(2)), Expression.AddChecked(Expression.Constant(3), Expression.Constant(4)) );
                yield return new TestCaseData( Expression.Divide(Expression.Constant(1), Expression.Constant(2)), Expression.Divide(Expression.Constant(3), Expression.Constant(4)) );
                yield return new TestCaseData( Expression.Modulo(Expression.Constant(1), Expression.Constant(2)), Expression.Modulo(Expression.Constant(3), Expression.Constant(4)) );
                yield return new TestCaseData( Expression.Multiply(Expression.Constant(1), Expression.Constant(2)), Expression.Multiply(Expression.Constant(3), Expression.Constant(4)) );
                yield return new TestCaseData( Expression.MultiplyChecked(Expression.Constant(1), Expression.Constant(2)), Expression.MultiplyChecked(Expression.Constant(3), Expression.Constant(4)) );
                yield return new TestCaseData( Expression.Power(Expression.Constant(2.0), Expression.Constant(2.0)), Expression.Power(Expression.Constant(3.0), Expression.Constant(3.0)) );
                yield return new TestCaseData( Expression.Subtract(Expression.Constant(1), Expression.Constant(2)), Expression.Subtract(Expression.Constant(3), Expression.Constant(4)) );
                yield return new TestCaseData( Expression.SubtractChecked(Expression.Constant(1), Expression.Constant(2)), Expression.SubtractChecked(Expression.Constant(3), Expression.Constant(4)) );
                yield return new TestCaseData( Expression.And(Expression.Constant(1), Expression.Constant(2)), Expression.And(Expression.Constant(3), Expression.Constant(4)) );
                yield return new TestCaseData( Expression.Or(Expression.Constant(1), Expression.Constant(2)), Expression.Or(Expression.Constant(3), Expression.Constant(4)) );
                yield return new TestCaseData( Expression.ExclusiveOr(Expression.Constant(1), Expression.Constant(2)), Expression.ExclusiveOr(Expression.Constant(3), Expression.Constant(4)) );
                yield return new TestCaseData( Expression.LeftShift(Expression.Constant(1), Expression.Constant(2)), Expression.LeftShift(Expression.Constant(3), Expression.Constant(4)) );
                yield return new TestCaseData( Expression.RightShift(Expression.Constant(1), Expression.Constant(2)), Expression.RightShift(Expression.Constant(3), Expression.Constant(4)) );
                yield return new TestCaseData( Expression.AndAlso(Expression.Constant(true), Expression.Constant(false)), Expression.AndAlso(Expression.Constant(false), Expression.Constant(true)) );
                yield return new TestCaseData( Expression.OrElse(Expression.Constant(true), Expression.Constant(false)), Expression.OrElse(Expression.Constant(false), Expression.Constant(true)) );
                yield return new TestCaseData( Expression.Equal(Expression.Constant(1), Expression.Constant(2)), Expression.Equal(Expression.Constant(3), Expression.Constant(4)) );
                yield return new TestCaseData( Expression.NotEqual(Expression.Constant(1), Expression.Constant(2)), Expression.NotEqual(Expression.Constant(3), Expression.Constant(4)) );
                yield return new TestCaseData( Expression.GreaterThanOrEqual(Expression.Constant(1), Expression.Constant(2)), Expression.GreaterThanOrEqual(Expression.Constant(3), Expression.Constant(4)) );
                yield return new TestCaseData( Expression.GreaterThan(Expression.Constant(1), Expression.Constant(2)), Expression.GreaterThan(Expression.Constant(3), Expression.Constant(4)) );
                yield return new TestCaseData( Expression.LessThan(Expression.Constant(1), Expression.Constant(2)), Expression.LessThan(Expression.Constant(3), Expression.Constant(4)) );
                yield return new TestCaseData( Expression.LessThanOrEqual(Expression.Constant(1), Expression.Constant(2)), Expression.LessThanOrEqual(Expression.Constant(3), Expression.Constant(4)) );
                yield return new TestCaseData( Expression.Coalesce(Expression.Parameter(typeof(string), "p"), Expression.Constant("test")), Expression.Coalesce(Expression.Parameter(typeof(object), "o"), Expression.Constant("object")) );
                yield return new TestCaseData( Expression.ArrayIndex(Expression.Constant(new int[1]), Expression.Constant(0)), Expression.ArrayIndex(Expression.Constant(new int[2]), Expression.Constant(1)) );
            }
        }
        
        public static IEnumerable<TestCaseData> DifferentUnaryExpressionsFromTheSameFactory
        {
            get
            {
                yield return new TestCaseData( Expression.ArrayLength(Expression.Constant(new int[0])), Expression.ArrayLength(Expression.Constant(new int[1] {0})) ); 
                yield return new TestCaseData( Expression.Convert(Expression.Constant(1), typeof(uint)), Expression.Convert(Expression.Constant(2), typeof(uint)) ); 
                yield return new TestCaseData( Expression.ConvertChecked(Expression.Constant(1), typeof(uint)), Expression.ConvertChecked(Expression.Constant(2), typeof(uint)) );
                yield return new TestCaseData( Expression.Negate(Expression.Constant(1)), Expression.Negate(Expression.Constant(2)) );
                //yield return new TestCaseData( Expression.NegateChecked(Expression.Constant(1)), Expression.NegateChecked(Expression.Constant(2)) );
                yield return new TestCaseData( Expression.Not(Expression.Constant(true)), Expression.Not(Expression.Constant(false)) );
                Expression<Func<int, bool>> toQuote1 = i => i > 0;
                Expression<Func<int, bool>> toQuote2 = i => i < 10;
                yield return new TestCaseData( Expression.Quote(toQuote1), Expression.Quote(toQuote2) );
                yield return new TestCaseData( Expression.TypeAs(Expression.Constant(new object()), typeof(string)), Expression.TypeAs(Expression.Constant(new object()), typeof(Type)) );
                yield return new TestCaseData( Expression.UnaryPlus(Expression.Constant(1)), Expression.UnaryPlus(Expression.Constant(2)) );
            }
        }
        
        public static IEnumerable<Expression> UnaryExpressions
        {
            get
            {
                yield return Expression.ArrayLength(Expression.Constant(new int[0])); 
                yield return Expression.Convert(Expression.Constant(1), typeof(uint));
                yield return Expression.ConvertChecked(Expression.Constant(1), typeof(uint));
                yield return Expression.Negate(Expression.Constant(1));
                //yield return Expression.NegateChecked(Expression.Constant(1));
                yield return Expression.Not(Expression.Constant(true));
                Expression<Func<int, bool>> toQuote = i => i > 0;
                yield return Expression.Quote(toQuote);
                yield return Expression.TypeAs(Expression.Constant(new object()), typeof(string));
                yield return Expression.UnaryPlus(Expression.Constant(1));
            }
        }
        
        public static TypeBinaryExpression GetNewTypeBinaryExpression<T>(T value)
        {
            return Expression.TypeIs(Expression.Constant(value), typeof(int));
        }
        
        public static ListInitExpression GetNewListInitExpression()
        {
            // from http://msdn.microsoft.com/it-it/library/system.linq.expressions.listinitexpression(v=VS.90).aspx
            string tree1 = "maple";
            string tree2 = "oak";
            
            System.Reflection.MethodInfo addMethod = typeof(Dictionary<int, string>).GetMethod("Add");
            
            ElementInit elementInit1 = Expression.ElementInit(addMethod, Expression.Constant(tree1.Length), Expression.Constant(tree1));
            ElementInit elementInit2 = Expression.ElementInit(addMethod, Expression.Constant(tree2.Length), Expression.Constant(tree2));
            
            NewExpression newDictionaryExpression = Expression.New(typeof(Dictionary<int, string>));
            
            
            return Expression.ListInit(newDictionaryExpression, elementInit1, elementInit2);
        }
        
        public static ListInitExpression[] GetTwoDifferentListInitExpressions()
        {
            // from http://msdn.microsoft.com/it-it/library/system.linq.expressions.listinitexpression(v=VS.90).aspx
            string tree1 = "lemon";
            string tree2 = "orange";
            
            ListInitExpression first = GetNewListInitExpression();
            
            System.Reflection.MethodInfo addMethod = typeof(DummyDictionary<int, string>).GetMethod("Add");
            ElementInit elementInit1 = Expression.ElementInit(addMethod, Expression.Constant(tree1.Length), Expression.Constant(tree1));
            ElementInit elementInit2 = Expression.ElementInit(addMethod, Expression.Constant(tree2.Length), Expression.Constant(tree2));
            
            NewExpression newDictionaryExpression = Expression.New(typeof(DummyDictionary<int, string>));
            ListInitExpression second = Expression.ListInit(newDictionaryExpression, elementInit1, elementInit2);
            
            
            return new ListInitExpression[] { first, second } ;
        }
        
        public static NewExpression GetNewExpressionWithMembers<KVKey, KVValue>(KVKey key, KVValue value)
        {
            NewExpression expression = Expression.New (
              typeof (KeyValuePair<KVKey, KVValue>).GetConstructor (new[] { typeof (KVKey), typeof (KVValue) }),
              new Expression[] { Expression.Constant (key), Expression.Constant (value) },
              typeof (KeyValuePair<KVKey, KVValue>).GetProperty ("Key"), typeof (KeyValuePair<KVKey, KVValue>).GetProperty ("Value"));
            return expression;
        }
        
        public static NewExpression GetNewExpressionWithoutMembers<KVKey, KVValue>(KVKey key, KVValue value)
        {
            NewExpression expression = Expression.New (
              typeof (KeyValuePair<KVKey, KVValue>).GetConstructor (new[] { typeof (KVKey), typeof (KVValue) }),
              new Expression[] { Expression.Constant (key), Expression.Constant (value) });
            return expression;
        }
        
        public static MemberInitExpression GetNewMemberInitExpression()
        {
            Type collectionType = typeof(ICollection<ClassToTestMemberBindings>);
            MethodInfo getCollectionAdd = collectionType.GetMethod("Add");
            Type type = typeof(ClassToTestMemberBindings);
            ConstructorInfo typeConstructor = type.GetConstructor(new Type[0]);
            MethodInfo setName = type.GetProperty("Name").GetSetMethod();
            MethodInfo getFather = type.GetProperty("Father").GetGetMethod();
            MethodInfo getChildren = type.GetProperty("Children").GetGetMethod();
            ParameterExpression parameterExpression = Expression.Parameter(typeof(string), "s");
            MemberInitExpression expression = Expression.MemberInit(
                Expression.New(typeConstructor, new Expression[0]),
                new MemberBinding[] { 
                    Expression.Bind(setName, parameterExpression),
                    Expression.MemberBind(getFather, 
                        new MemberBinding[] {
                            Expression.Bind(setName, Expression.Constant("Father", typeof(string)))
                        }),
                        Expression.ListBind(getChildren, new ElementInit[] {
                            Expression.ElementInit(getCollectionAdd, 
                                new Expression[]{ Expression.New(typeConstructor, new Expression[0]) }
                            ),
                            Expression.ElementInit(getCollectionAdd, 
                                new Expression[]{ Expression.New(typeConstructor, new Expression[0]) }
                            )
                        })
                    });
            return expression;
        }
        
        public static MemberInitExpression[] GetTwoDifferentMemberInitExpression()
        {
            MemberInitExpression expression1 = GetNewMemberInitExpression();
            Type collectionType = typeof(ICollection<ClassToTestMemberBindings>);
            MethodInfo getCollectionAdd = collectionType.GetMethod("Add");
            Type type = typeof(ClassToTestMemberBindings);
            ConstructorInfo typeConstructor = type.GetConstructor(new Type[]{typeof(string)});
            MethodInfo setName = type.GetProperty("Name").GetSetMethod();
            MethodInfo getFather = type.GetProperty("Father").GetGetMethod();
            MethodInfo getChildren = type.GetProperty("Children").GetGetMethod();
            ParameterExpression parameterExpression = Expression.Parameter(typeof(string), "s");
            MemberInitExpression expression2 = Expression.MemberInit(
                Expression.New(typeConstructor, new Expression[] { parameterExpression }),
                new MemberBinding[] { 
                    Expression.Bind(setName, parameterExpression),
                    Expression.MemberBind(getFather, 
                        new MemberBinding[] {
                            Expression.Bind(setName, Expression.Constant("NewName", typeof(string)))
                        }),
                        Expression.ListBind(getChildren, new ElementInit[] {
                            Expression.ElementInit(getCollectionAdd, 
                                new Expression[]{ Expression.New(typeConstructor, new Expression[] {
                                    Expression.Add(parameterExpression, Expression.Constant("1", typeof(string)), typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) }))
                                }) 
                            }),
                            Expression.ElementInit(getCollectionAdd, 
                                new Expression[]{ Expression.New(typeConstructor, new Expression[] {
                                    Expression.Add(parameterExpression, Expression.Constant("2", typeof(string)), typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) }))
                                }) 
                            })
                        })
                    });
            return new MemberInitExpression[] { expression1, expression2 };
        }
        
        #region dummy types
        class DummyDictionary<TKey, TValue> : Dictionary<TKey, TValue>
        {
            static DummyDictionary()
            {
                DummyDictionary<int, string> dict = new DummyDictionary<int, string>();
                dict.Add(1, "test");
            }
            public new void Add (TKey key, TValue value)
            {
                base.Add(key, value);
            }
        }
        
        class ClassToTestMemberBindings
        {
            public ClassToTestMemberBindings()
            {
            }
            public ClassToTestMemberBindings(string name)
                : this()
            {
                Name = name;
            }
            
            public string Name { get; set; }
            
            private ClassToTestMemberBindings _father = new ClassToTestMemberBindings();
            public ClassToTestMemberBindings Father { get { return _father; } set { _father = value; } }
            
            private IList<ClassToTestMemberBindings> _children = new List<ClassToTestMemberBindings>();
            public IList<ClassToTestMemberBindings> Children { get { return _children; } set { _children = value; } }
        }
        #endregion dummy types
    }
}

