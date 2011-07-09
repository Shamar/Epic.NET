//  
//  UnvisitableExpressionVisitor.cs
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
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;
using System.Collections.ObjectModel; 
using System.Diagnostics;

namespace Epic.Linq.Expressions.Visit
{
    internal sealed class UnvisitableExpressionVisitor : CompositeVisitorBase, ICompositeVisitor<Expression>,
        ICompositeVisitor<UnaryExpression>, 
        ICompositeVisitor<BinaryExpression>, 
        ICompositeVisitor<ConditionalExpression>,
        ICompositeVisitor<ConstantExpression>,
        ICompositeVisitor<InvocationExpression>,
        ICompositeVisitor<LambdaExpression>,
        ICompositeVisitor<MemberExpression>,
        ICompositeVisitor<MethodCallExpression>,
        ICompositeVisitor<NewExpression>,
        ICompositeVisitor<NewArrayExpression>,
        ICompositeVisitor<MemberInitExpression>,
        ICompositeVisitor<ListInitExpression>,
        ICompositeVisitor<ParameterExpression>,
        ICompositeVisitor<TypeBinaryExpression>
    {
        public UnvisitableExpressionVisitor (CompositeVisitorChain chain)
            : base(chain)
        {
        }


        #region ICompositeVisitor[Expression] implementation
        System.Linq.Expressions.Expression ICompositeVisitor<Expression>.Visit (Expression target)
        {
            return VisitExpression(target);
        }
        #endregion
   
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
    
        private Expression VisitExpression (Expression expression)
        {
            if (expression == null)
                return null;
    
            VisitableExpression visitable = expression as VisitableExpression;
            if (visitable != null)
                return visitable.Accept (this);
    
            switch (expression.NodeType) {
            case ExpressionType.ArrayLength:
            case ExpressionType.Convert:
            case ExpressionType.ConvertChecked:
            case ExpressionType.Negate:
            case ExpressionType.NegateChecked:
            case ExpressionType.Not:
            case ExpressionType.Quote:
            case ExpressionType.TypeAs:
            case ExpressionType.UnaryPlus:
                return Visit ((UnaryExpression)expression);
            case ExpressionType.Add:
            case ExpressionType.AddChecked:
            case ExpressionType.Divide:
            case ExpressionType.Modulo:
            case ExpressionType.Multiply:
            case ExpressionType.MultiplyChecked:
            case ExpressionType.Power:
            case ExpressionType.Subtract:
            case ExpressionType.SubtractChecked:
            case ExpressionType.And:
            case ExpressionType.Or:
            case ExpressionType.ExclusiveOr:
            case ExpressionType.LeftShift:
            case ExpressionType.RightShift:
            case ExpressionType.AndAlso:
            case ExpressionType.OrElse:
            case ExpressionType.Equal:
            case ExpressionType.NotEqual:
            case ExpressionType.GreaterThanOrEqual:
            case ExpressionType.GreaterThan:
            case ExpressionType.LessThan:
            case ExpressionType.LessThanOrEqual:
            case ExpressionType.Coalesce:
            case ExpressionType.ArrayIndex:
                return Visit ((BinaryExpression)expression);
            case ExpressionType.Conditional:
                return Visit ((ConditionalExpression)expression);
            case ExpressionType.Constant:
                return Visit ((ConstantExpression)expression);
            case ExpressionType.Invoke:
                return Visit ((InvocationExpression)expression);
            case ExpressionType.Lambda:
                return Visit ((LambdaExpression)expression);
            case ExpressionType.MemberAccess:
                return Visit ((MemberExpression)expression);
            case ExpressionType.Call:
                return Visit ((MethodCallExpression)expression);
            case ExpressionType.New:
                return Visit ((NewExpression)expression);
            case ExpressionType.NewArrayBounds:
            case ExpressionType.NewArrayInit:
                return Visit ((NewArrayExpression)expression);
            case ExpressionType.MemberInit:
                return Visit ((MemberInitExpression)expression);
            case ExpressionType.ListInit:
                return Visit ((ListInitExpression)expression);
            case ExpressionType.Parameter:
                return Visit ((ParameterExpression)expression);
            case ExpressionType.TypeIs:
                return Visit ((TypeBinaryExpression)expression);
    
            default:
                return VisitUnknownNonVisitableExpression (expression);
            }
        }
    
        private T VisitAndConvert<T> (T expression, string methodName) where T : Expression
        {
            if (expression == null)
                return null;
    
            var newExpression = VisitExpression (expression) as T;
    
            if (newExpression == null) {
                var message = string.Format (
                "When called from '{0}', expressions of type '{1}' can only be replaced with other non-null expressions of type '{2}'.",
                methodName,
                typeof(T).Name,
                typeof(T).Name);
    
                throw new InvalidOperationException (message);
            }
    
            return newExpression;
        }

        private ReadOnlyCollection<T> VisitAndConvert<T> (ReadOnlyCollection<T> expressions, string callerName) where T : Expression
        {
            return VisitList (expressions, expression => VisitAndConvert (expression, callerName));
        }

        public ReadOnlyCollection<T> VisitList<T> (ReadOnlyCollection<T> list, Func<T, T> visitMethod)
            where T : class
        {
            List<T> newList = null;
    
            for (int i = 0; i < list.Count; i++) {
                T element = list [i];
                T newElement = visitMethod (element);
                if (newElement == null)
                    throw new NotSupportedException ("The current list only supports objects of type '" + typeof(T).Name + "' as its elements.");
    
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
    
        private Expression VisitUnknownNonVisitableExpression (Expression expression)
        {
            var message = string.Format ("Expression type '{0}' is not supported by this {1}.", expression.GetType ().Name, GetType ().Name);
            throw new NotSupportedException (message);
        }
    
        public Expression Visit (UnaryExpression expression)
        {
            ICompositeVisitor<UnaryExpression> visitor = GetNextVisitor<UnaryExpression> (expression);
            if (this != visitor)
                return visitor.Visit (expression);
            Expression newOperand = VisitExpression (expression.Operand);
            if (newOperand != expression.Operand) {
                if (expression.NodeType == ExpressionType.UnaryPlus)
                    return Expression.UnaryPlus (newOperand, expression.Method);
                else
                    return Expression.MakeUnary (expression.NodeType, newOperand, expression.Type, expression.Method);
            } else
                return expression;
        }
    
        public Expression Visit (BinaryExpression expression)
        {
            ICompositeVisitor<BinaryExpression> visitor = GetNextVisitor<BinaryExpression> (expression);
            if (this != visitor)
                return visitor.Visit (expression);
            Expression newLeft = VisitExpression (expression.Left);
            Expression newRight = VisitExpression (expression.Right);
            var newConversion = (LambdaExpression)VisitExpression (expression.Conversion);
            if (newLeft != expression.Left || newRight != expression.Right || newConversion != expression.Conversion)
                return Expression.MakeBinary (expression.NodeType, newLeft, newRight, expression.IsLiftedToNull, expression.Method, newConversion);
            return expression;
        }
    
        public Expression Visit (TypeBinaryExpression expression)
        {
            ICompositeVisitor<TypeBinaryExpression> visitor = GetNextVisitor<TypeBinaryExpression> (expression);
            if (this != visitor)
                return visitor.Visit (expression);
            Expression newExpression = VisitExpression (expression.Expression);
            if (newExpression != expression.Expression)
                return Expression.TypeIs (newExpression, expression.TypeOperand);
            return expression;
        }
    
        public Expression Visit (ConstantExpression expression)
        {
            ICompositeVisitor<ConstantExpression> visitor = GetNextVisitor<ConstantExpression> (expression);
            if (this != visitor)
                return visitor.Visit (expression);
            return expression;
        }
    
        public Expression Visit (ConditionalExpression expression)
        {
            ICompositeVisitor<ConditionalExpression> visitor = GetNextVisitor<ConditionalExpression> (expression);
            if (this != visitor)
                return visitor.Visit (expression);
            Expression newTest = VisitExpression (expression.Test);
            Expression newFalse = VisitExpression (expression.IfFalse);
            Expression newTrue = VisitExpression (expression.IfTrue);
            if ((newTest != expression.Test) || (newFalse != expression.IfFalse) || (newTrue != expression.IfTrue))
                return Expression.Condition (newTest, newTrue, newFalse);
            return expression;
        }
    
        public Expression Visit (ParameterExpression expression)
        {
            ICompositeVisitor<ParameterExpression> visitor = GetNextVisitor<ParameterExpression> (expression);
            if (this != visitor)
                return visitor.Visit (expression);
            return expression;
        }
    
        public Expression Visit (LambdaExpression expression)
        {
            ICompositeVisitor<LambdaExpression> visitor = GetNextVisitor<LambdaExpression> (expression);
            if (this != visitor)
                return visitor.Visit (expression);
            ReadOnlyCollection<ParameterExpression> newParameters = VisitAndConvert (expression.Parameters, "Visit");
            Expression newBody = VisitExpression (expression.Body);
            if ((newBody != expression.Body) || (newParameters != expression.Parameters))
                return Expression.Lambda (expression.Type, newBody, newParameters);
            return expression;
        }
    
        public Expression Visit (MethodCallExpression expression)
        {
            ICompositeVisitor<MethodCallExpression> visitor = GetNextVisitor<MethodCallExpression> (expression);
            if (this != visitor)
                return visitor.Visit (expression);
            Expression newObject = VisitExpression (expression.Object);
            ReadOnlyCollection<Expression> newArguments = VisitAndConvert (expression.Arguments, "Visit");
            if ((newObject != expression.Object) || (newArguments != expression.Arguments))
                return Expression.Call (newObject, expression.Method, newArguments);
            return expression;
        }
    
        public Expression Visit (InvocationExpression expression)
        {
            ICompositeVisitor<InvocationExpression> visitor = GetNextVisitor<InvocationExpression> (expression);
            if (this != visitor)
                return visitor.Visit (expression);
            Expression newExpression = VisitExpression (expression.Expression);
            ReadOnlyCollection<Expression> newArguments = VisitAndConvert (expression.Arguments, "Visit");
            if ((newExpression != expression.Expression) || (newArguments != expression.Arguments))
                return Expression.Invoke (newExpression, newArguments);
            return expression;
        }
    
        public Expression Visit (MemberExpression expression)
        {
            ICompositeVisitor<MemberExpression> visitor = GetNextVisitor<MemberExpression> (expression);
            if (this != visitor)
                return visitor.Visit (expression);
            Expression newExpression = VisitExpression (expression.Expression);
            if (newExpression != expression.Expression)
                return Expression.MakeMemberAccess (newExpression, expression.Member);
            return expression;
        }
    
        public Expression Visit (NewExpression expression)
        {
            ICompositeVisitor<NewExpression> visitor = GetNextVisitor<NewExpression> (expression);
            if (this != visitor)
                return visitor.Visit (expression);
            ReadOnlyCollection<Expression> newArguments = VisitAndConvert (expression.Arguments, "Visit");
            if (newArguments != expression.Arguments) {
                if (expression.Members == null)
                    return Expression.New (expression.Constructor, newArguments);
                else
                    return Expression.New (expression.Constructor, AdjustArgumentsForNewExpression (newArguments, expression.Members), expression.Members);
            }
            return expression;
        }
    
        public Expression Visit (NewArrayExpression expression)
        {
            ICompositeVisitor<NewArrayExpression> visitor = GetNextVisitor<NewArrayExpression> (expression);
            if (this != visitor)
                return visitor.Visit (expression);
            ReadOnlyCollection<Expression> newExpressions = VisitAndConvert (expression.Expressions, "Visit");
            if (newExpressions != expression.Expressions) {
                var elementType = expression.Type.GetElementType ();
                if (expression.NodeType == ExpressionType.NewArrayInit)
                    return Expression.NewArrayInit (elementType, newExpressions);
                else
                    return Expression.NewArrayBounds (elementType, newExpressions);
            }
            return expression;
        }
    
        public Expression Visit (MemberInitExpression expression)
        {
            ICompositeVisitor<MemberInitExpression> visitor = GetNextVisitor<MemberInitExpression> (expression);
            if (this != visitor)
                return visitor.Visit (expression);
            var newNewExpression = VisitExpression (expression.NewExpression) as NewExpression;
            if (newNewExpression == null) {
                throw new NotSupportedException (
                "MemberInitExpressions only support non-null instances of type 'NewExpression' as their NewExpression member.");
            }
    
            ReadOnlyCollection<MemberBinding> newBindings = VisitMemberBindingList (expression.Bindings);
            if (newNewExpression != expression.NewExpression || newBindings != expression.Bindings)
                return Expression.MemberInit (newNewExpression, newBindings);
            return expression;
        }
    
        public Expression Visit (ListInitExpression expression)
        {
            ICompositeVisitor<ListInitExpression> visitor = GetNextVisitor<ListInitExpression> (expression);
            if (this != visitor)
                return visitor.Visit (expression);
            var newNewExpression = VisitExpression (expression.NewExpression) as NewExpression;
            if (newNewExpression == null)
                throw new NotSupportedException ("ListInitExpressions only support non-null instances of type 'NewExpression' as their NewExpression member.");
            ReadOnlyCollection<ElementInit> newInitializers = VisitElementInitList (expression.Initializers);
            if (newNewExpression != expression.NewExpression || newInitializers != expression.Initializers)
                return Expression.ListInit (newNewExpression, newInitializers);
            return expression;
        }
    
        private ElementInit VisitElementInit (ElementInit elementInit)
        {
            ReadOnlyCollection<Expression> newArguments = VisitAndConvert (elementInit.Arguments, "VisitElementInit");
            if (newArguments != elementInit.Arguments)
                return Expression.ElementInit (elementInit.AddMethod, newArguments);
            return elementInit;
        }
    
        private MemberBinding VisitMemberBinding (MemberBinding memberBinding)
        {
            switch (memberBinding.BindingType) {
            case MemberBindingType.Assignment:
                return VisitMemberAssignment ((MemberAssignment)memberBinding);
            case MemberBindingType.MemberBinding:
                return VisitMemberMemberBinding ((MemberMemberBinding)memberBinding);
            default:
                Debug.Assert (
                  memberBinding.BindingType == MemberBindingType.ListBinding, "Invalid member binding type " + memberBinding.GetType ().FullName);
                return VisitMemberListBinding ((MemberListBinding)memberBinding);
            }
        }
    
        private MemberBinding VisitMemberAssignment (MemberAssignment memberAssigment)
        {
            Expression expression = VisitExpression (memberAssigment.Expression);
            if (expression != memberAssigment.Expression)
                return Expression.Bind (memberAssigment.Member, expression);
            return memberAssigment;
        }
    
        private MemberBinding VisitMemberMemberBinding (MemberMemberBinding binding)
        {
            ReadOnlyCollection<MemberBinding> newBindings = VisitMemberBindingList (binding.Bindings);
            if (newBindings != binding.Bindings)
                return Expression.MemberBind (binding.Member, newBindings);
            return binding;
        }
    
        private MemberBinding VisitMemberListBinding (MemberListBinding listBinding)
        {
            ReadOnlyCollection<ElementInit> newInitializers = VisitElementInitList (listBinding.Initializers);
    
            if (newInitializers != listBinding.Initializers)
                return Expression.ListBind (listBinding.Member, newInitializers);
            return listBinding;
        }
    
        private ReadOnlyCollection<MemberBinding> VisitMemberBindingList (ReadOnlyCollection<MemberBinding> expressions)
        {
            return VisitList (expressions, VisitMemberBinding);
        }
    
        private ReadOnlyCollection<ElementInit> VisitElementInitList (ReadOnlyCollection<ElementInit> expressions)
        {
            return VisitList (expressions, VisitElementInit);
        }
    }
}

