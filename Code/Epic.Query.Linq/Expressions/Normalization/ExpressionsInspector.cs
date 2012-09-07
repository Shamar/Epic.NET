//  
//  ExpressionsInspector.cs
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
using System.Reflection;
using System.Collections.ObjectModel;

namespace Epic.Query.Linq.Expressions.Normalization
{
    /// <summary>
    /// Visit the members of expressions derived from <see cref="System.Linq.Expressions.Expression"/> 
    /// and merge them in a new expression of the same type.
    /// </summary>
    /// <exception cref='InvalidOperationException'>
    /// Is thrown when an operation cannot be performed.
    /// </exception>
    /// <exception cref='NotSupportedException'>
    /// Is thrown when an object cannot perform an operation.
    /// </exception>
    internal sealed class ExpressionsInspector : CompositeVisitor<Expression>.VisitorBase, 
        IVisitor<Expression, UnaryExpression>, 
        IVisitor<Expression, BinaryExpression>, 
        IVisitor<Expression, ConditionalExpression>,
        IVisitor<Expression, InvocationExpression>,
        IVisitor<Expression, LambdaExpression>,
        IVisitor<Expression, MemberExpression>,
        IVisitor<Expression, MethodCallExpression>,
        IVisitor<Expression, NewExpression>,
        IVisitor<Expression, NewArrayExpression>,
        IVisitor<Expression, MemberInitExpression>,
        IVisitor<Expression, ListInitExpression>,
        IVisitor<Expression, TypeBinaryExpression>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Epic.Query.Linq.Expressions.Normalization.ExpressionsInspector"/> class.
        /// </summary>
        /// <param name='composition'>
        /// Composition that owns this visitor.
        /// </param>
        public ExpressionsInspector (CompositeVisitor<Expression> composition)
            : base(composition)
        {
        }
    
        /// <summary>
        /// Visit the specified <see cref="UnaryExpression"/>.
        /// </summary>
        /// <param name='expression'>
        /// Expression.
        /// </param>
        /// <param name='context'>
        /// context.
        /// </param>
        public Expression Visit (UnaryExpression expression, IVisitContext context)
        {
            Expression newOperand = VisitExpression (expression.Operand, context);
            if (newOperand != expression.Operand) {
                if (expression.NodeType == ExpressionType.UnaryPlus)
                    return Expression.UnaryPlus (newOperand, expression.Method);
                else
                    return Expression.MakeUnary (expression.NodeType, newOperand, expression.Type, expression.Method);
            } else
                return expression;
        }
  
        /// <summary>
        /// Visit the specified <see cref="BinaryExpression"/>.
        /// </summary>
        /// <param name='expression'>
        /// Expression.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        public Expression Visit (BinaryExpression expression, IVisitContext context)
        {
            Expression newLeft = VisitExpression (expression.Left, context);
            Expression newRight = VisitExpression (expression.Right, context);
            var newConversion = (LambdaExpression)VisitExpression (expression.Conversion, context);
            if (newLeft != expression.Left || newRight != expression.Right || newConversion != expression.Conversion)
                return Expression.MakeBinary (expression.NodeType, newLeft, newRight, expression.IsLiftedToNull, expression.Method, newConversion);
            return expression;
        }
    
        /// <summary>
        /// Visit the specified <see cref="TypeBinaryExpression"/>.
        /// </summary>
        /// <param name='expression'>
        /// Expression.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        public Expression Visit (TypeBinaryExpression expression, IVisitContext context)
        {
            Expression newExpression = VisitExpression (expression.Expression, context);
            if (newExpression != expression.Expression)
                return Expression.TypeIs (newExpression, expression.TypeOperand);
            return expression;
        }
    
        /// <summary>
        /// Visit the specified <see cref="ConditionalExpression"/>.
        /// </summary>
        /// <param name='expression'>
        /// Expression.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        public Expression Visit (ConditionalExpression expression, IVisitContext context)
        {
            Expression newTest = VisitExpression (expression.Test, context);
            Expression newFalse = VisitExpression (expression.IfFalse, context);
            Expression newTrue = VisitExpression (expression.IfTrue, context);
            if ((newTest != expression.Test) || (newFalse != expression.IfFalse) || (newTrue != expression.IfTrue))
                return Expression.Condition (newTest, newTrue, newFalse);
            return expression;
        }

        /// <summary>
        /// Visit the specified <see cref="LambdaExpression"/>.
        /// </summary>
        /// <param name='expression'>
        /// Expression.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        public Expression Visit (LambdaExpression expression, IVisitContext context)
        {
            ReadOnlyCollection<ParameterExpression> newParameters = VisitChecked (expression.Parameters, "LambdaExpression", context);
            Expression newBody = VisitExpression (expression.Body, context);
            if ((newBody != expression.Body) || (newParameters != expression.Parameters))
                return Expression.Lambda (expression.Type, newBody, newParameters);
            return expression;

        }
    
