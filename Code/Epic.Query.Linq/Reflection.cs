//  
//  Reflection.cs
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
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace Epic.Query.Linq
{
	/// <summary>
	/// Reflection's utilities. Could become public in the future.
	/// </summary>
	/// <exception cref='ArgumentNullException'>
	/// Is thrown when an argument passed to a method is invalid because it is <see langword="null" /> .
	/// </exception>
	internal static class Reflection
	{
		/// <summary>
		/// Tries to find the item type of an enumerable's type.
		/// </summary>
		/// <returns>
		/// The item type of <paramref name="enumerableType"/>.
		/// </returns>
		/// <param name='itemType'>
		/// When the method returns <c>true</c>, itemType will contains the item type.
		/// </param>
		/// <param name='enumerableType'>
		/// Type to inspect.
		/// </param>
		/// <exception cref='ArgumentNullException'>
		/// Is thrown when <paramref name="enumerableType"/> is <see langword="null" /> .
		/// </exception>
		public static bool TryGetItemTypeOfEnumerable(Type enumerableType, out Type itemType)
		{
			if(null == enumerableType)
				throw new ArgumentNullException("enumerableType");
			if(!typeof(IEnumerable).IsAssignableFrom(enumerableType))
			{
				// it is not an IEnumerable
				itemType = null;
				return false;
			}
			
			if (enumerableType.IsArray)
			{
				// it is an array
				itemType = enumerableType.GetElementType ();
			}
			else if(IsGenericEnumerable(enumerableType))
			{
				// it is an IEnumerable<>
				itemType = enumerableType.GetGenericArguments ()[0];
			}
			else
			{
				// look for implementations of IEnumerable<>
				Type[] interfaces = enumerableType.GetInterfaces();
				for(int i = 0; i < interfaces.Length; ++i)
				{
					if(IsGenericEnumerable(interfaces[i]))
					{
						// interfaces[i] is IEnumerable<>
						itemType = interfaces[i].GetGenericArguments ()[0];
						return true;
					}
				}
                
                // if reached this line, it is an IEnumerable.
                itemType = typeof(object);
			}

			return true;
		}
		
		/// <summary>
		/// Determines whether the <paramref name="enumerableType"/>'s generic type definition is <see cref="IEnumerable{T}"/>.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the <paramref name="enumerableType"/>'s generic type definition is <see cref="IEnumerable{T}"/>; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='enumerableType'>
		/// Type to check.
		/// </param>
	    private static bool IsGenericEnumerable (Type enumerableType)
	    {
			return enumerableType.IsGenericType && enumerableType.GetGenericTypeDefinition().Equals(typeof (System.Collections.Generic.IEnumerable<>));
	    }
        
        /// <summary>
        /// Gets the type of the member return.
        /// </summary>
        /// <returns>
        /// The member return type.
        /// </returns>
        /// <param name='member'>
        /// Member to reflect.
        /// </param>
        /// <exception cref='ArgumentNullException'>
        /// Is thrown when an argument passed to a method is invalid because it is <see langword="null" /> .
        /// </exception>
        /// <exception cref='ArgumentException'>
        /// Is thrown when an argument passed to a method is invalid.
        /// </exception>
        public static Type GetMemberReturnType (MemberInfo member)
        {
            if(null == member)
                throw new ArgumentNullException("member");
            
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    return ((PropertyInfo) member).PropertyType;
                case MemberTypes.Field:
                    return ((FieldInfo) member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo) member).ReturnType;
                default:
                    throw new ArgumentException ("The member must be FieldInfo, PropertyInfo, or MethodInfo.", "member");
            }
        }

        private static MethodInfo GetGenericMethodInfoFromExpressionBody<T>(Expression<Func<T>> expression)
        {
            MethodInfo method = ((MethodCallExpression)expression.Body).Method;
            if(method.IsGenericMethod)
                return method.GetGenericMethodDefinition();
            return method;
        }

        /// <summary>
        /// Queryable related utilities.
        /// </summary>
        internal static class Queryable
        {
            /// <summary>
            /// Translations between methods of Queryable and of Enumerable.
            /// </summary>
            private readonly static Dictionary<MethodInfo, MethodInfo> _methodsTranslations;

            private static void AddTranslation(MethodInfo queryableMethod, MethodInfo enumerableEquivalent)
            {
                /* In case of addition, run once.
                 * 
                if (queryableMethod.DeclaringType != typeof(System.Linq.Queryable))
                    throw new ArgumentException("queryableMethod");
                if (enumerableEquivalent.DeclaringType != typeof(System.Linq.Enumerable))
                    throw new ArgumentException("enumerableEquivalent");
                 */ 
                _methodsTranslations.Add(queryableMethod, enumerableEquivalent);
            }

            /// <summary>
            /// Initialize (manually) the translations. 
            /// Done this way to keep explicit control over the .NET framework changes.
            /// </summary>
            static Queryable()
            {
                _methodsTranslations = new Dictionary<MethodInfo, MethodInfo>();
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

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Aggregate(0, (i, acc) => i.GetHashCode() + acc.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => e.Aggregate(0, (i, acc) => i.GetHashCode() + acc.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Aggregate((i, acc) => i.GetHashCode() + acc.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => e.Aggregate((i, acc) => i.GetHashCode() + acc.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Aggregate(0, (i, acc) => i.GetHashCode() + acc.GetHashCode(), acc => acc.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => e.Aggregate(0, (i, acc) => i.GetHashCode() + acc.GetHashCode(), acc => acc.GetHashCode()))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.All(i => i.GetHashCode() > 0)),
                    GetGenericMethodInfoFromExpressionBody(() => e.All(i => i.GetHashCode() > 0))
                    );
                
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Any()),
                    GetGenericMethodInfoFromExpressionBody(() => e.Any())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Any(i => i.GetHashCode() > 0)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Any(i => i.GetHashCode() > 0))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => qInt.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => eInt.Average())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => qIntN.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => eIntN.Average())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => qDecimal.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => eDecimal.Average())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => qDecimalN.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => eDecimalN.Average())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => qLong.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => eLong.Average())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => qLongN.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => eLongN.Average())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => qDouble.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => eDouble.Average())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => qDoubleN.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => eDoubleN.Average())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => qFloat.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => eFloat.Average())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => qFloatN.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => eFloatN.Average())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => Convert.ToDecimal(i))),
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => Convert.ToDecimal(i)))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => i.GetHashCode() > 0 ? new Nullable<decimal>(Convert.ToDecimal(i)) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => i.GetHashCode() > 0 ? new Nullable<decimal>(Convert.ToDecimal(i)) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => i.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => i.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => i.GetHashCode() > 0 ? new Nullable<int>(i.GetHashCode()) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => i.GetHashCode() > 0 ? new Nullable<int>(i.GetHashCode()) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => Convert.ToInt64(i))),
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => Convert.ToInt64(i)))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => i.GetHashCode() > 0 ? new Nullable<long>(Convert.ToInt64(i)) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => i.GetHashCode() > 0 ? new Nullable<long>(Convert.ToInt64(i)) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => Convert.ToDouble(i))),
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => Convert.ToDouble(i)))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => i.GetHashCode() > 0 ? new Nullable<double>(Convert.ToDouble(i)) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => i.GetHashCode() > 0 ? new Nullable<double>(Convert.ToDouble(i)) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => (float)i)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => (float)i))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => i.GetHashCode() > 0 ? new Nullable<float>((float)i) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => i.GetHashCode() > 0 ? new Nullable<float>((float)i) : null))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Cast<decimal>()),
                    GetGenericMethodInfoFromExpressionBody(() => e.Cast<decimal>())
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Concat(null)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Concat(null))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Contains(1)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Contains(1))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Contains(1, EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Contains(1, EqualityComparer<object>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Count()),
                    GetGenericMethodInfoFromExpressionBody(() => e.Count())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Count(i => true)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Count(i => true))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.DefaultIfEmpty()),
                    GetGenericMethodInfoFromExpressionBody(() => e.DefaultIfEmpty())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.DefaultIfEmpty(5)),
                    GetGenericMethodInfoFromExpressionBody(() => e.DefaultIfEmpty(5))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Distinct()),
                    GetGenericMethodInfoFromExpressionBody(() => e.Distinct())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Distinct(EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Distinct(EqualityComparer<object>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.ElementAt(1)),
                    GetGenericMethodInfoFromExpressionBody(() => e.ElementAt(1))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.ElementAtOrDefault(1)),
                    GetGenericMethodInfoFromExpressionBody(() => e.ElementAtOrDefault(1))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Except(dummyEnumerable)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Except(dummyEnumerable))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Except(dummyEnumerable, EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Except(dummyEnumerable, EqualityComparer<object>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.First()),
                    GetGenericMethodInfoFromExpressionBody(() => e.First())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.First(i => true)),
                    GetGenericMethodInfoFromExpressionBody(() => e.First(i => true))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.FirstOrDefault()),
                    GetGenericMethodInfoFromExpressionBody(() => e.FirstOrDefault())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.FirstOrDefault(i => false)),
                    GetGenericMethodInfoFromExpressionBody(() => e.FirstOrDefault(i => false))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupBy(i => i.GetType())),
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupBy(i => i.GetType()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupBy(i => i.GetType(), i => i.ToString())),
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupBy(i => i.GetType(), i => i.ToString()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupBy(i => i.GetType(), (i, g) => g.Count())),
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupBy(i => i.GetType(), (i, g) => g.Count()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupBy(i => i.GetType(), (i, g) => g.Count(), EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupBy(i => i.GetType(), (i, g) => g.Count(), EqualityComparer<object>.Default))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupBy(i => i.GetType(), EqualityComparer<Type>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupBy(i => i.GetType(), EqualityComparer<Type>.Default))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupBy(i => i.GetType(), i => i.ToString(), EqualityComparer<Type>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupBy(i => i.GetType(), i => i.ToString(), EqualityComparer<Type>.Default))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupBy(i => i.GetType(), i => i.ToString(), (k, g) => g.Count())),
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupBy(i => i.GetType(), i => i.ToString(), (k, g) => g.Count()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupBy(i => i.GetType(), i => i.ToString(), (k, g) => g.Count(), EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupBy(i => i.GetType(), i => i.ToString(), (k, g) => g.Count(), EqualityComparer<object>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupJoin(dummyEnumerable, i => i, j => j, (k, g) => g.Count())),
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupJoin(dummyEnumerable, i => i, j => j, (k, g) => g.Count()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupJoin(dummyEnumerable, i => i, j => j, (k, g) => g.Count(), EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupJoin(dummyEnumerable, i => i, j => j, (k, g) => g.Count(), EqualityComparer<object>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Intersect(dummyEnumerable)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Intersect(dummyEnumerable))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Intersect(dummyEnumerable, EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Intersect(dummyEnumerable, EqualityComparer<object>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Join(dummyEnumerable, i => i, j => j, (k, g) => k.GetHashCode() + g.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => e.Join(dummyEnumerable, i => i, j => j, (k, g) => k.GetHashCode() + g.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Join(dummyEnumerable, i => i, j => j, (k, g) => k.GetHashCode() + g.GetHashCode(), EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Join(dummyEnumerable, i => i, j => j, (k, g) => k.GetHashCode() + g.GetHashCode(), EqualityComparer<object>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Last()),
                    GetGenericMethodInfoFromExpressionBody(() => e.Last())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Last(i => true)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Last(i => true))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.LastOrDefault()),
                    GetGenericMethodInfoFromExpressionBody(() => e.LastOrDefault())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.LastOrDefault(i => false)),
                    GetGenericMethodInfoFromExpressionBody(() => e.LastOrDefault(i => false))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.LongCount()),
                    GetGenericMethodInfoFromExpressionBody(() => e.LongCount())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.LongCount(i => true)),
                    GetGenericMethodInfoFromExpressionBody(() => e.LongCount(i => true))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Max()),
                    GetGenericMethodInfoFromExpressionBody(() => e.Max())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Max(i => i.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Max<object, int>(e, i => i.GetHashCode()))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Min()),
                    GetGenericMethodInfoFromExpressionBody(() => e.Min())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Min(i => i.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Min<object, int>(e, i => i.GetHashCode()))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.OfType<string>()),
                    GetGenericMethodInfoFromExpressionBody(() => e.OfType<string>())
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.OrderBy(i => i.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => e.OrderBy(i => i.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.OrderBy(i => i.GetHashCode(), Comparer<int>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.OrderBy(i => i.GetHashCode(), Comparer<int>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.OrderByDescending(i => i.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => e.OrderByDescending(i => i.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.OrderByDescending(i => i.GetHashCode(), Comparer<int>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.OrderByDescending(i => i.GetHashCode(), Comparer<int>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.OrderBy(i => i.GetHashCode()).ThenBy(i => i.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => e.OrderBy(i => i.GetHashCode()).ThenBy(i => i.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.OrderBy(i => i.GetHashCode()).ThenBy(i => i.GetHashCode(), Comparer<int>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.OrderBy(i => i.GetHashCode()).ThenBy(i => i.GetHashCode(), Comparer<int>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.OrderBy(i => i.GetHashCode()).ThenByDescending(i => i.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => e.OrderBy(i => i.GetHashCode()).ThenByDescending(i => i.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.OrderBy(i => i.GetHashCode()).ThenByDescending(i => i.GetHashCode(), Comparer<int>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.OrderBy(i => i.GetHashCode()).ThenByDescending(i => i.GetHashCode(), Comparer<int>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Reverse()),
                    GetGenericMethodInfoFromExpressionBody(() => e.Reverse())
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Select(i => i.GetType())),
                    GetGenericMethodInfoFromExpressionBody(() => e.Select(i => i.GetType()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Select((i, p) => i.GetHashCode() % p)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Select((i, p) => i.GetHashCode() % p))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.SelectMany(i => new int[] { i.GetHashCode() })),
                    GetGenericMethodInfoFromExpressionBody(() => e.SelectMany(i => new int[] { i.GetHashCode() }))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.SelectMany((i, p) => new int[] { i.GetHashCode(), p })),
                    GetGenericMethodInfoFromExpressionBody(() => e.SelectMany((i, p) => new int[] { i.GetHashCode(), p }))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.SelectMany(i => new int[] { i.GetHashCode() }, (i, c) => i.GetHashCode() + c)),
                    GetGenericMethodInfoFromExpressionBody(() => e.SelectMany(i => new int[] { i.GetHashCode() }, (i, c) => i.GetHashCode() + c))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.SelectMany((i, p) => new int[] { i.GetHashCode(), p }, (i, c) => i.GetHashCode() + c)),
                    GetGenericMethodInfoFromExpressionBody(() => e.SelectMany((i, p) => new int[] { i.GetHashCode(), p }, (i, c) => i.GetHashCode() + c))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.SequenceEqual(dummyEnumerable)),
                    GetGenericMethodInfoFromExpressionBody(() => e.SequenceEqual(dummyEnumerable))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.SequenceEqual(dummyEnumerable, EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.SequenceEqual(dummyEnumerable, EqualityComparer<object>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Single()),
                    GetGenericMethodInfoFromExpressionBody(() => e.Single())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Single(i => true)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Single(i => true))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.SingleOrDefault()),
                    GetGenericMethodInfoFromExpressionBody(() => e.SingleOrDefault())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.SingleOrDefault(i => true)),
                    GetGenericMethodInfoFromExpressionBody(() => e.SingleOrDefault(i => true))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Skip(1)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Skip(1))
                    );
                
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.SkipWhile(i => false)),
                    GetGenericMethodInfoFromExpressionBody(() => e.SkipWhile(i => false))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.SkipWhile((i, p) => false)),
                    GetGenericMethodInfoFromExpressionBody(() => e.SkipWhile((i, p) => false))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Sum(i => Convert.ToDecimal(i))),
                    GetGenericMethodInfoFromExpressionBody(() => e.Sum(i => Convert.ToDecimal(i)))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Sum(i => i.GetHashCode() > 0 ? new Nullable<decimal>(Convert.ToDecimal(i)) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Sum(i => i.GetHashCode() > 0 ? new Nullable<decimal>(Convert.ToDecimal(i)) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Sum(i => i.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => e.Sum(i => i.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Sum(i => i.GetHashCode() > 0 ? new Nullable<int>(i.GetHashCode()) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Sum(i => i.GetHashCode() > 0 ? new Nullable<int>(i.GetHashCode()) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Sum(i => Convert.ToInt64(i))),
                    GetGenericMethodInfoFromExpressionBody(() => e.Sum(i => Convert.ToInt64(i)))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Sum(i => i.GetHashCode() > 0 ? new Nullable<long>(Convert.ToInt64(i)) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Sum(i => i.GetHashCode() > 0 ? new Nullable<long>(Convert.ToInt64(i)) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Sum(i => Convert.ToDouble(i))),
                    GetGenericMethodInfoFromExpressionBody(() => e.Sum(i => Convert.ToDouble(i)))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Sum(i => i.GetHashCode() > 0 ? new Nullable<double>(Convert.ToDouble(i)) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Sum(i => i.GetHashCode() > 0 ? new Nullable<double>(Convert.ToDouble(i)) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Sum(i => (float)i)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Sum(i => (float)i))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Sum(i => i.GetHashCode() > 0 ? new Nullable<float>((float)i) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Sum(i => i.GetHashCode() > 0 ? new Nullable<float>((float)i) : null))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Take(1)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Take(1))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.TakeWhile(i => true)),
                    GetGenericMethodInfoFromExpressionBody(() => e.TakeWhile(i => true))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.TakeWhile((i, p) => true)),
                    GetGenericMethodInfoFromExpressionBody(() => e.TakeWhile((i, p) => true))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Union(dummyEnumerable)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Union(dummyEnumerable))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Union(dummyEnumerable, EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Union(dummyEnumerable, EqualityComparer<object>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Where(i => true)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Where(i => true))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Where((i, p) => true)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Where((i, p) => true))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Zip(dummyEnumerable, (i, p) => true)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Zip(dummyEnumerable, (i, p) => true))
                    );

            }

            /// <summary>
            /// Returns the semantic equivalent of <paramref name="queryableMethod"/> in the <see cref="Enumerable"/> class.
            /// </summary>
            /// <param name="queryableMethod">Queryable method to translate.</param>
            /// <returns>The Enumerable's equivalent of <paramref name="queryableMethod"/>.</returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="queryableMethod"/> is null.</exception>
            /// <exception cref="ArgumentException">Thrown when <paramref name="queryableMethod"/> doesn't belong to <see cref="System.Linq.Queryable"/>.</exception>
            /// <exception cref="KeyNotFoundException">The <paramref name="queryableMethod"/> has no equivalent in <see cref="Enumerable"/>.</exception>
            public static MethodInfo GetEnumerableEquivalent(MethodInfo queryableMethod)
            {
                if (null == queryableMethod)
                    throw new ArgumentNullException("queryableMethod");
                if (!queryableMethod.DeclaringType.Equals(typeof(System.Linq.Queryable)))
                    throw new ArgumentException("The queryableMethod must belong to System.Linq.Queryable.", "queryableMethod");

                MethodInfo method = null;
                if (queryableMethod.IsGenericMethod)
                {
                    method = _methodsTranslations[queryableMethod.GetGenericMethodDefinition()];
                    return method.MakeGenericMethod(queryableMethod.GetGenericArguments());
                }
                return _methodsTranslations[queryableMethod];
            }
        }

        /// <summary>
        /// Enumerable related utilities.
        /// </summary>
        internal static class Enumerable
        {
            /// <summary>
            /// Translations between methods of Queryable and of Enumerable.
            /// </summary>
            private readonly static Dictionary<MethodInfo, MethodInfo> _methodsTranslations;

            private static void AddTranslation(MethodInfo enumerableMethod, MethodInfo queryableEquivalent)
            {
                /* In case of addition, run once.
                 *
                if (enumerableMethod.DeclaringType != typeof(System.Linq.Enumerable))
                    throw new ArgumentException("enumerableMethod");
                if (queryableEquivalent.DeclaringType != typeof(System.Linq.Queryable))
                    throw new ArgumentException("queryableEquivalent");
                 */
                _methodsTranslations.Add(enumerableMethod, queryableEquivalent);
            }

            /// <summary>
            /// Initialize (manually) the translations. 
            /// Done this way to keep explicit control over the .NET framework changes.
            /// </summary>            
            static Enumerable()
            {
                _methodsTranslations = new Dictionary<MethodInfo, MethodInfo>();
                IEnumerable<object> dummyEnumerable = null;
                IEnumerable<object> e = null;
                IQueryable<object> q = null;
                IEnumerable<decimal?> eDecimalN = null;
                IQueryable<decimal?> qDecimalN = null;
                IEnumerable<decimal> eDecimal = null;
                IQueryable<decimal> qDecimal = null;
                IEnumerable<int> eInt = null;
                IQueryable<int> qInt = null;
                IEnumerable<int?> eIntN = null;
                IQueryable<int?> qIntN = null;
                IEnumerable<long> eLong = null;
                IQueryable<long> qLong = null;
                IEnumerable<long?> eLongN = null;
                IQueryable<long?> qLongN = null;
                IEnumerable<double> eDouble = null;
                IQueryable<double> qDouble = null;
                IEnumerable<double?> eDoubleN = null;
                IQueryable<double?> qDoubleN = null;
                IEnumerable<float> eFloat = null;
                IQueryable<float> qFloat = null;
                IEnumerable<float?> eFloatN = null;
                IQueryable<float?> qFloatN = null;

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Aggregate(0, (i, acc) => i.GetHashCode() + acc.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => q.Aggregate(0, (i, acc) => i.GetHashCode() + acc.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Aggregate((i, acc) => i.GetHashCode() + acc.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => q.Aggregate((i, acc) => i.GetHashCode() + acc.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Aggregate(0, (i, acc) => i.GetHashCode() + acc.GetHashCode(), acc => acc.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => q.Aggregate(0, (i, acc) => i.GetHashCode() + acc.GetHashCode(), acc => acc.GetHashCode()))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.All(i => i.GetHashCode() > 0)),
                    GetGenericMethodInfoFromExpressionBody(() => q.All(i => i.GetHashCode() > 0))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Any()),
                    GetGenericMethodInfoFromExpressionBody(() => q.Any())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Any(i => i.GetHashCode() > 0)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Any(i => i.GetHashCode() > 0))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => eInt.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => qInt.Average())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => eIntN.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => qIntN.Average())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => eDecimal.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => qDecimal.Average())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => eDecimalN.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => qDecimalN.Average())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => eLong.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => qLong.Average())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => eLongN.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => qLongN.Average())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => eDouble.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => qDouble.Average())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => eDoubleN.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => qDoubleN.Average())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => eFloat.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => qFloat.Average())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => eFloatN.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => qFloatN.Average())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => Convert.ToDecimal(i))),
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => Convert.ToDecimal(i)))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => i.GetHashCode() > 0 ? new Nullable<decimal>(Convert.ToDecimal(i)) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => i.GetHashCode() > 0 ? new Nullable<decimal>(Convert.ToDecimal(i)) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => i.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => i.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => i.GetHashCode() > 0 ? new Nullable<int>(i.GetHashCode()) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => i.GetHashCode() > 0 ? new Nullable<int>(i.GetHashCode()) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => Convert.ToInt64(i))),
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => Convert.ToInt64(i)))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => i.GetHashCode() > 0 ? new Nullable<long>(Convert.ToInt64(i)) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => i.GetHashCode() > 0 ? new Nullable<long>(Convert.ToInt64(i)) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => Convert.ToDouble(i))),
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => Convert.ToDouble(i)))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => i.GetHashCode() > 0 ? new Nullable<double>(Convert.ToDouble(i)) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => i.GetHashCode() > 0 ? new Nullable<double>(Convert.ToDouble(i)) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => (float)i)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => (float)i))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => i.GetHashCode() > 0 ? new Nullable<float>((float)i) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => i.GetHashCode() > 0 ? new Nullable<float>((float)i) : null))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Cast<decimal>()),
                    GetGenericMethodInfoFromExpressionBody(() => q.Cast<decimal>())
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Concat(null)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Concat(null))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Contains(1)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Contains(1))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Contains(1, EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Contains(1, EqualityComparer<object>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Count()),
                    GetGenericMethodInfoFromExpressionBody(() => q.Count())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Count(i => true)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Count(i => true))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.DefaultIfEmpty()),
                    GetGenericMethodInfoFromExpressionBody(() => q.DefaultIfEmpty())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.DefaultIfEmpty(5)),
                    GetGenericMethodInfoFromExpressionBody(() => q.DefaultIfEmpty(5))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Distinct()),
                    GetGenericMethodInfoFromExpressionBody(() => q.Distinct())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Distinct(EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Distinct(EqualityComparer<object>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.ElementAt(1)),
                    GetGenericMethodInfoFromExpressionBody(() => q.ElementAt(1))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.ElementAtOrDefault(1)),
                    GetGenericMethodInfoFromExpressionBody(() => q.ElementAtOrDefault(1))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Except(dummyEnumerable)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Except(dummyEnumerable))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Except(dummyEnumerable, EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Except(dummyEnumerable, EqualityComparer<object>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.First()),
                    GetGenericMethodInfoFromExpressionBody(() => q.First())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.First(i => true)),
                    GetGenericMethodInfoFromExpressionBody(() => q.First(i => true))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.FirstOrDefault()),
                    GetGenericMethodInfoFromExpressionBody(() => q.FirstOrDefault())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.FirstOrDefault(i => false)),
                    GetGenericMethodInfoFromExpressionBody(() => q.FirstOrDefault(i => false))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupBy(i => i.GetType())),
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupBy(i => i.GetType()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupBy(i => i.GetType(), i => i.ToString())),
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupBy(i => i.GetType(), i => i.ToString()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupBy(i => i.GetType(), (i, g) => g.Count())),
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupBy(i => i.GetType(), (i, g) => g.Count()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupBy(i => i.GetType(), (i, g) => g.Count(), EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupBy(i => i.GetType(), (i, g) => g.Count(), EqualityComparer<object>.Default))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupBy(i => i.GetType(), EqualityComparer<Type>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupBy(i => i.GetType(), EqualityComparer<Type>.Default))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupBy(i => i.GetType(), i => i.ToString(), EqualityComparer<Type>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupBy(i => i.GetType(), i => i.ToString(), EqualityComparer<Type>.Default))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupBy(i => i.GetType(), i => i.ToString(), (k, g) => g.Count())),
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupBy(i => i.GetType(), i => i.ToString(), (k, g) => g.Count()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupBy(i => i.GetType(), i => i.ToString(), (k, g) => g.Count(), EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupBy(i => i.GetType(), i => i.ToString(), (k, g) => g.Count(), EqualityComparer<object>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupJoin(dummyEnumerable, i => i, j => j, (k, g) => g.Count())),
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupJoin(dummyEnumerable, i => i, j => j, (k, g) => g.Count()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupJoin(dummyEnumerable, i => i, j => j, (k, g) => g.Count(), EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupJoin(dummyEnumerable, i => i, j => j, (k, g) => g.Count(), EqualityComparer<object>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Intersect(dummyEnumerable)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Intersect(dummyEnumerable))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Intersect(dummyEnumerable, EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Intersect(dummyEnumerable, EqualityComparer<object>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Join(dummyEnumerable, i => i, j => j, (k, g) => k.GetHashCode() + g.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => q.Join(dummyEnumerable, i => i, j => j, (k, g) => k.GetHashCode() + g.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Join(dummyEnumerable, i => i, j => j, (k, g) => k.GetHashCode() + g.GetHashCode(), EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Join(dummyEnumerable, i => i, j => j, (k, g) => k.GetHashCode() + g.GetHashCode(), EqualityComparer<object>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Last()),
                    GetGenericMethodInfoFromExpressionBody(() => q.Last())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Last(i => true)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Last(i => true))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.LastOrDefault()),
                    GetGenericMethodInfoFromExpressionBody(() => q.LastOrDefault())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.LastOrDefault(i => false)),
                    GetGenericMethodInfoFromExpressionBody(() => q.LastOrDefault(i => false))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.LongCount()),
                    GetGenericMethodInfoFromExpressionBody(() => q.LongCount())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.LongCount(i => true)),
                    GetGenericMethodInfoFromExpressionBody(() => q.LongCount(i => true))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Max()),
                    GetGenericMethodInfoFromExpressionBody(() => q.Max())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Max<object, int>(e, i => i.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Max<object, int>(q, i => i.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Max(eDecimal)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Max(qDecimal))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Max(eDecimalN)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Max(qDecimalN))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Max(eDouble)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Max(qDouble))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Max(eDoubleN)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Max(qDoubleN))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Max(eFloat)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Max(qFloat))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Max(eFloatN)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Max(qFloatN))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Max(eInt)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Max(qInt))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Max(eIntN)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Max(qIntN))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Max(eLong)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Max(qLong))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Max(eLongN)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Max(qLongN))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Max(i => Convert.ToDecimal(i))),
                    GetGenericMethodInfoFromExpressionBody(() => q.Max(i => Convert.ToDecimal(i)))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Max(i => i.GetHashCode() > 0 ? new Nullable<decimal>(Convert.ToDecimal(i)) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Max(i => i.GetHashCode() > 0 ? new Nullable<decimal>(Convert.ToDecimal(i)) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Max(i => i.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => q.Max(i => i.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Max(i => i.GetHashCode() > 0 ? new Nullable<int>(i.GetHashCode()) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Max(i => i.GetHashCode() > 0 ? new Nullable<int>(i.GetHashCode()) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Max(i => Convert.ToInt64(i))),
                    GetGenericMethodInfoFromExpressionBody(() => q.Max(i => Convert.ToInt64(i)))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Max(i => i.GetHashCode() > 0 ? new Nullable<long>(Convert.ToInt64(i)) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Max(i => i.GetHashCode() > 0 ? new Nullable<long>(Convert.ToInt64(i)) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Max(i => Convert.ToDouble(i))),
                    GetGenericMethodInfoFromExpressionBody(() => q.Max(i => Convert.ToDouble(i)))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Max(i => i.GetHashCode() > 0 ? new Nullable<double>(Convert.ToDouble(i)) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Max(i => i.GetHashCode() > 0 ? new Nullable<double>(Convert.ToDouble(i)) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Max(i => (float)i)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Max(i => (float)i))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Max(i => i.GetHashCode() > 0 ? new Nullable<float>((float)i) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Max(i => i.GetHashCode() > 0 ? new Nullable<float>((float)i) : null))
                    );


                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Min()),
                    GetGenericMethodInfoFromExpressionBody(() => q.Min())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Min<object, int>(e, i => i.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Min<object, int>(q, i => i.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Min(eDecimal)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Min(qDecimal))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Min(eDecimalN)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Min(qDecimalN))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Min(eDouble)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Min(qDouble))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Min(eDoubleN)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Min(qDoubleN))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Min(eFloat)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Min(qFloat))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Min(eFloatN)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Min(qFloatN))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Min(eInt)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Min(qInt))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Min(eIntN)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Min(qIntN))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Min(eLong)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Min(qLong))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Min(eLongN)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Min(qLongN))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Min(i => Convert.ToDecimal(i))),
                    GetGenericMethodInfoFromExpressionBody(() => q.Min(i => Convert.ToDecimal(i)))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Min(i => i.GetHashCode() > 0 ? new Nullable<decimal>(Convert.ToDecimal(i)) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Min(i => i.GetHashCode() > 0 ? new Nullable<decimal>(Convert.ToDecimal(i)) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Min(i => i.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => q.Min(i => i.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Min(i => i.GetHashCode() > 0 ? new Nullable<int>(i.GetHashCode()) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Min(i => i.GetHashCode() > 0 ? new Nullable<int>(i.GetHashCode()) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Min(i => Convert.ToInt64(i))),
                    GetGenericMethodInfoFromExpressionBody(() => q.Min(i => Convert.ToInt64(i)))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Min(i => i.GetHashCode() > 0 ? new Nullable<long>(Convert.ToInt64(i)) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Min(i => i.GetHashCode() > 0 ? new Nullable<long>(Convert.ToInt64(i)) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Min(i => Convert.ToDouble(i))),
                    GetGenericMethodInfoFromExpressionBody(() => q.Min(i => Convert.ToDouble(i)))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Min(i => i.GetHashCode() > 0 ? new Nullable<double>(Convert.ToDouble(i)) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Min(i => i.GetHashCode() > 0 ? new Nullable<double>(Convert.ToDouble(i)) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Min(i => (float)i)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Min(i => (float)i))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Min(i => i.GetHashCode() > 0 ? new Nullable<float>((float)i) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Min(i => i.GetHashCode() > 0 ? new Nullable<float>((float)i) : null))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.OfType<string>()),
                    GetGenericMethodInfoFromExpressionBody(() => q.OfType<string>())
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.OrderBy(i => i.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => q.OrderBy(i => i.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.OrderBy(i => i.GetHashCode(), Comparer<int>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => q.OrderBy(i => i.GetHashCode(), Comparer<int>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.OrderByDescending(i => i.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => q.OrderByDescending(i => i.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.OrderByDescending(i => i.GetHashCode(), Comparer<int>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => q.OrderByDescending(i => i.GetHashCode(), Comparer<int>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.OrderBy(i => i.GetHashCode()).ThenBy(i => i.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => q.OrderBy(i => i.GetHashCode()).ThenBy(i => i.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.OrderBy(i => i.GetHashCode()).ThenBy(i => i.GetHashCode(), Comparer<int>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => q.OrderBy(i => i.GetHashCode()).ThenBy(i => i.GetHashCode(), Comparer<int>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.OrderBy(i => i.GetHashCode()).ThenByDescending(i => i.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => q.OrderBy(i => i.GetHashCode()).ThenByDescending(i => i.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.OrderBy(i => i.GetHashCode()).ThenByDescending(i => i.GetHashCode(), Comparer<int>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => q.OrderBy(i => i.GetHashCode()).ThenByDescending(i => i.GetHashCode(), Comparer<int>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Reverse()),
                    GetGenericMethodInfoFromExpressionBody(() => q.Reverse())
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Select(i => i.GetType())),
                    GetGenericMethodInfoFromExpressionBody(() => q.Select(i => i.GetType()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Select((i, p) => i.GetHashCode() % p)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Select((i, p) => i.GetHashCode() % p))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.SelectMany(i => new int[] { i.GetHashCode() })),
                    GetGenericMethodInfoFromExpressionBody(() => q.SelectMany(i => new int[] { i.GetHashCode() }))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.SelectMany((i, p) => new int[] { i.GetHashCode(), p })),
                    GetGenericMethodInfoFromExpressionBody(() => q.SelectMany((i, p) => new int[] { i.GetHashCode(), p }))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.SelectMany(i => new int[] { i.GetHashCode() }, (i, c) => i.GetHashCode() + c)),
                    GetGenericMethodInfoFromExpressionBody(() => q.SelectMany(i => new int[] { i.GetHashCode() }, (i, c) => i.GetHashCode() + c))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.SelectMany((i, p) => new int[] { i.GetHashCode(), p }, (i, c) => i.GetHashCode() + c)),
                    GetGenericMethodInfoFromExpressionBody(() => q.SelectMany((i, p) => new int[] { i.GetHashCode(), p }, (i, c) => i.GetHashCode() + c))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.SequenceEqual(dummyEnumerable)),
                    GetGenericMethodInfoFromExpressionBody(() => q.SequenceEqual(dummyEnumerable))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.SequenceEqual(dummyEnumerable, EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => q.SequenceEqual(dummyEnumerable, EqualityComparer<object>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Single()),
                    GetGenericMethodInfoFromExpressionBody(() => q.Single())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Single(i => true)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Single(i => true))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.SingleOrDefault()),
                    GetGenericMethodInfoFromExpressionBody(() => q.SingleOrDefault())
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.SingleOrDefault(i => true)),
                    GetGenericMethodInfoFromExpressionBody(() => q.SingleOrDefault(i => true))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Skip(1)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Skip(1))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.SkipWhile(i => false)),
                    GetGenericMethodInfoFromExpressionBody(() => q.SkipWhile(i => false))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.SkipWhile((i, p) => false)),
                    GetGenericMethodInfoFromExpressionBody(() => q.SkipWhile((i, p) => false))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Sum(eDecimal)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Sum(qDecimal))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Sum(eDecimalN)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Sum(qDecimalN))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Sum(eDouble)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Sum(qDouble))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Sum(eDoubleN)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Sum(qDoubleN))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Sum(eFloat)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Sum(qFloat))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Sum(eFloatN)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Sum(qFloatN))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Sum(eInt)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Sum(qInt))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Sum(eIntN)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Sum(qIntN))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Sum(eLong)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Sum(qLong))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Enumerable.Sum(eLongN)),
                    GetGenericMethodInfoFromExpressionBody(() => System.Linq.Queryable.Sum(qLongN))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Sum(i => Convert.ToDecimal(i))),
                    GetGenericMethodInfoFromExpressionBody(() => q.Sum(i => Convert.ToDecimal(i)))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Sum(i => i.GetHashCode() > 0 ? new Nullable<decimal>(Convert.ToDecimal(i)) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Sum(i => i.GetHashCode() > 0 ? new Nullable<decimal>(Convert.ToDecimal(i)) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Sum(i => i.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => q.Sum(i => i.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Sum(i => i.GetHashCode() > 0 ? new Nullable<int>(i.GetHashCode()) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Sum(i => i.GetHashCode() > 0 ? new Nullable<int>(i.GetHashCode()) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Sum(i => Convert.ToInt64(i))),
                    GetGenericMethodInfoFromExpressionBody(() => q.Sum(i => Convert.ToInt64(i)))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Sum(i => i.GetHashCode() > 0 ? new Nullable<long>(Convert.ToInt64(i)) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Sum(i => i.GetHashCode() > 0 ? new Nullable<long>(Convert.ToInt64(i)) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Sum(i => Convert.ToDouble(i))),
                    GetGenericMethodInfoFromExpressionBody(() => q.Sum(i => Convert.ToDouble(i)))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Sum(i => i.GetHashCode() > 0 ? new Nullable<double>(Convert.ToDouble(i)) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Sum(i => i.GetHashCode() > 0 ? new Nullable<double>(Convert.ToDouble(i)) : null))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Sum(i => (float)i)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Sum(i => (float)i))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Sum(i => i.GetHashCode() > 0 ? new Nullable<float>((float)i) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Sum(i => i.GetHashCode() > 0 ? new Nullable<float>((float)i) : null))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Take(1)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Take(1))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.TakeWhile(i => true)),
                    GetGenericMethodInfoFromExpressionBody(() => q.TakeWhile(i => true))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.TakeWhile((i, p) => true)),
                    GetGenericMethodInfoFromExpressionBody(() => q.TakeWhile((i, p) => true))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Union(dummyEnumerable)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Union(dummyEnumerable))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Union(dummyEnumerable, EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Union(dummyEnumerable, EqualityComparer<object>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Where(i => true)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Where(i => true))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Where((i, p) => true)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Where((i, p) => true))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => e.Zip(dummyEnumerable, (i, p) => true)),
                    GetGenericMethodInfoFromExpressionBody(() => q.Zip(dummyEnumerable, (i, p) => true))
                    );
            }

            /// <summary>
            /// Returns the semantic equivalent of <paramref name="enumerableMethod"/> in the <see cref="Queryable"/> class.
            /// </summary>
            /// <param name="enumerableMethod">Enumerable method to translate.</param>
            /// <returns>The Queryable's equivalent of <paramref name="enumerableMethod"/>.</returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="enumerableMethod"/> is null.</exception>
            /// <exception cref="ArgumentException">Thrown when <paramref name="enumerableMethod"/> doesn't belong to <see cref="System.Linq.Enumerable"/>.</exception>
            /// <exception cref="KeyNotFoundException">The <paramref name="enumerableMethod"/> has no equivalent in <see cref="System.Linq.Queryable"/>.</exception>
            public static MethodInfo GetQueryableEquivalent(MethodInfo enumerableMethod)
            {
                if (null == enumerableMethod)
                    throw new ArgumentNullException("enumerableMethod");
                if (!enumerableMethod.DeclaringType.Equals(typeof(System.Linq.Enumerable)))
                    throw new ArgumentException("The queryableMethod must belong to System.Linq.Enumerable.", "enumerableMethod");

                MethodInfo method = null;

                if (enumerableMethod.IsGenericMethod)
                {
                    Type[] enumerableTypeArgs = enumerableMethod.GetGenericArguments();
                    MethodInfo gerericEnumerableMethod = enumerableMethod.GetGenericMethodDefinition();
                    if(_methodsTranslations.TryGetValue(gerericEnumerableMethod, out method))
                    {
                        Type[] queryableTypeArgs = method.GetGenericArguments();

                        if(enumerableTypeArgs.Length == queryableTypeArgs.Length)
                        {
                            return method.MakeGenericMethod(enumerableTypeArgs);
                        }
                        else
                        {
                            // Some Enumerable's method (non generic Max and Min) have no direct equivalent in Queryable.
                            // Queryable only has generic version with different type arguments, where Enumerable
                            // has also vertical versions for common numerical structs (int, double, decimal...)
                            // and their nullable fashion.
                            List<Type> adjustedArguments = new List<Type>(enumerableTypeArgs);
                            adjustedArguments.Add(enumerableMethod.ReturnType);
                            return method.MakeGenericMethod(adjustedArguments.ToArray());
                        }
                    }
                }
                else if(_methodsTranslations.TryGetValue(enumerableMethod, out method))
                {
                    if(method.IsGenericMethodDefinition)
                        return method.MakeGenericMethod(enumerableMethod.ReturnType);
                    return method;
                }

                throw new KeyNotFoundException(string.Format("No equivalent of {0} is available in {1}.", enumerableMethod.ToString(), typeof(Queryable).FullName));
            }
        }
	}
}

