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
                Type enumerableType = typeof(System.Linq.Enumerable);
                Type queryableType = typeof(System.Linq.Queryable);

                MethodInfo[] enumerablePublicMethods = enumerableType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
                MethodInfo[] queryablePublicMethods = queryableType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

                Type funcType = typeof(Func<>);

                for (int i = 0; i < queryablePublicMethods.Length; ++i)
                {
                    MethodInfo enumerableMethod = queryablePublicMethods[i];
                    for (int j = 0; j < enumerablePublicMethods.Length; ++j)
                    {
                        MethodInfo queryableMethod = enumerablePublicMethods[j];
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
                                if (ePType.IsGenericType && funcType.Equals(ePType.GetGenericTypeDefinition()))
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
                }
            }

            /// <summary>
            /// Returns the semantic equivalent of <paramref name="queryableMethod"/> in the <see cref="Enumerable"/> class.
            /// </summary>
            /// <param name="queryableMethod">Queryable method to translate.</param>
            /// <returns>The Enumerable's equivalent of <paramref name="queryableMethod"/>.</returns>
            /// <exception cref="KeyNotFoundException">The <paramref name="queryableMethod"/> has no equivalent in <see cref="Enumerable"/>.</exception>
            public static MethodInfo GetEnumerableEquivalent(MethodInfo queryableMethod)
            {
                MethodInfo method = null;
                try
                {
                    method = _methodsTranslations[queryableMethod.GetGenericMethodDefinition()];
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