        /// <summary>
        /// Visit the specified <see cref="MethodCallExpression"/>.
        /// </summary>
        /// <param name='expression'>
        /// Expression.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        public Expression Visit (MethodCallExpression expression, IVisitContext context)
        {
            Expression newObject = VisitExpression (expression.Object, context);
            ReadOnlyCollection<Expression> newArguments = VisitChecked (expression.Arguments, "MethodCallExpression", context);
            if ((newObject != expression.Object) || (newArguments != expression.Arguments))
                return Expression.Call (newObject, expression.Method, newArguments);
            return expression;

        }
    
        /// <summary>
        /// Visit the specified <see cref="InvocationExpression"/>.
        /// </summary>
        /// <param name='expression'>
        /// Expression.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        public Expression Visit (InvocationExpression expression, IVisitContext context)
        {
            Expression newExpression = VisitExpression (expression.Expression, context);
            ReadOnlyCollection<Expression> newArguments = VisitChecked (expression.Arguments, "InvocationExpression", context);
            if ((newExpression != expression.Expression) || (newArguments != expression.Arguments))
                return Expression.Invoke (newExpression, newArguments);
            return expression;
        }
    
        /// <summary>
        /// Visit the specified <see cref="MemberExpression"/>.
        /// </summary>
        /// <param name='expression'>
        /// Expression.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        public Expression Visit (MemberExpression expression, IVisitContext context)
        {
            Expression newExpression = VisitExpression (expression.Expression, context);
            if (newExpression != expression.Expression)
                return Expression.MakeMemberAccess (newExpression, expression.Member);
            return expression;
        }
    
        /// <summary>
        /// Visit the specified <see cref="NewExpression"/>.
        /// </summary>
        /// <param name='expression'>
        /// Expression.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        public Expression Visit (NewExpression expression, IVisitContext context)
        {
            ReadOnlyCollection<Expression> newArguments = VisitChecked (expression.Arguments, "NewExpression", context);
            if (newArguments != expression.Arguments) {
                if (expression.Members == null)
                    return Expression.New (expression.Constructor, newArguments);
                else
                    return Expression.New (expression.Constructor, AdjustArgumentsForNewExpression (newArguments, expression.Members), expression.Members);
            }
            return expression;
        }
    
        /// <summary>
        /// Visit the specified <see cref="NewArrayExpression"/>.
        /// </summary>
        /// <param name='expression'>
        /// Expression.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        public Expression Visit (NewArrayExpression expression, IVisitContext context)
        {
            ReadOnlyCollection<Expression> newExpressions = VisitChecked (expression.Expressions, "NewArrayExpression", context);
            if (newExpressions != expression.Expressions) {
                var elementType = expression.Type.GetElementType ();
                if (expression.NodeType == ExpressionType.NewArrayInit)
                    return Expression.NewArrayInit (elementType, newExpressions);
                else
                    return Expression.NewArrayBounds (elementType, newExpressions);
            }
            return expression;
        }
    
        /// <summary>
        /// Visit the specified <see cref="MemberInitExpression"/>.
        /// </summary>
        /// <param name='expression'>
        /// Expression.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        /// <exception cref='NotSupportedException'>
        /// Is thrown when the visit of the <see cref="MemberInitExpression.NewExpression"/> does not return a <see cref="NewExpression"/>.
        /// </exception>
        public Expression Visit (MemberInitExpression expression, IVisitContext context)
        {
            NewExpression newNewExpression = VisitExpression (expression.NewExpression, context) as NewExpression;
            if (newNewExpression == null) {
                throw new NotSupportedException (
                    "MemberInitExpressions only support non-null instances of type 'NewExpression' as their NewExpression member.");
            }
        
            ReadOnlyCollection<MemberBinding> newBindings = VisitMemberBindingList (expression.Bindings, context);
            if (newNewExpression != expression.NewExpression || newBindings != expression.Bindings)
                return Expression.MemberInit (newNewExpression, newBindings);
            return expression;
        }
    
        /// <summary>
        /// Visit the specified <see cref="ListInitExpression"/>.
        /// </summary>
        /// <param name='expression'>
        /// Expression.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        /// <exception cref='NotSupportedException'>
        /// Is thrown when the visit of the <see cref="ListInitExpression.NewExpression"/> does not return a <see cref="NewExpression"/>.
        /// </exception>
        public Expression Visit (ListInitExpression expression, IVisitContext context)
        {
            NewExpression newNewExpression = VisitExpression (expression.NewExpression, context) as NewExpression;
            if (newNewExpression == null)
                throw new NotSupportedException ("ListInitExpressions only support non-null instances of type 'NewExpression' as their NewExpression member.");
            ReadOnlyCollection<ElementInit> newInitializers = VisitElementInitList (expression.Initializers, context);
            if (newNewExpression != expression.NewExpression || newInitializers != expression.Initializers)
                return Expression.ListInit (newNewExpression, newInitializers);
            return expression;
        }
    
        #region private methods
        
