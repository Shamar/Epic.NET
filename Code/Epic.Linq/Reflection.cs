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
            static Queryable()
            {
                _methodsTranslations = new Dictionary<MethodInfo, MethodInfo>();
                IQueryable<int> q = null;
                IEnumerable<int> e = null;
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

                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Aggregate(0, (i, acc) => i + acc)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Aggregate(0, (i, acc) => i + acc))
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Aggregate((i, acc) => i + acc)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Aggregate((i, acc) => i + acc))
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Aggregate(0, (i, acc) => i + acc, acc => acc)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Aggregate(0, (i, acc) => i + acc, acc => acc))
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.All(i => i > 0)),
                    GetGenericMethodInfoFromExpressionBody(() => e.All(i => i > 0))
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Any()),
                    GetGenericMethodInfoFromExpressionBody(() => e.Any())
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Any(i => i > 0)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Any(i => i > 0))
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => qInt.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => eInt.Average())
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => qIntN.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => eIntN.Average())
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => qDecimal.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => eDecimal.Average())
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => qDecimalN.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => eDecimalN.Average())
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => qLong.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => eLong.Average())
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => qLongN.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => eLongN.Average())
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => qDouble.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => eDouble.Average())
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => qDoubleN.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => eDoubleN.Average())
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => qFloat.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => eFloat.Average())
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => qFloatN.Average()),
                    GetGenericMethodInfoFromExpressionBody(() => eFloatN.Average())
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => Convert.ToDecimal(i))),
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => Convert.ToDecimal(i)))
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => i > 0 ? new Nullable<decimal>(Convert.ToDecimal(i)) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => i > 0 ? new Nullable<decimal>(Convert.ToDecimal(i)) : null))
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => i)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => i))
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => i > 0 ? new Nullable<int>(i) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => i > 0 ? new Nullable<int>(i) : null))
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => Convert.ToInt64(i))),
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => Convert.ToInt64(i)))
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => i > 0 ? new Nullable<long>(Convert.ToInt64(i)) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => i > 0 ? new Nullable<long>(Convert.ToInt64(i)) : null))
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => Convert.ToDouble(i))),
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => Convert.ToDouble(i)))
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => i > 0 ? new Nullable<double>(Convert.ToDouble(i)) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => i > 0 ? new Nullable<double>(Convert.ToDouble(i)) : null))
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => (float)i)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => (float)i))
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Average(i => i > 0 ? new Nullable<float>((float)i) : null)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Average(i => i > 0 ? new Nullable<float>((float)i) : null))
                    );

                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Cast<decimal>()),
                    GetGenericMethodInfoFromExpressionBody(() => e.Cast<decimal>())
                    );

                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Concat(null)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Concat(null))
                    );

                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Contains(1)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Contains(1))
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Contains(1, EqualityComparer<int>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Contains(1, EqualityComparer<int>.Default))
                    );

                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Count()),
                    GetGenericMethodInfoFromExpressionBody(() => e.Count())
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Count(i => i > 0)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Count(i => i > 0))
                    );

                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.DefaultIfEmpty()),
                    GetGenericMethodInfoFromExpressionBody(() => e.DefaultIfEmpty())
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.DefaultIfEmpty(5)),
                    GetGenericMethodInfoFromExpressionBody(() => e.DefaultIfEmpty(5))
                    );

                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Distinct()),
                    GetGenericMethodInfoFromExpressionBody(() => e.Distinct())
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Distinct(EqualityComparer<int>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Distinct(EqualityComparer<int>.Default))
                    );

                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.ElementAt(1)),
                    GetGenericMethodInfoFromExpressionBody(() => e.ElementAt(1))
                    );

                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.ElementAtOrDefault(1)),
                    GetGenericMethodInfoFromExpressionBody(() => e.ElementAtOrDefault(1))
                    );

                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Except(new int[0])),
                    GetGenericMethodInfoFromExpressionBody(() => e.Except(new int[0]))
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Except(new int[0], EqualityComparer<int>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Except(new int[0], EqualityComparer<int>.Default))
                    );

                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.First()),
                    GetGenericMethodInfoFromExpressionBody(() => e.First())
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.First(i => i > 0)),
                    GetGenericMethodInfoFromExpressionBody(() => e.First(i => i > 0))
                    );

                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.FirstOrDefault()),
                    GetGenericMethodInfoFromExpressionBody(() => e.FirstOrDefault())
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.FirstOrDefault(i => i > 0)),
                    GetGenericMethodInfoFromExpressionBody(() => e.FirstOrDefault(i => i > 0))
                    );

                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupBy(i => i % 2)),
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupBy(i => i % 2))
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupBy(i => i % 2, i => i.ToString())),
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupBy(i => i % 2, i => i.ToString()))
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupBy(i => i % 2, i => i.ToString(), (k, g) => g.Count())),
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupBy(i => i % 2, i => i.ToString(), (k, g) => g.Count()))
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupBy(i => i % 2, i => i.ToString(), (k, g) => g.Count(), EqualityComparer<int>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupBy(i => i % 2, i => i.ToString(), (k, g) => g.Count(), EqualityComparer<int>.Default))
                    );

                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupJoin(new int[0], i => i, j => j, (k, g) => g.Count())),
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupJoin(new int[0], i => i, j => j, (k, g) => g.Count()))
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.GroupJoin(new int[0], i => i, j => j, (k, g) => g.Count(), EqualityComparer<int>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.GroupJoin(new int[0], i => i, j => j, (k, g) => g.Count(), EqualityComparer<int>.Default))
                    );

                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Intersect(new int[0])),
                    GetGenericMethodInfoFromExpressionBody(() => e.Intersect(new int[0]))
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Intersect(new int[0], EqualityComparer<int>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Intersect(new int[0], EqualityComparer<int>.Default))
                    );

                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Join(new int[0], i => i, j => j, (k, g) => k + g)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Join(new int[0], i => i, j => j, (k, g) => k + g))
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Join(new int[0], i => i, j => j, (k, g) => k + g, EqualityComparer<int>.Default)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Join(new int[0], i => i, j => j, (k, g) => k + g, EqualityComparer<int>.Default))
                    );

                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Last()),
                    GetGenericMethodInfoFromExpressionBody(() => e.Last())
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Last(i => i > 0)),
                    GetGenericMethodInfoFromExpressionBody(() => e.Last(i => i > 0))
                    );

                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.LastOrDefault()),
                    GetGenericMethodInfoFromExpressionBody(() => e.LastOrDefault())
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.LastOrDefault(i => i > 0)),
                    GetGenericMethodInfoFromExpressionBody(() => e.LastOrDefault(i => i > 0))
                    );

                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.LongCount()),
                    GetGenericMethodInfoFromExpressionBody(() => e.LongCount())
                    );
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.LongCount(i => i > 0)),
                    GetGenericMethodInfoFromExpressionBody(() => e.LongCount(i => i > 0))
                    );

                // TODO: how to handle Max? There exists more signatures in Enumerable (for int, float etc) and less
                //       for Queryable (just TSource), but also Enumerable Have a TSource version.
                _methodsTranslations.Add(
                    GetGenericMethodInfoFromExpressionBody(() => q.Max()),
                    GetGenericMethodInfoFromExpressionBody(() => e.Max())
                    );

                /*
                Type enumerableType = typeof(System.Linq.Enumerable);
                Type queryableType = typeof(System.Linq.Queryable);

                MethodInfo[] enumerablePublicMethods = enumerableType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
                MethodInfo[] queryablePublicMethods = queryableType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

                Type[] funcTypes = new Type[] { typeof(Func<,>), typeof(Func<,,>), typeof(Func<,,,>) };

                for (int i = 0; i < queryablePublicMethods.Length; ++i)
                {
                    MethodInfo queryableMethod= queryablePublicMethods[i];
                    for (int j = 0; j < enumerablePublicMethods.Length; ++j)
                    {
                        MethodInfo enumerableMethod = enumerablePublicMethods[j];
                        ParameterInfo[] enumerableParameters = enumerableMethod.GetParameters();
                        ParameterInfo[] queryableParameters = queryableMethod.GetParameters();
                        if (enumerableParameters.Length == queryableParameters.Length && enumerableMethod.Name.Equals(queryableMethod.Name))
                        {
                            bool parametersMatches = true;
                            for (int p = 0; p < enumerableParameters.Length && parametersMatches; ++p)
                            {
                                Type ePType = enumerableParameters[p].ParameterType;
                                Type qPType = queryableParameters[p].ParameterType;
                                Type expectedQPType = ePType;
                                if (ePType.IsGenericType && funcTypes.Contains(ePType.GetGenericTypeDefinition()))
                                {
                                    expectedQPType = typeof(Expression<>).MakeGenericType(ePType);
                                }
                                string qPTypeName = qPType.FullName;
                                string expectedQPTypeName = expectedQPType.FullName;
                                if (p > 0 && !(ePType.Name.Equals(qPType.Name) || qPTypeName == expectedQPTypeName))
                                {
                                    parametersMatches = false;
                                }
                            }
                            if (parametersMatches)
                            {
                                _methodsTranslations[queryableMethod] = enumerableMethod;
                            }
                        }
                    }
                }*/
            }

            /// <summary>
            /// Returns the semantic equivalent of <paramref name="queryableMethod"/> in the <see cref="Enumerable"/> class.
            /// </summary>
            /// <param name="queryableMethod">Queryable method to translate.</param>
            /// <returns>The Enumerable's equivalent of <paramref name="queryableMethod"/>.</returns>
            /// <exception cref="KeyNotFoundException">The <paramref name="queryableMethod"/> has no equivalent in <see cref="Enumerable"/>.</exception>
            public static MethodInfo GetEnumerableEquivalent(MethodInfo queryableMethod)
            {
                MethodInfo genericQueryableMethodDefinition = queryableMethod.GetGenericMethodDefinition();
                MethodInfo method = null;
                try
                {
                    method = _methodsTranslations[genericQueryableMethodDefinition];
                }
                catch (KeyNotFoundException e)
                {
                    throw new KeyNotFoundException(string.Format("Can not find the Queryable equivalent of the Enumerable's {0} method.", queryableMethod.Name), e);
                }
                return method.MakeGenericMethod(queryableMethod.GetGenericArguments());
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

