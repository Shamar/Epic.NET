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
using ExprType = System.Linq.Expressions.ExpressionType;
using System.Linq.Expressions;
using System.Collections.Generic;
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
        
        public static bool CanBeCompiled(Expression expression, params ParameterExpression[] availableParameters)
        {
            switch(expression.NodeType)
            {
                case ExprType.ArrayLength:
                case ExprType.Convert:
                case ExprType.ConvertChecked:
                case ExprType.Negate:
                case ExprType.NegateChecked:
                case ExprType.Not:
                case ExprType.Quote:
                case ExprType.TypeAs:
                case ExprType.UnaryPlus:
                    return CanBeCompiled((expression as UnaryExpression).Operand, availableParameters);
                case ExprType.Add:
                case ExprType.AddChecked:
                case ExprType.Divide:
                case ExprType.Modulo:
                case ExprType.Multiply:
                case ExprType.MultiplyChecked:
                case ExprType.Power:
                case ExprType.Subtract:
                case ExprType.SubtractChecked:
                case ExprType.And:
                case ExprType.Or:
                case ExprType.ExclusiveOr:
                case ExprType.LeftShift:
                case ExprType.RightShift:
                case ExprType.AndAlso:
                case ExprType.OrElse:
                case ExprType.Equal:
                case ExprType.NotEqual:
                case ExprType.GreaterThanOrEqual:
                case ExprType.GreaterThan:
                case ExprType.LessThan:
                case ExprType.LessThanOrEqual:
                case ExprType.Coalesce:
                case ExprType.ArrayIndex:
                    BinaryExpression binaryExp = expression as BinaryExpression;
                    return CanBeCompiled(binaryExp.Left, availableParameters) && CanBeCompiled(binaryExp.Right, availableParameters);
                case ExprType.Conditional:
                    ConditionalExpression conditionalExp = expression as ConditionalExpression;
                    return CanBeCompiled(conditionalExp.Test, availableParameters) && CanBeCompiled(conditionalExp.IfTrue, availableParameters) && CanBeCompiled(conditionalExp.IfFalse, availableParameters);
                case ExprType.Constant:
                    return true;
                case ExprType.Invoke:
                    InvocationExpression invocationExp = expression as InvocationExpression;
                    foreach (Expression arg in invocationExp.Arguments)
                    {
                        if (!CanBeCompiled(arg, availableParameters))
                            return false;
                    }
                    return CanBeCompiled(invocationExp.Expression, availableParameters);
                case ExprType.Lambda:
                    LambdaExpression lambdaExp = expression as LambdaExpression;
                    if(lambdaExp.Parameters.Count > 0 || (null != availableParameters && availableParameters.Length > 0))
                    {
                        List<ParameterExpression> parameters = new List<ParameterExpression>(lambdaExp.Parameters);
                        if(null != availableParameters && availableParameters.Length > 0)
                        {
                            parameters.AddRange(availableParameters);
                        }
                        return CanBeCompiled(lambdaExp.Body, parameters.ToArray());
                    }
                    else
                    {
                        return CanBeCompiled(lambdaExp.Body, availableParameters);
                    }
                case ExprType.MemberAccess:
                    return CanBeCompiled((expression as MemberExpression).Expression, availableParameters);
                case ExprType.Call:
                    MethodCallExpression callExp = expression as MethodCallExpression;
                    foreach (Expression arg in callExp.Arguments)
                    {
                        if (!CanBeCompiled(arg, availableParameters))
                            return false;
                    }
                    return null == callExp.Object || CanBeCompiled(callExp.Object, availableParameters);
                case ExprType.New:
                    NewExpression newExp = expression as NewExpression;
                    foreach (Expression arg in newExp.Arguments)
                    {
                        if (!CanBeCompiled(arg, availableParameters))
                            return false;
                    }
                    return true;
                case ExprType.NewArrayBounds:
                case ExprType.NewArrayInit:
                    NewArrayExpression newArrExp = expression as NewArrayExpression;
                    foreach (Expression arg in newArrExp.Expressions)
                    {
                        if (!CanBeCompiled(arg, availableParameters))
                            return false;
                    }
                    return true;
                case ExprType.MemberInit:
                    MemberInitExpression initExp = expression as MemberInitExpression;
                    foreach (MemberBinding binding in initExp.Bindings)
                    {
                        switch (binding.BindingType)
                        {
                            case MemberBindingType.Assignment:
                                MemberAssignment assign = binding as MemberAssignment;
                                if (!CanBeCompiled(assign.Expression, availableParameters))
                                    return false;
                                break;
                            case MemberBindingType.MemberBinding:
                                MemberMemberBinding member = binding as MemberMemberBinding;
                                // TODO : complete
                                return false;
                            default:
                                MemberListBinding list = binding as MemberListBinding;
                                if(!list.Initializers.All(i => i.Arguments.All(e => CanBeCompiled(e, availableParameters))))
                                    return false;
                                break;
                        }
                    }
                    return CanBeCompiled(initExp.NewExpression, availableParameters);
                case ExprType.ListInit:
                    ListInitExpression listExp = expression as ListInitExpression;
                    return CanBeCompiled(listExp.NewExpression) && listExp.Initializers.All(i => i.Arguments.All(e => CanBeCompiled(e, availableParameters)));
                case ExprType.Parameter:
                    if(null != availableParameters)
                    {
                        for(int i = 0; i < availableParameters.Length; ++i)
                            if(expression == availableParameters[i])
                                return true;
                    }
                    return false;
                case ExprType.TypeIs:
                    return CanBeCompiled((expression as TypeBinaryExpression).Expression, availableParameters);
                default:
                    return false;
            }
        }
	
        public static class Queryable
        {
            private readonly static Dictionary<MethodInfo, MethodInfo> _methodsTranslations;
            static Queryable()
            {
                _methodsTranslations = new Dictionary<MethodInfo, MethodInfo>();
                Type enumerableType = typeof(System.Linq.Enumerable);
                Type queryableType = typeof(System.Linq.Queryable);
                
                MethodInfo[] enumerablePublicMethods = enumerableType.GetMethods(BindingFlags.Public|BindingFlags.Static|BindingFlags.DeclaredOnly);
                MethodInfo[] queryablePublicMethods = queryableType.GetMethods(BindingFlags.Public|BindingFlags.Static|BindingFlags.DeclaredOnly);
                
                for(int i = 0; i < queryablePublicMethods.Length; ++i)
                {
                    MethodInfo enumerableMethod = queryablePublicMethods[i];
                    for(int j = 0; j < enumerablePublicMethods.Length; ++j)
                    {
                        MethodInfo queryableMethod = enumerablePublicMethods[j];
                        ParameterInfo[] enumerableParameters = enumerableMethod.GetParameters();
                        ParameterInfo[] queryableParameters = queryableMethod.GetParameters();
                        if(enumerableParameters.Length == queryableParameters.Length && enumerableMethod.Name.Equals(queryableMethod.Name))
                        {
                            bool parametersMatches = true;
                            for(int p = 0; p < enumerableParameters.Length && parametersMatches; ++p)
                            {
                                Type ePType = enumerableParameters[p].ParameterType;
                                Type qPType = queryableParameters[p].ParameterType;
                                Type expectedQPType = typeof(Expression<>).MakeGenericType(ePType);
                                string qPTypeName = qPType.FullName;
                                string expectedQPTypeName = expectedQPType.FullName;
                                if(p > 0 && !(ePType.Name.Equals(qPType.Name) || qPTypeName == expectedQPTypeName))
                                {
                                    parametersMatches = false;
                                }
                            }
                            if(parametersMatches)
                            {
                                _methodsTranslations[queryableMethod] = enumerableMethod;
                            }
                        }
                    }
                }
            }
            
            public static MethodInfo GetEnumerableEquivalent(MethodInfo queryableMethod)
            {
                MethodInfo method = null;
                try
                {
                    method = _methodsTranslations[queryableMethod.GetGenericMethodDefinition()];
                }
                catch(KeyNotFoundException e)
                {
                    throw new KeyNotFoundException(string.Format("Can not find the Queryable equivalent of the Enumerable's {0} method.", queryableMethod.Name), e);
                }
                return method.MakeGenericMethod(queryableMethod.GetGenericArguments());
            }
        }
        
        public static class Enumerable
        {
            private readonly static Dictionary<MethodInfo, MethodInfo> _methodsTranslations;
            static Enumerable()
            {
                _methodsTranslations = new Dictionary<MethodInfo, MethodInfo>();
                Type enumerableType = typeof(System.Linq.Enumerable);
                Type queryableType = typeof(System.Linq.Queryable);
                
                MethodInfo[] enumerablePublicMethods = enumerableType.GetMethods(BindingFlags.Public|BindingFlags.Static|BindingFlags.DeclaredOnly);
                MethodInfo[] queryablePublicMethods = queryableType.GetMethods(BindingFlags.Public|BindingFlags.Static|BindingFlags.DeclaredOnly);
                
                for(int i = 0; i < enumerablePublicMethods.Length; ++i)
                {
                    MethodInfo enumerableMethod = enumerablePublicMethods[i];
                    for(int j = 0; j < queryablePublicMethods.Length; ++j)
                    {
                        MethodInfo queryableMethod = queryablePublicMethods[j];
                        ParameterInfo[] enumerableParameters = enumerableMethod.GetParameters();
                        ParameterInfo[] queryableParameters = queryableMethod.GetParameters();
                        if(enumerableParameters.Length == queryableParameters.Length && enumerableMethod.Name.Equals(queryableMethod.Name))
                        {
                            bool parametersMatches = true;
                            for(int p = 0; p < enumerableParameters.Length && parametersMatches; ++p)
                            {
                                Type ePType = enumerableParameters[p].ParameterType;
                                Type qPType = queryableParameters[p].ParameterType;
                                Type expectedQPType = typeof(Expression<>).MakeGenericType(ePType);
                                string qPTypeName = qPType.FullName;
                                string expectedQPTypeName = expectedQPType.FullName;
                                if(p > 0 && !(ePType.Name.Equals(qPType.Name) || qPTypeName == expectedQPTypeName))
                                {
                                    parametersMatches = false;
                                }
                            }
                            if(parametersMatches)
                            {
                                _methodsTranslations[enumerableMethod] = queryableMethod;
                            }
                        }
                    }
                }
            }
            
            public static MethodInfo GetQueryableEquivalent(MethodInfo enumerableMethod)
            {
                MethodInfo method = null;
                try
                {
                    method = _methodsTranslations[enumerableMethod.GetGenericMethodDefinition()];
                }
                catch(KeyNotFoundException e)
                {
                    throw new KeyNotFoundException(string.Format("Can not find the Queryable equivalent of the Enumerable's {0} method.", enumerableMethod.Name), e);
                }
                return method.MakeGenericMethod(enumerableMethod.GetGenericArguments());
            }
        }
    }
    
    
}

