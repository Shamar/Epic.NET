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
using System.Runtime.CompilerServices;

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
        
        public static bool IsCompilerGenerated(Type type)
        {
            return type.IsSealed
               && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic
               && Attribute.IsDefined(type, typeof (CompilerGeneratedAttribute), false);
        }
        
        public static bool IsAnonymous(Type type)
        {
            return type.IsGenericType && IsCompilerGenerated(type) && type.Name.StartsWith("<>"); // No need to work with VB
        }
	}
}

