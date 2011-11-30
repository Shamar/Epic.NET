//  
//  Reflection.cs
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
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace Epic.Linq
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
                    GetGenericMethodInfoFromExpressionBody(() => q.Except(new object[0])),
                    GetGenericMethodInfoFromExpressionBody(() => e.Except(new object[0]))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Except(new object[0], EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Except(new object[0], EqualityComparer<object>.Default))
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
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupBy(i => i.GetType(), i => i.ToString(), (k, g) => g.Count())),
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupBy(i => i.GetType(), i => i.ToString(), (k, g) => g.Count()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupBy(i => i.GetType(), i => i.ToString(), (k, g) => g.Count(), EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupBy(i => i.GetType(), i => i.ToString(), (k, g) => g.Count(), EqualityComparer<object>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupJoin(new object[0], i => i, j => j, (k, g) => g.Count())),
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupJoin(new object[0], i => i, j => j, (k, g) => g.Count()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupJoin(new object[0], i => i, j => j, (k, g) => g.Count(), EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupJoin(new object[0], i => i, j => j, (k, g) => g.Count(), EqualityComparer<object>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Intersect(new object[0])),
                    GetGenericMethodInfoFromExpressionBody(() => e.Intersect(new object[0]))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Intersect(new object[0], EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Intersect(new object[0], EqualityComparer<object>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Join(new object[0], i => i, j => j, (k, g) => k.GetHashCode() + g.GetHashCode())),
                    GetGenericMethodInfoFromExpressionBody(() => e.Join(new object[0], i => i, j => j, (k, g) => k.GetHashCode() + g.GetHashCode()))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Join(new object[0], i => i, j => j, (k, g) => k.GetHashCode() + g.GetHashCode(), EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Join(new object[0], i => i, j => j, (k, g) => k.GetHashCode() + g.GetHashCode(), EqualityComparer<object>.Default))
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
                    GetGenericMethodInfoFromExpressionBody(() => q.SequenceEqual(new object[0])),
                    GetGenericMethodInfoFromExpressionBody(() => e.SequenceEqual(new object[0]))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.SequenceEqual(new object[0], EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.SequenceEqual(new object[0], EqualityComparer<object>.Default))
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
                    GetGenericMethodInfoFromExpressionBody(() => q.Union(new object[0])),
                    GetGenericMethodInfoFromExpressionBody(() => e.Union(new object[0]))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Union(new object[0], EqualityComparer<object>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Union(new object[0], EqualityComparer<object>.Default))
                    );

                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Where(i => true)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Where(i => true))
                    );
                AddTranslation(
                    GetGenericMethodInfoFromExpressionBody(() => q.Where((i, p) => true)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Where((i, p) => true))
                    );
            }

            /// <summary>
            /// Returns the semantic equivalent of <paramref name="queryableMethod"/> in the <see cref="Enumerable"/> class.
            /// </summary>
            /// <param name="queryableMethod">Queryable method to translate.</param>
            /// <returns>The Enumerable's equivalent of <paramref name="queryableMethod"/>.</returns>
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
        /// Queryable related utilities.
        /// </summary>
        internal static class Enumerable
        {
            /// <summary>
            /// Translations between methods of Enumerable and of Queryable.
            /// </summary>
            private readonly static Dictionary<MethodInfo, MethodInfo> _methodsTranslations;
            static Enumerable()
            {
                _methodsTranslations = new Dictionary<MethodInfo, MethodInfo>();
                Type enumerableType = typeof(System.Linq.Enumerable);
                Type queryableType = typeof(System.Linq.Queryable);

                MethodInfo[] enumerablePublicMethods = enumerableType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
                MethodInfo[] queryablePublicMethods = queryableType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

                for (int i = 0; i < enumerablePublicMethods.Length; ++i)
                {
                    MethodInfo enumerableMethod = enumerablePublicMethods[i];
                    for (int j = 0; j < queryablePublicMethods.Length; ++j)
                    {
                        MethodInfo queryableMethod = queryablePublicMethods[j];
                        ParameterInfo[] enumerableParameters = enumerableMethod.GetParameters();
                        ParameterInfo[] queryableParameters = queryableMethod.GetParameters();
                        if (enumerableParameters.Length == queryableParameters.Length && enumerableMethod.Name.Equals(queryableMethod.Name))
                        {
                            bool parametersMatches = true;
                            for (int p = 0; p < enumerableParameters.Length && parametersMatches; ++p)
                            {
                                Type ePType = enumerableParameters[p].ParameterType;
                                Type qPType = queryableParameters[p].ParameterType;
                                Type expectedQPType = typeof(Expression<>).MakeGenericType(ePType);
                                string qPTypeName = qPType.FullName;
                                string expectedQPTypeName = expectedQPType.FullName;
                                if (p > 0 && !(ePType.Name.Equals(qPType.Name) || qPTypeName == expectedQPTypeName))
                                {
                                    parametersMatches = false;
                                }
                            }
                            if (parametersMatches)
                            {
                                _methodsTranslations[enumerableMethod] = queryableMethod;
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Returns the semantic equivalent of <paramref name="enumerableMethod"/> in the <see cref="Queryable"/> class.
            /// </summary>
            /// <param name="queryableMethod">Enumerable method to translate.</param>
            /// <returns>The Queryable's equivalent of <paramref name="queryableMethod"/>.</returns>
            /// <exception cref="KeyNotFoundException">The <paramref name="enumerableMethod"/> has no equivalent in <see cref="Queryable"/>.</exception>
            public static MethodInfo GetQueryableEquivalent(MethodInfo enumerableMethod)
            {
                MethodInfo method = null;
                try
                {
                    method = _methodsTranslations[enumerableMethod.GetGenericMethodDefinition()];
                }
                catch (KeyNotFoundException e)
                {
                    throw new KeyNotFoundException(string.Format("Can not find the Queryable equivalent of the Enumerable's {0} method.", enumerableMethod.Name), e);
                }
                return method.MakeGenericMethod(enumerableMethod.GetGenericArguments());
            }
        }
	}
}