        /// <summary>
        /// Require the composition to visit the specified <see cref="Expression"/>.
        /// </summary>
        /// <returns>
        /// The expression to visit.
        /// </returns>
        /// <param name='expression'>
        /// Expression.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        private Expression VisitExpression (Expression expression, IVisitContext context)
        {
            if (null == expression)
                return null;
            return VisitInner(expression, context);
        }
        
        /// <summary>
        /// Adjusts the arguments for a <see cref="NewExpression"/> so that they match the given members.
        /// </summary>
        /// <param name="arguments">The arguments to adjust.</param>
        /// <param name="members">The members defining the required argument types.</param>
        /// <returns>
        /// A sequence of expressions that are equivalent to <paramref name="arguments"/>, but converted to the associated member's
        /// result type if needed.
        /// </returns>
        private static IEnumerable<Expression> AdjustArgumentsForNewExpression (IList<Expression> arguments, IList<MemberInfo> members)
        {
            for (int i = 0; i < arguments.Count; ++i) {
                Type memberReturnType = Reflection.GetMemberReturnType (members [i]);
                if (arguments [i].Type == memberReturnType)
                    yield return arguments[i];
                else
                    yield return Expression.Convert (arguments[i], memberReturnType);
            }
        }
    
        private T VisitChecked<T> (T expression, string outerExpression, IVisitContext context) where T : Expression
        {
            T newExpression = VisitExpression (expression, context) as T;
    
            if (newExpression == null) {
                var message = string.Format (
                "While visiting a '{0}', expressions of type '{1}' can only be replaced with other non-null expressions of type '{2}'.",
                outerExpression,
                typeof(T).Name,
                typeof(T).Name);
    
                throw new InvalidOperationException (message);
            }
    
            return newExpression;
        }

        private ReadOnlyCollection<T> VisitChecked<T> (ReadOnlyCollection<T> expressions, string outerExpression, IVisitContext context) where T : Expression
        {
            return VisitReadOnlyCollection (expressions, (expression, s) => VisitChecked (expression, outerExpression, s), context);
        }

        private ReadOnlyCollection<T> VisitReadOnlyCollection<T> (ReadOnlyCollection<T> list, Func<T, IVisitContext, T> visitMethod, IVisitContext context)
            where T : class
        {
            List<T> newList = null;
    
            for (int i = 0; i < list.Count; i++) {
                T element = list [i];
                T newElement = visitMethod (element, context);
    
                if (element != newElement) {
                    if (newList == null)
                        newList = new List<T> (list);
    
                    newList [i] = newElement;
                }
            }
    
            if (newList != null)
                return newList.AsReadOnly ();
            else
                return list;
        }
        
        private ElementInit VisitElementInit (ElementInit elementInit, IVisitContext context)
        {
            ReadOnlyCollection<Expression> newArguments = VisitChecked (elementInit.Arguments, "ElementInit", context);
            if (newArguments != elementInit.Arguments)
                return Expression.ElementInit (elementInit.AddMethod, newArguments);
            return elementInit;
        }
    
        private MemberBinding VisitMemberBinding (MemberBinding memberBinding, IVisitContext context)
        {
            switch (memberBinding.BindingType) {
            case MemberBindingType.Assignment:
                return VisitMemberAssignment ((MemberAssignment)memberBinding, context);
            case MemberBindingType.MemberBinding:
                return VisitMemberMemberBinding ((MemberMemberBinding)memberBinding, context);
            default:
                return VisitMemberListBinding ((MemberListBinding)memberBinding, context);
            }
        }
    
        private MemberBinding VisitMemberAssignment (MemberAssignment memberAssigment, IVisitContext context)
        {
            Expression expression = VisitExpression (memberAssigment.Expression, context);
            if (expression != memberAssigment.Expression)
                return Expression.Bind (memberAssigment.Member, expression);
            return memberAssigment;
        }
    
        private MemberBinding VisitMemberMemberBinding (MemberMemberBinding binding, IVisitContext context)
        {
            ReadOnlyCollection<MemberBinding> newBindings = VisitMemberBindingList (binding.Bindings, context);
            if (newBindings != binding.Bindings)
                return Expression.MemberBind (binding.Member, newBindings);
            return binding;
        }
    
        private MemberBinding VisitMemberListBinding (MemberListBinding listBinding, IVisitContext context)
        {
            ReadOnlyCollection<ElementInit> newInitializers = VisitElementInitList (listBinding.Initializers, context);
    
            if (newInitializers != listBinding.Initializers)
                return Expression.ListBind (listBinding.Member, newInitializers);
            return listBinding;
        }
    
        private ReadOnlyCollection<MemberBinding> VisitMemberBindingList (ReadOnlyCollection<MemberBinding> expressions, IVisitContext context)
        {
            return VisitReadOnlyCollection (expressions, VisitMemberBinding, context);
        }
    
        private ReadOnlyCollection<ElementInit> VisitElementInitList (ReadOnlyCollection<ElementInit> expressions, IVisitContext context)
        {
            return VisitReadOnlyCollection (expressions, VisitElementInit, context);
        }
        
        #endregion private methods
    }
}

